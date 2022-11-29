using System.ComponentModel;
using VMSales.Logic;
namespace VMSales.ViewModels
{
    public class BaseViewModel : DataValidator, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        protected static string SetDataBase()
        {
            string dbfilepath = "TEST";
            
            if (dbfilepath == "PROD")
                {
                const string filepath = "C:\\Users\\Vicken\\source\\repos\\testsqllite\\testsqllite\\db\\sales.db";
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
