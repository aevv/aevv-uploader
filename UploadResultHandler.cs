using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using aevvuploader.ApiModels;
using aevvuploader.Extensions;
using Newtonsoft.Json;

namespace aevvuploader
{
    public class UploadResultHandler
    {
        private readonly IInvisibleForm _form;
        private readonly Config _config;

        public UploadResultHandler(IInvisibleForm form, Config config)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            if (config == null) throw new ArgumentNullException(nameof(config));

            _form = form;
            _config = config;
        }

        public void HandleResult(string result)
        {
            if (string.IsNullOrEmpty(result)) throw new ArgumentNullException(nameof(result));

            var uploadResult = JsonConvert.DeserializeObject<UploadResult>(result);
            if (uploadResult.Code == 0)
            {
                _form.SuccessfulUpload($"{_config.BaseUrl}i/{uploadResult.UploadId}");
            }
        }
    }
}
