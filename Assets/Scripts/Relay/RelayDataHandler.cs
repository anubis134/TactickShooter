using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class RelayDataHandler : MonoBehaviour
{
    private NetworkManager _networkManager;
    private UnityTransport _unityTransport;
    internal static Action OnPlayerWasStarted;

    private void Start()
    {
        _networkManager = NetworkManager.Singleton;
        _unityTransport = _networkManager.GetComponent<UnityTransport>();
        LobbyManager.OnRelayHostDataReady += SetConnectionData;
        LobbyManager.OnRelayJoinDataReady += SetConnectionData;
    }

    private void OnDestroy()
    {
        LobbyManager.OnRelayHostDataReady -= SetConnectionData;
        LobbyManager.OnRelayJoinDataReady -= SetConnectionData;
    }

    private void SetConnectionData(RelayHostData relayHostData) 
    {
        _unityTransport.SetRelayServerData(
            relayHostData.IPv4Adress,
            relayHostData.Port,
            relayHostData.AllocationIDBytes,
            relayHostData.Key,
            relayHostData.ConnectionData);

        _networkManager.StartHost();
        OnPlayerWasStarted?.Invoke();
    }

    private void SetConnectionData(RelayJoinData relayJoinData)
    {
        _unityTransport.SetRelayServerData(
            relayJoinData.IPv4Adress,
            relayJoinData.Port,
            relayJoinData.AllocationIDBytes,
            relayJoinData.Key,
            relayJoinData.ConnectionData,
            relayJoinData.HostConnectionData);

        _networkManager.StartClient();
        OnPlayerWasStarted?.Invoke();
    }
}
