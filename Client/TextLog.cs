using System;
using System.Collections.Generic;
using System.Text;

namespace BlowtorchesAndGunpowder
{
    public class TextLog
    {
        private List<String> fLog = new List<string>();
        public void AddLog(String aLog)
        {
            fLog.Add(DateTime.Now.ToString("HH:mm:ss") + " " + aLog);
            if(fLog.Count > 15)
            fLog.RemoveRange(0, fLog.Count -15);
        }
        public String[] GetLog()
        {
            //string result = String.Join(Environment.NewLine, fLog.ToArray()) + Environment.NewLine;
            return fLog.ToArray();
        }
    }
}
