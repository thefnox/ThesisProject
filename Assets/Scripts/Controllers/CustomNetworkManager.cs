using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class CustomNetworkManager : NetworkManager
    {
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            Debug.Log("CLIENT CONNECTED " + conn.address + " " + conn.connectionId);
            if (conn.address != "localClient" && conn.address != "localServer")
            {
                // We're clients, so we're player 2
                NetworkController.Instance.server = false;
                NetworkController.Instance.localPlayerIsPlayer1 = false;
                NetworkController.Instance.opponentId = conn.connectionId;
                NetworkController.Instance.InitNewRound();
                SceneManager.LoadScene(1);
            }
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            Debug.Log("SERVER CONNECTED " + conn.address + " " + conn.connectionId);
            if (conn.address != "localClient" && conn.address != "localServer")
            {
                // We're the server, so we're player 1
                NetworkController.Instance.server = true;
                NetworkController.Instance.localPlayerIsPlayer1 = true;
                NetworkController.Instance.opponentId = conn.connectionId;
                NetworkController.Instance.InitNewRound();
                SceneManager.LoadScene(1);
            }
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            NetworkController.Instance.disconnected = true;
            Debug.Log("CLIENT DISCONNECTED " + conn.address);
            StartHost();
        }
    }
}
