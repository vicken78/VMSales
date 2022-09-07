namespace VMSales.BaseType
{
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Input;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Abstract base class for all models.
    /// </summary>
    public class BaseModel : INotifyPropertyChanged
    {
   
        #region events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
       // public BaseModel()
       // {
       //     InitCommands();
       // }
        #endregion

        #region methods

        /// <summary>
        /// Override this method in derived types to initialize command logic.
        /// </summary>
    //    protected virtual void InitCommands()
    //    {
    //    }
       
        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName">The name of the property which value has changed.</param>
        public void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
        #endregion
    }
}