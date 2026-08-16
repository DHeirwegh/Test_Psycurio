using UnityEngine;


namespace XRPlayer
{
    /// <summary>
    /// Handles interaction with the Cashier character NPC.
    /// When clicked/interacted with, plays the waving animation inside the cashier.
    /// </summary>
    public class CashierController : MonoBehaviour
    {
        [Header("Animator Reference")]
        public Animator cashierAnimator;
        public string waveTriggerName = "Wave";

        public XRInteractableObject interactable;

        private void Awake()
        {
            if (cashierAnimator == null)
            {
                cashierAnimator = GetComponent<Animator>();
            }

            interactable = GetComponent<XRInteractableObject>();
            if (interactable == null)
            {
                interactable = gameObject.AddComponent<XRInteractableObject>();
            }

            interactable.onInteracted.AddListener(Wave);
        }

        public void Wave()
        {
            if (cashierAnimator != null)
            {
                cashierAnimator.SetTrigger(waveTriggerName);
                Debug.Log("[CashierController] Cashier is waving!");
            }
            else
            {
                Debug.LogWarning("[CashierController] Animator reference missing!");
            }
        }
    }
}
