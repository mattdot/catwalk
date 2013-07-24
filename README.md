catwalk
=======

Catwalk a simple portable MVVM framework.  It's the perfect foundation to showcase your Models and ViewModels.  It's designed to be a modern MVVM framework that utilizes features in C#5 to simplify creating modern MVVM apps.  Catwalk is implemented as a Portable Class Library (PCL) so it's usable on Windows Phone 8, Windows Store apps, and .NET 4.5.

Observable Properties
=======
The ObservableModel base class will automatically raise INotifyPropertyChanged.PropertyChanged events when you call SetValue.

    public class SampleModel : ObservableModel
    {
      public string FirstName
      {
        get { return GetValue<string>(); }
        set { SetValue(value); }
      }

      public string LastName
      {
        get { return GetValue<string>(); }
        set { SetValue(value); }
      }
    }
  
Calculated Properties
======
Calculated properties are read-only properties that are computed based on other observable properties.  The Observable Model base class raises a PropertyChanged event for calculated properties when the referenced properties change.

    public class SampleModel : ObservableModel
    {
      public string FirstName
      {
        get { return GetValue<string>(); }
        set { SetValue(value); }
      }
    
      public string LastName
      {
        get { return GetValue<string>(); }
        set { SetValue(value); }
      }
    
      public string FullName
      {
        get
        {
          return Calculated(() => this.FirstName + " " + this.LastName);
        }
      }
    }
    
Commands
======
Commands can be created by calling the base class' Command function. The base class creates an ObservableCommand, which will raise the ICommand.CanExecuteChanged automatically when any referenced observable properties change.

    public class SampleModel : ObservableModel
    {
      public ICommand Save
      {
        get
        {
          return Command(
              (p) => this.FirstName != null, //the condition for CanExecute 
              (p) => {
                //do some save logic here
              }
          );
        }
      }
    }

Sometimes you need to do something async in a command, so you can do it like this.

    public class SampleModel : ObservableModel
    {
      public ICommand Save
      {
        get
        {
          return Command(
            (p) => this.FirstName != null,  //the condition for CanExecute 
            async (p) => {
                //do some save logic here
            }
          );
        }
      }
    }
