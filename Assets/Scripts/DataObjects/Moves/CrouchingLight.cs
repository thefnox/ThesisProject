using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataObjects
{
    public class CrouchingLight : MoveData
    {
        public override string name => "Crouching Light";
        public override string sound => "hit1";
        public override bool canCancel => true;
        public override int pushSpeed => 200;
        public override int blockSpeed => 70;
        public override short damage => 4;
        public override short hitFrames => 11;
        public override short blockFrames => 7;
        private BoundsInt _bounds = new BoundsInt(new Vector3Int(50, 40, 10), new Vector3Int(150, 50, 10));
        private MoveFrameState[] _frames = new MoveFrameState[]
        {
            MoveFrameState.Startup,
            MoveFrameState.Startup,
            MoveFrameState.Startup,
            MoveFrameState.Startup,
            MoveFrameState.Active,
            MoveFrameState.Active,
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
