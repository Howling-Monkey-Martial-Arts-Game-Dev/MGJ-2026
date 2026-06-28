using UnityEngine;

/// <summary>
/// Stamped onto character GameObjects at spawn time.
/// Allows scene triggers (ReadyZone etc) to identify player slot
/// without a PlayerManager lookup.
/// Must be initialised immediately after Instantiate.
/// </summary>
public class SlotIdentifier : MonoBehaviour
{
    public int Slot { get; private set; }

    public void Initialise(int slot) => Slot = slot;
}
