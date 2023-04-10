using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.ViewModels
{
    class IndexViewModel : BindableBase
    {
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(); }
        }


        public IndexViewModel()
        {
            Title = DateTime.Now.GetDateTimeFormats('D')[1].ToString();
        }
    }
}
