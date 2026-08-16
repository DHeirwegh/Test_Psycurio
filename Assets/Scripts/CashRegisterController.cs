using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace XRPlayer
{
    /// <summary>
    /// Attached to the Cash Register. When clicked, scans all StoreItems on the counter,
    /// sums their total cost, displays the total on the register screen,
    /// updates the Cashier NPC's speech balloon text, and shows the balloon for ~3 seconds.
    /// </summary>
    public class CashRegisterController : MonoBehaviour
    {
        [Header("Register Display UI")]
        public TextMeshPro registerScreenText;

        [Header("Cashier Speech Balloon UI")]
        public TextMeshPro cashierSpeechText;
        public GameObject speechBalloonObject;
        [SerializeField]
        private float balloonDuration = 3.0f;

        private List<StoreItem> storeItems = new List<StoreItem>();

        public XRInteractableObject interactable;

        private void Awake()
        {
            interactable = GetComponent<XRInteractableObject>();
            if (interactable == null)
            {
                interactable = gameObject.AddComponent<XRInteractableObject>();
            }

            interactable.onInteracted.AddListener(OnRegisterClicked);
        }

        private void Start()
        {
            storeItems = new List<StoreItem>(FindObjectsByType<StoreItem>(FindObjectsSortMode.None));
            if (registerScreenText != null)
            {
                registerScreenText.text = "$0.00";
            }

            if (speechBalloonObject == null && cashierSpeechText != null && cashierSpeechText.transform.parent != null)
            {
                speechBalloonObject = cashierSpeechText.transform.parent.gameObject;
            }

            // Initially hide the speech balloon
            HideSpeechBalloon();
        }

        public void OnRegisterClicked()
        {
            CalculateAndDisplayTotal();
            ShowSpeechBalloon();
        }

        public void CalculateAndDisplayTotal()
        {
            // Find all store items in scene if list is empty
            if (storeItems == null || storeItems.Count == 0)
            {
                storeItems = new List<StoreItem>(FindObjectsByType<StoreItem>(FindObjectsSortMode.None));
            }

            float totalCost = 0f;
            List<string> boughtItems = new List<string>();

            foreach (var item in storeItems)
            {
                if (item != null && item.isOnCounter)
                {
                    totalCost += item.price;
                    boughtItems.Add(item.itemName);
                }
            }

            // Update Cash Register Display
            if (registerScreenText != null)
            {
                if (boughtItems.Count == 0)
                {
                    registerScreenText.text = "$0.00";
                }
                else
                {
                    registerScreenText.text = $"${totalCost:F2}";
                }
            }

            // Update Cashier Speech Balloon Text
            if (cashierSpeechText != null)
            {
                if (boughtItems.Count == 0)
                {
                    cashierSpeechText.text = "Your counter is empty!\nPlease click an item to place it on the counter.";
                }
                else
                {
                    string itemListStr = string.Join(", ", boughtItems);
                    cashierSpeechText.text = $"Thank you!\nYou bought: {itemListStr}\nThat will be ${totalCost:F2}!";
                }
            }

            Debug.Log($"[CashRegister] Scanned {boughtItems.Count} items on counter. Total: ${totalCost:F2}");
        }

        public void ShowSpeechBalloon()
        {
            if (speechBalloonObject == null && cashierSpeechText != null && cashierSpeechText.transform.parent != null)
            {
                speechBalloonObject = cashierSpeechText.transform.parent.gameObject;
            }

            if (speechBalloonObject != null)
            {
                speechBalloonObject.SetActive(true);
                CancelInvoke(nameof(HideSpeechBalloon));
                Invoke(nameof(HideSpeechBalloon), balloonDuration);
            }
        }

        public void HideSpeechBalloon()
        {
            if (speechBalloonObject == null && cashierSpeechText != null && cashierSpeechText.transform.parent != null)
            {
                speechBalloonObject = cashierSpeechText.transform.parent.gameObject;
            }

            if (speechBalloonObject != null)
            {
                speechBalloonObject.SetActive(false);
            }
        }
    }
}


