using System;
using Unity.Netcode;
using UnityEngine;

public class LobbyConnectionsHandler : NetworkBehaviour
{
    private NetworkList<LobbyPlayerData> _connectedPlayers;
    internal static Action<NetworkList<LobbyPlayerData>> OnPlayerConnectionHandled;

    private void OnEnable()
    {
        _connectedPlayers = new NetworkList<LobbyPlayerData>();
        LobbyPlayerDataTransporter.OnPlayerDataApplyed += OnPlayerDataApplyedHandler;
        _connectedPlayers.OnListChanged += ConnectedPlayers_OnListChanged;
    }

    private void OnDisable()
    {
        LobbyPlayerDataTransporter.OnPlayerDataApplyed -= OnPlayerDataApplyedHandler;
    }

    private void ConnectedPlayers_OnListChanged(NetworkListEvent<LobbyPlayerData> changeEvent)
    {
        OnPlayerConnectionHandled(_connectedPlayers);
    }

    private void OnPlayerDataApplyedHandler(LobbyPlayerData playerData) 
    {
        if (!IsHost) return;

        _connectedPlayers.Add(playerData);        

        foreach (var player in _connectedPlayers) 
        {
            print(player.PlayerName);
        }
    }
}
