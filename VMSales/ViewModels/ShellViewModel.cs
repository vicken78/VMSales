using Caliburn.Micro;
using System.Threading.Tasks;

namespace VMSales.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        public async Task LoadCategory()
        {
            await ActivateItemAsync(new CategoryViewModel());
        }

        public async Task LoadPurchaseOrder()
        {
            await ActivateItemAsync(new PurchaseOrderViewModel());
        }
     
        public async Task LoadSupplier()
        {
            await ActivateItemAsync (new SupplierViewModel());
        }

        public async Task LoadProduct()
        {
            await ActivateItemAsync(new ProductViewModel());   
        }


    }
}