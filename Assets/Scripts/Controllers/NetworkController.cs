using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Assets;
using Assets.Scripts;
using Assets.Scripts.DataObjects;
using Assets.Scripts.StateObjects;
using Assets.Scripts.Systems;
using Mirror;
using UnityEngine;
using System.IO;
using UnityEditor;

// Singleton that controls everything related to networking inputs
public class NetworkController : MonoBehaviour
{
    // Attributes used for the Singleton implementation
    private static bool shuttingDown = false;
    private static object _lock = new object();
    private static NetworkController _instance;
    
    // Input frames received from the network
    public InputFrame[] opponentFrames;
    // Amount of time to wait before sending another frame data packet.
    public float artificialLagMs = 0f;
    // Probability of avoiding
    public float artificialPacketDrop = 0f;
    // Connection ID of the opponent
    public int opponentId = 2;
    // Current local simulation frame
    public short currentFrame = 0;
    // Current number of frames sent by the opponent
    public short opponentFrameCount = 0;
    // Current frame the opponent simulation says it is on
    public short opponentFrame = 0;
    // Frames to wait due to a delay request
    public short delayFrames = 0;
    // Amount of frames already predicted;
    public short predictedFrames = 0;
    // Amount of frames to run forward due to rollback
    public short rollbackFrames = 0;
    // Amount of frames to wait before sending another delay request
    public short requestTimer = 0;
    // Frame to request
    public short requestFrame = 0;
    // Total amount of frames spent unable to act
    public short totalFramesDelayed = 0;
    // Total amount of time left unable to act
    public float totalMsDelayed = 0.0f;
    // Maximum combo in this round
    public int maxCombo = 0;
    // Total amount of hits in this round
    public int totalHits = 0;
    // Total amount of frames dropped
    public int totalDropped = 0;
    // Amount of frames to buffer local player input for
    public short inputDelay = Constants.INPUT_BUFFER_SIZE;
    // Is the local client the host
    public bool server = true;
    // Is the simulation ready to start
    public bool ready = false;
    // Employ rollbacks or delay?
    public bool useRollback = false;
    // Is the local client on the player 1 side
    public bool localPlayerIsPlayer1 = true;
    // Is the remote player disconnected;
    public bool disconnected = false;

    public delegate void OnConnectionSuccessful();
    public delegate void OnConnectionFailed();

    public static NetworkController Instance
    {
        get
        {
            if (shuttingDown) return null;
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (NetworkController)FindObjectOfType(typeof(NetworkController));
                    if (_instance == null)
                    {
                        var obj = new GameObject();
                        _instance = obj.AddComponent<NetworkController>();
                        obj.name = "NetworkController";
                        DontDestroyOnLoad(obj);
                    }
                }
                return _instance;
            }
        }
    }

    private void Awake()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 60;
        var manager = CustomNetworkManager.singleton;
        manager.StartHost();
        NetworkServer.RegisterHandler<SetInputDelayMessage>(OnInputDelaySet);
        NetworkServer.RegisterHandler<FrameMessage>(OnFrameMessage);
        NetworkServer.RegisterHandler<RequestFrameMessage>(OnRequestFrameMessage);
        NetworkClient.RegisterHandler<SetInputDelayMessage>(OnInputDelaySet);
        NetworkClient.RegisterHandler<FrameMessage>(OnFrameMessage);
        NetworkClient.RegisterHandler<RequestFrameMessage>(OnRequestFrameMessage);
        opponentFrames = new InputFrame[Constants.GAME_BUFFER_SIZE];

        DontDestroyOnLoad(this.gameObject);
    }

    public void InitNewRound()
    {
        Debug.Log("Starting new round");
        for (int i = 0; i < opponentFrames.Length; i++)
        {
            opponentFrames[i] = null;
        }
        opponentFrames[0] = new InputFrame();
        disconnected = false;
        delayFrames = 0;
        totalFramesDelayed = 0;
        totalMsDelayed = 0f;
        rollbackFrames = 0;
        requestTimer = 0;
        requestFrame = 0;
        totalDropped = 0;
        currentFrame = 0;
        opponentFrameCount = 1;
        opponentFrame = 0;
    }

    public void EndRound()
    {
        DumpFrameData();
        var manager = CustomNetworkManager.singleton;
        manager.StopClient();
        if (server)
        {
            manager.StopHost();
        }
        manager.StartHost();
    }

    public void SetupClient(string hostname, OnConnectionSuccessful successful, OnConnectionFailed failed)
    {
        var manager = CustomNetworkManager.singleton;
        manager.StopHost();
        while (NetworkServer.active)
        {
            //Waiting for the server to die
        }
        manager.networkAddress = hostname;
        manager.StartClient();
    }

    public void SendFrameRequest(short frame)
    {
        var msg = new RequestFrameMessage()
        {
            frame = frame
        };
        SendMessage<RequestFrameMessage>(msg);
    }

    public void SendInputDelay(float delay)
    {
        var msg = new SetInputDelayMessage()
        {
            delay = delay,
            useRollback = this.useRollback
        };
        SendMessage<SetInputDelayMessage>(msg);
    }

    public void SendMessage<T>(MessageBase msg) where T : MessageBase
    {
        if (server)
        {
            NetworkServer.SendToClient(opponentId, (T)msg);
        } else
        {
            NetworkClient.Send((T)msg);
        }
    }

    public void SendInputs(short frame, InputFrame[] data)
    {
        var buffer = new byte[inputDelay];
        for (int i=0; i<buffer.Length; i++)
        {
            buffer[i] = data[frame + i].GetInput();
        }
        StartCoroutine(SendInputMessage(frame, buffer));
    }

    IEnumerator SendInputMessage(short frame, byte[] frames)
    {
        var msg = new FrameMessage()
        {
            frame = frame,
            inputs = frames
        };
        if (artificialLagMs > 0.0f)
        {
            yield return new WaitForSecondsRealtime((artificialLagMs / 1000f) - Time.fixedDeltaTime);
        }
        if (artificialPacketDrop > 0.0f)
        {
            if (UnityEngine.Random.value > artificialPacketDrop)
            {
                SendMessage<FrameMessage>(msg);
            }
        } else
        {
            SendMessage<FrameMessage>(msg);
        }
        yield return null;
    }

    public void OnRequestFrameMessage(NetworkConnection con, RequestFrameMessage message)
    {
        var frame = message.frame;
        Debug.Log("Frame " + frame + " requested by remote player");
        if (frame <= currentFrame)
        {
            SendInputs(frame, GameSystem.inputData);
        } else
        {
            Debug.Log("Frame " + frame + " was requested, but it doesn't exist");
        }
    }

    public void OnFrameMessage(NetworkConnection con, FrameMessage message)
    {
        //Debug.Log("Frame " + message.frame + " received " + string.Join(", ", message.inputs));
        var receivedFrame = message.frame;
        Debug.Log("Frame " + receivedFrame + " received (My frame " + currentFrame + ")");
        if (opponentFrameCount > 0 && receivedFrame > opponentFrameCount)
        {
            // We're missing frames, request them
            Debug.Log("Frame drop detected, sent frame " + receivedFrame + " should be " + opponentFrameCount);
            totalDropped++;
            ready = false;
            requestFrame = (short) (opponentFrameCount - 1);
            requestTimer += inputDelay;
            SendFrameRequest(requestFrame);
        } else
        {
            ready = true;
        }
        opponentFrame = receivedFrame;
        for (int i=0; i<message.inputs.Length; i++)
        {
            opponentFrames[opponentFrame + i] = new InputFrame(message.inputs[i]);
        }
        if (useRollback)
        {
            if (rollbackFrames == 0 && currentFrame >= opponentFrameCount)
            {
                // Rollback by the amount of frames you predicted
                var rollbackAmount = predictedFrames + 1;
                Debug.Log("Rolling back " + rollbackAmount + " frames from " + currentFrame);
                rollbackFrames = (short)rollbackAmount;
                predictedFrames = 0;
            }
        }
        opponentFrameCount = (short) Math.Max(opponentFrame + message.inputs.Length, opponentFrameCount);
    }

    public void OnInputDelaySet(NetworkConnection con, SetInputDelayMessage message)
    {
        Debug.Log(Time.time - con.lastMessageTime);
        var totalLatency = (message.delay + artificialLagMs) / 1000f;
        var constant = Time.fixedDeltaTime * 5;
        inputDelay = (short) (Mathf.CeilToInt((totalLatency + constant) / (2f * Time.fixedDeltaTime)));
        useRollback = message.useRollback || useRollback;
        Debug.Log("Setting delay to: " + inputDelay);
        ready = true;
    }
    
    public bool HasNextInput(int frame)
    {
        var frameExists = (opponentFrameCount == 0 || frame < opponentFrameCount) && opponentFrames[frame] != null;
        if (requestTimer > 0)
        {
            requestTimer--;
        }
        if (requestTimer == 0 && !ready)
        {
            requestFrame = (short) (requestFrame > 0 ? requestFrame - 1 : 0);
            Debug.Log("Resending frame request for " + requestFrame);
            SendFrameRequest(requestFrame);
            requestTimer = inputDelay;
        }
        if (useRollback)
        {
            // If we're using rollback, we always have frames available
            if (rollbackFrames == 0)
            {
                // But if we're not currently rolling back we must track predicted frames
                if (!frameExists)
                {
                    // This frame doesn't exist, it must be predicted
                    predictedFrames++;
                } else
                {
                    predictedFrames = 0;
                }
            }
            return true;
        }
        if (opponentFrames[frame] == null && requestTimer <= 0)
        {
            Debug.Log("Frame " + frame + " is missing");
            delayFrames = inputDelay;
        }
        return frameExists;
    }

    public InputFrame GetNetworkInput(int frame)
    {
        if (frame < 0 || (useRollback && opponentFrames[frame] == null))
        {
            // input is assumed to have been neutral 
            return new InputFrame();
        }
        else if (frame < opponentFrameCount)
        {
            // We have this frame, return it
            return opponentFrames[frame];
        }
        else if (useRollback)
        {
            // If we use rollback, and we run out of frames, just run the last one as the current frame
            // And track the amount of frames we've predicted
            return frame > 0 && opponentFrames[frame - 1] != null ? opponentFrames[frame - 1] : new InputFrame();
        }
        return null;
    }

    private void DumpFrameData()
    {
        var folder = Path.Combine("logs", DateTime.Now.ToString("yyyy-MM-dd-HH-mm"));
        var j = 0;
        while (Directory.Exists(Path.Combine(Application.dataPath, folder)))
        {
            j++;
            folder = Path.Combine("logs", DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + "(" + j + ")");
        }
        folder = Path.Combine(Application.dataPath, folder);
        Directory.CreateDirectory(folder);
        var gameData = Path.Combine(folder, "gamedata.txt");
        var statsData = Path.Combine(folder, "stats.txt");
        var list = GameSystem.frameData.ToList().GetRange(0, currentFrame + 1);
        var isPlayer1 = localPlayerIsPlayer1;
        var data = string.Join("\n", list.Select((frame, i) => "(" + i + "+" + frame.delayCount + ") ["
        + (frame.localInput == null ? "NULL" : (isPlayer1 ? frame.localInput : frame.remoteInput).InputString)
        + ":"
        + (frame.remoteInput == null ? "NULL" : (!isPlayer1 ? frame.localInput : frame.remoteInput).InputString)
        + "] #Player1 - "
        + frame.player1.ToString()
        + "#Player2 - "
        + frame.player2.ToString()));

        var stats = $"Total Delay Frames: { totalFramesDelayed }\n" +
            $"Total Delay Seconds: { totalMsDelayed }\n" +
            $"Total Hits: { totalHits }\n" +
            $"Max Combo: { maxCombo }\n" +
            $"Round Frames: { currentFrame }\n" +
            $"Frames Dropped: { totalDropped }\n" +
            $"Rollback Enabled: { (useRollback ? "Yes" : "No") }";

        StreamWriter writer1 = new StreamWriter(gameData, false);
        writer1.WriteLine(data);
        writer1.Close();

        StreamWriter writer2 = new StreamWriter(statsData, false);
        writer2.WriteLine(stats);
        writer2.Close();
    }

    private void OnApplicationQuit()
    {
        shuttingDown = true;
    }
}
