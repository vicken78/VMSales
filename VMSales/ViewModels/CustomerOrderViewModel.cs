using System;
using System.Collections.Generic;
using VMSales.Models;
using VMSales.Logic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading.Tasks;
namespace VMSales.ViewModels
{
    public class CustomerOrderViewModel : BaseViewModel
    {
        private ObservableCollection<CustomerModel> _ObservableCollectionCustomerModel { get; set; }
        public ObservableCollection<CustomerModel> ObservableCollectionCustomerModel
        {
            get { return _ObservableCollectionCustomerModel; }
            set
            {
                if (_ObservableCollectionCustomerModel == value) return;
                _ObservableCollectionCustomerModel = value;
                RaisePropertyChanged("ObservableCollectionCustomerModel");
            }
        }

        public void initial_load()
        {
            // load customers
            IDatabaseProvider dataBaseProvider;
            try
            {
                ObservableCollectionCustomerModel = new ObservableCollection<CustomerModel>();
                dataBaseProvider = getprovider();
                DataBaseLayer.CustomerRepository CustomerRepo = new DataBaseLayer.CustomerRepository(dataBaseProvider);
                ObservableCollectionCustomerModel = CustomerRepo.GetAll().Result.ToObservable();
                CustomerRepo.Commit();
                CustomerRepo.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show("An error has occured loading customers." + e);
            }
        }
        public CustomerOrderViewModel()
        {
            initial_load();
        }
    }
}
