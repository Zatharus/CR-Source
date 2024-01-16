// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Viewer.Program
// Assembly: ComicRack, Version=1.0.5915.38777, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: E9032406-66BB-46BB-A802-D697DB44DA19
// Assembly location: C:\Program Files\ComicRack\ComicRack.exe

using cYo.Common;
using cYo.Common.Collections;
using cYo.Common.ComponentModel;
using cYo.Common.Compression;
using cYo.Common.Drawing;
using cYo.Common.IO;
using cYo.Common.Localize;
using cYo.Common.Mathematics;
using cYo.Common.Net;
using cYo.Common.Presentation.Tao;
using cYo.Common.Runtime;
using cYo.Common.Text;
using cYo.Common.Threading;
using cYo.Common.Win32;
using cYo.Common.Windows;
using cYo.Common.Windows.Forms;
using cYo.Projects.ComicRack.Engine;
using cYo.Projects.ComicRack.Engine.Database;
using cYo.Projects.ComicRack.Engine.Display.Forms;
using cYo.Projects.ComicRack.Engine.Drawing;
using cYo.Projects.ComicRack.Engine.IO;
using cYo.Projects.ComicRack.Engine.IO.Cache;
using cYo.Projects.ComicRack.Engine.IO.Provider;
using cYo.Projects.ComicRack.Engine.Sync;
using cYo.Projects.ComicRack.Plugins;
using cYo.Projects.ComicRack.Viewer.Config;
using cYo.Projects.ComicRack.Viewer.Dialogs;
using cYo.Projects.ComicRack.Viewer.Properties;
using cYo.Projects.ComicRack.Viewer.Remote;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace cYo.Projects.ComicRack.Viewer
{
  public static class Program
  {
    public const int DeadLockTestTime = 0;
    public const int MinThumbHeight = 96;
    public const int MaxThumbHeight = 512;
    public const int MinTileHeight = 64;
    public const int MaxTileHeight = 256;
    public const int MinRowHeight = 12;
    public const int MaxRowHeight = 48;
    public const string DefaultCrashUrl = "http://comicrack.cyolito.com/services/CrashReport.php";
    public const string DefaultWiki = "http://comicrack.cyolito.com/documentation/wiki";
    public const string DefaultWebSite = "http://comicrack.cyolito.com";
    public const string DefaultNewsFeed = "http://feeds.feedburner.com/ComicRackNews";
    public const string DefaultUserForm = "http://comicrack.cyolito.com/user-forum";
    public const string DefaultPayPalPage = "http://comicrack.cyolito.com/donate";
    public const string DefaultLocalizePage = "http://comicrack.cyolito.com/faqs/12-how-to-create-language-packs";
    public const string ComicRackTypeId = "cYo.ComicRack";
    public const string ComicRackDocumentName = "eComic";
    public const int NewsIntervalMinutes = 60;
    public const int ActivationRecheckDays = 30;
    private static ExtendedSettings extendedSettings;
    public static readonly SystemPaths Paths = new SystemPaths(Program.UseLocalSettings, Program.ExtendedSettings.AlternateConfig, Program.ExtendedSettings.DatabasePath, Program.ExtendedSettings.CachePath);
    public static readonly ContextHelp Help = new ContextHelp(Path.Combine(Application.StartupPath, nameof (Help)));
    public static readonly string QuickHelpManualFile = Path.Combine(Application.StartupPath, "Help\\ComicRack Introduction.djvu");
    private const string DefaultListsFile = "DefaultLists.txt";
    private static readonly string defaultSettingsFile = Path.Combine(Program.Paths.ApplicationDataPath, "Config.xml");
    private static readonly string defaultNewsFile = Path.Combine(Program.Paths.ApplicationDataPath, "NewsFeeds.xml");
    private const string DefaultBackgroundTexturesPath = "Resources\\Textures\\Backgrounds";
    private const string DefaultPaperTexturesPath = "Resources\\Textures\\Papers";
    private const string DefaultIconPackagesPath = "Resources\\Icons";
    public static readonly PackageManager ScriptPackages = new PackageManager(Program.Paths.ScriptPathSecondary, Program.Paths.PendingScriptsPath, true);
    public static readonly DatabaseManager DatabaseManager = new DatabaseManager();
    private static readonly object installedLanguagesLock = new object();
    private static List<TRInfo> installedLanguages;
    private static Splash splash;
    private static readonly Regex yearRegex = new Regex("\\((?<start>\\d{4})-(?<end>\\d{4})\\)", RegexOptions.Compiled);

    public static ExtendedSettings ExtendedSettings
    {
      get
      {
        if (Program.extendedSettings == null)
        {
          Program.extendedSettings = new ExtendedSettings();
          CommandLineParser.Parse((object) Program.extendedSettings, CommandLineParserOptions.None);
          if (!string.IsNullOrEmpty(Program.extendedSettings.AlternateConfig))
            IniFile.AddDefaultLocation(SystemPaths.MakeApplicationPath(Program.extendedSettings.AlternateConfig, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
          Program.extendedSettings = new ExtendedSettings();
          CommandLineParser.Parse((object) Program.extendedSettings);
          if (Program.extendedSettings.Restart)
          {
            Program.extendedSettings.InstallPlugin = (string) null;
            Program.extendedSettings.ImportList = (string) null;
            Program.extendedSettings.Files = Enumerable.Empty<string>();
          }
        }
        return Program.extendedSettings;
      }
    }

    public static MainForm MainForm { get; private set; }

    public static DefaultLists Lists { get; private set; }

    public static bool Restart { get; set; }

    public static ScriptOutputForm ScriptConsole { get; set; }

    public static bool UseLocalSettings
    {
      get => IniFile.Default.GetValue<bool>(nameof (UseLocalSettings));
    }

    public static Settings Settings { get; private set; }

    public static NewsStorage News { get; private set; }

    public static CacheManager CacheManager { get; private set; }

    public static ImagePool ImagePool => Program.CacheManager.ImagePool;

    public static NetworkManager NetworkManager { get; private set; }

    public static QueueManager QueueManager { get; private set; }

    public static ComicScanner Scanner => Program.QueueManager.Scanner;

    public static ComicDatabase Database => Program.DatabaseManager.Database;

    public static ComicBookFactory BookFactory => Program.DatabaseManager.BookFactory;

    public static FileCache InternetCache => Program.CacheManager.InternetCache;

    public static IEnumerable<string> CommandLineFiles
    {
      get => Program.ExtendedSettings.Files ?? Enumerable.Empty<string>();
    }

    public static ExportSettingCollection ExportComicRackPresets
    {
      get
      {
        ExportSettingCollection comicRackPresets = new ExportSettingCollection();
        comicRackPresets.Add(ExportSetting.ConvertToCBZ);
        comicRackPresets.Add(ExportSetting.ConvertToCB7);
        return comicRackPresets;
      }
    }

    public static IEnumerable<StringPair> DefaultKeyboardMapping { get; set; }

    public static TRInfo[] InstalledLanguages
    {
      get
      {
        using (ItemMonitor.Lock(Program.installedLanguagesLock))
        {
          if (Program.installedLanguages == null)
          {
            Program.installedLanguages = new List<TRInfo>();
            TRDictionary newPack = (TRDictionary) null;
            try
            {
              newPack = new TRDictionary(TR.ResourceFolder, "de");
            }
            catch (Exception ex)
            {
            }
            foreach (TRInfo languageInfo in TR.GetLanguageInfos())
            {
              TRDictionary trDictionary = new TRDictionary(TR.ResourceFolder, languageInfo.CultureName);
              if (newPack != null)
                languageInfo.CompletionPercent = trDictionary.CompletionPercent(newPack);
              Program.installedLanguages.Add(languageInfo);
            }
            Program.installedLanguages.Sort((Comparison<TRInfo>) ((a, b) =>
            {
              int num = b.CompletionPercent.CompareTo(a.CompletionPercent);
              return num == 0 ? string.Compare(a.CultureName, b.CultureName) : num;
            }));
          }
          return Program.installedLanguages.ToArray();
        }
      }
    }

    public static void RefreshAllWindows()
    {
      Program.ForAllForms((Action<Form>) (f => f.Refresh()));
    }

    public static void ForAllForms(Action<Form> action)
    {
      if (action == null)
        return;
      foreach (Form openForm in (ReadOnlyCollectionBase) Application.OpenForms)
        action(openForm);
    }

    public static string ShowComicOpenDialog(
      IWin32Window parent,
      string title,
      bool includeReadingLists)
    {
      string str = (string) null;
      using (OpenFileDialog openFileDialog = new OpenFileDialog())
      {
        IEnumerable<FileFormat> fileFormats = (IEnumerable<FileFormat>) Providers.Readers.GetSourceFormats().OrderBy<FileFormat, FileFormat>((Func<FileFormat, FileFormat>) (f => f));
        if (includeReadingLists)
          fileFormats = fileFormats.AddLast<FileFormat>(new FileFormat(TR.Load("FileFilter")["FormatReadingList", "ComicRack Reading List"], 10089, ".cbl"));
        openFileDialog.Title = title;
        openFileDialog.Filter = fileFormats.GetDialogFilter(true);
        openFileDialog.FilterIndex = Program.Settings.LastOpenFilterIndex;
        openFileDialog.CheckFileExists = true;
        foreach (string favoritePath in Program.GetFavoritePaths())
          openFileDialog.CustomPlaces.Add(favoritePath);
        if (openFileDialog.ShowDialog(parent) == DialogResult.OK)
          str = openFileDialog.FileName;
        Program.Settings.LastOpenFilterIndex = openFileDialog.FilterIndex;
      }
      return str;
    }

    public static bool AskQuestion(
      IWin32Window parent,
      string question,
      string okButton,
      HiddenMessageBoxes hmb,
      string askAgainText = null,
      string cancelButton = null)
    {
      if ((Program.Settings.HiddenMessageBoxes & hmb) != HiddenMessageBoxes.None)
        return true;
      if (string.IsNullOrEmpty(askAgainText))
        askAgainText = TR.Messages["NotAskAgain", "&Do not ask me again"];
      switch (QuestionDialog.AskQuestion(parent, question, okButton, askAgainText, cancelText: cancelButton))
      {
        case QuestionResult.Cancel:
          return false;
        case QuestionResult.OkWithOption:
          Program.Settings.HiddenMessageBoxes |= hmb;
          break;
      }
      return true;
    }

    public static void Collect() => GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

    public static void StartDocument(string document, string path = null)
    {
      try
      {
        ProcessStartInfo startInfo = new ProcessStartInfo(document);
        if (path != null && Directory.Exists(path))
          startInfo.WorkingDirectory = path;
        Process.Start(startInfo);
      }
      catch (Exception ex)
      {
      }
    }

    public static void StartProgram(string exe, string commandLine)
    {
      try
      {
        Process.Start(exe, commandLine);
      }
      catch (Exception ex)
      {
      }
    }

    public static void ShowPayPal() => Program.StartDocument("http://comicrack.cyolito.com/donate");

    public static IEnumerable<ComicBookValueMatcher> GetAvailableComicBookMatchers()
    {
      return ComicBookValueMatcher.GetAvailableMatchers().OfType<ComicBookValueMatcher>();
    }

    public static IEnumerable<ComicBookValueMatcher> GetUsedComicBookMatchers(int minUsage)
    {
      return (IEnumerable<ComicBookValueMatcher>) Program.Database.ComicLists.GetItems<ComicSmartListItem>().SelectMany<ComicSmartListItem, ComicBookValueMatcher>((Func<ComicSmartListItem, IEnumerable<ComicBookValueMatcher>>) (n => n.Matchers.Recurse<ComicBookValueMatcher>((Func<object, IEnumerable>) (o => !(o is ComicBookGroupMatcher) ? (IEnumerable) null : (IEnumerable) ((ComicBookGroupMatcher) o).Matchers)))).Select<ComicBookValueMatcher, Type>((Func<ComicBookValueMatcher, Type>) (n => n.GetType())).GroupBy<Type, Type>((Func<Type, Type>) (n => n)).Where<IGrouping<Type, Type>>((Func<IGrouping<Type, Type>, bool>) (g => g.Count<Type>() >= minUsage)).Select<IGrouping<Type, Type>, ComicBookValueMatcher>((Func<IGrouping<Type, Type>, ComicBookValueMatcher>) (t => Activator.CreateInstance(t.First<Type>()) as ComicBookValueMatcher)).OrderBy<ComicBookValueMatcher, string>((Func<ComicBookValueMatcher, string>) (n => n.Description));
    }

    public static ContextMenuStrip CreateComicBookMatchersMenu(
      Action<ComicBookValueMatcher> action,
      int minUsage = 20)
    {
      ContextMenuBuilder contextMenuBuilder = new ContextMenuBuilder();
      Type[] array = Program.GetUsedComicBookMatchers(5).Select<ComicBookValueMatcher, Type>((Func<ComicBookValueMatcher, Type>) (m => m.GetType())).ToArray<Type>();
      foreach (ComicBookValueMatcher comicBookMatcher in Program.GetAvailableComicBookMatchers())
      {
        ComicBookValueMatcher m = comicBookMatcher;
        contextMenuBuilder.Add(comicBookMatcher.Description, false, false, (EventHandler) ((_, __) => action(m)), (object) null, ((IEnumerable<Type>) array).Contains<Type>(comicBookMatcher.GetType()) ? DateTime.MaxValue : DateTime.MinValue);
      }
      ContextMenuStrip bookMatchersMenu = new ContextMenuStrip();
      bookMatchersMenu.Items.AddRange(contextMenuBuilder.Create(20));
      return bookMatchersMenu;
    }

    public static string[] LoadDefaultBackgroundTextures()
    {
      return FileUtility.GetFiles(IniFile.GetDefaultLocations("Resources\\Textures\\Backgrounds"), SearchOption.AllDirectories, ".jpg", ".gif", ".png").OrderBy<string, string>((Func<string, string>) (s => s)).ToArray<string>();
    }

    public static string[] LoadDefaultPaperTextures()
    {
      return FileUtility.GetFiles(IniFile.GetDefaultLocations("Resources\\Textures\\Papers"), SearchOption.AllDirectories, ".jpg", ".gif", ".png").OrderBy<string, string>((Func<string, string>) (s => s)).ToArray<string>();
    }

    public static void StartupProgress(string message, int progress)
    {
      Splash splash = Program.splash;
      if (splash == null)
        return;
      splash.Message = splash.Message.AppendWithSeparator("\n", message);
      if (progress < 0)
        return;
      splash.Progress = progress;
    }

    public static IEnumerable<string> GetFavoritePaths()
    {
      return Program.Settings.FavoriteFolders.Concat<string>(Program.Database.WatchFolders.Select<WatchFolder, string>((Func<WatchFolder, string>) (wf => wf.Folder))).Distinct<string>();
    }

    public static Image MakeBooksImage(
      IEnumerable<ComicBook> books,
      System.Drawing.Size size,
      int maxImages,
      bool onlyMemory)
    {
      int val2 = books.Count<ComicBook>();
      int count = Math.Min(maxImages, val2);
      int num1 = size.Width / (count + 1);
      int height = size.Height - (count - 1) * 3;
      Bitmap bitmap1 = new Bitmap(size.Width, size.Height);
      using (Graphics graphics = Graphics.FromImage((Image) bitmap1))
      {
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        int num2 = 0;
        foreach (ComicBook comicBook in books.Take<ComicBook>(count))
        {
          int x = num1 * num2;
          int num3 = num1 * (num2 + 2);
          using (IItemLock<ThumbnailImage> image = Program.ImagePool.Thumbs.GetImage((ImageKey) comicBook.GetFrontCoverThumbnailKey(), onlyMemory))
          {
            Image bitmap2 = (Image) image?.Item.Bitmap;
            if (bitmap2 != null)
              ThumbRenderer.DrawThumbnail(graphics, bitmap2, new Rectangle(x, 3 * num2, num3 - x, height), ThumbnailDrawingOptions.Default | ThumbnailDrawingOptions.KeepAspect, comicBook);
          }
          ++num2;
        }
        if (count != val2)
        {
          System.Drawing.Color color = System.Drawing.Color.FromArgb(192, SystemColors.Highlight);
          Font iconTitleFont = SystemFonts.IconTitleFont;
          string str = StringUtility.Format("{0} {1}", (object) val2, (object) TR.Default["Books", nameof (books)]);
          Rectangle rectangle1 = new Rectangle(System.Drawing.Point.Empty, graphics.MeasureString(str, iconTitleFont).ToSize());
          rectangle1.Inflate(4, 4);
          Rectangle rectangle2 = rectangle1.Align(new Rectangle(System.Drawing.Point.Empty, size), ContentAlignment.MiddleCenter);
          using (GraphicsPath path = rectangle2.ConvertToPath(5, 5))
          {
            using (Brush brush = (Brush) new SolidBrush(color))
              graphics.FillPath(brush, path);
          }
          using (StringFormat format = new StringFormat()
          {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center
          })
            graphics.DrawString(str, iconTitleFont, SystemBrushes.HighlightText, (RectangleF) rectangle2, format);
        }
      }
      return (Image) bitmap1;
    }

    public static bool ShowExplorer(string path)
    {
      if (string.IsNullOrEmpty(path))
        return false;
      try
      {
        if (System.IO.File.Exists(path))
        {
          Process.Start("explorer.exe", string.Format("/n,/select,\"{0}\"", (object) path));
          return true;
        }
        if (Directory.Exists(path))
        {
          Process.Start("explorer.exe", string.Format("\"{0}\"", (object) path));
          return true;
        }
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    public static bool RegisterFormats(string formats)
    {
      try
      {
        bool overwrite = formats.Contains("!");
        foreach (string s in ((IEnumerable<string>) formats.Remove("!").Split(',')).RemoveEmpty())
        {
          bool flag = !s.Contains("-");
          string name = s.Remove("-");
          FileFormat fileFormat = Providers.Readers.GetSourceFormats().FirstOrDefault<FileFormat>((Func<FileFormat, bool>) (sf => sf.Name == name));
          if (fileFormat != null)
          {
            if (flag)
              fileFormat.RegisterShell("cYo.ComicRack", "eComic", overwrite);
            else
              fileFormat.UnregisterShell("cYo.ComicRack");
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
      finally
      {
        ShellRegister.RefreshShell();
      }
    }

    public static string LoadCustomThumbnail(string file, IWin32Window parent = null, string title = null)
    {
      string str1 = (string) null;
      if (string.IsNullOrEmpty(file))
      {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
          if (!string.IsNullOrEmpty(title))
            openFileDialog.Title = title;
          openFileDialog.Filter = TR.Load("FileFilter")["LoadThumbnail", "JPEG Image|*.jpg|Windows Bitmap Image|*.bmp|PNG Image|*.png|GIF Image|*.gif|TIFF Image|*.tif|Icon Image|*.ico"];
          openFileDialog.CheckFileExists = true;
          if (openFileDialog.ShowDialog(parent) == DialogResult.OK)
            file = openFileDialog.FileName;
        }
      }
      if (!string.IsNullOrEmpty(file))
      {
        string str2 = file;
        string str3 = (string) null;
        try
        {
          if (Path.GetExtension(file).Equals(".ico", StringComparison.OrdinalIgnoreCase))
          {
            using (Bitmap bitmap = BitmapExtensions.LoadIcon(file, System.Drawing.Color.Transparent))
            {
              file = str3 = Path.GetTempFileName();
              bitmap.Save(str3, ImageFormat.Png);
            }
          }
          using (Bitmap bmp = BitmapExtensions.BitmapFromFile(file))
            str1 = Program.ImagePool.AddCustomThumbnail(bmp);
          Program.Settings.ThumbnailFiles.UpdateMostRecent(str2);
        }
        catch (Exception ex)
        {
          Program.Settings.ThumbnailFiles.Remove(file);
          int num = (int) MessageBox.Show(parent, string.Format(TR.Messages["CouldNotLoadThumbnail", "Could not load thumbnail!\nReason: {0}"], (object) ex.Message), TR.Messages["Attention", "Attention"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        finally
        {
          if (str3 != null)
            FileUtility.SafeDelete(str3);
        }
      }
      return str1;
    }

    public static bool ValidateActivation(string userEmail)
    {
      Program.Settings.UserEmail = userEmail;
      bool activated = !string.IsNullOrEmpty(userEmail) && HttpAccess.CallSoap<UserValidation, bool>((Func<UserValidation, bool>) (uv => uv.HasDonated(userEmail)));
      Program.Settings.SetActivated(activated);
      return activated;
    }

    public static void AutoValidateActivation()
    {
      if (!Program.Settings.IsActivated)
        return;
      if (DateTime.Now - Program.Settings.ValidationDate < TimeSpan.FromDays(30.0))
        return;
      try
      {
        Program.ValidateActivation(Program.Settings.UserEmail);
      }
      catch
      {
      }
    }

    public static IEnumerable<string> HelpSystems => Program.Help.HelpSystems;

    public static string HelpSystem
    {
      get => Program.Help.HelpName;
      set
      {
        if (Program.Help.HelpName == value)
          return;
        if (!Program.Help.Load(value))
          Program.Help.Load("ComicRack Wiki");
        Program.Help.Variables["APPEXE"] = Application.ExecutablePath;
        Program.Help.Variables["APPPATH"] = Path.GetDirectoryName(Application.ExecutablePath);
        Program.Help.Variables["APPDATA"] = Program.Paths.ApplicationDataPath;
        Program.Help.Variables["USERPATH"] = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        Program.Help.ShowKey = Program.ExtendedSettings.ShowContextHelpKey;
        Program.Help.Initialize();
      }
    }

    private static void RemoteServerStarted(
      object sender,
      NetworkManager.RemoteServerStartedEventArgs e)
    {
      Program.CallMainForm("Remote Server Started", (Action) (() => Program.MainForm.OnRemoteServerStarted(e.Information)));
    }

    private static void RemoteServerStopped(
      object sender,
      NetworkManager.RemoteServerStoppedEventArgs e)
    {
      Program.CallMainForm("Remote Server Stopped", (Action) (() => Program.MainForm.OnRemoteServerStopped(e.Address)));
    }

    private static void CallMainForm(string actionName, Action action)
    {
      ThreadUtility.RunInBackground(actionName, (ThreadStart) (() =>
      {
        while (Program.MainForm == null || !Program.MainForm.IsInitialized)
        {
          if (Program.MainForm != null && Program.MainForm.IsDisposed)
            return;
          Thread.Sleep(1000);
        }
        if (Program.MainForm.InvokeIfRequired(action))
          return;
        action();
      }));
    }

    private static void ScannerCheckFileIgnore(object sender, ComicScanNotifyEventArgs e)
    {
      if (Program.Settings.DontAddRemoveFiles && Program.Database.IsBlacklisted(e.File))
        e.IgnoreFile = true;
      if (!Program.ExtendedSettings.MacCompatibleScanning || !Path.GetFileName(e.File).StartsWith("._"))
        return;
      e.IgnoreFile = true;
    }

    private static void IgnoredCoverImagesChanged(object sender, EventArgs e)
    {
      ComicInfo.CoverKeyFilter = Program.Settings.IgnoredCoverImages;
    }

    private static void SystemEventsPowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
      switch (e.Mode)
      {
        case PowerModes.Resume:
          Program.NetworkManager.Start();
          break;
        case PowerModes.Suspend:
          Program.NetworkManager.Stop();
          break;
      }
    }

    private static void SetUICulture(string culture)
    {
      if (string.IsNullOrEmpty(culture))
        return;
      try
      {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
      }
      catch (Exception ex)
      {
      }
    }

    private static void StartNew(string[] args)
    {
      Thread.CurrentThread.Name = "GUI Thread";
      Diagnostic.StartWatchDog(new BarkEventHandler(CrashDialog.OnBark));
      ComicBookValueMatcher.RegisterMatcherType(typeof (ComicBookPluginMatcher));
      ComicBookValueMatcher.RegisterMatcherType(typeof (ComicBookExpressionMatcher));
      Program.Settings = Settings.Load(Program.defaultSettingsFile);
      ++Program.Settings.RunCount;
      CommandLineParser.Parse((object) ImageDisplayControl.HardwareSettings);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Program.AutoValidateActivation();
      Program.DatabaseManager.FirstDatabaseAccess += (EventHandler) ((_, __) => Program.StartupProgress(TR.Messages["OpenDatabase", "Opening Database"], -1));
      Program.DatabaseManager.BackgroundSaveInterval = Program.ExtendedSettings.DatabaseBackgroundSaving;
      WirelessSyncProvider.StartListen();
      WirelessSyncProvider.ClientSyncRequest += (EventHandler<WirelessSyncProvider.ClientSyncRequestArgs>) ((s, e) =>
      {
        if (Program.MainForm == null)
          return;
        IPAddress address = s as IPAddress;
        e.IsPaired = Program.QueueManager.Devices.Any<DeviceSyncSettings>((Func<DeviceSyncSettings, bool>) (d => d.DeviceKey == e.Key));
        if (!e.IsPaired || address == null)
          return;
        ControlExtensions.BeginInvoke(Program.MainForm, (Action) (() => Program.QueueManager.SynchronizeDevice(e.Key, address)));
      });
      if (!Program.ExtendedSettings.DisableAutoTuneSystem)
        Program.AutoTuneSystem();
      ListExtensions.ParallelEnabled = EngineConfiguration.Default.EnableParallelQueries;
      if (EngineConfiguration.Default.IgnoredArticles != null)
        StringUtility.Articles = EngineConfiguration.Default.IgnoredArticles;
      ComicLibrary.QueryCacheMode = Program.ExtendedSettings.QueryCacheMode;
      ComicLibrary.BackgroundQueryCacheUpdate = !Program.ExtendedSettings.DisableBackgroundQueryCacheUpdate;
      ComicBook.EnableGroupNameCompression = Program.ExtendedSettings.EnableGroupNameCompression;
      try
      {
        string str = Program.ExtendedSettings.Language ?? Program.Settings.CultureName;
        if (!string.IsNullOrEmpty(str))
        {
          Program.SetUICulture(str);
          TR.DefaultCulture = new CultureInfo(str);
        }
      }
      catch (Exception ex)
      {
      }
      if (!Program.ExtendedSettings.LoadDatabaseInForeground)
        ThreadUtility.RunInBackground("Loading Database", (ThreadStart) (() => Program.InitializeDatabase(0, (string) null)));
      if (Program.Settings.ShowSplash)
      {
        ManualResetEvent mre = new ManualResetEvent(false);
        ThreadUtility.RunInBackground("Splash Thread", (ThreadStart) (() =>
        {
          Program.splash = new Splash() { Fade = true };
          Program.splash.Location = Program.splash.Bounds.Align(Screen.FromPoint(Program.Settings.CurrentWorkspace.FormBounds.Location).Bounds, ContentAlignment.MiddleCenter).Location;
          Program.splash.VisibleChanged += (EventHandler) ((s, e) => mre.Set());
          Program.splash.Closed += (EventHandler) ((s, e) => Program.splash = (Splash) null);
          int num = (int) Program.splash.ShowDialog();
        }));
        mre.WaitOne(5000, false);
      }
      try
      {
        if (Program.ExtendedSettings.LoadDatabaseInForeground)
        {
          string message = TR.Messages["OpenDatabase", "Opening Database"];
          Program.StartupProgress(message, 0);
          Program.InitializeDatabase(0, message);
        }
        Program.StartupProgress(TR.Messages["LoadCustomSettings", "Loading custom settings"], 20);
        IEnumerable<string> defaultLocations = IniFile.GetDefaultLocations("Resources\\Icons");
        ComicBook.PublisherIcons.AddRange((IEnumerable<IVirtualFolder>) ZipFileFolder.CreateFromFiles(defaultLocations, "Publishers*.zip"), new Func<string, IEnumerable<string>>(Program.SplitIconKeysWithYear));
        ComicBook.AgeRatingIcons.AddRange((IEnumerable<IVirtualFolder>) ZipFileFolder.CreateFromFiles(defaultLocations, "AgeRatings*.zip"), new Func<string, IEnumerable<string>>(Program.SplitIconKeys));
        ComicBook.FormatIcons.AddRange((IEnumerable<IVirtualFolder>) ZipFileFolder.CreateFromFiles(defaultLocations, "Formats*.zip"), new Func<string, IEnumerable<string>>(Program.SplitIconKeys));
        ComicBook.SpecialIcons.AddRange((IEnumerable<IVirtualFolder>) ZipFileFolder.CreateFromFiles(defaultLocations, "Special*.zip"), new Func<string, IEnumerable<string>>(Program.SplitIconKeys));
        ToolStripRenderer toolStripRenderer;
        if (Program.ExtendedSettings.SystemToolBars)
          toolStripRenderer = (ToolStripRenderer) new ToolStripSystemRenderer();
        else
          toolStripRenderer = (ToolStripRenderer) new ToolStripProfessionalRenderer(!(Program.ExtendedSettings.ForceTanColorSchema | (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major == 5)) ? (ProfessionalColorTable) new OptimizedProfessionalColorTable() : (ProfessionalColorTable) new OptimizedTanColorTable())
          {
            RoundedEdges = false
          };
        ToolStripManager.Renderer = toolStripRenderer;
        ImageDisplayControl.HardwareAcceleration = !Program.ExtendedSettings.DisableHardware ? (Program.ExtendedSettings.ForceHardware ? ImageDisplayControl.HardwareAccelerationType.Forced : ImageDisplayControl.HardwareAccelerationType.Enabled) : ImageDisplayControl.HardwareAccelerationType.Disabled;
        if (Program.ExtendedSettings.DisableMipMapping)
          ImageDisplayControl.HardwareSettings.MipMapping = false;
        Program.Lists = new DefaultLists((Func<IEnumerable<ComicBook>>) (() => (IEnumerable<ComicBook>) Program.Database.Books), IniFile.GetDefaultLocations("DefaultLists.txt"));
        Program.StartupProgress(TR.Messages["InitCache", "Initialize Disk Caches"], 30);
        Program.CacheManager = new CacheManager(Program.DatabaseManager, Program.Paths, (ICacheSettings) Program.Settings, Resources.ResourceManager);
        Program.QueueManager = new QueueManager(Program.DatabaseManager, Program.CacheManager, (IComicUpdateSettings) Program.Settings, (IEnumerable<DeviceSyncSettings>) Program.Settings.Devices);
        Program.QueueManager.ComicScanned += new EventHandler<ComicScanNotifyEventArgs>(Program.ScannerCheckFileIgnore);
        Program.Settings.IgnoredCoverImagesChanged += new EventHandler(Program.IgnoredCoverImagesChanged);
        Program.IgnoredCoverImagesChanged((object) null, EventArgs.Empty);
        SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(Program.SystemEventsPowerModeChanged);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(StringUtility.Format(TR.Messages["FailedToInitialize", "Failed to initialize ComicRack: {0}"], (object) ex.Message));
        return;
      }
      Program.StartupProgress(TR.Messages["ReadNewsFeed", "Reading News Feed"], 40);
      Program.News = NewsStorage.Load(Program.defaultNewsFile);
      if (Program.News.Subscriptions.Count == 0)
        Program.News.Subscriptions.Add(new NewsStorage.Subscription("http://feeds.feedburner.com/ComicRackNews", "ComicRack News"));
      Program.StartupProgress(TR.Messages["CreateMainWindow", "Creating Main Window"], 50);
      if (Program.ExtendedSettings.DisableScriptOptimization)
        PythonCommand.Optimized = false;
      if (Program.ExtendedSettings.ShowScriptConsole)
      {
        Program.ScriptConsole = new ScriptOutputForm();
        TextBoxStream ts = new TextBoxStream((TextBoxBase) Program.ScriptConsole.Log);
        PythonCommand.Output = (Stream) ts;
        PythonCommand.EnableLog = true;
        WebComic.SetLogOutput((Stream) ts);
        Program.ScriptConsole.Show();
      }
      Program.NetworkManager = new NetworkManager(Program.DatabaseManager, Program.CacheManager, (ISharesSettings) Program.Settings, Program.ExtendedSettings.PrivateServerPort, Program.ExtendedSettings.InternetServerPort, Program.ExtendedSettings.DisableBroadcast);
      Program.MainForm = new MainForm();
      Program.MainForm.FormClosed += new FormClosedEventHandler(Program.MainFormFormClosed);
      Program.MainForm.FormClosing += (FormClosingEventHandler) ((s, e) =>
      {
        bool flag = e.CloseReason == CloseReason.UserClosing;
        foreach (Command command in ScriptUtility.GetCommands("Shutdown"))
        {
          try
          {
            if (!(bool) command.Invoke(new object[1]
            {
              (object) flag
            }) & flag)
            {
              e.Cancel = true;
              break;
            }
          }
          catch (Exception ex)
          {
          }
        }
      });
      Application.AddMessageFilter((IMessageFilter) new Program.MouseWheelDelegater());
      Program.MainForm.Show();
      Program.MainForm.Update();
      Program.MainForm.Activate();
      if (Program.splash != null)
        ControlExtensions.Invoke(Program.splash, new Action(((Form) Program.splash).Close));
      ThreadUtility.RunInBackground("Starting Network", new ThreadStart(Program.NetworkManager.Start));
      int length;
      ThreadUtility.RunInBackground("Generate Language Pack Info", (ThreadStart) (() => length = Program.InstalledLanguages.Length));
      if (!string.IsNullOrEmpty(Program.DatabaseManager.OpenMessage))
      {
        int num1 = (int) MessageBox.Show((IWin32Window) Program.MainForm, Program.DatabaseManager.OpenMessage, TR.Messages["Attention", "Attention"], MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      if (Program.Settings.NewsStartup)
        Program.MainForm.ShowNews(false);
      Application.Run((Form) Program.MainForm);
    }

    private static IEnumerable<string> SplitIconKeys(string value)
    {
      return (IEnumerable<string>) value.Split(',', '#');
    }

    private static IEnumerable<string> SplitIconKeysWithYear(string value)
    {
      foreach (string key in Program.SplitIconKeys(value))
      {
        Match m = Program.yearRegex.Match(key);
        if (!m.Success)
        {
          yield return key;
        }
        else
        {
          int num = int.Parse(m.Groups["start"].Value);
          int end = int.Parse(m.Groups["end"].Value);
          string key2 = Program.yearRegex.Replace(key, string.Empty);
          for (int i = num; i <= end; ++i)
            yield return key2 + "(" + (object) i + ")";
          key2 = (string) null;
        }
        m = (Match) null;
      }
    }

    private static bool InitializeDatabase(int startPercent, string readDbMessage)
    {
      return Program.DatabaseManager.Open(Program.Paths.DatabasePath, Program.ExtendedSettings.DataSource, Program.ExtendedSettings.DoNotLoadQueryCaches, string.IsNullOrEmpty(readDbMessage) ? (Action<int>) null : (Action<int>) (percent => Program.StartupProgress(readDbMessage, startPercent + percent / 5)));
    }

    private static void MainFormFormClosed(object sender, FormClosedEventArgs e)
    {
      AutomaticProgressDialog.Process((Form) null, TR.Messages["ClosingComicRack", "Closing ComicRack"], TR.Messages["SaveAllData", "Saving all Data to Disk..."], 1500, new Action(Program.CleanUp), AutomaticProgressDialogOptions.None);
    }

    private static void CleanUp()
    {
      try
      {
        Program.NetworkManager.Dispose();
        SystemEvents.PowerModeChanged -= new PowerModeChangedEventHandler(Program.SystemEventsPowerModeChanged);
        Program.QueueManager.Dispose();
        Program.News.Save(Program.defaultNewsFile);
        Program.Settings.Save(Program.defaultSettingsFile);
        Program.ImagePool.Dispose();
        Program.DatabaseManager.Dispose();
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(StringUtility.Format(TR.Messages["ErrorShutDown", "There was an error shutting down the application:\r\n{0}"], (object) ex.Message), TR.Messages["Error", "Error"], MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
    }

    private static void StartLast(string[] args)
    {
      ControlExtensions.BeginInvoke(Program.MainForm, (Action) (() =>
      {
        Program.MainForm.RestoreToFront();
        try
        {
          ExtendedSettings sw = new ExtendedSettings();
          IEnumerable<string> strings = CommandLineParser.Parse((object) sw, (IEnumerable<string>) args);
          if (!string.IsNullOrEmpty(sw.ImportList))
            Program.MainForm.ImportComicList(sw.ImportList);
          if (!string.IsNullOrEmpty(sw.InstallPlugin))
            Program.MainForm.ShowPreferences(sw.InstallPlugin);
          if (!strings.Any<string>())
            return;
          strings.ForEach<string>((Action<string>) (file => Program.MainForm.OpenSupportedFile(file, true, sw.Page, true)));
        }
        catch (Exception ex)
        {
        }
      }));
    }

    private static void AutoTuneSystem()
    {
      if (!Program.ExtendedSettings.IsQueryCacheModeDefault || !EngineConfiguration.Default.IsEnableParallelQueriesDefault || !ImageDisplayControl.HardwareSettings.IsMaxTextureMemoryMBDefault || !ImageDisplayControl.HardwareSettings.IsTextureManagerOptionsDefault)
        return;
      int processorCount = Environment.ProcessorCount;
      int num = (int) (MemoryInfo.InstalledPhysicalMemory / 1024L / 1024L);
      int cpuSpeedInHz = MemoryInfo.CpuSpeedInHz;
      if (num <= 512)
        Program.ExtendedSettings.QueryCacheMode = QueryCacheMode.Disabled;
      EngineConfiguration.Default.EnableParallelQueries = processorCount > 1;
      if (cpuSpeedInHz > 2000)
        Program.ExtendedSettings.OptimizedListScrolling = false;
      ImageDisplayControl.HardwareSettings.MaxTextureMemoryMB = (num / 8).Clamp(32, 256);
      if (ImageDisplayControl.HardwareSettings.MaxTextureMemoryMB > 64)
        return;
      ImageDisplayControl.HardwareSettings.TextureManagerOptions |= TextureManagerOptions.BigTexturesAs16Bit;
      ImageDisplayControl.HardwareSettings.TextureManagerOptions &= ~TextureManagerOptions.MipMapFilter;
    }

    [DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();

    [STAThread]
    private static int Main(string[] args)
    {
      Program.SetProcessDPIAware();
      ServicePointManager.Expect100Continue = false;
      if (Program.ExtendedSettings.WaitPid != 0)
      {
        try
        {
          Process.GetProcessById(Program.ExtendedSettings.WaitPid).WaitForExit(30000);
        }
        catch
        {
        }
      }
      if (!string.IsNullOrEmpty(Program.ExtendedSettings.RegisterFormats))
        return !Program.RegisterFormats(Program.ExtendedSettings.RegisterFormats) ? 1 : 0;
      TR.ResourceFolder = (IVirtualFolder) new PackedLocalize(TR.ResourceFolder);
      Control.CheckForIllegalCrossThreadCalls = false;
      ItemMonitor.CatchThreadInterruptException = true;
      new SingleInstance("ComicRackSingleInstance", new Action<string[]>(Program.StartNew), new Action<string[]>(Program.StartLast)).Run(args);
      if (Program.Restart)
      {
        Application.Exit();
        Process.Start(Application.ExecutablePath, "-restart -waitpid " + (object) Process.GetCurrentProcess().Id + " " + Environment.CommandLine);
      }
      return 0;
    }

    private class MouseWheelDelegater : IMessageFilter
    {
      private IntPtr lastHandle;

      [DllImport("user32.dll")]
      private static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

      public bool PreFilterMessage(ref Message m)
      {
        switch (m.Msg)
        {
          case 512:
            this.lastHandle = m.HWnd;
            break;
          case 522:
          case 526:
            if (!(m.HWnd == this.lastHandle))
            {
              Program.MouseWheelDelegater.SendMessage(this.lastHandle, m.Msg, m.WParam, m.LParam);
              return true;
            }
            break;
        }
        return false;
      }
    }
  }
}
