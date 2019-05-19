using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataObjects
{
    public enum MoveFrameState
    {
        Startup = 0,
        Active = 1,
        Recovery = 2,
        Invalid = 255
    }

    public class MoveData
    {
        public virtual string name
        {
            get { return "Move"; }
        }
        public virtual string sound
        {
            get { return "hit1"; }
        }
        public virtual bool canCancel
        {
            get { return false; }
        }
        public virtual short damage
        {
            get { return 0; }
        }
        public virtual int pushSpeed
        {
            get { return 250; }
        }
        public virtual int blockSpeed
        {
            get { return 250; }
        }
        public virtual short hitFrames
        {
            get { return 1; }
        }
        public virtual short blockFrames
        {
            get { return 1; }
        }
        public virtual BoundsInt bounds {
            get { return new BoundsInt(); }
        }
        public virtual MoveFrameState[] frames
        {
            get {
                return new MoveFrameState[] { MoveFrameState.Startup, MoveFrameState.Active, MoveFrameState.Recovery };
            }
        }
        public MoveFrameState GetFrame(int i)
        {
            if (i >= frames.Length)
            {
                return MoveFrameState.Invalid;
            }
            return frames[i];
        }
    }
}
