using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace BlowtorchesAndGunpowder
{
    public class Client
    {
        public int fIndex;
        public IPEndPoint fEndPoint;
        public string fName;

        public Client(int aIndex, IPEndPoint aEndPoint,  string aName)
        {
            this.fIndex = aIndex;
            this.fEndPoint = aEndPoint;
            this.fName = aName;
        }
    }
}
