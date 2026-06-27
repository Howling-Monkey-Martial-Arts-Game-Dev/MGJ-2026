using UnityEngine;
using TMPro;

/// <summary>
/// Manages the 4-slot lobby UI.
/// Only LobbyManager calls into this — UI never reads PlayerManager directly.
/// </summary>
public class LobbyUI : Singleton<LobbyUI>
{
    [System.Serializable]
    private struct SlotUI
    {
        public GameObject      joinPrompt;
        public GameObject      playerPanel;
        public TextMeshProUGUI playerLabel;
        public GameObject      readyIndicator;
    }

    [SerializeField] private SlotUI[] slots;

    public void SetSlotOccupied(int slot, bool occupied)
    {
        if (!ValidSlot(slot)) return;
        slots[slot].joinPrompt.SetActive(!occupied);
        slots[slot].playerPanel.SetActive(occupied);

        // Only update label text when occupying — avoids setting text on
        // a panel that's about to be hidden
        if (occupied)
            slots[slot].playerLabel.text = $"Player {slot + 1}";

        slots[slot].readyIndicator.SetActive(false);
    }

    public void SetSlotReady(int slot, bool ready)
    {
        if (!ValidSlot(slot)) return;
        slots[slot].readyIndicator.SetActive(ready);
    }

    private bool ValidSlot(int slot) => slot >= 0 && slot < slots.Length;
}
