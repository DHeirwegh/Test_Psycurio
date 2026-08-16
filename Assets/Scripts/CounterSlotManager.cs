using UnityEngine;

namespace XRPlayer
{
    /// <summary>
    /// Manages a fixed set of counter slot Transforms. Slots are serialized in the inspector.
    /// Call ReserveNextSlot() to reserve the next free slot (returns the Transform) and
    /// ReleaseSlot(transform) to free it.
    /// </summary>
    public class CounterSlotManager : MonoBehaviour
    {
        [Header("Counter Slots (assign up to 5)")]
        [SerializeField]
        [Tooltip("Assign the transforms that represent available slots on the counter. Leave empty slots if unused.")]
        private Transform[] slots = new Transform[5];

        private bool[] occupied;

        private void Awake()
        {
            if (slots == null)
                slots = new Transform[5];

            occupied = new bool[slots.Length];
        }

        /// <summary>
        /// Reserve the next available slot. Returns the Transform or null if none free.
        /// </summary>
        public Transform ReserveNextSlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null) continue;
                if (!occupied[i])
                {
                    occupied[i] = true;
                    return slots[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Release a previously reserved slot. If the slot is not found nothing happens.
        /// </summary>
        public void ReleaseSlot(Transform slot)
        {
            if (slot == null || slots == null) return;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == slot)
                {
                    occupied[i] = false;
                    return;
                }
            }
        }

        /// <summary>
        /// Check if any slot is available.
        /// </summary>
        public bool HasAvailableSlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null && !occupied[i]) return true;
            }
            return false;
        }
    }
}
