using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Catwalk
{
    /// <summary>
    /// An ObservableCommand is a command that automatically raises ICommand.CanExecuteChanged notifications 
    /// when observable properties in it's CanExecute function change.
    /// </summary>
    public sealed class ObservableCommand : ICommand
    {
        private Func<object, Task> executeAsync;
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public ObservableCommand(Expression<Func<object, bool>> canExecute, Action<object> execute)
        {
            //Guard against bad input
            if (null == canExecute)
            {
                throw new ArgumentNullException("canExecute");
            }

            if (null == execute)
            {
                throw new ArgumentNullException("execute");
            }

            //save the execute action
            this.execute = execute;

            //walk the expression to find observable properties
            var visitor = new ObservablePropertyExpressionVisitor(typeof(object));
            visitor.NotifyAction = () => { this.RaiseCanExecuteChanged(EventArgs.Empty); };
            visitor.Visit(canExecute);
            
            //save the compiled canExecute func
            this.canExecute = canExecute.Compile();
        }

        /// <summary>
        /// Creates an ObservableCommand that can always execute.
        /// </summary>
        /// <param name="execute"></param>
        public ObservableCommand(Action<object> execute)
            : this((p) => true, execute)
        {

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
