using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace ConBrain.Extensions
{
    public static class IFormFileExtension
    {
        /// <summary>
        /// Преобразует в изображение
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Image From64bitToImage(this IFormFile self)
        {
            using var stream = self.OpenReadStream();
            var memorystream = new MemoryStream();
            var bytes = new byte[self.Length];

            var sInt = sizeof(int);
            for (int i = 0; i < sizeof(long) / sInt; i++)
                if (stream.Read(bytes, i, (int)(self.Length << i * sInt)) == self.Length + i * sInt)
                    break;
            bytes = Convert.FromBase64String(Encoding.Default.GetString(bytes));

            memorystream.Write(bytes);
            memorystream.Position = 0;
            return Image.FromStream(memorystream);
        }

        /// <summary>
        /// Преобразует в изображение
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public async static Task<Image> From64bitToImageAsync(this IFormFile self)
        {
            using var stream = self.OpenReadStream();
            var memorystream = new MemoryStream();
            var bytes = new byte[self.Length];

            var sInt = sizeof(int);
            for (int i = 0; i < sizeof(long) / sInt; i++)
                if (await stream.ReadAsync(bytes, i, (int)(self.Length << i * sInt)) == self.Length + i * sInt)
                    break;
            bytes = Convert.FromBase64String(Encoding.Default.GetString(bytes));
            memorystream.Write(bytes);
            memorystream.Position = 0;
            return Image.FromStream(memorystream);
        }

        private static List<string> _imageTypes = new()
        {
            "image/jpg", "image/jpeg", "image/pjpeg", "image/gif", "image/x-png", "image/png"
        };

        public static bool CanImage(this IFormFile form)
        {
            return _imageTypes.Contains(form.ContentType.ToLower());
        }
    }
}
