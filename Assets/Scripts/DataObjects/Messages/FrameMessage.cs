using System;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.DataObjects
{
    public class FrameMessage : MessageBase
    {
        public short frame;
        public byte[] inputs;
    }
}
