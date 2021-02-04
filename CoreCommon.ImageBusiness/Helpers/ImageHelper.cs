using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace CoreCommon.ImageBusiness.Helpers
{
    public class ImageHelper
    {
        public static MemoryStream ResizeImage(Stream stream, int targetWidth, int targetHeight, bool isThumb = false, string extension = "jpg")
        {
            using (var image = Image.Load(stream))
            {
                int destWidth = image.Width, destHeight = image.Height;
                if (!isThumb)
                {
                    if (image.Width > targetWidth)
                    {
                        double percent = (double)image.Width / targetWidth;
                        destWidth = (int)(image.Width / percent);
                        destHeight = (int)(image.Height / percent);
                    }

                    if (destHeight > targetHeight)
                    {
                        double percent = (double)destHeight / targetHeight;
                        destWidth = (int)(destWidth / percent);
                        destHeight = (int)(destHeight / percent);
                    }
                }
                else
                {
                    destWidth = targetWidth;
                    destHeight = targetHeight;
                }

                if (destWidth <= 0 || destWidth <= 0)
                    return null;

                image.Mutate(x => x
                        .Resize(destWidth, destHeight)
                     );
                var memoryStream = new MemoryStream();
                IImageEncoder encoder;
                if (extension == "jpg" || extension == ".jpg")
                {
                    encoder = new JpegEncoder
                    {
                        Quality = 75
                    };
                }
                else
                {
                    encoder = new PngEncoder();
                }

                image.Save(memoryStream, encoder);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }
        }

        public static void ResizeImage(Stream stream, string path, int targetWidth, int targetHeight, bool makeBigger = false, bool noAlpha = false)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using (var image = Image.Load(stream))
            {
                int destWidth = image.Width, destHeight = image.Height;
                if (image.Width > targetWidth)
                {
                    double percent = (double)image.Width / targetWidth;
                    destWidth = (int)(image.Width / percent);
                    destHeight = (int)(image.Height / percent);
                }

                if (destHeight > targetHeight)
                {
                    double percent = (double)destHeight / targetHeight;
                    destWidth = (int)(destWidth / percent);
                    destHeight = (int)(destHeight / percent);
                }

                if (!makeBigger && (destWidth <= 0 || destWidth <= 0))
                    return;

                image.Mutate(x => x
                        .Resize(destWidth, destHeight)
                     );
                var encoder = new PngEncoder();
                if (noAlpha) encoder.ColorType = PngColorType.Rgb;
                image.Save(path, encoder);
            }
        }

        public static MemoryStream CropImage(Stream stream, int x, int y, int w, int h, string extension = "jpg")
        {
            using (var image = Image.Load(stream))
            {
                return CropImage(image, x, y, w, h, extension);
            }
        }
        
        public static MemoryStream CropImage(string url, int x, int y, int w, int h, string extension = "jpg")
        {
            using (var image = Image.Load(url))
            {
                return CropImage(image, x, y, w, h, extension);
            }
        }

        public static MemoryStream CropImage(Image image, int x, int y, int w, int h, string extension = "jpg")
        {
            image.Mutate(o => o
                    .Crop(new Rectangle(x, y, w, h))
                 );
            var memoryStream = new MemoryStream();
            IImageEncoder encoder;
            if (extension == "jpg" || extension == ".jpg")
            {
                encoder = new JpegEncoder
                {
                    Quality = 75
                };
            }
            else
            {
                encoder = new PngEncoder();
            }

            image.Save(memoryStream, encoder);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}
