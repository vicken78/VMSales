using System;

using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Data;
using VMSales.Database;
using VMSales.Models;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using VMSales.ChangeTrack;
using System.Windows;
using VMSales.Models;
using VMSales.Logic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System;

/*
this is purchase order detail
SELECT po.purchase_order_pk, po.supplier_fk, po.invoice_number, po.purchase_date,
                         pod.purchase_order_fk as purchase_order_pk, pod.lot_number, pod.lot_cost, pod.lot_qty,
                      pod.lot_name, pod.lot_description, pod.sales_tax, pod.shipping_cost from purchase_order po
                      INNER JOIN purchase_order_detail pod on po.purchase_order_pk = pod.purchase_order_fk
                      WHERE po.supplier_fk = 1

product category
                    var sql = @"select productid, productname, p.categoryid, categoryname 
                   from products p 
                   inner join categories c on p.categoryid = c.categoryid";

 
 */

/*
namespace VMSales.ViewModels
{
        public class Temp : BaseViewModel
        {
            private int _selected_supplier_pk { get; set; }
            public int selected_supplier_pk
            {
                get { return _selected_supplier_pk; }
                set
                {
                    if (_selected_supplier_pk == value) return;
                    _selected_supplier_pk = value;
                    RaisePropertyChanged("selected_supplier_pk");
                }
            }

            public ObservableCollection<SupplierModel> ObservableCollectionSupplierModel { get; set; }

            private ObservableCollection<PurchaseOrderModel> ObservableCollectionPurchaseOrderModelclean { get; set; }
            public ObservableCollection<PurchaseOrderModel> ObservableCollectionPurchaseOrderModel
            {
                get { return ObservableCollectionPurchaseOrderModelclean; }
                set
                {
                    if (ObservableCollectionPurchaseOrderModelclean == value) return;
                    ObservableCollectionPurchaseOrderModelclean = value;
                    RaisePropertyChanged("ObservableCollectionPurchaseOrderModel");
                }
            }

            IDatabaseProvider dataBaseProvider;


            #region tracking
            public ChangeTracker<PurchaseOrderModel> changetracker { get; set; }
            List<PurchaseOrderModel> POListUpdate = new List<PurchaseOrderModel>();
            List<PurchaseOrderModel> POListCreate = new List<PurchaseOrderModel>();
            List<PurchaseOrderModel> POListDelete = new List<PurchaseOrderModel>();

            #endregion

            #region collectionmodel
            public ObservableCollection<PurchaseOrderModel> ocPurchaseOrderView { get; set; } = new ObservableCollection<PurchaseOrderModel>();

            //private CollectionViewSource cvsPurchaseOrderView { get; set; } = new CollectionViewSource();
           


            public ICollectionView PurchaseOrderView
            {
                get { return CollectionViewSource.GetDefaultView(ocPurchaseOrderView); }
            }
            #endregion
            #region PrivateMembers

            private string _selectedinvoicenumber;
            private string _selectedpurchasedate;
            private SupplierModel _selectedsname;
            private ObservableCollection<string> _invoicenumber;
            private ObservableCollection<string> _purchasedate;
            private bool _cancanremoveinvoicenumberfilter;
            private bool _cancanremovepurchasedatefilter;
            private decimal _totallotcost { get; set; }
            private decimal _totalsalestax { get; set; }
            private decimal _totalshippingcost { get; set; }
            private decimal _totalcost { get; set; }
            private enum FilterField
            {
                InvoiceNumber,
                PurchaseDate,
                None
            }
            private void ApplyFilter(FilterField field)
            {
                switch (field)
                {
                    case FilterField.InvoiceNumber:
                        AddInvoiceNumberFilter();
                        PurchaseOrderView.Filter = new Predicate<object>(x => ((PurchaseOrderModel)x).invoice_number.ToString() == _selectedinvoicenumber);
                        RaisePropertyChanged("PurchaseOrderView");
                        break;
                    case FilterField.PurchaseDate:
                        AddPurchaseDateFilter();
                        PurchaseOrderView.Filter = new Predicate<object>(x => ((PurchaseOrderModel)x).purchase_date.ToString() == _selectedpurchasedate);

                        break;
                    default:
                        break;
                }
            }
            #endregion
            #region publicfilters
   


            public bool CanRemoveInvoiceNumberFilter
            {
                get { return _cancanremoveinvoicenumberfilter; }
                set
                {
                    _cancanremoveinvoicenumberfilter = value;
                    RaisePropertyChanged("CanRemoveInvoiceNumberFilter");
                }
            }
            public bool CanRemovePurchaseDateFilter
            {
                get { return _cancanremovepurchasedatefilter; }
                set
                {
                    _cancanremovepurchasedatefilter = value;
                    RaisePropertyChanged("CanRemovePurchaseDateFilter");
                }
            }

            #endregion

            #region Collections
            public List<SupplierModel> Supplier { get; set; } = new List<SupplierModel>();
            public ObservableCollection<string> InvoiceNumber
            {
                get { return _invoicenumber; }
                set
                {
                    if (_invoicenumber == value)
                        return;
                    _invoicenumber = value;
                    RaisePropertyChanged("InvoiceNumber");
                }
            }
            public ObservableCollection<string> PurchaseDate
            {
                get { return _purchasedate; }
                set
                {
                    if (_purchasedate == value)
                        return;
                    _purchasedate = value;
                    RaisePropertyChanged("PurchaseDate");
                }
            }


            #endregion
            #region Selected

            public SupplierModel supplier_pk
            {
                get { return supplier_pk; }
                set
                {
                    RaisePropertyChanged("supplier_pk");
                    LoadPurchaseOrder(supplier_pk.ToString());
                }
            }

            public string SelectedInvoiceNumber
            {
                get { return _selectedinvoicenumber; }
                set
                {
                    if (_selectedinvoicenumber == value)
                        return;
                    _selectedinvoicenumber = value;
                    RaisePropertyChanged("SelectedInvoiceNumber");
                    ApplyFilter(!string.IsNullOrEmpty(_selectedinvoicenumber) ? FilterField.InvoiceNumber : FilterField.None);
                }
            }
            /// <summary>
            /// Gets or sets the selected PurchaseDate in the list to filter the collection
            /// </summary>
            public string SelectedPurchaseDate
            {
                get { return _selectedpurchasedate; }
                set
                {
                    if (_selectedpurchasedate == value)
                        return;
                    _selectedpurchasedate = value;
                    RaisePropertyChanged("SelectedPurchaseDate");
                    ApplyFilter(!string.IsNullOrEmpty(_selectedpurchasedate) ? FilterField.PurchaseDate : FilterField.None);
                }
            }



            #endregion
            #region Totals
            public decimal Totallotcost
            {
                get { return _totallotcost; }
                set
                {
                    if (_totallotcost == value) return;
                    _totallotcost = value;
                    RaisePropertyChanged("Totallotcost");
                }
            }

            public decimal Totalsalestax
            {
                get { return _totalsalestax; }
                set
                {
                    if (_totalsalestax == value) return;
                    _totalsalestax = value;
                    RaisePropertyChanged("Totalsalestax");
                }
            }
            public decimal Totalshippingcost
            {
                get { return _totalshippingcost; }
                set
                {
                    if (_totalshippingcost == value) return;
                    _totalshippingcost = value;
                    RaisePropertyChanged("Totalshippingcost");
                }
            }
            public decimal Totalcost
            {
                get { return _totalcost; }
                set
                {
                    if (_totalcost == value) return;
                    _totalcost = value;
                    RaisePropertyChanged("Totalcost");
                }
            }

            #endregion
            #region ButtonCommands
            public void SaveCommand()
            {
                //  if (POListUpdate.Any(x => x != null))
                //  {
                POListUpdate = changetracker.RowsUpdated;
                foreach (PurchaseOrderModel PO in POListUpdate)
                {
                    /*   MessageBox.Show(PO.invoicenum.ToString());
                       MessageBox.Show(PO.Purchasedate.ToString());
                       MessageBox.Show(PO.Lotnumber.ToString()); 
                       MessageBox.Show(PO.Lotqty.ToString());
                       MessageBox.Show(PO.Lotname.ToString());
                       MessageBox.Show(PO.Lotdescription.ToString());
                       MessageBox.Show(PO.Lotcost.ToString());
                       MessageBox.Show(PO.Salestax.ToString());
                       MessageBox.Show(PO.Shippingcost.ToString());
                    */
                //}
                //}

            //}
/*
            public void AddCommand()
            {
            }
            public void DeleteCommand()
            {
            }
            public void ResetCommand()
            {
                if (changetracker != null)
                {

                    changetracker.Dispose();
                    changetracker.ClearTracking();
                    SelectedInvoiceNumber = null;
                    SelectedPurchaseDate = null;
                    CanRemoveInvoiceNumberFilter = false;
                    CanRemovePurchaseDateFilter = false;
                    PurchaseOrderView.Filter = null;
                    //LoadPurchaseOrder(SelectedSupplier_pk);
                }
            }
            public void RemoveInvoiceNumberFilterCommand()
            {
                cvsPurchaseOrderView.Filter -= new FilterEventHandler(FilterByInvoiceNumber);
                SelectedInvoiceNumber = null;
                PurchaseOrderView.Filter = null;
                CanRemoveInvoiceNumberFilter = false;
                RaisePropertyChanged("PurchaseOrderView");

            }
            public void RemovePurchaseDateFilterCommand()
            {
                cvsPurchaseOrderView.Filter -= new FilterEventHandler(FilterByPurchaseDate);
                SelectedPurchaseDate = null;
                PurchaseOrderView.Filter = null;
                CanRemovePurchaseDateFilter = false;
                RaisePropertyChanged("PurchaseOrderView");
            }

            #endregion
            #region Load Purchase Order Data
            #region filterfunctions

            /* Notes on Adding Filters:
            *   Each filter is added by subscribing a filter method to the Filter event
            *   of the CVS.  Filters are applied in the order in which they were added. 
            *   To prevent adding filters mulitple times ( because we are using drop down lists
            *   in the view), the CanRemove***Filter flags are used to ensure each filter
            *   is added only once.  If a filter has been added, its corresponding CanRemove***Filter
            *   is set to true.       
            *   
            *   If a filter has been applied already and then they change their selection to another value 
            *   we need to undo the previous filter then apply the new one.
            *   This does not completey Reset the filter, just allows it to be changed to another filter value. 
            */


    /*
            public void AddInvoiceNumberFilter()
            {
                // see Notes on Adding Filters:
                if (CanRemoveInvoiceNumberFilter)
                {
                    cvsPurchaseOrderView.Filter -= new FilterEventHandler(FilterByInvoiceNumber);
                    cvsPurchaseOrderView.Filter += new FilterEventHandler(FilterByInvoiceNumber);
                }
                else
                {
                    cvsPurchaseOrderView.Filter += new FilterEventHandler(FilterByInvoiceNumber);
                    CanRemoveInvoiceNumberFilter = true;
                }
            }
            public void AddPurchaseDateFilter()
            {
                // see Notes on Adding Filters:
                if (CanRemovePurchaseDateFilter)
                {
                    cvsPurchaseOrderView.Filter -= new FilterEventHandler(FilterByPurchaseDate);
                    cvsPurchaseOrderView.Filter += new FilterEventHandler(FilterByPurchaseDate);
                }
                else
                {
                    cvsPurchaseOrderView.Filter += new FilterEventHandler(FilterByPurchaseDate);
                    CanRemovePurchaseDateFilter = true;
                }
            }

            /* Notes on Filter Methods:
             * When using multiple filters, do not explicitly set anything to true.  Rather,
            * only hide things which do not match the filter criteria
            * by setting e.Accepted = false.  If you set e.Accept = true, if effectively
            * clears out any previous filters applied to it.  
            */
            // This is where purchase data is filtered.


/*
            private void FilterByInvoiceNumber(object sender, FilterEventArgs e)
            {
                var src = e.Item as PurchaseOrderModel;
                if (src == null)
                    e.Accepted = false;
                else if (string.Compare(SelectedInvoiceNumber, src.invoice_number.ToString()) != 0)
                    e.Accepted = false;
            }
            private void FilterByPurchaseDate(object sender, FilterEventArgs e)
            {
                var src = e.Item as PurchaseOrderModel;
                if (src == null)
                    e.Accepted = false;
                else if (string.Compare(SelectedPurchaseDate, src.purchase_date.ToString()) != 0)
                    e.Accepted = false;
            }

            #endregion

            public void LoadPurchaseOrder(string supplier_pk)
            {
                //fix

                MessageBox.Show("HIT");
                MessageBox.Show(supplier_pk.ToString());

                if (supplier_pk != null)
                {

                    // clear old data
                    ocPurchaseOrderView.Clear();
                    InvoiceNumber = new ObservableCollection<string>();
                    PurchaseDate = new ObservableCollection<string>();
                    Totallotcost = 0;
                    Totalsalestax = 0;
                    Totalshippingcost = 0;
                    Totalcost = 0;

                    string sqlquery = "purchase_order, purchase_order_detail, supplier WHERE purchase_order.purchaseorder_pk = purchase_order_detail.purchaseorder_fk AND supplier.supplier_pk = purchase_order.supplier_fk AND supplier.supplier_pk='" + supplier_pk + "'";
                    DataTable Purchaseorderdt = DataBaseOps.SQLiteDataTableWithQuery(sqlquery);

                    if ((Purchaseorderdt?.Rows?.Count ?? 0) > 0)
                    {
                        var InvoiceNumberList = (from r in Purchaseorderdt.AsEnumerable()
                                                 select r["Invoicenum"]).Distinct().ToList();

                        var PurchaseDateList = Purchaseorderdt.AsEnumerable()
                               .Select(r => r.Field<DateTime>("Purchasedate").ToString()).Distinct().ToList();

                        InvoiceNumber = new ObservableCollection<string>(InvoiceNumberList.Cast<String>());
                        PurchaseDate = new ObservableCollection<string>(PurchaseDateList.Cast<String>());

                        foreach (DataRow row in Purchaseorderdt.Rows)
                        {
                            var obj = new PurchaseOrderModel()
                            {
                                //         Lotnumber = (string)row["lotnum"],
                                //         Lotname = (string)row["Lotname"],
                                //         Lotdescription = (string)row["Lotdescription"],
                                //         Lotqty = (int)row["Lotqty"],
                                //         Lotcost = (decimal)row["Lotcost"],
                                //         Salestax = (decimal)row["Salestax"],
                                //         Shippingcost = (decimal)row["Shippingcost"],
                                invoice_number = (string)row["invoice_number"],
                                purchase_date = (DateTime)row["purchase_date"],
                            };
                            //     Totallotcost = Totallotcost + obj.Lotcost;
                            //     Totalsalestax = Totalsalestax + obj.Salestax;
                            //     Totalshippingcost = Totalshippingcost + obj.Shippingcost;
                            Totalcost = Totallotcost + Totalsalestax + Totalshippingcost;
                            RaisePropertyChanged("Totallotcost");
                            RaisePropertyChanged("Totalsalestax");
                            RaisePropertyChanged("Totalshippingcost");
                            RaisePropertyChanged("Totalcost");
                            ocPurchaseOrderView.Add(obj);
                        }

                    }
                    cvsPurchaseOrderView.Source = ocPurchaseOrderView;

                    changetracker = new ChangeTracker<PurchaseOrderModel>(ocPurchaseOrderView);
                    changetracker.StartTracking(ocPurchaseOrderView);


                    //var Totalsalestax = (from a in cvsPurchaseOrderView.OfType<Totalsalestax>() where a = )
                    //Totalsalestax = Totalsalestax + obj.Salestax;
                    //Totalshippingcost = Totalshippingcost + obj.Shippingcost;
                    //Totalcost = Totallotcost + Totalsalestax + Totalshippingcost;
                    //RaisePropertyChanged("Totallotcost");
                    //RaisePropertyChanged("Totalsalestax");
                    //RaisePropertyChanged("Totalshippingcost");
                    //RaisePropertyChanged("Totalcost");
                    //Totalcost = Totallotcost + Totalsalestax + Totalshippingcost;


                    //CollectionViewSource.GetDefaultView(ocPurchaseOrderView).Refresh();

                }
            }
            #endregion

            //startup
            #region startup
            public void PurchaseOrderViewModel()
            {
                // Get Suppliers
                ObservableCollectionSupplierModel = new ObservableCollection<SupplierModel>();
                dataBaseProvider = BaseViewModel.getprovider();
                DataBaseLayer.SupplierRepository SupplierRepo = new DataBaseLayer.SupplierRepository(dataBaseProvider);
                ObservableCollectionSupplierModel = SupplierRepo.GetAll().Result.ToObservable();
                SupplierRepo.Commit();
                SupplierRepo.Dispose();
            }
        }
        #endregion
    }
*/
