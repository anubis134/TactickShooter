using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyPlayerDataTransporter : NetworkBehaviour
{
    private LobbyPlayerData _lobbyPlayerData = new LobbyPlayerData();
    internal static Action<LobbyPlayerData> OnPlayerDataApplyed;

    private void OnEnable()
    {
        RelayDataHandler.OnPlayerWasStarted += OnJoinedInLobbyHandler;
    }

    private void OnDisable()
    {
        RelayDataHandler.OnPlayerWasStarted -= OnJoinedInLobbyHandler;
    }

    private void OnJoinedInLobbyHandler() 
    {
        _lobbyPlayerData.PlayerName = $"Player" + NetworkManager.Singleton.LocalClientId;
        _lobbyPlayerData.ClientID = NetworkManager.Singleton.LocalClientId;
        SetPlayerDataServerRpc(_lobbyPlayerData);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerDataServerRpc(LobbyPlayerData lobbyPlayerData) 
    {       
        OnPlayerDataApplyed?.Invoke(lobbyPlayerData);
    }
}
