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
            fLog.Add(aLog);
        }
        public string PullLog()
        {
            string result = String.Join(Environment.NewLine, fLog.ToArray()) + Environment.NewLine;
            fLog.Clear();
            return result;
        }
    }
}
