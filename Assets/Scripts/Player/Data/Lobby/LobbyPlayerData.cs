using System;
using Unity.Collections;
using Unity.Netcode;

public struct LobbyPlayerData : INetworkSerializable, IEquatable<LobbyPlayerData>
{
    internal ulong ClientID;
    internal FixedString128Bytes PlayerName;

    internal LobbyPlayerData(ulong clientID, FixedString128Bytes playerName)
    {
        ClientID = clientID;
        PlayerName = playerName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out ClientID);
            reader.ReadValueSafe(out PlayerName);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(ClientID);
            writer.WriteValueSafe(PlayerName);
        }
    }

    public bool Equals(LobbyPlayerData other)
    {
        return ClientID == other.ClientID && PlayerName == other.PlayerName;
    }
}
