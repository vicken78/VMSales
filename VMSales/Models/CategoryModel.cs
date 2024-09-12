using VMSales.Logic;
using Dapper.Contrib.Extensions;
using System;
using System.Windows.Forms;
using VMSales.ViewModels;
using System.Windows.Media;
using System.Diagnostics;

namespace VMSales.Models
{
    [Table("category")]
    public class CategoryModel : BaseViewModel
    {
        private int _category_pk;
        public int category_pk
        {
            get => _category_pk;
            set
            {
                if (_category_pk != value)
                {
                    _category_pk = value;
                    NotifyOfPropertyChange(() => category_pk);
                }
            }
        }

        private string _category_name;
        public string category_name

        {
            get => _category_name;
            set
            {
                if (_category_name != null)
                {
                    FontColor = Brushes.Red;
                    NotifyOfPropertyChange(() => FontColor);
                }

                if (_category_name != value)
                {
                    _category_name = value;
                    NotifyOfPropertyChange(() => category_name);
                }
            }
        }
        private string _description;
        public string description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    if (_description != null)
                    {
                        FontColor = Brushes.Red;
                        NotifyOfPropertyChange(() => FontColor);
                    }

                    // Update the property and notify change
                    _description = value;
                    NotifyOfPropertyChange(() => description);

                    
                }
            }
        }


        private DateTime _creation_date;
        public DateTime creation_date
        {
            get => _creation_date;
            set
            {
                if (_creation_date != null)
                {
                    FontColor = Brushes.Black;
                    NotifyOfPropertyChange(() => FontColor);
                }

                if (_creation_date != value)
                {
                    _creation_date = value;
                    NotifyOfPropertyChange(() => creation_date);
                }
            }
        }
    }
}