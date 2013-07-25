using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Catwalk.Tests
{
    [TestClass]
    public class CalculatedTests
    {
        [TestMethod]
        public void SelfRefTest1()
        {
            var propertiesThatChanged = new List<string>();
            var model = new SelfReferencingModel();
            model.PropertyChanged += (s, e) => { propertiesThatChanged.Add(e.PropertyName); };
            var x = model.C;
            model.A = 2;
            model.B = 3;
            CollectionAssert.Contains(propertiesThatChanged, "C");

            propertiesThatChanged.Clear();
            model = new SelfReferencingModel();
            model.PropertyChanged += (s, e) => { propertiesThatChanged.Add(e.PropertyName); };
            var y = model.C;
            model.A = 2;
            model.B = 3;
            CollectionAssert.Contains(propertiesThatChanged, "C");
        }

        [TestMethod]
        public void SelfRefTest2()
        {
            var propertiesThatChanged = new List<string>();
            var model = new SelfReferencingModel(true);
            model.PropertyChanged += (s, e) => { propertiesThatChanged.Add(e.PropertyName); };
            model.A = 2;
            model.B = 3;
            CollectionAssert.Contains(propertiesThatChanged, "C");
        }

        [TestMethod]
        public void ExternalRefTest1()
        {
            List<string> notifications = new List<string>();
            var s = new SourceModel();
            var r = new RefModel(s);
            r.PropertyChanged += (sender, e) => { notifications.Add(e.PropertyName); };

            var foo = r.Reference;
            //setting source should trigger a notification for referencing property
            s.Source = 99;

            CollectionAssert.Contains(notifications, "Reference");
            /////////////////////////
            notifications.Clear();
            s = new SourceModel();
            r = new RefModel(s);
            r.PropertyChanged += (sender, e) => { notifications.Add(e.PropertyName); };

            foo = r.Reference;
            //setting source should trigger a notification for referencing property
            s.Source = 99;

            CollectionAssert.Contains(notifications, "Reference");
        }
    }

    public class SelfReferencingModel : ObservableModel
    {
        public SelfReferencingModel(bool compile = false)
        {
            if (compile)
            {
                Compile(() => this.C);
            }
        }

        public int A
        {
            get
            {
                return GetValue<int>();
            }
            set
            {
                SetValue(value);
            }
        }

        public int B
        {
            get
            {
                return GetValue<int>();
            }
            set
            {
                SetValue(value);
            }
        }

        public int C
        {
            get
            {
                return Calculated(() => this.A + this.B);
            }
        }
    }

    public class SourceModel : ObservableModel
    {
        public int Source { get { return this.GetValue<int>(); } set { this.SetValue(value); } }
    }

    public class RefModel : ObservableModel
    {
        public RefModel(SourceModel source)
        {
            this.sm = source;
        }
        SourceModel sm;

        public int Reference
        {
            get
            {
                return Calculated(() => sm.Source * 10);
            }
        }
    }
}
