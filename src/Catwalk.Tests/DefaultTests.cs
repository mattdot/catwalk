using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace Catwalk.Tests
{
    [TestClass]
    public class DefaultTests
    {
        [TestMethod]
        public void DefaultValueTest()
        {
            var model = new DefaultModel();

            Assert.AreEqual<string>("Hello", model.Foo);
        }
    }

    public class DefaultModel : ObservableModel
    {
        public DefaultModel()
        {
            Compile(() => new[] { this.Fooc, this.Foo });
        }

        [DefaultValue("Hello")]
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
