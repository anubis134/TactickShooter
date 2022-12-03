using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RelayJoinData represents the necesary informations
/// for a Host to client a game on a Relay
/// </summary>
public struct RelayJoinData
{
    internal string JoinCode { get; set; }
    internal string IPv4Adress { get; set; }
    internal ushort Port { get; set; }
    internal Guid AllocationID { get; set; }
    internal byte[] AllocationIDBytes { get; set; }
    internal byte[] ConnectionData { get; set; }
    internal byte[] HostConnectionData { get; set; }
    internal byte[] Key { get; set; }
}
