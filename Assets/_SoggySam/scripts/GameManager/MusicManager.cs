using System.Collections.Generic;
using UnityEngine;

namespace _SoggySam.scripts.GameManager
{
    public class MusicManager : MonoBehaviour
    {
        public List<AudioTriggerZone> zonesToTriggerWhenBoss;

        public void PrepareForBossFight()
        {
            foreach (AudioTriggerZone zone in zonesToTriggerWhenBoss)
            {
                zone.DoAction();
            }
        }
    }
}
