using Huan.Character;
using Huan.Framework.Camera;
using Huan.Framework.Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Huan.Framework.Character
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        private IA_Input _playerInputActions;
        private Vector2 _movement;
        private float _verticalRotation;

        [Header("Camera")] [SerializeField] private BaseCameraController cameraController;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            _playerInputActions = InputManager.InputMaps;

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnEnable()
        {
            _playerInputActions.Enable();
            _playerInputActions.Player.Move.performed += OnMove;
            _playerInputActions.Player.Move.canceled += OnMove; // 记得注册停止移动
        }


        private void OnDisable()
        {
            _playerInputActions.Player.Move.canceled -= OnMove;
            _playerInputActions.Player.Move.performed -= OnMove;
            _playerInputActions?.Disable();
        }


        private void Update()
        {
            HandleMovement();
            cameraController.UpdateCamera();
        }


        private void HandleMovement()
        {
            Vector3 cameraForward = cameraController.cameraComponent.transform.forward;
            Vector3 cameraRight = cameraController.cameraComponent.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward = cameraForward.normalized;
            cameraRight = cameraRight.normalized;
            Vector3 movement = cameraForward * _movement.y + cameraRight * _movement.x;
            characterController.Move(movement * Time.deltaTime * 5f);

            if (movement != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _movement = context.ReadValue<Vector2>();
        }
    }
}