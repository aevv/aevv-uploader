using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace aevvuploader
{
    public class Config
    {
        // TODO: Refactor so this class can be made immutable
        public string ApiKey { get; set; }
        public string Username { get; set; }

        // TODO: encrypt/hide in some way from disk - maybe sha256 this before send and assert all passwords handed to server
        // are already sha256'd
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
