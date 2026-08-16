using NUnit.Framework;
using TMPro;
using UnityEngine;
using static Codice.Client.Common.EventTracking.TrackFeatureUseEvent.Features.DesktopGUI.Filters;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

namespace XRPlayer
{
    /// <summary>
    /// Represents an item on the store shelf. When clicked/interacted with,
    /// it moves between the shelf and the store counter.
    /// </summary>
    public class StoreItem : MonoBehaviour
    {
        [Header("Item Info")]
        public string itemName = "Item";
        public float price = 5.00f;

        private Vector3 shelfPosition;
        [SerializeField]
        private Transform counterPosition;

        private XRPlayer.CounterSlotManager counterManager;

        [Header("State")]
        public bool isOnCounter = false;

        // Marks whether this GameObject is a runtime copy placed on the counter
        public bool isCopy = false;

        // Reference back to the original shelf item when this is a copy
        public StoreItem originalItemRef;

        private Vector3 targetPosition;
        private bool isMoving = false;
        private float moveSpeed = 6.0f;
        private XRInteractableObject interactable;
        private bool targetInitialized = false;
        private Transform reservedSlot;

        // Track counter copies and enforce max capacity
        private static readonly System.Collections.Generic.List<StoreItem> counterCopies = new System.Collections.Generic.List<StoreItem>();
        private const int maxCounterItems = 5;

        private void Awake()
        {
            shelfPosition = transform.position;
            interactable = GetComponent<XRInteractableObject>();
            if (interactable == null)
            {
                interactable = gameObject.AddComponent<XRInteractableObject>();
            }

            // Hook interaction event
            interactable.onInteracted.AddListener(OnItemClicked);
        }

        private void Start()
        {
            counterManager = FindFirstObjectByType<CounterSlotManager>();
            if (shelfPosition == Vector3.zero)
            {
                shelfPosition = transform.position;
            }
            // Only initialize the target position if it hasn't been set by runtime code
            if (!targetInitialized)
            {
                targetPosition = transform.position;
                targetInitialized = true;
            }

        }

        private void Update()
        {
            if (isMoving)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    transform.position = targetPosition;
                    isMoving = false;
                    // If this is a runtime copy that just returned to shelf, remove it from counter and destroy
                    if (isCopy && !isOnCounter)
                    {
                        // Release reserved slot if any
                        if (reservedSlot != null && counterManager != null)
                        {
                            counterManager.ReleaseSlot(reservedSlot);
                            reservedSlot = null;
                        }

                        counterCopies.Remove(this);
                        Debug.Log($"[StoreItem] Copy of {itemName} reached shelf and will be destroyed.");
                        Destroy(gameObject);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            // Ensure we don't leave stale references in the counter list
            if (isCopy)
            {
                counterCopies.Remove(this);
            }
            // Release reserved slot if any
            if (reservedSlot != null && counterManager != null)
            {
                counterManager.ReleaseSlot(reservedSlot);
                reservedSlot = null;
            }

            // Unsubscribe listener if needed
            if (interactable != null)
            {
                interactable.onInteracted.RemoveListener(OnItemClicked);
            }
        }

        public void OnItemClicked()
        {
            // If this is an original shelf item and not a copy
            if (!isCopy)
            {
                // If a CounterSlotManager is assigned, create a runtime copy and send it to a reserved slot
                if (counterManager != null)
                {
                    // Try to reserve a slot
                    Transform slot = counterManager.ReserveNextSlot();
                    if (slot == null)
                    {
                        Debug.Log($"[StoreItem] Cannot place {itemName} on counter: no free slots.");
                        return;
                    }

                    // Instantiate a runtime copy and initialize it
                    var copyObj = Instantiate(gameObject, shelfPosition, transform.rotation);
                    var copyItem = copyObj.GetComponent<StoreItem>();
                    if (copyItem != null)
                    {
                        copyItem.isCopy = true;
                        copyItem.originalItemRef = this;
                        copyItem.shelfPosition = this.shelfPosition;
                        copyItem.counterManager = this.counterManager;
                        copyItem.reservedSlot = slot;
                        copyItem.isOnCounter = true;
                        // Prevent the copy's Start() from overwriting our target
                        copyItem.targetInitialized = true;
                        copyItem.targetPosition = slot.position;
                        copyItem.isMoving = true;
                        copyItem.gameObject.name = $"{itemName}_Copy";
                        counterCopies.Add(copyItem);
                    }

                    Debug.Log($"[StoreItem] Created copy of {itemName} and sent to counter. Counter count: {counterCopies.Count}");
                    return;
                }

                // No manager assigned: fallback to original behavior (toggle this item's counter state)
                isOnCounter = !isOnCounter;
                targetPosition = isOnCounter ? counterPosition.position : shelfPosition;
                isMoving = true;

                Debug.Log($"[StoreItem] {itemName} clicked! IsOnCounter: {isOnCounter}. Target Pos: {targetPosition}");
                return;
            }

            // If this is a copy on the counter, send it back to the shelf; it will be destroyed on arrival
            if (isCopy && isOnCounter)
            {
                // release the reserved slot immediately so others can use it while this copy returns
                if (reservedSlot != null && counterManager != null)
                {
                    counterManager.ReleaseSlot(reservedSlot);
                    reservedSlot = null;
                }

                isOnCounter = false;
                targetPosition = shelfPosition;
                isMoving = true;
                Debug.Log($"[StoreItem] Copy of {itemName} returning to shelf.");
                return;
            }

            // Fallback: toggle position for non-copy items
            isOnCounter = !isOnCounter;
            if (isOnCounter)
            {
                // try to reserve a slot via manager if available
                Transform slot = null;
                if (counterManager != null)
                {
                    slot = counterManager.ReserveNextSlot();
                    if (slot == null)
                    {
                        Debug.Log($"[StoreItem] Cannot place {itemName} on counter: no free slots.");
                        isOnCounter = false;
                        return;
                    }
                    reservedSlot = slot;
                    targetPosition = slot.position;
                }
                else
                {
                    targetPosition = counterPosition != null ? counterPosition.position : transform.position;
                }
            }
            else
            {
                // leaving counter, release slot if we had one
                if (reservedSlot != null && counterManager != null)
                {
                    counterManager.ReleaseSlot(reservedSlot);
                    reservedSlot = null;
                }
                targetPosition = shelfPosition;
            }
            isMoving = true;

            Debug.Log($"[StoreItem] {itemName} clicked! IsOnCounter: {isOnCounter}. Target Pos: {targetPosition}");
        }


    }
}


