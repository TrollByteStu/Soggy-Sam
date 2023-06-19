using UnityEngine;

namespace _SoggySam.scripts.GameManager
{
    public class SoundManager : MonoBehaviour
    {
        public Camera cam;
        
        private AudioReverbFilter _arf;
        private AudioLowPassFilter _alpf;

        private void Awake()
        {
            if (cam == null) cam = Camera.main;
            if (cam == null)
            {
                Debug.LogWarning($"No camera set on {this}. Disabling...");
                enabled = false;
                return;
            }
            _arf = cam.GetComponent<AudioReverbFilter>();
            _alpf = cam.GetComponent<AudioLowPassFilter>();

        }

        public void SetUnderwaterEffectsEnabled(bool enable)
        {
            _arf.enabled = enable;
            _alpf.enabled = enable;
        }
    }
}
