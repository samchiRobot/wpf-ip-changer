using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IP_changer.ViewModel
{
    public class CmdViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private IP_changer.Model.CCmdPromptManager CmdManager;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public RelayCommand<Window> HideWindowCommand { get; private set; }
        public CmdViewModel()
        {
            CmdManager = new Model.CCmdPromptManager();
            this.HideWindowCommand = new RelayCommand<Window>(this.HideWindow);
            m_sRefreshTime = System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            CmdManager.RunCmd("ipconfig /all");
            LogList = new ObservableCollection<string>();
            LogList.Add(CmdManager.ReadLog());
        }

        private ObservableCollection<string> _logList;
        public ObservableCollection<string> LogList
        {
            get { return _logList; }
            set { _logList = value; }
        }

        private string _sRefreshTime;
        public string m_sRefreshTime
        {
            get { return _sRefreshTime; }
            set 
            { 
                _sRefreshTime = value;
                OnPropertyChanged("m_sRefreshTime");
            }
        }
        private void HideWindow(Window window)
        {
            if (window != null)
            {
                window.Hide();
            }
        }

        private ICommand refreshCommand;
        public ICommand RefreshCommand
        {
            get { return (this.refreshCommand) ?? (this.refreshCommand = new DelegateCommand(Refresh)); }
        }
        private void Refresh()
        {
            m_sRefreshTime = System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            LogList.Clear();
            CmdManager.ClearLog();
            CmdManager.RunCmd("ipconfig /all");
            LogList.Add(CmdManager.ReadLog());
        }
    }
}
