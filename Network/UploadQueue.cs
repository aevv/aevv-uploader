using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aevvuploader.Network
{
    public class UploadQueue
    {
        private readonly Queue<Bitmap> _queue;
        private readonly object _sync;
        private readonly ServerApi _api;
        private readonly UploadResultHandler _handler;

        public UploadQueue(ServerApi api, UploadResultHandler handler)
        {
            if (api == null) throw new ArgumentNullException(nameof(api));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _sync = new object();
            _queue = new Queue<Bitmap>();
            _api = api;
            _handler = handler;

            Task.Factory.StartNew(Upload);
        }

        public void QueueImage(Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            lock (_sync)
            {
                _queue.Enqueue(bitmap);
            }
        }

        private void Upload()
        {
            Bitmap bitmap = null;
            // TODO: Cancellation
            while (true)
            {
                try
                {
                    lock (_sync)
                    {
                        if (_queue.Count == 0)
                        {
                            Thread.Sleep(100);
                            continue;
                        }

                        bitmap = _queue.Dequeue();
                    }

                    var result = _api.UploadSync(bitmap);

                    _handler.HandleResult(result);

                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    // If we fail, try again
                    _queue.Enqueue(bitmap);
                }
            }
        }
    }
}