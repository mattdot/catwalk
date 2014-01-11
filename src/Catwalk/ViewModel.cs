using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace Catwalk
{
    public abstract class ViewModel : ObservableModel
    {
        private static Lazy<bool> DesignModeEnabled = new Lazy<bool>(CheckDesignMode);


        private static bool CheckDesignMode()
        {
            var designMode = Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime");
            if (null != designMode)
            {
                //windows runtime
                var designModeProp = designMode.GetTypeInfo().GetDeclaredProperty("DesignModeEnabled");
                if(null != designModeProp)
                {
                    return (bool)designModeProp.GetValue(null);
                }
            }

            var designerProperties = Type.GetType("System.ComponentModel.DesignerProperties");
            if(null != designerProperties)
            {
                //silverlight
                var designerPropertiesProp = designerProperties.GetTypeInfo().GetDeclaredProperty("IsInDesignMode");
                if(null != designerPropertiesProp)
                {
                    return (bool)designerPropertiesProp.GetValue(null);
                }
            }

            return false;
        }

        public static bool DesignMode
        {
            get
            {
                return ViewModel.DesignModeEnabled.Value;
            }
        }

        protected ViewModel(ViewModelOptions options = null)
        {
            this.Initialize();

            if (ViewModel.DesignMode)
            {
                LoadDesignTimeData();
            }
        }

        protected virtual void Initialize()
        {

        }

        /// <summary>
        /// Use in derived classes to load data for the designer.
        /// </summary>
        protected virtual void LoadDesignTimeData()
        {

        }

        public abstract Task InitializeAsync(params object[] parameters);
    }

    public sealed class ViewModelOptions
    {
        public SynchronizationContext SyncContext { get; set; }
    }
}
