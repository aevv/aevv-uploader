using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
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

        public string UploadSync(Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            var bitmapBytes = bitmap.ToPngByteArray();

            lock (_sync)
            {
                // TODO: credential management - relog/renew
                var nvc = new NameValueCollection
                {
                    {"key", _config.ApiKey}
                };

                return UploadFile("http://aevv.net/i/api/push", bitmapBytes, "upload", "image/png", nvc);

                // TODO: Consider reading MSDN Documentation about how to use Try...Catch => http://msdn.microsoft.com/en-us/library/0yd65esw.aspx

                // TODO: json responses
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private static string UploadFile(string url, byte[] file, string paramName, string contentType, NameValueCollection nvc)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
            if(file == null) throw new ArgumentNullException(nameof(file));
            if(string.IsNullOrEmpty(paramName))throw new ArgumentNullException(nameof(paramName));
            if(string.IsNullOrEmpty(contentType)) throw new ArgumentNullException(nameof(contentType));
            if(nvc == null) throw  new ArgumentNullException(nameof(nvc));

            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            var boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webRequest.Method = "POST";
            webRequest.KeepAlive = true;
            webRequest.Credentials = CredentialCache.DefaultCredentials;

            var requestStream = webRequest.GetRequestStream();

            var formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                var formItem = string.Format(formdataTemplate, key, nvc[key]);
                var formItemBytes = Encoding.UTF8.GetBytes(formItem);
                requestStream.Write(formItemBytes, 0, formItemBytes.Length);
            }
            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);

            // TODO: filename if exists, for manual upload
            var header = $"Content-Disposition: form-data; name=\"{paramName}\"; filename=\"test\"\r\nContent-Type: {contentType}\r\n\r\n";
            var headerBytes = Encoding.UTF8.GetBytes(header);
            requestStream.Write(headerBytes, 0, headerBytes.Length);

            using (
            var byteStream = new MemoryStream(file))
            {
                var buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = byteStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }
                byteStream.Close();

                var trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                requestStream.Write(trailer, 0, trailer.Length);
                requestStream.Close();

                using (var wresp = webRequest.GetResponse())
                {
                    var responseStream = wresp.GetResponseStream();
                    using (var responseReader = new StreamReader(responseStream))
                    {
                        return responseReader.ReadToEnd();
                    }
                }
            }
        }
    }
}