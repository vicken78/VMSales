using System.ComponentModel;
using VMSales.Logic;
namespace VMSales.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        const string filepath = "C:\\Users\\Vicken\\source\\repos\\testsqllite\\testsqllite\\db\\testsales2.db";

        public static IDatabaseProvider getprovider()
        {
            SQLiteDatabase dataBaseProvider = new SQLiteDatabase(filepath);
            return dataBaseProvider;
        }
    }
}
