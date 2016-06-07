using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace aevvuploader
{
    public class Config
    {
        public string ApiKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string BaseUrl { get; set; }

        public void Save(string fileName)
        {
            using (var writer = new StreamWriter(fileName, false))
            {
                writer.Write(JsonConvert.SerializeObject(this));
            }
        }

        public static Config Load(string fileName)
        {
            if (!File.Exists(fileName))
                return new Config();

            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(fileName));
        }
    }
}
