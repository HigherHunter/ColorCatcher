using UnityEngine;

namespace Managers
{
    public class SoundManager : MonoBehaviour
    {
        private AudioSource _mainAudioSource;
        [SerializeField] private AudioClip buttonPressAudioClip;
        [SerializeField] private AudioClip correctHitAudioClip;
        [SerializeField] private AudioClip gameOverAudioClip;

        public static SoundManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            _mainAudioSource = GetComponent<AudioSource>();
        }

        public void PlayButtonPressSound() => _mainAudioSource.PlayOneShot(buttonPressAudioClip);

        public void PlayHitSound() => _mainAudioSource.PlayOneShot(correctHitAudioClip);

        public void PlayGameOverSound() => _mainAudioSource.PlayOneShot(gameOverAudioClip);

        public void Mute()
        {
            _mainAudioSource.mute = !_mainAudioSource.mute;
        }

        public bool IsPlaying()
        {
            return _mainAudioSource.isPlaying;
        }
    }
}