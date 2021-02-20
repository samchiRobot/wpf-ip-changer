using IP_changer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace IP_changer.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private NetworkInterface[] _networkInterfaces;
        public NetworkInterface[] NetworkInterfaces
        {
            get { return _networkInterfaces; }
            set { _networkInterfaces = value; }
        }

        private ObservableCollection<string> _logList;
        public ObservableCollection<string> LogList
        {
            get { return _logList; }
            set { _logList = value; }
        }

        private NetworkInterface _selectedNetworkInterface;
        public NetworkInterface SelectedNetworkInterface
        {
            get { return _selectedNetworkInterface; }
            set 
            { 
                _selectedNetworkInterface = value;
                iPManager.m_SelectedNetworkInterface = _selectedNetworkInterface;
            }
        }

        IP_changer.Model.CIPManager iPManager;

        private SettingsWindow settings;
        private CmdWindow cmdWindow;
        public MainViewModel()
        {
            iPManager = new Model.CIPManager();
            NetworkInterfaces = iPManager.m_ArrayNetworkInterface;
            SelectedNetworkInterface = NetworkInterfaces[0];
            LogList = new ObservableCollection<string>();

            settings = new SettingsWindow();
            cmdWindow = new CmdWindow();
            _setDataManager();
            _applyDataManager();
        }

        private void _setDataManager()
        {
            CDataManager.m_sIPv4 = "192.168.0.10";
            CDataManager.m_sMask = "255.255.255.0";
            CDataManager.m_sGateway = "192.168.0.1";
            CDataManager.m_sDNS = "8.8.8.8";
            CDataManager.m_sTargetIP = "192.168.0.101";
            CDataManager.m_bApplyEnable = false;
        }

        private void _applyDataManager()
        {
            iPManager.m_sIPv4 =  CDataManager.m_sIPv4;
            iPManager.m_sMask = CDataManager.m_sMask;
            iPManager.m_sGateway = CDataManager.m_sGateway;
            iPManager.m_sDNS = CDataManager.m_sDNS;
        }

        private ICommand settingsCommand;
        public ICommand SettingsCommand
        {
            get { return (this.settingsCommand)??(this.settingsCommand=new DelegateCommand(Settings)); }
        }
        private void Settings()
        {
            settings.Show();          
        }

        private ICommand pingCommand;
        public ICommand PingCommand
        {
            get { return (this.pingCommand) ?? (this.pingCommand = new DelegateCommand(Ping)); }
        }
        // FIXIT
        private void Ping()
        {
            string targetIP = CDataManager.m_sTargetIP;
            bool bFlag = iPManager.PingTarget(targetIP);
            LogList.Add("[Execute] Ping to " + targetIP);

            if(bFlag)
                LogList.Add("[Result] Ping Success");
            else
                LogList.Add("[Result] Ping Failed");
        }

        private ICommand clearCommand;
        public ICommand ClearCommand
        {
            get { return (this.clearCommand) ?? (this.clearCommand = new DelegateCommand(Clear)); }
        }
        private void Clear()
        {
            LogList.Clear();
        }

        private ICommand setDHCPCommand;
        public ICommand SetDHCPCommand
        {
            get { return (this.setDHCPCommand) ?? (this.setDHCPCommand = new DelegateCommand(SetDHCP)); }
        }
        private void SetDHCP()
        {
            bool bFlag = iPManager.SetDHCP();
            if(bFlag)
                LogList.Add("[Result] Set DHCP Success");
            else
                LogList.Add("[Result] Set DHCP Failed");
        }

        private ICommand setStaticCommand;
        public ICommand SetStaticCommand
        {
            get { return (this.setStaticCommand) ?? (this.setStaticCommand = new DelegateCommand(SetStatic)); }
        }
        private void SetStatic()
        {
            LogList.Add("[Execute] Set static");
            if(CDataManager.m_bApplyEnable)
            {
                _applyDataManager();
                CDataManager.m_bApplyEnable = false;
            }
            LogList.Add("IP: "+ iPManager.m_sIPv4);
            LogList.Add("Mask: " + iPManager.m_sMask);
            LogList.Add("GateWay: " + iPManager.m_sGateway);
            LogList.Add("DNS: " + iPManager.m_sDNS);
            bool bFlag = iPManager.SetNetStaticAddress();
            if (bFlag)
                LogList.Add("[Result] Set Static Success");
            else
                LogList.Add("[Result] Set Static Failed");
        }

        private ICommand iPInfoCommand;
        public ICommand IPInfoCommand
        {
            get { return (this.iPInfoCommand) ?? (this.iPInfoCommand = new DelegateCommand(IPInfo)); }
        }
        private void IPInfo()
        {
            cmdWindow.Show();
        }
    }

    #region DelegateCommand Class
    public class DelegateCommand : ICommand
    {

        private readonly Func<bool> canExecute;
        private readonly Action execute;

        /// <summary>
        /// Initializes a new instance of the DelegateCommand class.
        /// </summary>
        /// <param name="execute">indicate an execute function</param>
        public DelegateCommand(Action execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DelegateCommand class.
        /// </summary>
        /// <param name="execute">execute function </param>
        /// <param name="canExecute">can execute function</param>
        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        /// <summary>
        /// can executes event handler
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// implement of icommand can execute method
        /// </summary>
        /// <param name="o">parameter by default of icomand interface</param>
        /// <returns>can execute or not</returns>
        public bool CanExecute(object o)
        {
            if (this.canExecute == null)
            {
                return true;
            }
            return this.canExecute();
        }

        /// <summary>
        /// implement of icommand interface execute method
        /// </summary>
        /// <param name="o">parameter by default of icomand interface</param>
        public void Execute(object o)
        {
            this.execute();
        }

        /// <summary>
        /// raise ca excute changed when property changed
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
    #endregion
}
