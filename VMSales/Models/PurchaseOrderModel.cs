using System;
using VMSales.ViewModels;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace VMSales.Models
{

    public partial class PurchaseOrderModel : BaseViewModel
    {
        private int _changecolor { get; set; }
        public int changecolor { get; set; }
        private int _purchase_order_pk { get; set; }
        private string _invoice_number { get; set; }
        private DateTime _purchase_date { get; set; }
        public int supplier_fk { get; set; }

        [ExplicitKey]
        public int purchase_order_pk
        {
            get { return _purchase_order_pk; }
            set
            {
                if (_purchase_order_pk == value) return;
                _purchase_order_pk = value;
                RaisePropertyChanged("purchase_order_pk");
            }
        }

        public string invoice_number
        {
            get { return _invoice_number; }
            set
            {
                if (_invoice_number == value) return;
                _invoice_number = value;
                RaisePropertyChanged("invoice_number");
            }
        }
        public DateTime purchase_date
        {
            get { return _purchase_date; }
            set
            {
                if (_purchase_date == value) return;
                _purchase_date = value;
                RaisePropertyChanged("purchase_date");
                changecolor = 1;
            }
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

    }
}