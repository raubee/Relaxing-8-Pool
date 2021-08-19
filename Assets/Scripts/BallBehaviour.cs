using UnityEngine;

namespace RP
{
    public enum BallColor
    {
        White,
        Red,
        Yellow
    }

    /// <summary>
    /// <para> Handles the ball behaviour during a game. </para>
    /// <para> Particularly it updates stationary status of the ball and play sounds when ball hit something. </para>
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(AudioSource))]
    public class BallBehaviour : MonoBehaviour
    {
        private const string BALL_TAG = "Ball";
        private const string CUSHION_TAG = "Cushion";
        private const float THRESHOLD_STATIC = .01f; // Velocity magnitude considering whether ball is moving or not

        /// <summary>
        /// <para> Color of the ball used to calculate score when it was pocketed. </para>
        /// </summary>
        public BallColor Color;

        /// <summary>
        /// <para> Is the ball is stationary ? </para>
        /// </summary>
        [HideInInspector] public bool Stationary = true;

        [Header("Audio settings")] [SerializeField]
        private float relativeForceToPlaySound = 2.0f;

        /// <summary>
        /// <para> Audio clips played when the ball collided another ball. </para>
        /// </summary>
        [SerializeField] private AudioClip[] ballCollidedBallClips;

        /// <summary>
        /// <para> Audio clip played when the ball collided a pool table cushion. </para>
        /// </summary>
        [SerializeField] private AudioClip ballCollidedCushionClip;

        private Vector3 _startPosition;
        private Rigidbody2D _rigidbody;
        private AudioSource _audioSource;

        #region MonoBehaviour Functions

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();
            _startPosition = transform.position;
        }

        private void OnDisable()
        {
            ResetAndFreezePosition();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // Is the impact force enough powerful ?
            if (other.relativeVelocity.magnitude > relativeForceToPlaySound)
            {
                // With another ball
                if (other.gameObject.CompareTag(BALL_TAG))
                {
                    PlayRandomCollidedBallSound();
                }
                // With a cushion
                else if (other.gameObject.CompareTag(CUSHION_TAG))
                {
                    PlayCollidedCushionSound();
                }
            }
        }

        private void FixedUpdate()
        {
            if (_rigidbody.velocity.magnitude < THRESHOLD_STATIC)
            {
                Freeze();
            }
            else
            {
                Stationary = false;
            }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// <para> Resets the start ball position on pool table. </para> 
        /// </summary>
        public void ResetAndFreezePosition()
        {
            transform.position = _startPosition;
            Freeze();
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// <para> Sets the ball stationary. </para>
        /// </summary>
        private void Freeze()
        {
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
            Stationary = true;
        }

        /// <summary>
        /// <para> Plays random sound when the ball collided another ball. </para>
        /// </summary>
        private void PlayRandomCollidedBallSound()
        {
            if (ballCollidedBallClips.TryPickRandom(out var clip))
            {
                _audioSource.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// <para> Plays a sound when the ball collided a pool table cushion. </para>
        /// </summary>
        private void PlayCollidedCushionSound()
        {
            _audioSource.PlayOneShot(ballCollidedCushionClip);
        }

        #endregion
    }
}