using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.StateObjects
{
    public enum MoveInputs
    {
        DownLeft = 1,
        Down = 2,
        DownRight = 3,
        Left = 4,
        Neutral = 5,
        Right = 6,
        UpLeft = 7,
        Up = 8,
        UpRight = 9
    }

    [Flags]
    public enum ButtonInputs
    {
        None = 0,
        Light = 1,
        Medium = 2,
        Heavy = 4,
        Special = 8,
    }

    [Serializable]
    public class InputFrame
    {
        public MoveInputs moves;
        public ButtonInputs inputs;

        public bool IsNone
        {
            get { return inputs == ButtonInputs.None; }
        }

        public bool IsLight
        {
            get { return (inputs & ButtonInputs.Light) > 0; }
        }

        public bool IsMedium
        {
            get { return (inputs & ButtonInputs.Medium) > 0; }
        }

        public bool IsHeavy
        {
            get { return (inputs & ButtonInputs.Heavy) > 0; }
        }

        public bool IsSpecial
        {
            get { return (inputs & ButtonInputs.Special) > 0; }
        }

        public bool IsNeutral
        {
            get { return moves == MoveInputs.Neutral; }
        }

        public bool IsUp
        {
            get { return moves == MoveInputs.Up; }
        }

        public bool IsDown
        {
            get { return moves == MoveInputs.Down; }
        }

        public bool IsLeft
        {
            get { return moves == MoveInputs.Left; }
        }

        public bool IsRight
        {
            get { return moves == MoveInputs.Right; }
        }

        public bool IsUpLeft
        {
            get { return moves == MoveInputs.UpLeft; }
        }

        public bool IsUpRight
        {
            get { return moves == MoveInputs.UpRight; }
        }

        public bool IsDownLeft
        {
            get { return moves == MoveInputs.DownLeft; }
        }

        public bool IsDownRight
        {
            get { return moves == MoveInputs.DownRight; }
        }

        public string InputString
        {
            get
            {
                string moveStr = "N";
                List<string> buttons = new List<string>();

                if (IsUpRight)
                {
                    moveStr = "↗";
                }
                else if (IsUpLeft)
                {
                    moveStr = "↖";
                }
                else if (IsDownLeft)
                {
                    moveStr = "↙";
                }
                else if (IsDownRight)
                {
                    moveStr = "↘";
                }
                else if (IsUp)
                {
                    moveStr = "↑";
                }
                else if (IsRight)
                {
                    moveStr = "→";
                }
                else if (IsLeft)
                {
                    moveStr = "←";
                }
                else if (IsDown)
                {
                    moveStr = "↓";
                }

                if (IsLight)
                {
                    buttons.Add("L");
                }
                if (IsMedium)
                {
                    buttons.Add("M");
                }
                if (IsHeavy)
                {
                    buttons.Add("H");
                }
                if (IsSpecial)
                {
                    buttons.Add("S");
                }

                return "" + moveStr + string.Join(" + ", buttons);
            }
        }

        public InputFrame()
        {
            moves = MoveInputs.Neutral;
            inputs = ButtonInputs.None;
        }

        public InputFrame(short input)
        {
            moves = (MoveInputs) (input >> 4);
            inputs = (ButtonInputs)(input & 15);
        }

        public InputFrame(byte input)
        {
            moves = (MoveInputs)(input >> 4);
            inputs = (ButtonInputs)(input & 15);
        }

        public InputFrame(InputFrame frame)
        {
            moves = frame.moves;
            inputs = frame.inputs;
        }

        public void MakeRandom()
        {
            inputs = (ButtonInputs) (UnityEngine.Random.value > 0.9f ? UnityEngine.Random.Range(0, 8) : 0);
            moves = (MoveInputs) UnityEngine.Random.Range(1, 10);
        }

        public InputFrame(MoveInputs move, ButtonInputs input)
        {
            moves = move;
            inputs = input;
        }

        public byte GetInput()
        {
            return (byte) (((byte) moves << 4) + (byte) inputs);
        }

        public override string ToString()
        {
            return "" + InputString;
        }
    }
}
