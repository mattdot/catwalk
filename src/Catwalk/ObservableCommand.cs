using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Catwalk
{
    public sealed class ObservableCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public ObservableCommand(Expression<Func<object, bool>> canExecute, Action<object> execute)
        {
            if (null == execute)
            {
                throw new ArgumentNullException("execute");
            }
            this.execute = execute;

            if (null == canExecute)
            {
                this.canExecute = (o => true);
            }
            else
            {
                //todo: walk the expression to find observable properties
                this.canExecute = canExecute.Compile();
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        private void RaiseCanExecuteChanged(EventArgs e)
        {
            if (null != CanExecuteChanged)
            {
                this.CanExecuteChanged(this, e);
            }
        }
    }
}
