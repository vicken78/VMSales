using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VMSales.Logic;

namespace VMSales.ViewModels
{
    public abstract class BaseViewModel : PropertyChangedBase
    {
        public string Action { get; set; }  // This will hold Insert, Update, Delete actions
        private Brush _FontColor = Brushes.Black;
        public Brush FontColor
        {
            get => _FontColor;
            set
            {
                if (_FontColor != value)
                {
                    _FontColor = value;
                }
                NotifyOfPropertyChange(() => FontColor);
            }
        }

        // keep for now, eventually remove
        /*     public event PropertyChangedEventHandler PropertyChanged;

             protected void //RaisePropertyChanged([CallerMemberName] string propertyName = null)
             {
                 if (PropertyChanged != null)
                 {
                     PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                 }
             }
        */
        
        protected static string SetDataBase()
        {

            // See app.config for database setting
            string filePath = ConfigurationManager.AppSettings["DatabaseFilePath"];

            // if doesn't exist or not set
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                //test
                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testsales2.db");
                //prod
                //filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sales.db");
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
        
    
    }
}