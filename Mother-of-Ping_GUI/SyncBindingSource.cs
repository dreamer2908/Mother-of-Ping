using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mother_of_Ping_GUI
{
    public class SyncBindingSource : BindingSource
    {
        private SynchronizationContext syncContext;
        public SyncBindingSource()
        {
            syncContext = SynchronizationContext.Current;
        }
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (syncContext != null)
                syncContext.Send(_ => base.OnListChanged(e), null);
            else
                base.OnListChanged(e);
        }
    }
}
