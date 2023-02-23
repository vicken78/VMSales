using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMSales.Logic;

namespace VMSales.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
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

    //INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
              if (PropertyChanged == null) return;
              PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    
    /// <summary>
    /// Database setting, change dbfilepath to TEST or PROD
    /// </summary>
    /// <returns></returns>
    protected static string SetDataBase()
        {
            string dbfilepath = "TEST";

            if (dbfilepath == "PROD")
            {

                const string filepath = "C:\\Users\\Vicken\\source\\repos\\VMSales\\VMSales\\sales.db";
                return filepath;
            }
            else
            {
                const string filepath = "C:\\Users\\Vicken\\source\\repos\\testsqllite\\testsqllite\\db\\testsales2.db";
                return filepath;
            }
        }

        public static IDatabaseProvider getprovider()
        {
            string filepath = SetDataBase();
            SQLiteDatabase dataBaseProvider = new SQLiteDatabase(filepath);
            return dataBaseProvider;
        }
    }
}