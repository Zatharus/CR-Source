// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.DjVuImage
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using cYo.Common.IO;
using cYo.Common.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  public static class DjVuImage
  {
    private static readonly string encodeExe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources\\c44.exe");
    private static readonly string unpackExe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources\\ddjvu.exe");

    private static Size SizeLimit => EngineConfiguration.Default.DjVuSizeLimit;

    public static byte[] ConvertToJpeg(byte[] data)
    {
      if (!DjVuImage.IsDjvu(data))
        return data;
      string tempFileName = Path.GetTempFileName();
      try
      {
        File.WriteAllBytes(tempFileName, data);
        using (Bitmap bitmap = DjVuImage.GetBitmap(tempFileName))
          return bitmap.ImageToJpegBytes();
      }
      catch (Exception ex)
      {
        return data;
      }
      finally
      {
        FileUtility.SafeDelete(tempFileName);
      }
    }

    public static byte[] ConvertToDjVu(Bitmap bmp)
    {
      string tempFileName1 = Path.GetTempFileName();
      string tempFileName2 = Path.GetTempFileName();
      try
      {
        DjVuImage.SaveDjVu(bmp, tempFileName2);
        return File.ReadAllBytes(tempFileName2);
      }
      finally
      {
        FileUtility.SafeDelete(tempFileName1);
        FileUtility.SafeDelete(tempFileName2);
      }
    }

    public static void SaveDjVu(Bitmap bmp, string encodedFile)
    {
      string tempFileName = Path.GetTempFileName();
      try
      {
        bmp.SaveJpeg(tempFileName);
        if (ExecuteProcess.Execute(DjVuImage.encodeExe, string.Format("\"{0}\" \"{1}\"", (object) tempFileName, (object) encodedFile), ExecuteProcess.Options.None).ExitCode != 0)
          throw new InvalidDataException();
      }
      catch (Exception ex)
      {
        FileUtility.SafeDelete(encodedFile);
        throw;
      }
      finally
      {
        FileUtility.SafeDelete(tempFileName);
      }
    }

    public static Bitmap GetBitmap(string source, int index = 0)
    {
      string tempFileName = Path.GetTempFileName();
      try
      {
        if (ExecuteProcess.Execute(DjVuImage.unpackExe, string.Format("-format=tiff -size={0}x{1} -page={2} \"{3}\" \"{4}\"", (object) DjVuImage.SizeLimit.Width, (object) DjVuImage.SizeLimit.Height, (object) (index + 1), (object) source, (object) tempFileName), ExecuteProcess.Options.None).ExitCode != 0)
          throw new FileLoadException();
        return BitmapExtensions.BitmapFromFile(tempFileName);
      }
      finally
      {
        FileUtility.SafeDelete(tempFileName);
      }
    }

    public static bool IsDjvu(string uri)
    {
      return uri.EndsWith(".djvu", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsDjvu(byte[] sig)
    {
      if (sig == null)
        return false;
      try
      {
        return sig[0] == (byte) 65 && sig[1] == (byte) 84 && sig[2] == (byte) 38 && sig[3] == (byte) 84 && sig[4] == (byte) 70;
      }
      catch (Exception ex)
      {
        return false;
      }
    }
  }
}
