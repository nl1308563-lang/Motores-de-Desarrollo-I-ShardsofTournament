using System.Collections.Generic;
using UnityEngine;

public class PlayerAmmoInventory : MonoBehaviour
{
    [System.Serializable]
    public class AmmoSlot
    {
        public AmmoType ammoType;
        public int currentReserve = 60;
        public int maxReserve = 120;
    }

    [Header("Reservas de Munición")]
    public List<AmmoSlot> startingAmmo = new List<AmmoSlot>();

    private Dictionary<AmmoType, AmmoSlot> ammoDictionary = new Dictionary<AmmoType, AmmoSlot>();

    void Awake()
    {
        foreach (var slot in startingAmmo)
        {
            if (slot != null && !ammoDictionary.ContainsKey(slot.ammoType))
            {
                slot.currentReserve = Mathf.Clamp(slot.currentReserve, 0, slot.maxReserve);
                ammoDictionary[slot.ammoType] = slot;
            }
        }
    }

    public int GetReserveAmmo(AmmoType type)
    {
        return ammoDictionary.TryGetValue(type, out var slot) ? slot.currentReserve : 0;
    }

    public int GetMaxReserveAmmo(AmmoType type)
    {
        return ammoDictionary.TryGetValue(type, out var slot) ? slot.maxReserve : 0;
    }

    public bool IsAmmoFull(AmmoType type)
    {
        if (ammoDictionary.TryGetValue(type, out var slot))
        {
            return slot.currentReserve >= slot.maxReserve;
        }
        return true;
    }

    public int ExtractAmmo(AmmoType type, int amountNeeded)
    {
        if (!ammoDictionary.TryGetValue(type, out var slot)) return 0;

        int amountToGive = Mathf.Min(slot.currentReserve, amountNeeded);
        slot.currentReserve -= amountToGive;
        return amountToGive;
    }

    public int AddAmmo(AmmoType type, int amount)
    {
        if (!ammoDictionary.TryGetValue(type, out var slot)) return 0;

        int spaceAvailable = slot.maxReserve - slot.currentReserve;
        if (spaceAvailable <= 0) return 0;

        int amountToAdd = Mathf.Min(spaceAvailable, amount);
        slot.currentReserve += amountToAdd;

        return amountToAdd;
    }
}