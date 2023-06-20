using System;
using UnityEngine;

namespace _SoggySam.scripts.GameManager
{
    [RequireComponent(typeof(BoxCollider))]
    public class AudioTriggerZone : MonoBehaviour
    {
        public AudioSource source;
        public AudioTriggerActionType action;

        private void Awake()
        {
            BoxCollider bc = GetComponent<BoxCollider>();
            bc.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            switch (action)
            {
                case AudioTriggerActionType.Play:
                    FadeIn();
                    break;
                case AudioTriggerActionType.Stop:
                    FadeOut();
                    break;
                case AudioTriggerActionType.Pause:
                    FadeOut();
                    break;
                case AudioTriggerActionType.Unpause:
                    FadeIn();
                    break;
                default:
                    Debug.LogWarning("How did we end at this??");
                    break;
            }
        }

        private void FadeIn()
        {
            if (!source.isPlaying)
            {
                if (action == AudioTriggerActionType.Play) source.Play();
                if (action == AudioTriggerActionType.Unpause) source.UnPause();
            }
            if (source.volume >= .5f) return;
            source.volume = Mathf.Clamp01(source.volume += .05f);
            Invoke(nameof(FadeIn), .1f);
        }

        private void FadeOut()
        {
            if (source.volume <= 0f)
            {
                if (action == AudioTriggerActionType.Stop) source.Stop();
                if (action == AudioTriggerActionType.Pause) source.Pause();
                return;
            }
            source.volume = Mathf.Clamp01(source.volume -= .05f);
            Invoke(nameof(FadeOut), .1f);
        }
    }

    public enum AudioTriggerActionType
    {
        Play,
        Stop,
        Pause,
        Unpause
    }
}
