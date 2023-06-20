using System;
using UnityEngine;

namespace _SoggySam.scripts.Spawner
{
    [RequireComponent(typeof(BoxCollider))]
    public class Spawner : MonoBehaviour
    {
        private Transform _transform;
        private int _spawnedCount;
        private bool _withinView;
        
        public GameObject prefabToSpawn;
        [Range(1,100)] public int spawnCount = 1;
        [Range(0,100)] public float spawnRateDelay, preSpawnDelay;

        // is this a boss spawner
        [SerializeField] private bool isBoss = false;

        private void Awake()
        {
            if (prefabToSpawn == null)
            {
                Debug.LogWarning($"No prefab set on spawner \'{gameObject.name}\'. Disabling spawner...");
                enabled = false;
            }

            gameObject.layer = 7;
        }

        private void Start()
        {
            SetupCollider();

            //  boss does not spawn unless called
            if ( !isBoss )
                InvokeRepeating(nameof(CheckCanSpawn), preSpawnDelay, spawnRateDelay);
        }

        private void CheckCanSpawn()
        {
            _spawnedCount = transform.childCount;
            if (_spawnedCount < spawnCount && !_withinView) Spawn();
        }

        public void Spawn()
        {
            _transform = transform;
            Instantiate(prefabToSpawn, _transform.position, _transform.rotation, _transform);
        }

        private void SetupCollider()
        {
            BoxCollider prefabCollider = prefabToSpawn.GetComponent<BoxCollider>();
            if (prefabCollider == null)
            {
                Debug.LogWarning($"No BoxCollider is set on \'{prefabToSpawn.name}\'. May spawn within view.");
                return;
            }
            
            BoxCollider spawnerCollider = GetComponent<BoxCollider>();
            spawnerCollider.center = prefabCollider.center;
            spawnerCollider.size = prefabCollider.size;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != 6) return;
            _withinView = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != 6) return;
            _withinView = true;
        }

        public void EnableSpawner() => enabled = true;

        public void DisableSpawner() => enabled = false;

        public void RemoveAllSpawned()
        {
            while (transform.childCount > 0)
            {
                Destroy(transform.GetChild(0));
            }
        }
    }
}
