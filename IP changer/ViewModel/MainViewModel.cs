using IP_changer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace IP_changer.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Parameters
        private int _windowHeight;
        public int WindowHeight
        {
            get { return _windowHeight; }
            set { _windowHeight = value; OnPropertyChanged("WindowHeight"); }
        }

        private int _windowWidth;
        public int WindowWidth
        {
            get { return _windowWidth; }
            set { _windowWidth = value; }
        }

        private bool _isDHCPEnabled;
        public bool IsDHCPEnabled
        {
            get { return _isDHCPEnabled; }
            set { _isDHCPEnabled = value; OnPropertyChanged("IsDHCPEnabled"); }
        }

        private Visibility _isStaticVisible;
        public Visibility IsStaticVisible
        {
            get { return _isStaticVisible; }
            set { _isStaticVisible = value; OnPropertyChanged("IsStaticVisible"); }
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

        private int _selectedIndexLog;
        public int SelectedIndexLog
        {
            get { return _selectedIndexLog; }
            set 
            {
                _selectedIndexLog = value;
                OnPropertyChanged("SelectedIndexLog");
            }
        }

        private NetworkInterface _selectedNetworkInterface;
        public NetworkInterface SelectedNetworkInterface
        {
            get { return _selectedNetworkInterface; }
            set 
            { 
                _selectedNetworkInterface = value;
                IPControlManager.m_SelectedNetworkInterface = _selectedNetworkInterface;
                InsertLog("[info] Adaptor : " + _selectedNetworkInterface.Name);
                if (IPControlManager.IsDHCPInterface())
                {
                    InsertLog("[info] Mode : DHCP");
                    ClearStaticIP();
                }
                else
                {
                    InsertLog("[info] Mode : Static");
                    ReadSetStaticIP();
                }
            }
        }

        private string _targetIP;
        public string TargetIP
        {
            get { return _targetIP; }
            set
            {
                _targetIP = value;
                OnPropertyChanged("TargetIP");
            }
        }


        private string _addressIPv4;
        public string AddressIPv4 
        {
            get { return _addressIPv4; }
            set 
            { 
                _addressIPv4 = value;
                OnPropertyChanged("AddressIPv4");
                if(IPControlManager.CheckValidAddress(_addressIPv4))
                {
                    IPControlManager.m_sIPv4 = _addressIPv4;
                }
            }
        }
        private string _addressNetMask;
        public string AddressNetMask
        {
            get { return _addressNetMask; }
            set
            {
                _addressNetMask = value;
                OnPropertyChanged("AddressNetMask");
                if(IPControlManager.CheckValidAddress(_addressNetMask))
                {
                    IPControlManager.m_sMask = _addressNetMask;
                }
            }
        }

        private string _addressGateway;
        public string AddressGateway
        {
            get { return _addressGateway; }
            set
            {
                _addressGateway = value;
                OnPropertyChanged("AddressGateway");
                if (IPControlManager.CheckValidAddress(_addressGateway))
                {
                    IPControlManager.m_sGateway = _addressGateway;
                }
            }
        }

        private string _addressDNS;
        public string AddressDNS
        {
            get { return _addressDNS; }
            set
            {
                _addressDNS = value;
                OnPropertyChanged("AddressDNS");
                if (IPControlManager.CheckValidAddress(_addressDNS))
                {
                    IPControlManager.m_sDNS = _addressDNS;
                }
            }
        }

        private bool isAdmin;
        private CIPManager IPControlManager;
        private CmdWindow cmdWindow;

        #endregion

        public MainViewModel()
        {

            IPControlManager = new CIPManager();
            ClearStaticIP();

            LogList = new ObservableCollection<string>();
            isAdmin = IsAdministrator();
            if (!isAdmin)
                InsertLog("[Error] Please execute as Admin");

            WindowHeight = 400;
            WindowWidth = 260;
            IsStaticVisible = Visibility.Collapsed;

            cmdWindow = new CmdWindow();
            TargetIP = "192.168.0.101";
            NetworkInterfaces = IPControlManager.m_ArrayNetworkInterface;
            SelectedNetworkInterface = NetworkInterfaces[0];
        }

        public void ReadSetStaticIP()
        {
            IPControlManager.ReadStaticIPv4();
            AddressIPv4 = IPControlManager.m_sIPv4;
            AddressNetMask = IPControlManager.m_sMask;
            AddressDNS = IPControlManager.m_sDNS;
            AddressGateway = IPControlManager.m_sGateway;

            InsertLog("[info] IP : " + AddressIPv4);
            InsertLog("[info] Mask : " + AddressNetMask);
            InsertLog("[info] Gateway : " + AddressGateway);
            if(AddressDNS!=null)
                InsertLog("[info] DNS : " + AddressDNS);
        }

        public void ClearStaticIP()
        {
            AddressIPv4 = string.Empty;
            AddressNetMask = string.Empty;
            AddressGateway = string.Empty;
            AddressDNS = string.Empty;
        }

        private ICommand f1Command;
        public ICommand F1Command
        {
            get { return (this.f1Command) ?? (this.f1Command = new DelegateCommand(F1Pushed)); }
        }


        private void F1Pushed()
        {
            InsertLog("[info] Open Github Site");
            System.Diagnostics.Process.Start("https://github.com/samchiRobot/wpf-ip-changer");
        }

        private ICommand f2Command;
        public ICommand F2Command
        {
            get { return (this.f2Command) ?? (this.f2Command = new DelegateCommand(F2Pushed)); }
        }

        private void F2Pushed()
        {
            InsertLog("[info] Open IP setting JSON");
        }

        private ICommand clearLogCommand;
        public ICommand ClearLogCommand
        {
            get { return (this.clearLogCommand) ?? (this.clearLogCommand = new DelegateCommand(ClearLog)); }
        }
        private void ClearLog()
        {
            LogList.Clear();
        }

        private ICommand enableSet1command;
        public ICommand EnableSet1Command
        {
            get { return (this.enableSet1command) ?? (this.enableSet1command = new DelegateCommand(EnableSet1)); }
        }

        private void EnableSet1()
        {
            AddressIPv4 = "192.168.0.21";
            AddressNetMask = "255.255.255.0";
            AddressGateway = "192.168.0.1";
            TargetIP = "192.168.0.101";
        }

        private ICommand enableSet2command;
        public ICommand EnableSet2Command
        {
            get { return (this.enableSet2command) ?? (this.enableSet2command = new DelegateCommand(EnableSet2)); }
        }

        private void EnableSet2()
        {
            AddressIPv4 = "192.168.7.21";
            AddressNetMask = "255.255.255.0";
            AddressGateway = "192.168.7.1";
            TargetIP = "192.168.7.101";
        }

        private ICommand pingCommand;
        public ICommand PingCommand
        {
            get { return (this.pingCommand) ?? (this.pingCommand = new DelegateCommand(Ping)); }
        }

        private void Ping()
        {
            int nTImeOut = 80;
            if (!IPControlManager.CheckValidAddress(TargetIP))
            {
                InsertLog("[Error] Invalid Target IP");
                return;
            }
            InsertLog("[Info] Ping to " + TargetIP);
            if (PingProcess(1,nTImeOut))
                InsertLog("[Result] Ping Success");
            else
                InsertLog("[Result] Ping Failed");
        }

        private bool PingProcess(object obj, int nTimeout)
        {
            int cnt = 0;
            for (cnt = 0; cnt < (int)obj; cnt++)
            {
                if (IPControlManager.PingTarget(TargetIP, nTimeout))
                    break;
            }
            if (cnt < (int)obj)
                return true;
            else
                return false;
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
            IsStaticVisible = Visibility.Collapsed;
            WindowHeight = 400;
            if(IPControlManager.IsDHCPInterface())
            {
                InsertLog("[Info] DHCP Already Set");
                return;
            }

            bool bFlag = IPControlManager.SetDHCP();

            if (bFlag & isAdmin)
            {
                InsertLog("[Result] Set DHCP Success");
                ClearStaticIP();
            }
            else
                InsertLog("[Result] Set DHCP Failed");
        }

        #region Static Command

        private ICommand enableStaticCommand;
        public ICommand EnableStaticCommand
        {
            get { return (this.enableStaticCommand) ?? (this.enableStaticCommand = new DelegateCommand(EnableStatic)); }
        }
        private void EnableStatic()
        {
            WindowHeight = 580;
            IsStaticVisible = Visibility.Visible;
        }

        private ICommand applyStaticCommand;
        public ICommand ApplyStaticCommand
        {
            get { return (this.applyStaticCommand) ?? (this.applyStaticCommand = new DelegateCommand(ApplyStatic)); }
        }
        private void ApplyStatic()
        {
            if ((AddressIPv4 != IPControlManager.m_sIPv4) || (AddressNetMask != IPControlManager.m_sMask) || (AddressGateway != IPControlManager.m_sGateway))
            {
                InsertLog("[Error] Invalid address");
                return;
            }
            InsertLog("[Info] Set static");
            InsertLog("IP: " + IPControlManager.m_sIPv4);
            InsertLog("Mask: " + IPControlManager.m_sMask);
            InsertLog("GateWay: " + IPControlManager.m_sGateway);
            bool bFlag = false;
            if ((AddressDNS!=null)&&(AddressDNS==IPControlManager.m_sDNS))
            {
                InsertLog("DNS: " + IPControlManager.m_sDNS);
                bFlag = IPControlManager.SetNetStaticAddress(true);
            }
            else
            {
                bFlag = IPControlManager.SetNetStaticAddress(false);
            }

            if (bFlag & isAdmin)
                InsertLog("[Result] Set Static Success");
            else
                InsertLog("[Result] Set Static Failed");
        }

        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (null != identity)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return false;
        }

        public void InsertLog(string newItem)
        {
            SelectedIndexLog = LogList.Count;
            var printIdx = (SelectedIndexLog % 1000).ToString("D2");
            LogList.Add(printIdx + ": " + newItem);
        }

        #region ShowIPconfig
        private ICommand iPInfoCommand;
        public ICommand IPInfoCommand
        {
            get { return (this.iPInfoCommand) ?? (this.iPInfoCommand = new DelegateCommand(IPInfo)); }
        }
        private void IPInfo()
        {
            cmdWindow.Show();
        }
        #endregion
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
