using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scene-level coordinator for the lobby.
/// Subscribes to PlayerController events (interact, leave) rather than
/// being called directly — keeps Core assembly free of Lobby references.
///
/// Owns ready state directly. PlayerManager has no concept of "ready" —
/// it's a lobby-only idea, and tracking it here means it naturally resets
/// every time the lobby scene reloads (e.g. returning from a finished game),
/// with no separate reset call needed.
///
/// Spawn flow:
///   Start() spawns characters for already-connected players (returning from game).
///   HandlePlayerJoined() spawns characters for new joins after Start.
///   OnEnable() subscribes events only — never spawns, to avoid racing with Start().
/// </summary>
public class LobbyManager : Singleton<LobbyManager>
{
    [Header("Spawning")]
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Ready Zone")]
    [SerializeField] private ReadyZone readyZone;

    private readonly Dictionary<int, GameObject> _characters = new();
    private readonly Dictionary<int, bool> _readyState = new();
    private bool _transitioning;

    private void OnEnable()
    {
        PlayerManager.Instance.OnPlayerJoined += HandlePlayerJoined;
        PlayerManager.Instance.OnPlayerLeft += HandlePlayerLeft;

        // Subscribe PlayerController events for players already connected
        // (returning from game scene). New joins are handled in HandlePlayerJoined.
        foreach (var slot in PlayerManager.Instance.GetActiveSlots())
            SubscribeToPlayerController(slot);
    }

    private void OnDisable()
    {
        if (PlayerManager.Instance == null) return;

        PlayerManager.Instance.OnPlayerJoined -= HandlePlayerJoined;
        PlayerManager.Instance.OnPlayerLeft -= HandlePlayerLeft;

        foreach (var slot in PlayerManager.Instance.GetActiveSlots())
            UnsubscribeFromPlayerController(slot);
    }

    private void Start()
    {
        // Spawn characters for already-connected players.
        // OnEnable has already subscribed events, so new joins after this
        // point are handled by HandlePlayerJoined — no double-spawn risk.
        foreach (var slot in PlayerManager.Instance.GetActiveSlots())
        {
            SpawnCharacterForSlot(slot);
            _readyState[slot] = false;
            LobbyUI.Instance?.SetSlotOccupied(slot, true);
        }
    }

    // -------------------------------------------------------------------------
    // PlayerManager event handlers
    // -------------------------------------------------------------------------

    private void HandlePlayerJoined(int slot)
    {
        // Only fires for joins that happen after Start() — safe to spawn here
        SubscribeToPlayerController(slot);
        SpawnCharacterForSlot(slot);
        _readyState[slot] = false;
        LobbyUI.Instance?.SetSlotOccupied(slot, true);
    }

    private void HandlePlayerLeft(int slot)
    {
        UnsubscribeFromPlayerController(slot);
        DestroyCharacterForSlot(slot);
        _readyState.Remove(slot);
        LobbyUI.Instance?.SetSlotOccupied(slot, false);
    }

    // -------------------------------------------------------------------------
    // PlayerController event handlers
    // -------------------------------------------------------------------------

    private void HandleInteract(int slot)
    {
        if (_transitioning) return;
        // if (readyZone != null && !readyZone.IsPlayerInZone(slot)) return;

        bool ready = !_readyState.GetValueOrDefault(slot, false);
        _readyState[slot] = ready;
        LobbyUI.Instance?.SetSlotReady(slot, ready);

        if (AllPlayersReady())
            TransitionToGame();
    }

    private void HandleLeave(int slot)
    {
        if (_transitioning) return;
        PlayerManager.Instance.RemovePlayer(slot);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Editor-only. Lets EditorTestPlayerInjector mark an injected test player
    /// as ready without needing to simulate standing in the zone and pressing
    /// Interact. Not part of the normal gameplay path — HandleInteract remains
    /// the only way a real player readies up.
    /// </summary>
    public void ForceReady(int slot, bool ready)
    {
        if (!_readyState.ContainsKey(slot)) return;
        _readyState[slot] = ready;
        LobbyUI.Instance?.SetSlotReady(slot, ready);

        if (AllPlayersReady())
            TransitionToGame();
    }
#endif

    // -------------------------------------------------------------------------

    private bool AllPlayersReady()
    {
        if (_readyState.Count == 0) return false;
        foreach (var ready in _readyState.Values)
            if (!ready) return false;
        return true;
    }

    private void SubscribeToPlayerController(int slot)
    {
        var pc = GetPlayerController(slot);
        if (pc == null) return;
        pc.OnInteractPressed += HandleInteract;
        pc.OnLeavePressed += HandleLeave;
    }

    private void UnsubscribeFromPlayerController(int slot)
    {
        var pc = GetPlayerController(slot);
        if (pc == null) return;
        pc.OnInteractPressed -= HandleInteract;
        pc.OnLeavePressed -= HandleLeave;
    }

    private CharacterInputManager GetPlayerController(int slot) =>
        PlayerManager.Instance.GetInput(slot)?.GetComponent<CharacterInputManager>();

    private void SpawnCharacterForSlot(int slot)
    {
        if (_characters.ContainsKey(slot)) return;

        Transform spawn = spawnPoints.Length > slot ? spawnPoints[slot] : transform;
        var character = PlayerManager.Instance.SpawnCharacter(slot, characterPrefab, spawn.position);
        _characters[slot] = character.gameObject;
    }

    private void DestroyCharacterForSlot(int slot)
    {
        if (!_characters.TryGetValue(slot, out var go)) return;
        Destroy(go);
        _characters.Remove(slot);
    }

    private void TransitionToGame()
    {
        _transitioning = true;
        SceneTransitionManager.Instance.GoToGame();
        // No reset needed — _readyState is scoped to this LobbyManager instance
        // and is naturally rebuilt fresh next time the lobby scene loads.
    }
}
