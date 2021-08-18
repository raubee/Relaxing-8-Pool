using UnityEngine;
using UnityEngine.Audio;

namespace RP.UI.Menu
{
    /// <summary>
    /// <para> Allows to change controls during the game. </para>
    /// <para> Allows to mute audio mixer groups by setting the exposed volume properties to mute. </para>
    /// </summary>
    public sealed class UISettingsPanel : UIMenuPanel
    {
        private const string AUDIO_MIXER_PROPERTY_VOLUME_MUSIC = "VolumeMusic";
        private const string AUDIO_MIXER_PROPERTY_VOLUME_FX = "VolumeFx";
        private const float MUTE_DB = -80f;

        [Header("Controls")]
        [SerializeField] private GameObject controlsBlock;

        [Header("Audios")] 
        [SerializeField] private AudioMixerGroup musicMixerGroup;
        [SerializeField] private AudioMixerGroup fxMixerGroup;

        private float _savedFxVolume;
        private float _savedMusicVolume;

        private void Start()
        {
            musicMixerGroup.audioMixer.GetFloat(AUDIO_MIXER_PROPERTY_VOLUME_MUSIC, out _savedMusicVolume);
            fxMixerGroup.audioMixer.GetFloat(AUDIO_MIXER_PROPERTY_VOLUME_FX, out _savedFxVolume);

            SetupControlsLayout();
        }

        /// <summary>
        /// <para> Enables controls settings depending on runtime platform. </para>
        /// <remarks> Called once at start. </remarks>
        /// </summary>
        private void SetupControlsLayout()
        {
            if (Application.isMobilePlatform)
            {
                controlsBlock.SetActive(false);
            }
        }

        public void MuteMusic(bool mute)
        {
            musicMixerGroup.audioMixer.SetFloat(AUDIO_MIXER_PROPERTY_VOLUME_MUSIC, mute ? MUTE_DB : _savedMusicVolume);
        }

        public void MuteFX(bool mute)
        {
            fxMixerGroup.audioMixer.SetFloat(AUDIO_MIXER_PROPERTY_VOLUME_FX, mute ? MUTE_DB : _savedFxVolume);
        }
    }
}