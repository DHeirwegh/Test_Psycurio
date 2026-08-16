using UnityEngine;
using TMPro;

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

        [Header("Positions")]
        public Vector3 shelfPosition;
        public Vector3 counterPosition;

        [Header("State")]
        public bool isOnCounter = false;

        [Header("UI Reference")]
        public TextMeshPro priceLabel;

        private Vector3 targetPosition;
        private bool isMoving = false;
        private float moveSpeed = 6.0f;
        private XRInteractableObject interactable;

        private void Awake()
        {
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
            if (shelfPosition == Vector3.zero)
            {
                shelfPosition = transform.position;
            }
            targetPosition = transform.position;

            UpdateLabel();
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
                }
            }
        }

        public void OnItemClicked()
        {
            isOnCounter = !isOnCounter;
            targetPosition = isOnCounter ? counterPosition : shelfPosition;
            isMoving = true;

            Debug.Log($"[StoreItem] {itemName} clicked! IsOnCounter: {isOnCounter}. Target Pos: {targetPosition}");
        }

        public void UpdateLabel()
        {
            if (priceLabel != null)
            {
                priceLabel.text = $"{itemName}\n${price:F2}";
            }
        }
    }
}


