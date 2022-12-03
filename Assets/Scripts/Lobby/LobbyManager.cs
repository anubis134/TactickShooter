using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    internal const int _maxConnections = 5;
    private string _savedLobbyID { get; set; }
    private const float _waitTimeSeconds = 15F;
    private RelayHostData _relayHostData;
    private RelayJoinData _relayJoinData;
    internal static Action<Lobby> OnJoinedInLobby;
    internal static Action<RelayHostData> OnRelayHostDataReady;
    internal static Action<RelayJoinData> OnRelayJoinDataReady;

    private void OnDestroy()
    {
        DeleteLobby();
    }

    public async void FindRandomLobby() 
    {
        try
        {
            QuickJoinLobbyOptions quickJoinLobbyOptions = new QuickJoinLobbyOptions();

            Lobby lobby = await Lobbies.Instance.QuickJoinLobbyAsync(quickJoinLobbyOptions);

            Debug.Log($"Joined to lobby {lobby.Id}");
            Debug.Log($"Current lobby code = {lobby.LobbyCode}");
            Debug.Log($"Players in lobby = {lobby.Players.Count}");

            string joinCode = lobby.Data["joinCode"].Value;

            Debug.Log($"Recived code {joinCode}");

            JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

            _relayJoinData = new RelayJoinData
            {
                Key = allocation.Key,
                Port = (ushort)allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                HostConnectionData = allocation.HostConnectionData,
                IPv4Adress = allocation.RelayServer.IpV4

            };

            OnRelayJoinDataReady?.Invoke(_relayJoinData);
            OnJoinedInLobby?.Invoke(lobby);
        }
        catch 
        {
            CreateLobby("customLobby");
        }
           
       
    }

    public async void CreateLobby(string lobbyName) 
    {
        try
        {
            Allocation allocation = await Relay.Instance.CreateAllocationAsync(_maxConnections);
            _relayHostData = new RelayHostData
            {
                Key = allocation.Key,
                Port = (ushort)allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                IPv4Adress = allocation.RelayServer.IpV4
                
            };

            _relayHostData.JoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);

            OnRelayHostDataReady?.Invoke(_relayHostData);

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions();
            createLobbyOptions.IsPrivate = false;
            createLobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
              "joinCode", new DataObject(
                  DataObject.VisibilityOptions.Member,
                  _relayHostData.JoinCode)
                }
            }; 

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, _maxConnections, createLobbyOptions);

            _savedLobbyID = lobby.Id;

            Debug.Log($"Created lobby {lobby.LobbyCode}");

            StartCoroutine(HeartBeatLobbyRoutine(lobby.Id, _waitTimeSeconds));

            OnJoinedInLobby?.Invoke(lobby);
        }
        catch (LobbyServiceException exeption) 
        {
            Debug.LogError(exeption.Message);
        }
    }

    public async void JoinLobby(TMP_InputField inputField) 
    {
        try
        {
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(inputField.text);

            Debug.LogError(inputField.text);

            Debug.Log($"Joined to lobby {lobby.Id}");
            Debug.Log($"Current lobby code = {lobby.LobbyCode}");
            Debug.Log($"Players in lobby = {lobby.Players.Count}");

            string joinCode = lobby.Data["joinCode"].Value;

            JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

            _relayJoinData = new RelayJoinData
            {
                Key = allocation.Key,
                Port = (ushort)allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                HostConnectionData = allocation.HostConnectionData,
                IPv4Adress = allocation.RelayServer.IpV4

            };

            OnRelayJoinDataReady?.Invoke(_relayJoinData);
            OnJoinedInLobby?.Invoke(lobby);

        }
        catch (LobbyServiceException exeption) 
        {
            Debug.LogError(exeption.Message);
        }
    }

    internal int GetMaxConnections() 
    {
        return _maxConnections;
    }

    private void DeleteLobby() 
    {
        Lobbies.Instance.DeleteLobbyAsync(_savedLobbyID);
    }


    private IEnumerator HeartBeatLobbyRoutine(string lobbyID, float waitTimeSeconds) 
    {
        while (true) 
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyID);
            yield return new WaitForSecondsRealtime(waitTimeSeconds);
        }
    }
}
