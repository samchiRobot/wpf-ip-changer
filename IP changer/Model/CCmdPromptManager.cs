using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace IP_changer.Model
{ 
    class CCmdPromptManager
    {
        private Process m_Process;
        private ProcessStartInfo m_ProcessInfo;
        private StringBuilder m_sbLog;
        public CCmdPromptManager()
        {
            m_ProcessInfo = new ProcessStartInfo();
            m_Process = new Process();
            m_sbLog = new StringBuilder();

            m_ProcessInfo.FileName = @"cmd";
            m_ProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;    // cmd창 숨기기
            m_ProcessInfo.CreateNoWindow = true;                      // cmd창 띄우지 않기

            m_ProcessInfo.UseShellExecute = false;
            m_ProcessInfo.RedirectStandardInput = true;               // cmd창에서 데이터 가져오기
            m_ProcessInfo.RedirectStandardOutput = true;              // cmd창으로 데이터 보내기
            m_ProcessInfo.RedirectStandardError = true;               // cmd창에서 오류내용 가져오기

            m_Process.EnableRaisingEvents = false;
            m_Process.StartInfo = m_ProcessInfo;
        }

        public void RunCmd(string sCmd)
        {
            this.m_Process.Start();
            this.m_Process.StandardInput.Write(sCmd + Environment.NewLine);
            this.m_Process.StandardInput.Close();
            this.WriteLog();
            this.m_Process.WaitForExit();
            this.m_Process.Close();
        }

        public string ReadLog()
        {
            return this.m_sbLog.ToString();
        }
        public void ClearLog()
        {
            this.m_sbLog.Clear();
        }

        private void WriteLog()
        {
            string sResult = this.m_Process.StandardOutput.ReadToEnd();
            this.m_sbLog.Append(sResult);
        }
    }
}
