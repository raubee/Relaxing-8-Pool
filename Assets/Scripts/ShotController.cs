using UnityEngine;
using UnityEngine.Events;

namespace RP
{
    /// <summary>
    /// <para> Performs white ball shots. </para>
    /// <remarks> Should be used on the white ball game object. </remarks>
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class ShotController : MonoBehaviour
    {
        /// <summary>
        /// <para> Current shot provider implementation. </para>
        /// </summary>
        [SerializeField] private ShotCommandProvider commandProvider;

        /// <summary>
        /// <para> Reference the pool cue transform. Updated in UpdateCueTransform. </para>
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

        private Vector3 _force;

        private Rigidbody2D _rigidBody;

        #region MonoBehaviour Functions

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
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
            if (!commandProvider)
                return;

            UpdateShotForce();
            UpdateCueTransform();

            if (commandProvider.TriggerShot())
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

        public void SetController(ShotCommandProvider controller)
        {
            commandProvider = controller;
        }

        #endregion

        #region Private Functions

        private void UpdateShotForce()
        {
            _force = commandProvider.GetShotForce();
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