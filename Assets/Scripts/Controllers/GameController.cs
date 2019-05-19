using System.Collections;
using System.Collections.Generic;
using Assets;
using Assets.Scripts.DataObjects;
using Assets.Scripts.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public GameSystem system = new GameSystem();
    public SoundController soundController;
    public PlayerController player1;
    public PlayerController player2;
    public Text timeText;
    public Text frameCounterText;
    public Text player1Combo;
    public Text player2Combo;
    public Text player1Health;
    public Text player2Health;
    public Text winText;

    public string inputString = "";
    public int frameCount;
    public float fadeComboTimer = 0.0f;
    public float finishCounter;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameRoutine());
    }

    IEnumerator GameRoutine()
    {
        NetworkController.Instance.SendInputDelay(NetworkController.Instance.inputDelay);
        while (!NetworkController.Instance.ready)
        {
            yield return null;
        }
        system = new GameSystem(NetworkController.Instance.inputDelay);
        system.Initialize((string move) => OnPlayer1Hit(move), (string move) => OnPlayer2Hit(move));
        while (!system.IsGameOver())
        {
            if (system != null)
            {
                system.ProcessFrame(Mathf.CeilToInt(Time.fixedDeltaTime * 100f));
            }
            if (system.rollbackFrames > 0)
            {
                // Run it as fast as possible to get us back to where we were
                NetworkController.Instance.totalMsDelayed += Time.deltaTime;
                yield return null;
            } else
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }

    void OnPlayer1Hit(string move)
    {
        soundController.PlayerOneHit(move);
    }

    void OnPlayer2Hit(string move)
    {
        soundController.PlayerTwoHit(move);
    }

    void EndRound()
    {
        NetworkController.Instance.EndRound();
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndRound();
        }
        if (system == null)
        {
            return;
        }
        var currentFrame = system.GetCurrentFrame();
        var opponentFrame = NetworkController.Instance.opponentFrame;
        inputString = system.inputSystem.GetCurrentInput().InputString;
        frameCount = system.currentFrame;

        var player1State = system.GetCurrentPlayerOneState();
        var player2State = system.GetCurrentPlayerTwoState();
        var player1direction = player1State.frame.isFacingRight ? 1f : -1f;
        var player2direction = player2State.frame.isFacingRight ? 1f : -1f;

        player1.state = player1State;
        player2.state = player2State;

        player1Health.text = Mathf.Max(player1State.frame.health, 0).ToString();
        player2Health.text = Mathf.Max(player2State.frame.health, 0).ToString();

        if (player1State.frame.hitCount > 1)
        {
            player1Combo.text = player1State.frame.hitCount + " HITS";
            fadeComboTimer = 1.0f;
        }
        else if (fadeComboTimer >= 0.0f)
        {
            fadeComboTimer -= Time.deltaTime;
        }
        else
        {
            player1Combo.text = " ";
        }

        if (player2State.frame.hitCount > 1)
        {
            player2Combo.text = player2State.frame.hitCount + " HITS";
            fadeComboTimer = 1.0f;
        }
        else if (fadeComboTimer >= 0.0f)
        {
            fadeComboTimer -= Time.deltaTime;
        }
        else
        {
            player2Combo.text = " ";
        }

        if (!system.IsGameOver())
        {
            timeText.text = ((Constants.GAME_TIME - frameCount) / 60F).ToString("0.0");
            frameCounterText.text = frameCount + " - " + opponentFrame + " (" +  (frameCount - opponentFrame) + ")";
            var player1PosY = player1State.PosY / 100f + (player1State.IsCrouching() ? -0.5f : 0f);
            var player2PosY = player2State.PosY / 100f + (player2State.IsCrouching() ? -0.5f : 0f);
            player1.transform.position = new Vector3(player1State.PosX / 100f, player1PosY, player1.transform.position.z);
            player1.transform.rotation = Quaternion.Euler(new Vector3(player1State.IsHit() ? 30f * player1direction : 0f, player1direction > 0f ? 0f : 180f, 0f));
            player2.transform.position = new Vector3(player2State.PosX / 100f, player2PosY, player2.transform.position.z);
            player2.transform.rotation = Quaternion.Euler(new Vector3(player2State.IsHit() ? 30f * player2direction : 0f, player2direction > 0f ? 0f : 180f, 0f));
        } else
        {
            winText.text = system.WinCondition();
            if (finishCounter <= 5f)
            {
                finishCounter += Time.deltaTime;
                if (finishCounter >= 5f)
                {
                    EndRound();
                }
            }
        }
    }
}
