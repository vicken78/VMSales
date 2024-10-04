using System;
using VMSales.ViewModels;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace VMSales.Models
{
    [Table("purchase_order")]
    public partial class PurchaseOrderModel : BaseViewModel
    {
        private int _purchase_order_pk;
        [ExplicitKey]
        public int purchase_order_pk
        {
            get => _purchase_order_pk;
            set
            {
                if (_purchase_order_pk != value)
                _purchase_order_pk = value;
                NotifyOfPropertyChange(() => purchase_order_pk);
            }
        }
/*
        private int _checkqtycolor;
        public int checkqtycolor
        {
            get => _checkqtycolor;
            set 
            {
                if (_checkqtycolor != value)
                    _checkqtycolor = value;
                NotifyOfPropertyChange(() => checkqtycolor);
            }
        }
*/
        private string _invoice_number;
        public string invoice_number
        {
            get => _invoice_number;
            set
            {
                if (_invoice_number != value)
                _invoice_number = value;
                NotifyOfPropertyChange(() => invoice_number);
            }
        }

        private DateTime _purchase_date;
        public DateTime purchase_date
        {
            get => _purchase_date; 
            set
            {
                if (_purchase_date != value)
                _purchase_date = value;
                NotifyOfPropertyChange(() => purchase_date);
  //              checkqtycolor = 1;
            }
        }

        private int _supplier_fk;
        public int supplier_fk
        {
            get => _supplier_fk;
            set 
            {
                if (_supplier_fk != value)
                    _supplier_fk = value;
                NotifyOfPropertyChange(() => supplier_fk);
            }
        }

        private string _supplier_name;
        public string supplier_name
        {
            get => _supplier_name;
            set
            {
                if (_supplier_name != value)
                    _supplier_name = value;
                NotifyOfPropertyChange(() => supplier_name);
            }
        }




        private bool _IsSelected;
        public bool IsSelected
        {
            get => _IsSelected; 
            set
            {
                if (_IsSelected != value)
                _IsSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }
    }
}