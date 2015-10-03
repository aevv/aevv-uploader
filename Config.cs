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
