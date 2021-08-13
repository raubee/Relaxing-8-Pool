using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RP
{
    public enum ControllerType
    {
        None = -1,
        Mouse,
        Keyboard,
        Touch
    }

    /// <summary>
    /// <para> Performs white ball shots. </para>
    /// <remarks> Should be used on the white ball game object. </remarks>
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class ShotController : MonoBehaviour
    {
        [SerializeField] private ControllerType _defaultControllerType;

        /// <summary>
        /// <para> Reference the pool cue transform. Updated in UpdateCueTransform </para>
        /// </summary>
        [SerializeField] private Transform cueTransform;

        /// <summary>
        /// <para> Multiplies the force get by the shot command provider implementation. </para>
        /// </summary>
        [SerializeField] private float ForceMultiplier;

        #region Events

        /// <summary>
        /// <para> Event sent when a shot just occurred. </para>
        /// </summary>
        public UnityEvent OnShotTriggeredEvent = new UnityEvent();

        #endregion

        private ControllerType _controller = ControllerType.None;
        private IShotCommandProvider _commandProvider;

        private Vector3 _force;

        private Rigidbody2D _rigidBody;

        #region MonoBehaviour Functions

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            if (_controller == ControllerType.None)
                SetController(_defaultControllerType);
        }

        private void OnEnable()
        {
            cueTransform.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            if (cueTransform != null)
            {
                cueTransform.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (_commandProvider == null)
                return;

            UpdateShotForce();
            UpdateCueTransform();

            if (_commandProvider.TriggerShot())
            {
                Shot();
            }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// <para> Returns the shot force multiplied by the force multiplier. </para>
        /// </summary>
        /// <returns> Multiplied force vector. </returns>
        public Vector3 GetShotForce()
        {
            return _force * ForceMultiplier;
        }

        /// <summary>
        /// <para> Returns the raw force from the shot command provider implementation with magnitude clamped between 0f and 1f. </para>
        /// </summary>
        /// <returns> Raw vector clamped to 1f. </returns>
        public Vector2 GetRawForce()
        {
            return _force;
        }

        public void SetMouseController()
        {
            if (_controller == ControllerType.Mouse)
                return;

            SetController(ControllerType.Mouse);
        }

        public void SetKeyboardController()
        {
            if (_controller == ControllerType.Keyboard)
                return;

            SetController(ControllerType.Keyboard);
        }

        public void SetController(in ControllerType controller)
        {
            _controller = controller;

            switch (_controller)
            {
                case ControllerType.Mouse:
                    _commandProvider = new MouseShotCommandProvider();
                    break;

                case ControllerType.Keyboard:
                    _commandProvider = new KeyboardShotCommandProvider();
                    break;

                case ControllerType.Touch:
                    _commandProvider = new TouchShotCommandProvider();
                    break;
            }
        }

        public ControllerType[] GetSupportedControllers()
        {
            var controllerList = new List<ControllerType>(4);

#if UNITY_STANDALONE || UNITY_EDITOR
            controllerList.Add(ControllerType.Mouse);
            controllerList.Add(ControllerType.Keyboard);
#elif (UNITY_ANDROID || UNITY_IPHONE || UNITY_IOS)
            if (Input.touchSupported)
            {
                controllerList.Add(ControllerType.Touch);
            }
#endif
            return controllerList.ToArray();
        }

        #endregion

        #region Private Functions

        private void UpdateShotForce()
        {
            _force = _commandProvider.GetShotForce();
        }

        private void UpdateCueTransform()
        {
            cueTransform.position = transform.position - _force;
            cueTransform.rotation = Quaternion.LookRotation(Vector3.back, _force);
        }

        private void Shot()
        {
            _rigidBody.AddForce(GetShotForce(), ForceMode2D.Impulse);
            OnShotTriggeredEvent.Invoke();
        }

        #endregion
    }
}