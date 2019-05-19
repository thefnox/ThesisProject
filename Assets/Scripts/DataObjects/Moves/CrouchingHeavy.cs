using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataObjects
{
    public class CrouchingHeavy : MoveData
    {
        public override string name => "Crouching Heavy";
        public override string sound => "hit3";
        public override int pushSpeed => 800;
        public override int blockSpeed => 400;
        public override short damage => 18;
        public override short hitFrames => 26;
        public override short blockFrames => 15;
        private BoundsInt _bounds = new BoundsInt(new Vector3Int(120, 120, 10), new Vector3Int(50, 180, 10));
        private MoveFrameState[] _frames = new MoveFrameState[]
        {
            MoveFrameState.Startup,
            MoveFrameState.Startup,
            MoveFrameState.Startup,
            MoveFrameState.Startup,
            MoveFrameState.Startup,
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
