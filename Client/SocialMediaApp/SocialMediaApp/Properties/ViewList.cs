using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApp.Properties
{
    public class ViewList : INotifyPropertyChanged
    {
        private ObservableCollection<string> list;

        public ObservableCollection<string> List
        {
            get { return list; }
            set 
            { 
                list = value;
                RaisePropertyChanged(nameof(List));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
