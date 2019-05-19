using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using Assets.Scripts.DataObjects;
using Assets.Scripts.StateObjects;
using UnityEngine;

[Flags]
public enum PlayerStates
{
    Neutral = 0,
    MoveRight = 1,
    MoveLeft = 2,
    Crouch = 4,
    Jump = 8,
    Attacking = 16,
    Dashing = 32,
    Blocking = 64,
    Hit = 128,
    Dead = 255
}

[System.Serializable]
public class PlayerState
{
    public PlayerFrame frame;
    public PlayerStates state {
        get
        {
            return (PlayerStates) frame.state;
        }
        set
        {
            frame.state = (short) value;
        }
    }

    public BoundsInt bounds
    {
        get
        {
            var crouchModifier = IsCrouching() ? CharacterData.sizeY / 2 : 0;
            var SizeY = CharacterData.sizeY - crouchModifier;
            var SizeX = CharacterData.sizeX;
            return new BoundsInt(
                new Vector3Int(PosX + SizeX / 2, PosY + SizeY / 2, 5), 
                new Vector3Int(SizeX, SizeY, 5)
            );
        }
    }

    public bool IsIntersecting(BoundsInt other)
    {
        return bounds.x <= other.x + other.size.x &&
            bounds.x + bounds.size.x >= other.x &&
            bounds.y <= other.y + other.size.y &&
            bounds.y + bounds.size.y >= other.y;
    }

    public bool WillIntersect(Vector3Int move, BoundsInt other)
    {
        var newBounds = new BoundsInt(bounds.position + move, bounds.size);
        return newBounds.x <= other.x + other.size.x &&
            newBounds.x + newBounds.size.x >= other.x &&
            newBounds.y <= other.y + other.size.y &&
            newBounds.y + newBounds.size.y >= other.y;
    }

    public int PosX
    {
        get
        {
            return frame.posX;
        }
        set
        {
            frame.posX = value;
        }
    }

    public int PosY
    {
        get
        {
            return frame.posY;
        }
        set
        {
            frame.posY = value;
        }
    }

    public PlayerState()
    {
        frame = new PlayerFrame();
        state = PlayerStates.Neutral;
    }

    public void ProcessMovement(int FrameTime, PlayerState opponent)
    {
        var OpponentPosX = opponent.PosX;
        int MoveX = 0;
        int MoveY = 0;
        if (IsMovingRight())
        {
            MoveX = (CharacterData.moveSpeed * FrameTime) / 100;
        }
        else if (IsMovingLeft())
        {
            MoveX = (-CharacterData.moveSpeed * FrameTime) / 100;
        }
        if (IsCrouching() || IsBlockStunned())
        {
            MoveX = 0;
        }
        if (frame.pushFrame > 0)
        {
            var move = GetMove(frame.hitBy);
            var pushSpeed = frame.hitBlocked ? move.blockSpeed : move.pushSpeed;
            MoveX = ((frame.isFacingRight ? -1 : 1) * FrameTime * pushSpeed) / 100;
            frame.pushFrame--;
        }
        if (IsAirborne())
        {
            var totalJumpTime = ++frame.jumpFrame * FrameTime;
            var deaccel = Constants.GRAVITY_VALUES[frame.jumpFrame - 1];
            MoveY = (CharacterData.jumpSpeed - deaccel) / 100;
            PosY = Math.Min(Math.Max(PosY + (CharacterData.jumpSpeed - deaccel) / 100, -Constants.BOUNDARY_Y), Constants.BOUNDARY_Y);
            PosX = Mathf.Min(Mathf.Max(PosX + MoveX, -Constants.BOUNDARY_X), Constants.BOUNDARY_X);
        }
        else
        {
            if (IsDashing())
            {
                if (frame.animFrame >= CharacterData.dashFrames)
                {
                    state = PlayerStates.Neutral;
                    frame.animFrame = 0;
                }
                else
                {
                    MoveX *= Math.Max((CharacterData.dashFrames / 2) - (frame.animFrame * 2), 0);
                    frame.animFrame++;
                }
            }
            if (opponent.IsGrounded() && WillIntersect(new Vector3Int(MoveX, MoveY, 0), opponent.bounds)) {
                MoveX = 0;
                PosX = OpponentPosX + opponent.bounds.size.x * (PosX < OpponentPosX ? -1 : 1);
            }
            PosY = Math.Min(Math.Max(PosY + MoveY, 0), Constants.BOUNDARY_Y);
            PosX = Mathf.Min(Mathf.Max(PosX + MoveX, -Constants.BOUNDARY_X), Constants.BOUNDARY_X);
        }
    }

    public void ProcessAttacks(int FrameTime, PlayerState opponent)
    {
        if (frame.animState != 0)
        {
            frame.animFrame++;
        }
        if ((IsBlockStunned() || IsHit()) && frame.pushFrame == 0)
        {
            state = state & ~PlayerStates.Hit;
            state = state & ~PlayerStates.Blocking;
            frame.hitBy = 0;
            frame.hitCount = 0;
            frame.hitBlocked = false;
        }
        if (IsAnimationOver())
        {
            state = state & ~PlayerStates.Attacking;
            frame.animState = 0;
            frame.animFrame = 0;
            frame.hitCount = 0;
            frame.hitBlocked = false;
            frame.didHit = false;
        }
    }

    public string ProcessHits(int FrameTime, PlayerState opponent)
    {
        if (IsAttacking() && !frame.didHit)
        {
            if (IsInActive())
            {
                var move = GetMove(frame.animState);
                var attackBounds = move.bounds;
                var orientation = frame.isFacingRight ? 1 : -1;
                var calcBounds = new BoundsInt(
                    new Vector3Int(bounds.position.x + attackBounds.position.x * orientation, bounds.position.y + attackBounds.position.y, 0), attackBounds.size);
                
                if (opponent.IsIntersecting(calcBounds))
                {
                    // Move hits.
                    short hitFrames = 0;
                    var hitSound = move.sound;
                    var blocked = false;
                    if ((IsAirborne() && opponent.IsBlockingHigh()) || (opponent.IsBlocking() && IsGrounded()))
                    {
                        // Move blocked
                        opponent.state = opponent.state | PlayerStates.Blocking;
                        hitFrames = move.blockFrames;
                        blocked = true;
                        hitSound = "block";
                    } else
                    {
                        opponent.state = opponent.state | PlayerStates.Hit;
                        hitFrames = move.hitFrames;
                        opponent.frame.health -= move.damage;
                        opponent.frame.hitCount++;
                    }
                    if (opponent.IsCornered() && IsGrounded())
                    {
                        // If the opponent is cornered and player is on the ground, push applies to the attacking player
                        frame.pushFrame = hitFrames;
                        frame.hitBy = frame.animState;
                        frame.hitBlocked = blocked;
                    }
                    opponent.frame.pushFrame = hitFrames;
                    opponent.frame.hitBy = frame.animState;
                    opponent.frame.hitBlocked = blocked;
                    frame.didHit = true;
                    return hitSound;
                }
            }
        }
        return null;
    }

    public MoveData GetMove(int i)
    {
        if (i <= 0) return null;
        return CharacterData.moveData[i - 1];
    }

    public bool IsAirborne()
    {
        return (this.state & PlayerStates.Jump) > 0;
    }

    public bool IsDashing()
    {
        return (this.state & PlayerStates.Dashing) > 0;
    }

    public bool IsAttacking()
    {
        return (this.state & PlayerStates.Attacking) > 0;
    }

    public bool IsCrouching()
    {
        return (this.state & PlayerStates.Crouch) > 0;
    }

    public bool IsBlockStunned()
    {
        return (this.state & PlayerStates.Blocking) > 0;
    }

    public bool IsMovingRight()
    {
        return (this.state & PlayerStates.MoveRight) > 0;
    }

    public bool IsMovingLeft()
    {
        return (this.state & PlayerStates.MoveLeft) > 0;
    }

    public bool IsMoving()
    {
        return IsMovingLeft() || IsMovingRight();
    }

    public bool IsGrounded()
    {
        return !IsAirborne();
    }

    public bool IsAnimationOver()
    {
        var move = GetMove(frame.animState);
        return move != null && move.frames.Length <= frame.animFrame;
    }

    public bool IsInStartup()
    {
        if (frame.animState == 0)
        {
            return false;
        }
        return GetMove(frame.animState).GetFrame(frame.animFrame) == MoveFrameState.Startup;
    }

    public bool IsInActive()
    {
        if (frame.animState == 0)
        {
            return false;
        }
        return GetMove(frame.animState).GetFrame(frame.animFrame) == MoveFrameState.Active;
    }

    public bool IsInRecovery()
    {
        if (frame.animState == 0)
        {
            return false;
        }
        return GetMove(frame.animState).GetFrame(frame.animFrame) == MoveFrameState.Recovery;
    }

    public bool CanCancel()
    {
        if (frame.animState == 0)
        {
            return false;
        }
        return GetMove(frame.animState).canCancel;
    }

    public bool IsHit()
    {
        return (this.state & PlayerStates.Hit) > 0;
    }

    public bool IsBlocking()
    {
        if (IsBlockStunned())
        {
            return true;
        }
        if (CanMove())
        {
            return (frame.isFacingRight && IsMovingLeft()) || (!frame.isFacingRight && IsMovingRight());
        }
        return false;
    }

    public bool IsBlockingHigh()
    {
        return IsBlocking() && !IsCrouching();
    }

    public bool IsCornered()
    {
        return Mathf.Abs(PosX) >= Constants.BOUNDARY_X;
    }

    public bool CanMove()
    {
        return !IsAirborne() && !IsDashing() && !IsAttacking() && !IsHit();
    }

    public bool CanAttack(PlayerState opponent)
    {
        if (IsHit() || IsDashing() || IsBlockStunned() || IsInActive() || IsInStartup())
        {
            return false;
        }
        if (IsInRecovery())
        {
            return opponent.IsHit() && IsGrounded() && CanCancel();
        }
        return true;
    }

    public void AttemptNeutral()
    {
        if (!CanMove())
        {
            return;
        }
        state = PlayerStates.Neutral;
        frame.animState = 0;
        frame.animFrame = 0;
        frame.hitCount = 0;
        frame.jumpFrame = 0;
    }

    public void AttemptDashLeft()
    {
        if (!CanMove())
        {
            return;
        }
        frame.animFrame = 0;
        state = (state | PlayerStates.Dashing | PlayerStates.MoveLeft) & ~PlayerStates.MoveRight;
    }

    public void AttemptDashRight()
    {
        if (!CanMove())
        {
            return;
        }
        frame.animFrame = 0;
        state = (state | PlayerStates.Dashing | PlayerStates.MoveRight) & ~PlayerStates.MoveLeft;
    }

    public void AttemptMoveRight()
    {
        if (!CanMove())
        {
            return;
        }
        state = state | PlayerStates.MoveRight & ~PlayerStates.MoveLeft;
    }

    public void AttemptMoveLeft()
    {
        if (!CanMove())
        {
            return;
        }
        state = state | PlayerStates.MoveLeft & ~PlayerStates.MoveRight;
    }

    public void AttemptCrouch()
    {
        if (!CanMove())
        {
            return;
        }
        state = state | PlayerStates.Crouch;
    }

    public void AttemptStandUp()
    {
        state = PlayerStates.Neutral;
    }

    public void AttemptJumpNeutral()
    {
        if (!CanMove())
        {
            return;
        }
        state = state | PlayerStates.Jump & ~PlayerStates.Crouch;
        frame.jumpFrame = 0;
    }

    public void AttemptAttack(short attack, PlayerState opponent)
    {
        if (!CanAttack(opponent))
        {
            return;
        }
        if (IsGrounded())
        {
            state = state & ~PlayerStates.MoveLeft;
            state = state & ~PlayerStates.MoveRight;
        }
        state = state | PlayerStates.Attacking;
        frame.animState = attack;
        frame.didHit = false;
        frame.animFrame = -1;
    }

    public void AttemptJumpRight()
    {
        if (!CanMove())
        {
            return;
        }
        state = (state | PlayerStates.Jump | PlayerStates.MoveRight) & ~PlayerStates.MoveLeft & ~PlayerStates.Crouch;
    }

    public void AttemptJumpLeft()
    {
        if (!CanMove())
        {
            return;
        }
        state = (state | PlayerStates.Jump | PlayerStates.MoveLeft) & ~PlayerStates.MoveRight & ~PlayerStates.Crouch;
    }

    public bool AttemptLand()
    {
        if (!IsAirborne() || frame.posY >= 0)
        {
            return false;
        }
        state = state & ~PlayerStates.Jump & ~PlayerStates.Attacking & ~PlayerStates.Hit & ~PlayerStates.Crouch;
        frame.animState = 0;
        frame.animFrame = 0;
        frame.pushFrame = 0;
        frame.hitBy = 0;
        frame.hitBlocked = false;
        frame.didHit = false;
        frame.jumpFrame = 0;
        frame.posY = 0;
        return true;
    }

    public PlayerState(PlayerFrame frame)
    {
        this.frame = new PlayerFrame(frame);
    }

}
