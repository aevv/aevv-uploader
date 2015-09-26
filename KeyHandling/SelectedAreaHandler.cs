using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aevvuploader.KeyHandling
{
    class SelectedAreaHandler : IInputHandler
    {
        public void Handle(IVisibleForm form)
        {
            throw new NotImplementedException();
        }

        public Keys TriggerKey => Keys.D4;
    }
}
