// Decompiled with JetBrains decompiler
// Type: cYo.Projects.ComicRack.Engine.IO.Provider.StorageProvider
// Assembly: ComicRack.Engine, Version=1.0.5915.38775, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 50A24596-A6F6-49CD-8D4B-D3DB0D74DFBF
// Assembly location: C:\Program Files\ComicRack\ComicRack.Engine.dll

using cYo.Common.Drawing;
using cYo.Common.IO;
using cYo.Common.Localize;
using cYo.Common.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

#nullable disable
namespace cYo.Projects.ComicRack.Engine.IO.Provider
{
  public abstract class StorageProvider : FileProviderBase, IStorageProvider
  {
    public ComicInfo Store(
      IImageProvider provider,
      ComicInfo info,
      string target,
      StorageSetting setting)
    {
      if (provider == null || provider.Count <= 0)
        throw new InvalidDataException(TR.Messages["SourceComicNotValid", "Source Book is not valid"]);
      string destFileName = target;
      string str = (string) null;
      try
      {
        if (string.Equals(provider.Source, target, StringComparison.OrdinalIgnoreCase))
          target = str = EngineConfiguration.Default.GetTempFileName();
        ComicInfo comicInfo = this.OnStore(provider, info, target, setting);
        if (str != null)
          File.Copy(str, destFileName, true);
        return comicInfo;
      }
      finally
      {
        if (str != null)
          FileUtility.SafeDelete(str);
      }
    }

    public event EventHandler<StorageProgressEventArgs> Progress;

    protected bool FireProgressEvent(int percent)
    {
      if (this.Progress == null)
        return false;
      StorageProgressEventArgs e = new StorageProgressEventArgs(percent);
      this.Progress((object) this, e);
      return e.Cancel;
    }

    protected abstract ComicInfo OnStore(
      IImageProvider provider,
      ComicInfo info,
      string target,
      StorageSetting setting);

    public virtual string DefaultExtension
    {
      get => this.DefaultFileFormat.Extensions.FirstOrDefault<string>();
    }

    private static Bitmap[] GetSubPages(Bitmap bmp, bool splitDouble, bool reverseSplit)
    {
      if (!splitDouble || bmp.Height > bmp.Width)
        return new Bitmap[1]{ bmp };
      Bitmap copy1 = bmp.CreateCopy(new Rectangle(0, 0, bmp.Width / 2, bmp.Height));
      Bitmap copy2 = bmp.CreateCopy(new Rectangle(bmp.Width / 2, 0, bmp.Width / 2, bmp.Height));
      return !reverseSplit ? new Bitmap[2]{ copy1, copy2 } : new Bitmap[2]
      {
        copy2,
        copy1
      };
    }

    public static StorageProvider.PageResult[] GetImages(
      IImageProvider provider,
      ComicPageInfo cpi,
      string ext,
      StorageSetting setting,
      bool reverseSplit,
      bool createThumbnail)
    {
      byte[] numArray = (byte[]) null;
      if (setting.RemovePages && cpi.IsTypeOf(setting.RemovePageFilter))
        return new StorageProvider.PageResult[0];
      if (!string.IsNullOrEmpty(ext) && setting.PageResize == StoragePageResize.Original && (setting.PageType == StoragePageType.Original || setting.PageType == StorageProvider.GetStoragePageTypeFromExtension(ext)) && cpi.Rotation == ImageRotation.None && setting.DoublePages == DoublePageHandling.Keep && setting.ImageProcessing.IsEmpty)
      {
        numArray = provider.GetByteImage(cpi.ImageIndex);
        if (setting.PageType == StoragePageType.Jpeg)
        {
          using (MemoryStream s = new MemoryStream(numArray))
          {
            Size size;
            if (!JpegFile.GetImageSize((Stream) s, out size))
            {
              numArray = (byte[]) null;
            }
            else
            {
              cpi.ImageWidth = size.Width;
              cpi.ImageHeight = size.Height;
            }
          }
        }
        if (numArray != null && numArray.Length != 0)
          return new StorageProvider.PageResult[1]
          {
            new StorageProvider.PageResult(numArray, (byte[]) null, cpi, ext)
          };
      }
      Bitmap bitmap1 = provider.GetImage(cpi.ImageIndex);
      if (bitmap1 == null)
      {
        if (setting.IgnoreErrorPages)
          return new StorageProvider.PageResult[0];
        throw new InvalidOperationException(StringUtility.Format(TR.Messages["FailedToReadImage", "Failed to read Image {0}"], (object) (cpi.ImageIndex + 1)));
      }
      if (numArray != null && !string.IsNullOrEmpty(ext) && setting.PageResize == StoragePageResize.Original && (setting.PageType == StoragePageType.Original || setting.PageType == StorageProvider.GetStoragePageTypeFromExtension(ext)) && cpi.Rotation == ImageRotation.None && (setting.DoublePages == DoublePageHandling.Keep || bitmap1.Height <= bitmap1.Width) && !setting.ImageProcessing.IsEmpty)
      {
        bitmap1.Dispose();
        return new StorageProvider.PageResult[1]
        {
          new StorageProvider.PageResult(numArray, (byte[]) null, cpi, ext)
        };
      }
      List<StorageProvider.PageResult> pageResultList = new List<StorageProvider.PageResult>();
      Bitmap[] bitmapArray = new Bitmap[0];
      try
      {
        ImageRotation imageRotation = cpi.Rotation;
        if (setting.DoublePages == DoublePageHandling.Rotate)
        {
          Size size = bitmap1.Size.Rotate(imageRotation);
          if (size.Width > size.Height)
            imageRotation = imageRotation.RotateLeft();
        }
        if (imageRotation != ImageRotation.None)
        {
          Bitmap bitmap2 = bitmap1.Rotate(imageRotation);
          bitmap1.Dispose();
          bitmap1 = bitmap2;
        }
        bitmapArray = StorageProvider.GetSubPages(bitmap1, setting.DoublePages == DoublePageHandling.Split, reverseSplit);
        if (bitmapArray.Length > 1)
          bitmap1.Dispose();
        bitmap1 = (Bitmap) null;
        for (int index = 0; index < bitmapArray.Length; ++index)
        {
          Bitmap bitmap3 = (Bitmap) null;
          try
          {
            Bitmap bitmap4 = bitmapArray[index];
            int width = setting.DontEnlarge ? Math.Min(bitmap4.Width, setting.PageWidth) : setting.PageWidth;
            int height = setting.DontEnlarge ? Math.Min(bitmap4.Height, setting.PageHeight) : setting.PageHeight;
            switch (setting.PageResize)
            {
              case StoragePageResize.WidthHeight:
                bitmap3 = bitmap4.Scale(new Size(width, height), setting.Resampling, PixelFormat.Format24bppRgb);
                break;
              case StoragePageResize.Width:
                if (setting.DoublePages == DoublePageHandling.AdaptWidth && bitmap4.Width > bitmap4.Height)
                  width *= 2;
                bitmap3 = bitmap4.Scale(new Size(width, 0), setting.Resampling, PixelFormat.Format24bppRgb);
                break;
              case StoragePageResize.Height:
                bitmap3 = bitmap4.Scale(new Size(0, height), setting.Resampling, PixelFormat.Format24bppRgb);
                break;
            }
            if (bitmap3 != null)
            {
              bitmapArray[index].Dispose();
              bitmap4 = bitmapArray[index] = bitmap3;
              bitmap3 = (Bitmap) null;
            }
            cpi.ImageWidth = bitmap4.Width;
            cpi.ImageHeight = bitmap4.Height;
            if (!setting.ImageProcessing.IsEmpty)
            {
              try
              {
                Bitmap bitmap5 = bitmap4;
                bitmapArray[index] = bitmap4 = bitmap4.CreateAdjustedBitmap(setting.ImageProcessing, PixelFormat.Format24bppRgb, true);
                bitmap5.Dispose();
              }
              catch
              {
              }
            }
            StoragePageType storagePageType = setting.PageType != StoragePageType.Original ? setting.PageType : StorageProvider.GetStoragePageTypeFromExtension(ext);
            if ((storagePageType == StoragePageType.Bmp || storagePageType == StoragePageType.Png) && bitmap4.PixelFormat != PixelFormat.Format24bppRgb)
            {
              Bitmap bitmap6 = bitmap4;
              bitmapArray[index] = bitmap4 = bitmap4.CreateCopy(PixelFormat.Format24bppRgb);
              if (bitmap4 != bitmap6)
                bitmap6.Dispose();
            }
            byte[] data;
            switch (storagePageType)
            {
              case StoragePageType.Png:
                data = bitmap4.ImageToBytes(ImageFormat.Png, 24);
                ext = ".png";
                break;
              case StoragePageType.Gif:
                data = bitmap4.ImageToBytes(ImageFormat.Gif, 8);
                ext = ".gif";
                break;
              case StoragePageType.Tiff:
                data = bitmap4.ImageToBytes(ImageFormat.Tiff, 24);
                ext = ".tif";
                break;
              case StoragePageType.Bmp:
                data = bitmap4.ImageToBytes(ImageFormat.Bmp, 24);
                ext = ".bmp";
                break;
              case StoragePageType.Djvu:
                data = DjVuImage.ConvertToDjVu(bitmap4);
                ext = ".djvu";
                break;
              case StoragePageType.Webp:
                data = WebpImage.ConvertoToWebp(bitmap4, setting.PageCompression);
                ext = ".webp";
                break;
              default:
                data = bitmap4.ImageToBytes(ImageFormat.Jpeg, 24, setting.PageCompression);
                ext = ".jpg";
                break;
            }
            cpi.ImageFileSize = data.Length;
            pageResultList.Add(new StorageProvider.PageResult(data, createThumbnail ? StorageProvider.PageResult.CreateThumbnail(bitmap4, setting) : (byte[]) null, cpi, ext));
          }
          finally
          {
            bitmap3?.Dispose();
          }
        }
      }
      finally
      {
        for (int index = 0; index < bitmapArray.Length; ++index)
        {
          if (bitmapArray[index] != null)
            bitmapArray[index].Dispose();
        }
        bitmap1?.Dispose();
      }
      return pageResultList.ToArray();
    }

    private static StoragePageType GetStoragePageTypeFromExtension(string ext)
    {
      switch ((ext ?? string.Empty).ToLower())
      {
        case ".bmp":
          return StoragePageType.Bmp;
        case ".djvu":
          return StoragePageType.Djvu;
        case ".png":
          return StoragePageType.Png;
        case ".tif":
        case ".tiff":
          return StoragePageType.Tiff;
        case ".webp":
          return StoragePageType.Webp;
        default:
          return StoragePageType.Jpeg;
      }
    }

    public class PageResult
    {
      private readonly ComicPageInfo info;
      private readonly string extension;
      private byte[] data;
      private byte[] thumbnailData;
      private string dataStoragePath;

      public PageResult(byte[] data, byte[] thumbnailData, ComicPageInfo info, string extension)
      {
        this.data = data;
        this.thumbnailData = thumbnailData;
        this.info = info;
        this.extension = extension;
      }

      public ComicPageInfo Info => this.info;

      public string Extension => this.extension;

      public byte[] Data => this.data;

      public byte[] ThumbnailData => this.thumbnailData;

      public Bitmap GetImage()
      {
        return BitmapExtensions.BitmapFromBytes(DjVuImage.ConvertToJpeg(WebpImage.ConvertToJpeg(this.data)));
      }

      public byte[] GetThumbnailData(StorageSetting setting)
      {
        if (this.ThumbnailData != null)
          return this.thumbnailData;
        using (Bitmap image = this.GetImage())
          return StorageProvider.PageResult.CreateThumbnail(image, setting);
      }

      public static byte[] CreateThumbnail(Bitmap bmp, StorageSetting setting)
      {
        using (Bitmap bitmap = bmp.Scale(setting.ThumbnailSize))
          return setting.PageType == StoragePageType.Webp ? WebpImage.ConvertoToWebp(bitmap, setting.PageCompression) : bitmap.ImageToJpegBytes(setting.PageCompression);
      }

      public void Store()
      {
        if (this.dataStoragePath != null || this.data == null)
          return;
        this.dataStoragePath = Path.GetTempFileName();
        try
        {
          File.WriteAllBytes(this.dataStoragePath, this.data);
          this.data = (byte[]) null;
        }
        catch
        {
          FileUtility.SafeDelete(this.dataStoragePath);
          this.dataStoragePath = (string) null;
        }
      }

      public void Restore()
      {
        if (this.dataStoragePath == null)
          return;
        try
        {
          this.data = File.ReadAllBytes(this.dataStoragePath);
        }
        finally
        {
          this.DeleteDataStorage();
        }
      }

      public void Clear()
      {
        this.data = (byte[]) null;
        this.thumbnailData = (byte[]) null;
        this.DeleteDataStorage();
      }

      private void DeleteDataStorage()
      {
        if (this.dataStoragePath == null)
          return;
        FileUtility.SafeDelete(this.dataStoragePath);
        this.dataStoragePath = (string) null;
      }
    }
  }
}
