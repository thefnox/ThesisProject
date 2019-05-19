using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Struct for each one of the simulation frames
namespace Assets.Scripts.StateObjects
{
    [Serializable]
    public struct GameFrame
    {
        public PlayerFrame player1;
        public PlayerFrame player2;
        public InputFrame localInput;
        public InputFrame remoteInput;
        public int delayCount;

        public GameFrame(PlayerFrame player1, PlayerFrame player2, InputFrame localInput, InputFrame remoteInput, int delayCount = 0)
        {
            this.player1 = player1;
            this.player2 = player2;
            this.localInput = localInput;
            this.remoteInput = remoteInput;
            this.delayCount = delayCount;
        }
    }
}
