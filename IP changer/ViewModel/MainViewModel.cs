using IP_changer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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


        private NetworkInterface _selectedNetworkInterface;
        public NetworkInterface SelectedNetworkInterface
        {
            get { return _selectedNetworkInterface; }
            set 
            { 
                _selectedNetworkInterface = value;
                IPControlManager.m_SelectedNetworkInterface = _selectedNetworkInterface;
                IsDHCPEnabled = IPControlManager.IsDHCPInterface();
            }
        }

        private string _targetIP;
        public string TargetIP
        {
            get { return _targetIP; }
            set
            {
                _targetIP = value;
            }
        }


        private string _addressIPv4;
        public string AddressIPv4 
        {
            get { return _addressIPv4; }
            set 
            { 
                _addressIPv4 = value;
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
            NetworkInterfaces = IPControlManager.m_ArrayNetworkInterface;
            SelectedNetworkInterface = NetworkInterfaces[0];

            LogList = new ObservableCollection<string>();

            cmdWindow = new CmdWindow();
            WindowHeight = 400;
            WindowWidth = 250;
            IsStaticVisible = Visibility.Collapsed;

            IPControlManager.ReadStaticIPv4();

            AddressIPv4 = IPControlManager.m_sIPv4;
            AddressNetMask = IPControlManager.m_sMask;
            AddressGateway = IPControlManager.m_sGateway;
            AddressDNS = IPControlManager.m_sDNS;
            TargetIP = "192.168.0.101";
            isAdmin = IsAdministrator();
            if (!isAdmin)
                LogList.Add("[Error] Please execute as Admin");
        }

        private ICommand pingCommand;
        public ICommand PingCommand
        {
            get { return (this.pingCommand) ?? (this.pingCommand = new DelegateCommand(Ping)); }
        }
        // FIXIT
        private void Ping()
        {
            if (!IPControlManager.CheckValidAddress(TargetIP))
            {
                LogList.Add("[Error] Invalid Target IP");
                return;
            }
            bool bFlag = IPControlManager.PingTarget(TargetIP);
            LogList.Add("[Execute] Ping to " + TargetIP);

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
            IsStaticVisible = Visibility.Collapsed;
            WindowHeight = 400;
            bool bFlag = IPControlManager.SetDHCP();

            if (bFlag & isAdmin)
                LogList.Add("[Result] Set DHCP Success");
            else
                LogList.Add("[Result] Set DHCP Failed");
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
            IPControlManager.ReadStaticIPv4();
            AddressIPv4 = IPControlManager.m_sIPv4;
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
                LogList.Add("[Error] Invalid address");
                return;
            }
            LogList.Add("[Execute] Set static");
            LogList.Add("IP: " + IPControlManager.m_sIPv4);
            LogList.Add("Mask: " + IPControlManager.m_sMask);
            LogList.Add("GateWay: " + IPControlManager.m_sGateway);
            bool bFlag = false;
            if ((AddressDNS!=null)&&(AddressDNS==IPControlManager.m_sDNS))
            {
                LogList.Add("DNS: " + IPControlManager.m_sDNS);
                bFlag = IPControlManager.SetNetStaticAddress(true);
            }
            else
            {
                bFlag = IPControlManager.SetNetStaticAddress(false);
            }

            if (bFlag & isAdmin)
                LogList.Add("[Result] Set Static Success");
            else
                LogList.Add("[Result] Set Static Failed");
        }

        #endregion

        #region timer

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
