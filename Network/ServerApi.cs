using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using aevvuploader.ApiModels;
using Newtonsoft.Json;

namespace aevvuploader.Network
{
    public class ServerApi
    {
        private readonly ImageUploader _uploader;
        private readonly Config _config;
        private CookieContainer _container;

        private bool _loggedIn;

        public ServerApi(Config config)
        {
            _config = config;
            _uploader = new ImageUploader(_config);
            _container = new CookieContainer();
            _loggedIn = false;
        }

        public string UploadSync(Bitmap bitmap)
        {
            EnsureLogin();
            return _uploader.UploadSync(bitmap, _container);
        }

        public bool LogIn()
        {
            using (var wc = new CookieWebClient(_container))
            {
                var loginResult = wc.UploadValues($"{_config.BaseUrl}Account/ApiLogin", new NameValueCollection
                    {
                        {"Email", _config.Username},
                        {"Password", _config.Password},
                        {"RememberMe", "false"}
                    });

                var result = JsonConvert.DeserializeObject<LoginResult>(Encoding.UTF8.GetString(loginResult));
                if (result.Code == 0)
                {
                    return true;
                }
                return false;
            }
        }

        private void EnsureLogin()
        {
            if (!LogIn())
            {
                throw new Exception("Failed to login");
            }
        }
    }
}
