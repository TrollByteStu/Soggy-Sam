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


    // characters main sound effect
    private AudioSource myMainSound;

    // when you enter water
    protected override void OnEnterWater()
    {
        // underwater ambience on
        AudioUnderwater.volume = 1f;

        //play splash
        myMainSound.clip = SoundSplash[0];
        myMainSound.Play();
    }

    protected override void OnExitWater()
    {
        // underwater ambience off
        AudioUnderwater.volume = 0f;

        //play splash
        myMainSound.clip = SoundSplash[0];
        myMainSound.Play();
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
