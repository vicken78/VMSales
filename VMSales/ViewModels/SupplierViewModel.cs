using VMSales.Models;
using VMSales.Logic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace VMSales.ViewModels
{
    public class SupplierViewModel : BaseViewModel
    {
        private ObservableCollection<SupplierModel> _ObservableCollectionSupplierModelDirty { get; set; }

        public ObservableCollection<SupplierModel> ObservableCollectionSupplierModelDirty
        {
            get => _ObservableCollectionSupplierModelDirty;
            set
            {
                _ObservableCollectionSupplierModelDirty = value;
                NotifyOfPropertyChange(() => ObservableCollectionSupplierModelDirty);
            }
        }
        public ObservableCollection<SupplierModel> ObservableCollectionSupplierModelClean { get; protected set; }

        IDatabaseProvider dataBaseProvider;

        //Commands
        public async Task SaveCommand()
        {
            // Create an instance of DataProcessor with SupplierModel type
            var dataProcessor = new DataProcessor<SupplierModel>();
            // Call the Compare method
            ObservableCollection<SupplierModel> differences = dataProcessor.Compare(ObservableCollectionSupplierModelClean, ObservableCollectionSupplierModelDirty);

            foreach (var item in differences)
            {
                DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
                try
                {
                    // implement check for foreign keys, if foreign key exists, warn user.
                    switch (item.Action)
                    {
                        case "Update":
                            bool Update_Supplier = SupplierRepo.Update(item).Result;
                            if (Update_Supplier == false)
                            { throw new Exception("Update Failed"); }
                            else
                                SupplierRepo.Commit();
                            break;
                        case "Insert":
                            int supplier_pk = await SupplierRepo.Insert(item);
                            if (supplier_pk == 0)
                            { throw new Exception("Insert Failed"); }
                            else
                                SupplierRepo.Commit();
                            break;
                        case "Delete":
                            bool Supplier_Category = SupplierRepo.Delete(item).Result;
                            if (Supplier_Category == false)
                            { throw new Exception("Delete Failed"); }
                            else
                                SupplierRepo.Commit();
                            break;
                        default:
                            throw new InvalidOperationException("Unexpected Action read, expected Update Insert or Delete.");
                    }
                    SupplierRepo.Dispose();
                    initial_load();
                }
                catch (Exception e)
                {
                    MessageBox.Show("An unexpected error has occured." + e);
                    SupplierRepo.Revert();
                    SupplierRepo.Dispose();
                }
            }
        }
        public void ResetCommand()
        {
            initial_load();
        }

        public void initial_load()
        {
            ObservableCollectionSupplierModelDirty = new ObservableCollection<SupplierModel>();
            ObservableCollectionSupplierModelClean = new ObservableCollection<SupplierModel>();

            dataBaseProvider = getprovider();
            DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
            ObservableCollectionSupplierModelDirty = SupplierRepo.GetAll().Result.ToObservable();

            ObservableCollectionSupplierModelClean = new ObservableCollection<SupplierModel>(ObservableCollectionSupplierModelDirty.Select(item => new SupplierModel
            {
                // Copy properties from the item, or use a copy constructor if available
                supplier_pk = item.supplier_pk,
                supplier_name = item.supplier_name,
                address = item.address,
                city = item.city,
                state = item.state,
                country = item.country,
                phone = item.phone,
                email = item.email
            }
            ));
            SupplierRepo.Commit();
            SupplierRepo.Dispose();
        }
    
        public SupplierViewModel()
        {
            initial_load();
        }
    }
}