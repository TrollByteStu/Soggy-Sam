using System;
using UnityEngine;

namespace _SoggySam.scripts.Utils
{
    public class WaterStateHelper : MonoBehaviour
    {
        public bool InWater => _inWater > 0;
        
        private int _inWater;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Water"))
            {
                _inWater++;
                if (_inWater != 1) return;
                OnEnterWater();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Water"))
            {
                _inWater--;
                if (_inWater != 0) return;
                OnExitWater();
            }
        }

        protected virtual void OnEnterWater()
        {
            
        }

        protected virtual void OnExitWater()
        {
            
        }
    }
}
