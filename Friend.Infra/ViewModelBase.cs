using Microsoft.Practices.Prism;
using System.ComponentModel;
using System;

namespace Friend.Infra
{
    public class ViewModelBase : INotifyPropertyChanged //IActiveAware
    {
         public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
