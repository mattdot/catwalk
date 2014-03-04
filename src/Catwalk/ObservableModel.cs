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

    public class ModelPropertyInfo
    {
        private List<ModelProperty> depends = new List<ModelProperty>();
        public IEnumerable<ModelProperty> Dependencies
        {
            get
            {
                return this.depends.AsEnumerable();
            }
        }

        internal bool AddDependency(ModelProperty prop)
        {
            if (!this.Dependencies.Contains(prop))
            {
                this.depends.Add(prop);

                return true;
            }

            return false;
        }

        public object DefaultValue { get; set; }
    }

    public abstract class ObservableModel : INotifyPropertyChanged
    {
        //metadata is stored statically so we don't duplicate per instance.
        private static IDictionary<ModelProperty, ModelPropertyInfo> metadata;

        //a dictionary that stores all the values of the properties.
        private IDictionary<string, object> values;
        private ObservablePropertyExpressionVisitor visitor;
        private Type type;

        static ObservableModel()
        {
            ObservableModel.metadata = new Dictionary<ModelProperty, ModelPropertyInfo>();
        }

        private static ModelPropertyInfo EnsurePropertyInfo(ModelProperty property, Func<ModelPropertyInfo> factory = null)
        {
            if (null == factory)
            {
                factory = () => new ModelPropertyInfo();
            }

            ModelPropertyInfo meta;
            if (!ObservableModel.metadata.TryGetValue(property, out meta))
            {
                lock (ObservableModel.metadata)
                {
                    if (!ObservableModel.metadata.TryGetValue(property, out meta))
                    {
                        meta = ObservableModel.metadata[property] = factory();
                    }
                }
            }

            return meta;
        }

        private static void SubscribeToProperty(ModelProperty publisher, ModelProperty subscriber)
        {
            var info = ObservableModel.EnsurePropertyInfo(publisher);
            info.AddDependency(subscriber);
        }

        protected ObservableModel()
        {
            //this.properties = new Dictionary<string, ModelProperty>();
            this.values = new Dictionary<string, object>();
            this.type = this.GetType();

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
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        protected void SetValue<T>(T value, [CallerMemberName]string propertyName = "")
        {
            this.values[propertyName] = value;

            RaisePropertyChanged(new ModelProperty(propertyName, typeof(T), this.type));
        }

        protected void RaisePropertyChanged(ModelProperty property)
        {
            if (null != this.PropertyChanged)
            {
                //raise the property changed event for the property
                this.PropertyChanged(this, new PropertyChangedEventArgs(property.Name));

                //raise the property changed event for all subscribed properties
                var propMeta = ObservableModel.EnsurePropertyInfo(property);
                foreach (var dep in propMeta.Dependencies)
                {
                    RaisePropertyChanged(dep);
                }
            }
        }

        /// <summary>
        /// Used in property getters
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="propertyName">The name of the property.  This should be automatically set by the compiler.</param>
        /// <returns>Returns the current value of the property, or the default value.</returns>
        protected T GetValue<T>([CallerMemberName]string propertyName = "")
        {
            object value;
            if (!this.values.TryGetValue(propertyName, out value))
            {
                return default(T);
            }

            return (T)value;
        }

        /// <summary>
        /// Calculated properties are read-only properties that are computed based on other 
        /// observable properties. The Observable Model base class raises a PropertyChanged 
        /// event for calculated properties when the referenced properties change.
        /// </summary>
        /// <typeparam name="T">The type of the calculated result</typeparam>
        /// <param name="expr">A lambda expression that returns the value of the property.</param>
        /// <param name="propertyName">The name of the property. This should be filled in automatically by the compiler.</param>
        /// <returns>The calculated value of the property</returns>
        /// <example>
        /// <code>
        /// public string FullName
        /// {
        ///     get
        ///     {
        ///         return Calculated(() => this.FirstName + " " + this.LastName);
        ///     }
        /// }
        /// </code></example>
        protected T Calculated<T>(Expression<Func<T>> expr, [CallerMemberName]string propertyName = "")
        {
            var prop = new ModelProperty(propertyName, typeof(T), this.type);

            ObservableModel.EnsurePropertyInfo(prop);

            object f;
            if (!this.values.TryGetValue(prop.Name, out f))
            {
                var visitor = new ObservablePropertyExpressionVisitor(this.GetType());
                visitor.NotifyAction = () => { RaisePropertyChanged(prop); };
                visitor.Visit(expr);
                var graph = visitor.Properties.ToList();
                foreach (var item in graph)
                {
                    ObservableModel.SubscribeToProperty(item, prop);
                }

                //cache the compiled version of this property
                f = this.values[prop.Name] = expr.Compile();
            }

            return ((Func<T>)f)();
        }

        protected ICommand Command(Action<object> execute, [CallerMemberName]string propertyName = "")
        {
            return Command((p) => true, execute, propertyName);
        }

        protected ICommand Command(Expression<Func<object, bool>> canExecute, Action<object> execute, [CallerMemberName]string propertyName = "")
        {
            object cmd;
            if (!this.values.TryGetValue(propertyName, out cmd))
            {
                cmd = this.values[propertyName] = new ObservableCommand(canExecute, execute);
            }

            return (ICommand)cmd;
        }

        protected void Compile<T>(Func<T> propertyExpression)
        {
            propertyExpression();
        }
    }
}
