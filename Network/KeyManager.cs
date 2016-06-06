using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace aevvuploader.Network
{
    class KeyManager
    {
        public Config Load()
        {
            var config = Config.Load("config.json");

            using (var webClient = new WebClient())
            {
                var result = webClient.UploadValues(config.BaseUrl + "api/check", new NameValueCollection
                {
                    {"key", config.ApiKey}
                });

                int isKeyValid = int.Parse(Encoding.UTF8.GetString(result));

                if (isKeyValid == 1)
                {
                    result = webClient.UploadValues(config.BaseUrl + "api/renew", new NameValueCollection
                    {
                        {"username", config.Username},
                        {"password", config.Password }
                    });

                    var newKey = Encoding.UTF8.GetString(result);
                    config.ApiKey = newKey;
                }
            }

            return config;
        }

        // TODO: Proper post class
    }
}
