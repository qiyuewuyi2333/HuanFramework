using Huan.Character;
using Huan.Framework.Core.Input;
using UnityEngine;

namespace Huan.Framework.Camera
{
    /// <summary>
    /// 在需要的地方调用更新
    /// </summary>
    public abstract class BaseCameraController : MonoBehaviour
    {
        [Header("BaseSetting")] public Transform target;
        public bool isActive = true;
        public bool isFollowing = true;
        public int priority = 0;

        [Header("Smooth")] public float positionSmoothTime = 0.1f;
        public float rotationSmoothTime = 0.1f;

        [Header("Input")] protected IA_Input InputMaps;
        public bool enableInput = true;

        [SerializeField] public UnityEngine.Camera cameraComponent;

        // Data
        protected Vector2 LookInput;
        protected float ZoomInput;
        private bool _isInitialized = false;

        public virtual void Init()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("CameraController has already been initialized.");
                return;
            }

            cameraComponent = GetComponent<UnityEngine.Camera>();
            if (cameraComponent == null) cameraComponent = UnityEngine.Camera.main;
            if (cameraComponent == null)
            {
                Debug.LogError("No camera found.");
                return;
            }

            InputMaps = InputManager.InputMaps;

            _isInitialized = true;
        }

        public void UpdateCamera()
        {
            if (!isActive) return;

            if (enableInput)
            {
                HandleInput();
                UpdateCameraLogic();
            }

        }

        protected abstract void HandleInput();

        public virtual void SetActive(bool active)
        {
            isActive = active;
            cameraComponent?.gameObject.SetActive(active);
        }

        public virtual void SetTarget(Transform newTarget)
        {
            this.target = newTarget;
        }

        public UnityEngine.Camera GetCamera()
        {
            return cameraComponent;
        }

        protected abstract void UpdateCameraLogic();
    }
}