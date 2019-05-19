using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.DataObjects;
using Assets.Scripts.StateObjects;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    public class GameSystem
    {
        public static GameFrame[] frameData = new GameFrame[Constants.GAME_BUFFER_SIZE];
        public static InputFrame[] inputData = new InputFrame[Constants.GAME_BUFFER_SIZE];
        public delegate void OnPlayerHit(string sound);
        public OnPlayerHit player1HitCallback;
        public OnPlayerHit player2HitCallback;

        public short rollbackFrames
        {
            get
            {
                return NetworkController.Instance.rollbackFrames;
            }
            set
            {
                NetworkController.Instance.rollbackFrames = value;
            }
        }
        public short inputBufferSize;
        public int delayCount = 0;
        public InputSystem inputSystem = new InputSystem();
        public int sentFrame = -1;
        public short currentFrame {
            get
            {
                return NetworkController.Instance.currentFrame;
            }
            set
            {
                NetworkController.Instance.currentFrame = value;
            }
        }
        public bool localPlayerOne = true;
        public bool paused = false;

        public GameFrame initializeFrame(int frame)
        {
            var newFrame = new GameFrame(
                new PlayerFrame(true, -Constants.START_POS_X),
                new PlayerFrame(false, Constants.START_POS_X),
                new InputFrame(),
                new InputFrame()
            );
            frameData[frame] = newFrame;
            return newFrame;
        }

        public GameFrame initializeFrame(int frame, PlayerFrame player1, PlayerFrame player2, InputFrame local, InputFrame remote, int delayCount = 0)
        {
            var newFrame = new GameFrame(player1, player2, local, remote, delayCount);
            frameData[frame] = newFrame;
            return newFrame;
        }

        public GameFrame GetCurrentFrame()
        {
            short actualFrame = (short)(currentFrame - rollbackFrames);
            return frameData[actualFrame];
        }

        public PlayerState GetCurrentPlayerOneState()
        {
            short actualFrame = (short)(currentFrame - rollbackFrames);
            return new PlayerState(frameData[actualFrame].player1);
        }

        public PlayerState GetCurrentPlayerTwoState()
        {
            short actualFrame = (short)(currentFrame - rollbackFrames);
            return new PlayerState(frameData[actualFrame].player2);
        }

        public void Initialize(OnPlayerHit player1Hit, OnPlayerHit player2Hit)
        {
            var localIs1 = NetworkController.Instance.localPlayerIsPlayer1;
            paused = false;
            for (int i=0; i<inputData.Length; i++)
            {
                inputData[i] = new InputFrame();
            }
            player1HitCallback = player1Hit;
            player2HitCallback = player2Hit;
        }

        public void ProcessFrame(int frameTime)
        {
            var isRollingBack = rollbackFrames > 0;
            short actualFrame = (short) (currentFrame - rollbackFrames);
            var localIs1 = NetworkController.Instance.localPlayerIsPlayer1;
            if (paused 
                || NetworkController.Instance.delayFrames > 0
                || !NetworkController.Instance.HasNextInput(actualFrame)
                || !NetworkController.Instance.ready)
            {
                if (NetworkController.Instance.delayFrames > 0)
                {
                    NetworkController.Instance.delayFrames--;
                    Debug.Log("Simulation delayed");
                }
                else
                {
                    Debug.Log("Simulation waiting");
                }
                delayCount++;
                if (!IsGameOver())
                {
                    NetworkController.Instance.totalMsDelayed += Time.fixedDeltaTime;
                    NetworkController.Instance.totalFramesDelayed++;
                } else
                {
                    // network last frame
                    NetworkController.Instance.SendInputs(actualFrame, inputData);
                }
                return;
            }
            var prevFrame = frameData[actualFrame];
            var currentInput = inputData[actualFrame];
            var prevInput = actualFrame > 0 ? inputData[actualFrame - 1] : new InputFrame();
            var currentRemoteInput = NetworkController.Instance.GetNetworkInput(actualFrame);
            var prevRemoteInput = NetworkController.Instance.GetNetworkInput(actualFrame - 1);

            var player1Input = localIs1 ? currentInput : currentRemoteInput;
            var player1PrevInput = localIs1 ? prevInput : prevRemoteInput;
            var player2Input = localIs1 ? currentRemoteInput : currentInput;
            var player2PrevInput = localIs1 ? prevRemoteInput : prevInput;
            var curPlayer1 = new PlayerState(prevFrame.player1);
            var curPlayer2 = new PlayerState(prevFrame.player2);

            if (!isRollingBack && sentFrame != actualFrame)
            {
                // Don't actually rewrite or send any inputs if we're running the simulation forward after a rollback
                inputData[actualFrame + inputBufferSize - 1] = inputSystem.GetCurrentInput();
                NetworkController.Instance.SendInputs(actualFrame, inputData);
                sentFrame = actualFrame;
            }

            inputSystem.ProcessInput(player1Input, player1PrevInput, curPlayer1, curPlayer2);
            inputSystem.ProcessInput(player2Input, player2PrevInput, curPlayer2, curPlayer1);

            curPlayer1.ProcessMovement(frameTime, curPlayer2);
            curPlayer2.ProcessMovement(frameTime, curPlayer1);
            curPlayer1.ProcessAttacks(frameTime, curPlayer2);
            curPlayer2.ProcessAttacks(frameTime, curPlayer1);
            var player1Hit = curPlayer1.ProcessHits(frameTime, curPlayer2);
            var player2Hit = curPlayer2.ProcessHits(frameTime, curPlayer1);

            if (curPlayer1.AttemptLand() || curPlayer2.AttemptLand())
            {
                inputSystem.CalcDirection(curPlayer1, curPlayer2);
            }
            if (rollbackFrames > 0)
            {
                rollbackFrames--;
                delayCount++;
                NetworkController.Instance.totalFramesDelayed++;
            } else {
                delayCount = 0;
                currentFrame++;
            }
            if (player1Hit != null && player1HitCallback != null)
            {
                player1HitCallback(player1Hit);
                NetworkController.Instance.totalHits++;
                NetworkController.Instance.maxCombo = Mathf.Max(NetworkController.Instance.maxCombo, curPlayer1.frame.hitCount);
            }
            if (player2Hit != null && player2HitCallback != null)
            {
                player2HitCallback(player2Hit);
                NetworkController.Instance.totalHits++;
                NetworkController.Instance.maxCombo = Mathf.Max(NetworkController.Instance.maxCombo, curPlayer2.frame.hitCount);
            }
            initializeFrame(actualFrame + 1, curPlayer1.frame, curPlayer2.frame, currentInput, currentRemoteInput, delayCount);
        }

        public bool IsGameOver()
        {
            short actualFrame = (short)(currentFrame - rollbackFrames);
            var frame = frameData[actualFrame];
            var player1 = frame.player1;
            var player2 = frame.player2;

            if (player2.health <= 0 || player1.health <= 0)
            {
                return true;
            }
            return actualFrame >= Constants.GAME_TIME 
                || actualFrame >= (Constants.GAME_BUFFER_SIZE - 1)
                || NetworkController.Instance.disconnected;
        }

        public string WinCondition()
        {
            short actualFrame = (short)(currentFrame - rollbackFrames);
            var frame = frameData[actualFrame];
            var player1 = frame.player1;
            var player2 = frame.player2;

            if (player1.health > player2.health)
            {
                return "Player 1 Wins";
            }
            else if (player2.health > player1.health)
            {
                return "Player 2 Wins"; 
            }
            return "Draw";
        }

        public GameSystem(short bufferSize = Constants.INPUT_BUFFER_SIZE)
        {
            inputBufferSize = bufferSize;
            frameData = new GameFrame[Constants.GAME_BUFFER_SIZE];
            initializeFrame(0);
        }
    }
}
