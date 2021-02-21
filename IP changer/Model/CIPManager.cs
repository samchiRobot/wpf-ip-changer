using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace IP_changer.Model
{
    class CIPManager
    {
        private CCmdPromptManager m_cCmdPromptManager;

        private NetworkInterface[] _ArrayNetworkInterface;
        public NetworkInterface[] m_ArrayNetworkInterface
        {
            get
            { 
                return _ArrayNetworkInterface; 
            }
            set
            { 
                _ArrayNetworkInterface = value; 
            }
        }
        private NetworkInterface _SelectedNetworkInterface;
        public NetworkInterface m_SelectedNetworkInterface
        {
            get
            {
                return _SelectedNetworkInterface;
            }
            set
            {
                _SelectedNetworkInterface = value;
            }
        }

        private string _sIPv4;
        public string m_sIPv4
        {
            get
            {
                return _sIPv4;
            }
            set
            {
                if (CheckValidAddress(value))
                    _sIPv4 = value;
            }
        }

        private string _sMask;
        public string m_sMask
        {
            get
            {
                return _sMask;
            }
            set
            {
                if (CheckValidAddress(value))
                    _sMask = value;
            }
        }

        private string _sGateway;
        public string m_sGateway
        {
            get
            {
                return _sGateway;
            }
            set
            {
                if (CheckValidAddress(value))
                    _sGateway = value;
            }
        }


        private string _sDNS;
        public string m_sDNS
        {
            get
            {
                return _sDNS;
            }
            set
            {
                if (CheckValidAddress(value))
                    _sDNS = value;
            }
        }

        private const int m_IPv4MaxSize = 15;
        private const int m_AddrMaxParse = 4;

        public CIPManager()
        {
            this.m_cCmdPromptManager = new CCmdPromptManager();
            this.m_ArrayNetworkInterface = NetworkInterface.GetAllNetworkInterfaces();   // 사용가능한 모든 네트워크 interface를 반환
            m_sIPv4 = "192.168.0.1";
            m_sMask = "255.255.255.0";
            m_sGateway = "192.168.0.1";
            m_sDNS = "";
        }
        public bool PingTarget(string sPingTarget, int nTimeOut = 120, string sData = "PING")
        {
            Ping ping = new Ping();
            PingOptions pingOptions = new PingOptions();
            pingOptions.DontFragment = true;

            byte[] buffer = ASCIIEncoding.ASCII.GetBytes(sData);
            try
            {
                PingReply reply = ping.Send(IPAddress.Parse(sPingTarget), nTimeOut, buffer, pingOptions);
                if (reply.Status == IPStatus.Success)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool SetNetStaticAddress(bool bSetDNS)
        {
            if (m_sIPv4 == null | m_sMask == null)
                return false;
            string sCmd = "";
            sCmd = "netsh interface ip set address name=\"" + m_SelectedNetworkInterface.Name + "\" static " + m_sIPv4 + " " + m_sMask + " " + m_sGateway;
            m_cCmdPromptManager.RunCmd(sCmd);
            if(bSetDNS)
            {
                sCmd = "netsh interface ip add dns name=\"" + m_SelectedNetworkInterface.Name + "\" " + m_sDNS;
                m_cCmdPromptManager.RunCmd(sCmd);
            }
            return true;
        }

        public bool IsDHCPInterface()
        {
            return m_SelectedNetworkInterface.GetIPProperties().GetIPv4Properties().IsDhcpEnabled;
        }

        public bool ReadStaticIPv4()
        {
            if (IsDHCPInterface())
                return false;
            m_sIPv4 = m_SelectedNetworkInterface.GetIPProperties().UnicastAddresses[1].Address.ToString();
            return true;
        }

        public bool SetDHCP()
        {
            if (m_SelectedNetworkInterface == null)
                return false;
            string sCmd = "netsh interface ip set address name=\"" + m_SelectedNetworkInterface.Name + "\" source=dhcp";
            m_cCmdPromptManager.RunCmd(sCmd);
            sCmd = "netsh interface ip set dns name=\"" + m_SelectedNetworkInterface.Name + "\" source=dhcp";
            m_cCmdPromptManager.RunCmd(sCmd);

            return true;
        }
        public void GetIP()
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addr = ipEntry.AddressList;
            foreach(var item in addr)
            {
                if(CheckIPv4(item))
                {
                    Console.WriteLine("IP:{0}", item.ToString());
                }
            }
        }
        public string[] GetAdapterList()
        {
            List<string> sListAdapter = new List<string>(); 
            foreach (var net in m_ArrayNetworkInterface)
            {
                sListAdapter.Add(net.Name);
            }
            return sListAdapter.ToArray();
        }
        public bool SelectAdapter(int nIdx)
        {
            if (m_ArrayNetworkInterface == null)
                return false;
            m_SelectedNetworkInterface = m_ArrayNetworkInterface[nIdx];
            return true;
        }
        public bool CheckIPv4(IPAddress iPAddress)
        {
            if (iPAddress.ToString().Length > m_IPv4MaxSize)
                return false;
            return true;
        }
        public bool CheckValidAddress(string sAddress)
        {
            if (sAddress == null)
                return false;
            string[] vs = sAddress.Split('.');
            if (vs.Length > m_AddrMaxParse)
                return false;
            foreach(var item in vs)
            {
                try
                {
                    var nItem = Convert.ToInt32(item);
                    if (nItem > 255 || nItem < 0)
                        return false;
                }
                catch(FormatException)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
