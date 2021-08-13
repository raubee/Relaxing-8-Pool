using RP.Events;
using UnityEngine;

namespace RP
{
    /// <summary>
    /// <para> Handles the pool table pockets behaviour during the game. </para>
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class PocketBehaviour : MonoBehaviour
    {
        /// <summary>
        /// <para> Event sent when ball was pocketed. </para>
        /// <para> Can be static because no need to know in which hole the ball has been pocketed. </para>
        /// </summary>
        public static BallEvent OnPocketedEvent = new BallEvent();

        [SerializeField] private AudioClip ballPocketedClip;

        private AudioSource _audioSource;

        #region MonoBehaviour Functions

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var ball = other.GetComponent<BallBehaviour>();

            if (ball != null)
            {
                PlayBallPocketedSound();
                OnPocketedEvent.Invoke(ball);
            }
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// <para> Plays a sound when ball was pocketed. </para>
        /// </summary>
        private void PlayBallPocketedSound()
        {
            _audioSource.PlayOneShot(ballPocketedClip);
        }

        #endregion
    }
}