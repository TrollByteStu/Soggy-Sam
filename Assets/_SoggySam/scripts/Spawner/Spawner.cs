using UnityEngine;

namespace _SoggySam.scripts.Spawner
{
    public class Spawner : MonoBehaviour
    {
        private Transform _transform;
        private int _spawnedCount;
        
        public GameObject prefabToSpawn;
        public int spawnCount;
        public float spawnRateDelay;
        public float preSpawnDelay;

        private void Awake()
        {
            if (prefabToSpawn == null)
            {
                Debug.LogWarning("No prefab set. Disabling...");
                enabled = false;
            }
        }

        private void Start()
        {
            InvokeRepeating(nameof(CheckCanSpawn), preSpawnDelay, spawnRateDelay);
        }

        private void CheckCanSpawn()
        {
            _spawnedCount = transform.childCount;
            if (_spawnedCount < spawnCount)
            {
                Spawn();
            }
        }

        private void Spawn()
        {
            _transform = transform;
            Instantiate(prefabToSpawn, _transform.position, _transform.rotation, _transform);
        }
    }
}
