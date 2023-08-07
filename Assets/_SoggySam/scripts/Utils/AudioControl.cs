using System;
using UnityEngine;

namespace _SoggySam.scripts.Utils
{
    public class AudioControl : MonoBehaviour
    {
        public AudioSource AudioSource => _as;
        public float CurrentVolume => _volume;
        public float FadeTargetVolume => _targetVolume;
        public bool IsFading => _isFading;
        public bool IsPaused => _isPaused;
        public bool IsPlaying => _isPlaying;
        
        private AudioSource _as;
        private float _volume, _targetVolume;
        private bool _isFading, _isPaused, _isPlaying;

        private void Start()
        {
            _as = GetComponent<AudioSource>();
            if (_as == null)
            {
                enabled = false;
                return;
            }
        }
        
        
    }
}
