using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VMSales.Logic;

namespace VMSales.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {

        // Tracks whether any property in the ViewModel has changed
        private bool _isInitialized = false;
        private bool _isDirty = false;
        public bool IsDirty
        {
            get => _isDirty;
            protected set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    RaisePropertyChanged(nameof(IsDirty));
                }
            }
        }

        //Tracks initialization
        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return;

            field = value;

            if (_isInitialized)
            {
                IsDirty = true;
                FontColor = Brushes.Red;
            }

            RaisePropertyChanged(propertyName);
        }

        public void Initialize()
        {
            _isInitialized = true;
        }

        // Tracks the font color for modified properties
        private Brush _FontColor = Brushes.Black; // Default color
        public Brush FontColor
        {
            get => _FontColor;
            protected set
            {
                if (_FontColor != value)
                {
                    _FontColor = value;
                    RaisePropertyChanged(nameof(FontColor));
                }
            }
        }

     /*     protected virtual void RaiseAnyPropertyChanged(object sender, PropertyChangedEventArgs e)
          {
              //IsDirty = true;
              //FontColor = Brushes.Red;
          }
     */

        // INotifyDateErrorInfo
        private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();
        public bool HasErrors => _errorsByPropertyName.Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName) ?
                _errorsByPropertyName[propertyName] : null;
        }
        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged == null) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    
        public void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
                _errorsByPropertyName[propertyName] = new List<string>();

            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        protected static string SetDataBase()
        {
            string filePath = ConfigurationManager.AppSettings["DatabaseFilePath"];

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                //test
                //filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testsales2.db");
                //prod
                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sales.db");
            }

            // Check if the file exists, and throw an exception if it doesn't
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Database file not found.", filePath);
            }

            return filePath;
        }

        public static IDatabaseProvider getprovider()
        {
            string filepath = SetDataBase();
            SQLiteDatabase dataBaseProvider = new SQLiteDatabase(filepath);
            return dataBaseProvider;
        }

/*        public BaseViewModel()
        {
              PropertyChanged += RaiseAnyPropertyChanged;
        }
*/
    }
}