using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Win_Info.DataClasses
{
    public class ConnectionInfo : INotifyPropertyChanged
    {
        private string _connectStatus;
        public string ConnectStatus
        {
            get
            {
                return this._connectStatus;
            }
            set
            {
                _connectStatus = value;
                OnPropertyRaised("ConnectStatus");
            }
        }
        private string _lastConnection;
        public string LastConnection
        {
            get
            {
                return this._lastConnection;
            }
            set
            {
                _lastConnection = value;
                OnPropertyRaised("LastConnection");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
}
