using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataObjects
{
    public class JumpingMedium : MoveData
    {
        public override string name => "Jumping Medium";
        public override string sound => "hit2";
        public override bool canCancel => true;
        public override int pushSpeed => 300;
        public override int blockSpeed => 100;
        public override short damage => 9;
        public override short hitFrames => 28;
        public override short blockFrames => 8;
        private BoundsInt _bounds = new BoundsInt(new Vector3Int(80, -60, 10), new Vector3Int(50, 110, 10));
        private MoveFrameState[] _frames = new MoveFrameState[]
        {
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
