using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Catwalk.Tests
{
    [TestClass]
    public class DefaultTests
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
    }

    public class DefaultModel : ObservableModel
    {
        public DefaultModel()
        {
            Compile(() => this.Fooc);
        }

        public string Foo
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public string Fooc
        {
            get
            {
                return Calculated(() => this.Foo);
            }
        }

    }
}
