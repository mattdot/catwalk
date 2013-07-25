using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Input;

namespace Catwalk.Tests
{
    [TestClass]
    public class CommandTests
    {
        [TestMethod]
        public void CanExecuteChangedTest()
        {
            var model = new CommandTestModel();
            model.Save.CanExecuteChanged += (sender, e) =>
            {
                model.Save.Execute(null);
            };

            model.CanSave = true;

            Assert.IsTrue(model.SaveCount > 0);
        }
    }


    public class CommandTestModel : ObservableModel
    {
        public int SaveCount { get; set; }

        public bool CanSave
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }


        public ICommand Save
        {
            get
            {
                return Command(
                  (p) => this.CanSave, //CanExecute condition
                  (p) =>
                  {
                      //Execute command
                      this.SaveCount++;
                  }
                 );
            }
        }

    }
}
