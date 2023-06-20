using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _SoggySam.scripts.Utils;

public class playerEffects : WaterStateHelper
{
    /* .:. Sounds and Particle controller for player .:. */

    // the audiosource that plays underwater
    [SerializeField] private AudioSource AudioUnderwater;

    // a list of splash sounds
    [SerializeField] private AudioClip[] SoundSplash;
    [SerializeField] private GameObject SplashPrefab;
    private float lastSpawnedPrefab = 0f;

    // characters main sound effect
    private AudioSource myMainSound;

    // when you enter water
    protected override void OnEnterWater()
    {
        // underwater ambience on
        AudioUnderwater.volume = 1f;

        if ( lastSpawnedPrefab < Time.fixedTime )
        {
            Instantiate(SplashPrefab, transform.position,Quaternion.identity);
            lastSpawnedPrefab = Time.fixedTime + 0.5f;
        }
        
    }

    protected override void OnExitWater()
    {
        // underwater ambience off
        AudioUnderwater.volume = 0f;

        if (lastSpawnedPrefab < Time.fixedTime)
        {
            Instantiate(SplashPrefab, transform.position, Quaternion.identity);
            lastSpawnedPrefab = Time.fixedTime + 0.5f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myMainSound = GetComponent<AudioSource>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
