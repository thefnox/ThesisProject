using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataObjects
{
    public class JumpingHeavy : MoveData
    {
        public override string name => "Jumping Heavy";
        public override string sound => "hit3";
        public override int pushSpeed => 400;
        public override int blockSpeed => 100;
        public override short damage => 15;
        public override short hitFrames => 35;
        public override short blockFrames => 10;
        private BoundsInt _bounds = new BoundsInt(new Vector3Int(100, -50, 10), new Vector3Int(200, 50, 10));
        private MoveFrameState[] _frames = new MoveFrameState[]
        {
            MoveFrameState.Startup,
            MoveFrameState.Startup,
            MoveFrameState.Startup,
            MoveFrameState.Startup,
            MoveFrameState.Startup,
            MoveFrameState.Active,
            MoveFrameState.Active,
            MoveFrameState.Active,
            MoveFrameState.Active,
            MoveFrameState.Active,
            MoveFrameState.Active,
            MoveFrameState.Active,
            MoveFrameState.Active,
            MoveFrameState.Active,
            MoveFrameState.Active,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery,
            MoveFrameState.Recovery
        };
        public override BoundsInt bounds => _bounds;
        public override MoveFrameState[] frames => _frames;
    }
}
