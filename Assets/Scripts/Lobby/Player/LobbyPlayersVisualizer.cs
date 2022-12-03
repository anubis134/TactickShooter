using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public sealed class LobbyPlayersVisualizer : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    private List<PlayerPlaceHolder> _playerPlaceHolders = new List<PlayerPlaceHolder>();
    [SerializeField]
    private List<GameObject> _players = new List<GameObject>();

    private void OnEnable()
    {
        LobbyConnectionsHandler.OnPlayerConnectionHandled += OnPlayerConnectionHandler;
    }

    private void OnDisable()
    {
        LobbyConnectionsHandler.OnPlayerConnectionHandled -= OnPlayerConnectionHandler;
    }

    private void OnPlayerConnectionHandler(NetworkList<LobbyPlayerData> connectedPlayers) 
    {
        VisualizePlayers(connectedPlayers);
    }

    private void VisualizePlayers(NetworkList<LobbyPlayerData> connectedPlayers) 
    {
        ClearPlayers();

        foreach (var player in connectedPlayers) 
        {
            Transform playerTransform = Instantiate(_playerPrefab).transform;
            playerTransform.position = _playerPlaceHolders[(int)player.ClientID].transform.position;
            _players.Add(playerTransform.gameObject);
        }
    }

    private void ClearPlayers() 
    {
        foreach (var player in _players) 
        {
            Destroy(player);
        }

        _players.Clear();
    }
}
