using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Huan.Framework.Camera
{
    public class ThirdPersonCameraController : BaseCameraController
    {
        [Header("ThirdPerson")] [SerializeField] private float cameraDistance = 5f;
        [SerializeField] private float currentHorizontalAngle = 0f;
        [SerializeField] private float currentVerticalAngle = 45f;
        [SerializeField] private float maxVerticalAngle = 80f;
        [SerializeField] private float minVerticalAngle = -80f;
        [SerializeField] private float horizontalSensitivity = 1f;
        [SerializeField] private float verticalSensitivity = 1f;


        private void Awake()
        {
            base.Init();
            isFollowing = true;
        }

        private void OnEnable()
        {
            InputMaps.Enable();

            InputMaps.Player.Look.performed += OnLook;
            InputMaps.Player.Look.canceled += OnLook;
        }

        private void OnDisable()
        {
            InputMaps.Player.Look.performed -= OnLook;
            InputMaps.Player.Look.canceled -= OnLook;

            InputMaps.Disable();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        protected override void HandleInput()
        {
            // 将LookInput处理
            currentHorizontalAngle += LookInput.x * horizontalSensitivity;
            currentVerticalAngle += LookInput.y * verticalSensitivity;
            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);
        }

        protected override void UpdateCameraLogic()
        {
            UpdateCameraPosition();
            UpdateCameraRotation();
        }

        private void UpdateCameraPosition()
        {
            cameraComponent.transform.position = target.position +
                                                 Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0) *
                                                 Vector3.forward * cameraDistance;
        }

        private void UpdateCameraRotation()
        {
            cameraComponent.transform.LookAt(target);
        }
    }
}