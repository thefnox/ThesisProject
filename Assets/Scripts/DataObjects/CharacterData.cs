using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.DataObjects
{
    static class CharacterData
    {
        public static int moveSpeed = 500;
        public static int jumpSpeed = 1000;
        public static int dashFrames = 12;
        public static int sizeX = 100;
        public static int sizeY = 200;
        public static MoveData[] moveData = new MoveData[]{
            // Standing Light
            new StandingLight(),
            // Standing Medium
            new StandingMedium(),
            // Standing Heavy
            new StandingHeavy(),
            // Crouching Light
            new CrouchingLight(),
            // Crouching Medium
            new CrouchingMedium(),
            // Crouching Heavy
            new CrouchingHeavy(),
            // Jumping Light
            new JumpingLight(),
            // Jumping Medium
            new JumpingMedium(),
            // Jumping Heavy
            new JumpingHeavy()
        };
    }
}
