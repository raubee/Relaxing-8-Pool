using UnityEngine;
using UnityEngine.UI;

namespace RP.UI
{
    /// <summary>
    /// <para> Populates the game over panel. </para>
    /// <para> It listens an game over event and shows winning or losing message. </para>
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(AudioSource))]
    public class UIGameOverPanel : MonoBehaviour
    {
        [SerializeField] private Text gameOverText;

        [Header("GameOverMessage")] 
        [SerializeField] private string wonMessage;
        [SerializeField] private string loseMessage;

        [Header("Audio")] 
        [SerializeField] private AudioClip wonAudioClip;
        [SerializeField] private AudioClip loseAudioClip;

        private Canvas _canvas;
        private AudioSource _audioSource;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _audioSource = GetComponent<AudioSource>();
        }

        public void OnGameOver(bool won)
        {
            if (won)
            {
                SetWonState();
            }
            else
            {
                SetLoseState();
            }

            _canvas.enabled = true;
        }

        private void SetWonState()
        {
            gameOverText.text = wonMessage;
            _audioSource.PlayOneShot(wonAudioClip);
        }

        private void SetLoseState()
        {
            gameOverText.text = loseMessage;
            _audioSource.PlayOneShot(loseAudioClip);
        }
    }
}