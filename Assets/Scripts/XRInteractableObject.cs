using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace XRPlayer
{
    /// <summary>
    /// Attach to any object in the scene to allow interaction via XR Controllers or Desktop Mouse Clicks.
    /// Provides hover/select visual feedback and customizable UnityEvents.
    /// </summary>
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class XRInteractableObject : MonoBehaviour
    {
        [Header("Visual Feedback")]
        public Color hoverColor = Color.cyan;
        [SerializeField]
        private Color selectColor = Color.green;

        [Header("Events")]
        public UnityEvent onInteracted = new UnityEvent();

        private XRSimpleInteractable interactable;
        private Renderer objectRenderer;
        private Color originalColor;

        private void Awake()
        {
            interactable = GetComponent<XRSimpleInteractable>();
            objectRenderer = GetComponent<Renderer>();

            if (objectRenderer != null && objectRenderer.material != null)
            {
                originalColor = objectRenderer.material.color;
            }

            if (interactable != null)
            {
                interactable.hoverEntered.AddListener((args) => OnHoverEnter());
                interactable.hoverExited.AddListener((args) => OnHoverExit());
                interactable.selectEntered.AddListener((args) => Interact());
            }
        }

        public void OnHoverEnter()
        {
            if (objectRenderer != null && objectRenderer.material != null)
            {
                objectRenderer.material.color = hoverColor;
            }
        }

        public void OnHoverExit()
        {
            if (objectRenderer != null && objectRenderer.material != null)
            {
                objectRenderer.material.color = originalColor;
            }
        }

        public void Interact()
        {
            if (objectRenderer != null && objectRenderer.material != null)
            {
                objectRenderer.material.color = selectColor;
                Invoke(nameof(OnHoverExit), 0.3f);
            }

            Debug.Log($"[XRInteractableObject] Interacted with {gameObject.name}");
            onInteracted?.Invoke();
        }

    }
}
