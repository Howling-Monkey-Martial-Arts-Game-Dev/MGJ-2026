using UnityEngine;

/// <summary>
/// Trigger zone in the lobby level. Players must stand here to toggle ready.
/// All overlap tracking is inherited from PlayerZone — this class only
/// exists to give the zone a distinct type for LobbyManager to reference.
/// </summary>
public class ReadyZone : MonoBehaviour
{
    // No additional behaviour needed — LobbyManager calls IsPlayerInZone(slot)
    // directly when handling the Interact action. Override OnPlayerEntered/
    // OnPlayerExited here if you want a "press Interact to ready up" prompt
    // to appear/disappear as players step in and out.
}
