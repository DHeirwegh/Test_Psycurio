using UnityEngine;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace XRPlayer
{
    /// <summary>
    /// Provides full Extended Reality character locomotion, WASD movement, mouse look,
    /// teleportation, and object clicking in Editor play mode without a headset.
    /// </summary>
    [RequireComponent(typeof(XROrigin))]
    public class XRDesktopPlayerController : MonoBehaviour
    {
        [Header("Desktop Movement & Look")]
        [Tooltip("Movement speed when walking with WASD in editor/desktop mode.")]
        public float moveSpeed = 4.0f;

        [Tooltip("Sensitivity of mouse rotation when holding Right Mouse Button.")]
        public float mouseSensitivity = 2.0f;

        [Header("XR References")]
        public Camera playerCamera;
        public TeleportationProvider teleportationProvider;

        private XROrigin xrOrigin;
        private CharacterController characterController;
        private float rotationX = 0f;
        private float rotationY = 0f;

        private void Awake()
        {
            xrOrigin = GetComponent<XROrigin>();
            characterController = GetComponent<CharacterController>();

            if (playerCamera == null && xrOrigin != null)
            {
                playerCamera = xrOrigin.Camera;
            }

            if (teleportationProvider == null)
            {
                teleportationProvider = GetComponentInChildren<TeleportationProvider>();
            }

            if (playerCamera != null)
            {
                Vector3 currentRot = playerCamera.transform.localEulerAngles;
                rotationX = currentRot.x;
                rotationY = transform.eulerAngles.y;
            }
        }

        private void Update()
        {
            HandleDesktopMovement();
            HandleDesktopLook();
            HandleDesktopInteraction();
        }

        private void HandleDesktopMovement()
        {
            if (Keyboard.current == null) return;

            float moveX = 0f;
            float moveZ = 0f;

            if (Keyboard.current.wKey.isPressed) moveZ += 1f;
            if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
            if (Keyboard.current.aKey.isPressed) moveX -= 1f;
            if (Keyboard.current.dKey.isPressed) moveX += 1f;

            Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;
            if (inputDir.sqrMagnitude > 0.001f)
            {
                Transform camTransform = playerCamera != null ? playerCamera.transform : transform;
                Vector3 forward = camTransform.forward;
                Vector3 right = camTransform.right;
                forward.y = 0f;
                right.y = 0f;
                forward.Normalize();
                right.Normalize();

                Vector3 moveDelta = (forward * inputDir.z + right * inputDir.x) * moveSpeed * Time.deltaTime;

                if (characterController != null && characterController.enabled)
                {
                    characterController.Move(moveDelta);
                }
                else
                {
                    transform.position += moveDelta;
                }
            }
        }

        private void HandleDesktopLook()
        {
            if (Mouse.current == null || playerCamera == null) return;

            // Hold Right Mouse Button or click to look around
            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 mouseDelta = Mouse.current.delta.ReadValue() * 0.15f * mouseSensitivity;

                rotationY += mouseDelta.x;
                rotationX -= mouseDelta.y;
                rotationX = Mathf.Clamp(rotationX, -85f, 85f);

                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
                transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
            }
        }

        private void HandleDesktopInteraction()
        {
            if (Mouse.current == null || playerCamera == null) return;

            // Left Mouse Button to click/interact or teleport
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                {
                    // 1. Check for Teleportation Area or Anchor
                    TeleportationArea teleportArea = hit.collider.GetComponentInParent<TeleportationArea>();
                    TeleportationAnchor teleportAnchor = hit.collider.GetComponentInParent<TeleportationAnchor>();

                    if (teleportationProvider != null && (teleportArea != null || teleportAnchor != null))
                    {
                        Vector3 targetPos = teleportAnchor != null ? teleportAnchor.teleportAnchorTransform.position : hit.point;
                        Quaternion targetRot = teleportAnchor != null ? teleportAnchor.teleportAnchorTransform.rotation : transform.rotation;

                        TeleportRequest req = new TeleportRequest
                        {
                            destinationPosition = targetPos,
                            destinationRotation = targetRot,
                            matchOrientation = MatchOrientation.TargetUp
                        };
                        teleportationProvider.QueueTeleportRequest(req);
                        Debug.Log($"[XRDesktopPlayerController] Teleported to {targetPos}");
                        return;
                    }

                    // 2. Check for Interactable objects
                    XRInteractableObject interactable = hit.collider.GetComponentInParent<XRInteractableObject>();
                    if (interactable != null)
                    {
                        interactable.Interact();
                        Debug.Log($"[XRDesktopPlayerController] Interacted with {interactable.gameObject.name}");
                    }
                }
            }
        }
    }
}
