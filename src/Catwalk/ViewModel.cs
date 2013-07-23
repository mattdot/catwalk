using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catwalk
{
    public abstract class ViewModel : ObservableModel
    {
        public abstract Task InitializeAsyc(params object[] parameters);
    }
}
