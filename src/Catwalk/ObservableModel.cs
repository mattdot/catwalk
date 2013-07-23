using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Catwalk
{
    public delegate T CalculateFunc<T>(params object[] values);

    public abstract class ObservableModel : INotifyPropertyChanged
    {
        private static HashSet<ModelProperty> propertySet;

        private IDictionary<string, ModelProperty> properties;
        private IDictionary<string, object> values;
        private ObservablePropertyExpressionVisitor visitor;

        static ObservableModel()
        {
            propertySet = new HashSet<ModelProperty>();
        }

        public ObservableModel()
        {
            this.properties = new Dictionary<string, ModelProperty>();
            this.values = new Dictionary<string, object>();

            this.visitor = new ObservablePropertyExpressionVisitor(this.GetType());
        }

        /// <summary>
        /// Event is fired when the property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Used in property setters to set the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void SetValue<T>(T value, [CallerMemberName]string key = "")
        {
            ModelProperty prop;
            EnsureProperty<T>(key, out prop);

            this.values[prop.Name] = value;

            RaisePropertyChanged(prop);
        }

        protected void RaisePropertyChanged(ModelProperty prop)
        {
            if (null != this.PropertyChanged)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop.Name));

                foreach (var dep in prop.Dependencies)
                {
                    RaisePropertyChanged(dep);
                }
            }
        }

        /// <summary>
        /// Used in property getters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        protected T GetValue<T>([CallerMemberName]string key = "")
        {
            ModelProperty prop;
            EnsureProperty<T>(key, out prop);

            object value;
            if (!this.values.TryGetValue(key, out value))
            {
                return default(T);
            }

            return (T)value;
        }

        protected T Calculated<T>(Expression<Func<T>> expr, [CallerMemberName]string key = "")
        {
            ModelProperty prop;

            if (!EnsureProperty<T>(key, out prop))
            {
                var visitor = new ObservablePropertyExpressionVisitor(this.GetType());
                visitor.NotifyAction = () => { RaisePropertyChanged(prop); };
                visitor.Visit(expr);
                var graph = visitor.Properties.ToList();
                foreach (var item in graph)
                {
                    ModelProperty dp;
                    EnsureProperty<T>(item.Name, out dp);
                    dp.AddDependency(prop);
                }
            }

            object f;
            if (!this.values.TryGetValue(prop.Name, out f))
            {
                //cache the compiled version of this property
                f = this.values[prop.Name] = expr.Compile();
            }

            return ((Func<T>)f)();
        }

        protected ICommand Command(Action<object> execute, [CallerMemberName]string key = "")
        {
            return Command(null, execute, key);
        }

        protected ICommand Command(Expression<Func<object, bool>> canExecute, Action<object> execute, [CallerMemberName]string key = "")
        {
            object cmd;
            if (!this.values.TryGetValue(key, out cmd))
            {
                cmd = this.values[key] = new ObservableCommand(canExecute, execute);
            }

            return (ICommand)cmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="prop"></param>
        /// <returns>true if property existed previous to call; otherwise false</returns>
        private bool EnsureProperty<T>(string key, out ModelProperty prop)
        {
            if (!this.properties.TryGetValue(key, out prop))
            {
                prop = new ModelProperty(key, typeof(T), this.GetType());
                this.properties.Add(key, prop);

                return false;
            }

            return true;
        }

    }
}
