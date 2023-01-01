using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using VMSales.Logic;
namespace VMSales.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        internal class EntityViewModel<T> 
        {
            
        }
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
            string dbfilepath = "PROD";

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