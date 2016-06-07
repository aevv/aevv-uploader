using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using aevvuploader.Extensions;

namespace aevvuploader.Network
{
    public class ImageUploader
    {
        private readonly object _sync = new object();
        private readonly Config _config;

        public ImageUploader(Config config)
        {
            _config = config;
        }

        public string UploadSync(Bitmap bitmap, CookieContainer container)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            var bitmapBytes = bitmap.ToPngByteArray();

            lock (_sync)
            {
                return Upload(_config.BaseUrl + "api/Upload", bitmapBytes, container).Result;

            }
        }

        public async Task<string> Upload(string url, byte[] image, CookieContainer cookies)
        {
            using (var handler = new HttpClientHandler { CookieContainer = cookies })
            using (var client = new HttpClient(handler))
            {
                using (var content =
                    new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                {
                    content.Add(new StreamContent(new MemoryStream(image)), "data", "upload.png");

                    using (var message = await client.PostAsync(url, content))
                    {
                        return await message.Content.ReadAsStringAsync();
                    }
                }
            }
        }
    }
}