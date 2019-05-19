using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.StateObjects
{
    [Serializable]
    public struct PlayerFrame
    {
        public bool isFacingRight;
        public bool didHit;
        public bool hitBlocked;
        public int posX;
        public int posY;
        public short state;
        public short health;
        public short animFrame;
        public short animState;
        public short hitBy;
        public short jumpFrame;
        public short pushFrame;
        public short hitCount;

        public PlayerFrame(
            bool isFacingRight = true,
            int posX = Constants.START_POS_X,
            int posY = Constants.START_POS_Y,
            short state = 0,
            short health = 100,
            short animFrame = 0,
            short animState = 0,
            short jumpFrame = 0,
            short pushFrame = 0,
            bool didHit = false,
            short hitBy = 0,
            short hitCount = 0,
            bool hitBlocked = false
        )
        {
            this.isFacingRight = isFacingRight;
            this.posX = posX;
            this.posY = posY;
            this.state = state;
            this.health = health;
            this.animFrame = animFrame;
            this.animState = animState;
            this.jumpFrame = jumpFrame;
            this.pushFrame = pushFrame;
            this.didHit = didHit;
            this.hitBy = hitBy;
            this.hitCount = hitCount;
            this.hitBlocked = hitBlocked;
        }

        public PlayerFrame(PlayerFrame frame)
        {
            isFacingRight = frame.isFacingRight;
            posX = frame.posX;
            posY = frame.posY;
            state = frame.state;
            health = frame.health;
            animFrame = frame.animFrame;
            animState = frame.animState;
            jumpFrame = frame.jumpFrame;
            pushFrame = frame.pushFrame;
            didHit = frame.didHit;
            hitBy = frame.hitBy;
            hitCount = frame.hitCount;
            hitBlocked = frame.hitBlocked;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PlayerFrame))
            {
                return false;
            }

            var frame = (PlayerFrame)obj;
            return isFacingRight == frame.isFacingRight &&
                   posX == frame.posX &&
                   posY == frame.posY &&
                   state == frame.state &&
                   health == frame.health &&
                   animFrame == frame.animFrame &&
                   animState == frame.animState &&
                   jumpFrame == frame.jumpFrame &&
                   pushFrame == frame.pushFrame &&
                   didHit == frame.didHit &&
                   hitBy == frame.hitBy &&
                   hitCount == frame.hitCount &&
                   hitBlocked == frame.hitBlocked;
        }

        public override string ToString()
        {
            return "Facing: " + (isFacingRight ? "Right" : "Left") +
            ", PosX: " + posX +
            ", PosY: " + posY +
            ", state: " + state +
            ", health: " + health +
            ", animFrame: " + animFrame +
            ", jumpFrame: " + jumpFrame +
            ", pushFrame: " + pushFrame +
            ", didHit: " + didHit.ToString() +
            ", hitBy: " + hitBy +
            ", hitCount: " + hitCount +
            ", hitBlocked: " + hitBlocked;
        }

        public override int GetHashCode()
        {
            var hashCode = 554832929;
            hashCode = hashCode * -1521134295 + isFacingRight.GetHashCode();
            hashCode = hashCode * -1521134295 + posX.GetHashCode();
            hashCode = hashCode * -1521134295 + posY.GetHashCode();
            hashCode = hashCode * -1521134295 + state.GetHashCode();
            hashCode = hashCode * -1521134295 + health.GetHashCode();
            hashCode = hashCode * -1521134295 + animFrame.GetHashCode();
            hashCode = hashCode * -1521134295 + animState.GetHashCode();
            hashCode = hashCode * -1521134295 + jumpFrame.GetHashCode();
            hashCode = hashCode * -1521134295 + pushFrame.GetHashCode();
            hashCode = hashCode * -1521134295 + didHit.GetHashCode();
            hashCode = hashCode * -1521134295 + hitBy.GetHashCode();
            hashCode = hashCode * -1521134295 + hitCount.GetHashCode();
            hashCode = hashCode * -1521134295 + hitBlocked.GetHashCode();
            return hashCode;
        }
    }
}
