using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirror;

namespace Assets.Scripts.DataObjects
{
    public class SetInputDelayMessage : MessageBase
    {
        public float delay;
        public bool useRollback;
    }
}
