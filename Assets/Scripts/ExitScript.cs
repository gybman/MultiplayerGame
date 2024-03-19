using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ExitScript : NetworkBehaviour
{
    [SerializeField] private GameObject playerExitUI;
    [SerializeField] private GameObject gun;
    private SimpleRelayMenu relayScript;
    private bool displaying;

    private void Start()
    {
        displaying = false;
        playerExitUI.SetActive(false);
        relayScript = GameObject.Find("Relay").GetComponent<SimpleRelayMenu>();
    }

    private void Update()
    {
        // When pressing Esc, either turn off or on the exit option
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!displaying)
            {
                playerExitUI.SetActive(true);
                gun.SetActive(false);   // disable weapon when exit screen is active
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                displaying = true;
            }
            else
            {
                playerExitUI.SetActive(false);
                gun.SetActive(true);    // disable weapon when exit screen is active
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                displaying = false;
            }
        }
    }
    public void ExitGame()
    {
        relayScript.ActivateMenu();
        // If using Unity Netcode, disconnect the client or stop the host
        if (NetworkManager.Singleton.IsHost)
        {
            RemovePlayersClientRpc();   // if host disconnects, disconnect all clients too
            NetworkManager.Singleton.Shutdown();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    [ClientRpc]
    public void RemovePlayersClientRpc()
    {
        relayScript.ActivateMenu();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        NetworkManager.Singleton.Shutdown();
    }
}
