using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

// This script is used to allow global matchmaking using unity's relay system
public class SimpleRelayMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text _joinCodeText;
    [SerializeField] private TMP_InputField _joinInput;
    [SerializeField] private GameObject _buttons;
    [SerializeField] private GameObject _timer;
    [SerializeField] private GameObject _scoreboard;

    private UnityTransport _transport;
    private const int MaxPlayers = 5;

    private async void Awake()
    {
        _transport = FindObjectOfType<UnityTransport>();

        _buttons.SetActive(false);

        await Authenticate();

        _buttons.SetActive(true);
    }

    private static async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // Function for the the create button which makes the player a host and gives them a unique code so others can join
    public async void CreateGame()
    {
        _buttons.SetActive(false);
        _timer.SetActive(true);
        _scoreboard.SetActive(true);

        Allocation a = await RelayService.Instance.CreateAllocationAsync(MaxPlayers);
        _joinCodeText.text = "Join Code: " + await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

        _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

        NetworkManager.Singleton.StartHost();
    }

    // used to join a host as a client by entering the unique code assigned to the host's game
    public async void JoinGame()
    {
        _buttons.SetActive(false);
        

        try
        {
            _timer.SetActive(true);
            _scoreboard.SetActive(true);
            JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(_joinInput.text);
            _joinCodeText.text = "";
            _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to join the game: {e.Message}");
            // Optionally, display an error message to the player using UI
            _joinCodeText.text = "Invalid code. Please try again.";
            // Reactivate the buttons to allow the player to try again
            _buttons.SetActive(true);
            _scoreboard.SetActive(false);
        }
    }

    // used to turn on or off the menu when the player returns to the menu screen
    public void ActivateMenu()
    {
        _buttons.SetActive(true);
        _timer.SetActive(false);
        _scoreboard.SetActive(false);
        _joinCodeText.text = "";
    }
}