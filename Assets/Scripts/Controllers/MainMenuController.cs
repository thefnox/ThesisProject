using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public string hostname = "127.0.0.1";
    public bool useRollback = false;
    public Text statusString;
    public Toggle rollbackToggle;
    public InputField artificialLagText;
    public InputField artificialPacketDropText;

    // Start is called before the first frame update
    void Start()
    {
        useRollback = NetworkController.Instance.useRollback;
        rollbackToggle.isOn = useRollback;
    }

    // Update is called once per frame
    void Update()
    {
        artificialLagText.text = NetworkController.Instance.artificialLagMs.ToString();
        artificialPacketDropText.text = Mathf.RoundToInt(NetworkController.Instance.artificialPacketDrop * 100f).ToString();
    }

    public void UseRollback(bool rollback)
    {
        useRollback = rollback;
        NetworkController.Instance.useRollback = rollback;
    }

    public void SetArtificialLag(string ms)
    {
        try
        {
            NetworkController.Instance.artificialLagMs = short.Parse(ms);
        } catch (Exception ex)
        {

        }
    }

    public void SetArtificialPacketDrop(string percent)
    {
        try
        {
            NetworkController.Instance.artificialPacketDrop = int.Parse(percent) / 100f;
        }
        catch (Exception ex)
        {

        }
    }
    

    public void SetHostname(string host)
    {
        hostname = host;
    }

    public void Connect()
    {
        statusString.text = "Connecting";
        
        NetworkController.Instance.SetupClient(hostname, () =>
        {
            statusString.text = "Connected";
            SceneManager.LoadScene(1);
        }, () =>
        {
            statusString.text = "Connection failed";
        });
        //SceneManager.LoadScene(1);
    }
}
