using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using aevvuploader.Extensions;

namespace aevvuploader
{
    public class UploadResultHandler
    {
        private readonly IInvisibleForm _form;

        public UploadResultHandler(IInvisibleForm form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));
            _form = form;
        }

        public void HandleResult(string result)
        {
            if (string.IsNullOrEmpty(result)) throw new ArgumentNullException(nameof(result));
            // TODO: real response
            // TODO: handle key expired - attempt renew and reupload?

            var values = result.Split(',').ToIntArray();

            if (values[0] == 1)
            {
                var id = values[1];
                var url = $"http://aevv.net/i/{id}";

                _form.SuccessfulUpload(url);
            }
        }
    }
}
