using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aevvuploader.ApiModels
{
    class LoginResult
    {
        public string Message { get; set; }
        public int Code { get; set; }
        public string UserName { get; set; }
    }
}
