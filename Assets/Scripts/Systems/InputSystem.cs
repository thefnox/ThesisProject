using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.StateObjects;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public class InputSystem
    {
        public InputSystem()
        {

        }

        public void ProcessInput(InputFrame prevInput, InputFrame curInput, PlayerState state, PlayerState opponent)
        {
            var curMove = curInput.moves;
            var curButtons = curInput.inputs;
            var prevMove = prevInput.moves;
            var prevButtons = prevInput.inputs;

            if ((curButtons ^ prevButtons) > 0)
            {
                if (state.IsAirborne())
                {
                    if ((curButtons & ButtonInputs.Special) > 0 && (prevButtons & ButtonInputs.Special) == 0)
                    {
                        // Special
                        return;
                    }
                    else if ((curButtons & ButtonInputs.Heavy) > 0 && (prevButtons & ButtonInputs.Heavy) == 0)
                    {
                        // Heavy
                        state.AttemptAttack(9, opponent);
                        return;
                    }
                    else if ((curButtons & ButtonInputs.Medium) > 0 && (prevButtons & ButtonInputs.Medium) == 0)
                    {
                        // Medium
                        state.AttemptAttack(8, opponent);
                        return;
                    }
                    else if ((curButtons & ButtonInputs.Light) > 0 && (prevButtons & ButtonInputs.Light) == 0)
                    {
                        // Light
                        state.AttemptAttack(7, opponent);
                        return;
                    }
                }
                else if (state.IsCrouching())
                {
                    if ((curButtons & ButtonInputs.Special) > 0 && (prevButtons & ButtonInputs.Special) == 0)
                    {
                        // Special
                        return;
                    }
                    else if ((curButtons & ButtonInputs.Heavy) > 0 && (prevButtons & ButtonInputs.Heavy) == 0)
                    {
                        // Heavy
                        state.AttemptAttack(6, opponent);
                        return;
                    }
                    else if ((curButtons & ButtonInputs.Medium) > 0 && (prevButtons & ButtonInputs.Medium) == 0)
                    {
                        // Medium
                        state.AttemptAttack(5, opponent);
                        return;
                    }
                    else if ((curButtons & ButtonInputs.Light) > 0 && (prevButtons & ButtonInputs.Light) == 0)
                    {
                        // Light
                        state.AttemptAttack(4, opponent);
                        return;
                    }
                } else
                {
                    if ((curButtons & ButtonInputs.Special) > 0 && (prevButtons & ButtonInputs.Special) == 0)
                    {
                        // Special
                        return;
                    }
                    else if ((curButtons & ButtonInputs.Heavy) > 0 && (prevButtons & ButtonInputs.Heavy) == 0)
                    {
                        // Heavy
                        state.AttemptAttack(3, opponent);
                        return;
                    }
                    else if ((curButtons & ButtonInputs.Medium) > 0 && (prevButtons & ButtonInputs.Medium) == 0)
                    {
                        // Medium
                        state.AttemptAttack(2, opponent);
                        return;
                    }
                    else if ((curButtons & ButtonInputs.Light) > 0 && (prevButtons & ButtonInputs.Light) == 0)
                    {
                        // Light
                        state.AttemptAttack(1, opponent);
                        return;
                    }
                }
            }
            if (curMove == MoveInputs.Up)
            {
                state.AttemptJumpNeutral();
                return;
            }
            else if (curMove == MoveInputs.UpRight)
            {
                state.AttemptJumpRight();
                return;
            }
            else if (curMove == MoveInputs.UpLeft)
            {
                state.AttemptJumpLeft();
                return;
            }
            if (curMove == MoveInputs.DownRight || curMove == MoveInputs.Down || curMove == MoveInputs.DownLeft)
            {
                state.AttemptCrouch();
                if (curMove == MoveInputs.DownRight)
                {
                    state.AttemptMoveRight();
                }
                if (curMove == MoveInputs.DownLeft)
                {
                    state.AttemptMoveLeft();
                }
                return;
            }
            else if (state.IsCrouching())
            {
                state.AttemptStandUp();
            }
            if (curMove == MoveInputs.Right)
            {
                state.AttemptMoveRight();
                return;
            }
            if (curMove == MoveInputs.Left)
            {
                state.AttemptMoveLeft();
                return;
            }
            state.AttemptNeutral();
        }

        public void CalcDirection(PlayerState player1, PlayerState player2)
        {
            player1.frame.isFacingRight = player1.frame.posX < player2.frame.posX;
            player2.frame.isFacingRight = !player1.frame.isFacingRight;
        }
             
        public InputFrame GetCurrentInput()
        {
            InputFrame frame = new InputFrame();
            if (Input.GetButton("Light"))
            {
                frame.inputs = frame.inputs | ButtonInputs.Light;
            }
            if (Input.GetButton("Medium"))
            {
                frame.inputs = frame.inputs | ButtonInputs.Medium;
            }
            if (Input.GetButton("Heavy"))
            {
                frame.inputs = frame.inputs | ButtonInputs.Heavy;
            }
            if (Input.GetButton("Special"))
            {
                frame.inputs = frame.inputs | ButtonInputs.Special;
            }
            var curVert = Input.GetAxis("Vertical");
            var curHorz = Input.GetAxis("Horizontal");

            if (curVert >= 0.5f)
            {
                if (curHorz >= 0.5f)
                {
                    frame.moves = MoveInputs.UpRight;
                }
                else if (curHorz <= -0.5f)
                {
                    frame.moves = MoveInputs.UpLeft;
                }
                else
                {
                    frame.moves = MoveInputs.Up;
                }
            } else if (curVert <= -0.5f)
            {
                if (curHorz >= 0.5f)
                {
                    frame.moves = MoveInputs.DownRight;
                }
                else if (curHorz <= -0.5f)
                {
                    frame.moves = MoveInputs.DownLeft;
                }
                else
                {
                    frame.moves = MoveInputs.Down;
                }
            } else {
                if (curHorz >= 0.5f)
                {
                    frame.moves = MoveInputs.Right;
                } else if (curHorz <= -0.5f)
                {
                    frame.moves = MoveInputs.Left;
                } else
                {
                    frame.moves = MoveInputs.Neutral;
                }
            }
            return frame;
        }
    }
}
