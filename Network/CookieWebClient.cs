using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace aevvuploader.Network
{
    class CookieWebClient : WebClient
    {
        public CookieContainer CookieContainer { get; }

        public CookieWebClient() : this(new CookieContainer())
        {

        }

        public CookieWebClient(CookieContainer container)
        {
            CookieContainer = container;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            if (request == null) return base.GetWebRequest(address);

            request.CookieContainer = CookieContainer;
            return request;
        }
    }
}
