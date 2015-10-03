using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using aevvuploader.Extensions;

namespace aevvuploader.Network
{
    public class ImageUploader
    {
        private readonly object _sync = new object();

        public string UploadSync(Bitmap bitmap)
        {
            var bitmapBytes = bitmap.ToPngByteArray();

            lock (_sync)
            {
                // TODO: credential management
                var nvc = new NameValueCollection
                {
                    {"key", "aevv"}
                };

                // TODO: config or whatever
                try
                {
                    return UploadFile("http://aevv.net/i/api/push", bitmapBytes, "upload", "image/png", nvc);
                }
                catch (Exception)
                {
                    // TODO: logging
                }

                // TODO: json responses
                return "0,0";
            }
        }

        private string UploadFile(string url, byte[] file, string paramName, string contentType, NameValueCollection nvc)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webRequest.Method = "POST";
            webRequest.KeepAlive = true;
            webRequest.Credentials = CredentialCache.DefaultCredentials;

            Stream requestStream = webRequest.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                string formItem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formItemBytes = Encoding.UTF8.GetBytes(formItem);
                requestStream.Write(formItemBytes, 0, formItemBytes.Length);
            }
            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);

            // TODO: filename if exists
            string header = $"Content-Disposition: form-data; name=\"{paramName}\"; filename=\"test\"\r\nContent-Type: {contentType}\r\n\r\n";
            byte[] headerBytes = Encoding.UTF8.GetBytes(header);
            requestStream.Write(headerBytes, 0, headerBytes.Length);

            var byteStream = new MemoryStream(file);
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = byteStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                requestStream.Write(buffer, 0, bytesRead);
            }
            byteStream.Close();

            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            requestStream.Write(trailer, 0, trailer.Length);
            requestStream.Close();

            using (var wresp = webRequest.GetResponse())
            {
                Stream responseStream = wresp.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);
                return responseReader.ReadToEnd();
            }
        }
    }
}