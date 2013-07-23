catwalk
=======

Catwalk a simple MVVM framework.  It's the perfect foundation to showcase your Models and ViewModels.  It's designed to be a modern MVVM framework that utilizes features in C#5 to simplify creating modern MVVM apps.  Catwlk is implemented as a Portable Class Library (PCL) so it's usable on Windows Phone, Windows Store apps, and .NET 4.5.

Observable Properties
=======
    ///<summary>
    /// A basic model that will raise INotifyPropertyChanged.PropertyChanged events
    ///</summary>
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
  
calculated properties
======
Calculated properties are read-only properties that are computed based on other observable properties.  A calculated property raises a PropertyChanged event when the underlying properties it references change.

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
    
commands
======
Commands can be created, and their CanExecute state will automatically change when underlying observable properties change.

    public class SampleModel : ObservableModel
    {
      public ICommand Save
      {
        get
        {
          return Command(() => this.FirstName != null, () => {
            //do some save logic here
          });
        }
      }
    }
