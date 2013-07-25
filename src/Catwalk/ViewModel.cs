using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Catwalk
{
    public abstract class ViewModel : ObservableModel
    {
        public ViewModel(ViewModelOptions options = null)
        {

        }

        public abstract Task InitializeAsync(params object[] parameters);
    }

    public sealed class ViewModelOptions
    {
        public SynchronizationContext SyncContext { get; set; }
    }
}
