using Fithub.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaApp.Properties
{
    public class SocialManagementViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<SocialManagement> management = new ObservableCollection<SocialManagement>();

        public ObservableCollection<SocialManagement> Management
        {
            get { return management; }
            private set { management = value; }
        }

        public SocialManagementViewModel(List<SocialManagement> sm) 
        {
            sm.ForEach(x => management.Add(x));
            RaisePropertyChanged(nameof(Management));
        }

        //public void UpdateName(string name)
        //{
        //    Management.Username = name;
        //    RaisePropertyChanged(nameof(Management));
        //}

        public SocialManagementViewModel() { }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
