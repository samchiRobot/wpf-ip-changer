using GalaSoft.MvvmLight.Command;
using IP_changer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace IP_changer.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public RelayCommand<Window> HideWindowCommand { get; private set; }
        public SettingsViewModel()
        {
            this.HideWindowCommand = new RelayCommand<Window>(this.HideWindow);
        }

        private void HideWindow(Window window)
        {
            if (window != null)
            {
                window.Hide();
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ICommand applySettingCommand;
        public ICommand ApplySettingCommand
        {
            get { return (this.applySettingCommand) ?? (this.applySettingCommand = new DelegateCommand(ApplySetting)); }
        }
        private void ApplySetting()
        {
            if(_sIPv4!=null)
                CDataManager.m_sIPv4 = _sIPv4;
            if(_sMask != null)
                CDataManager.m_sMask = _sMask;
            if(_sGateway != null)
                CDataManager.m_sGateway = _sGateway;
            if (_sDNS != null)
                CDataManager.m_sDNS = _sDNS;
            if (_sTargetIP != null)
                CDataManager.m_sTargetIP = _sTargetIP;
            CDataManager.m_bApplyEnable = true;
        }

        private ICommand readSettingCommand;
        public ICommand ReadSettingCommand
        {
            get { return (this.readSettingCommand) ?? (this.readSettingCommand = new DelegateCommand(ReadSetting)); }
        }
        private void ReadSetting()
        {
            m_sIPv4 = CDataManager.m_sIPv4;
            m_sMask = CDataManager.m_sMask;
            m_sGateway = CDataManager.m_sGateway;
            m_sDNS = CDataManager.m_sDNS;
            m_sTargetIP = CDataManager.m_sTargetIP;
        }


        private string _sIPv4;
        public string m_sIPv4
        {
            get { return _sIPv4; }
            set
            {
                _sIPv4 = value;
                OnPropertyChanged("m_sIPv4");
            }
        }

        private string _sMask;
        public string m_sMask
        {
            get { return _sMask; }
            set
            {
                _sMask = value;
                OnPropertyChanged("m_sMask");
            }
        }

        private string _sGateway;
        public string m_sGateway
        {
            get { return _sGateway; }
            set
            {
                _sGateway = value;
                OnPropertyChanged("m_sGateway");
            }
        }

        private string _sDNS;
        public string m_sDNS
        {
            get { return _sDNS; }
            set
            {
                _sDNS = value;
                OnPropertyChanged("m_sDNS");
            }
        }

        private string _sTargetIP;
        public string m_sTargetIP
        {
            get { return _sTargetIP; }
            set
            {
                _sTargetIP = value;
                OnPropertyChanged("m_sTargetIP");
            }
        }
    }
}
