using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "weapon",menuName ="ScriptableObjects/Weapon", order = 2)]
public class SO_Weapon : ScriptableObject
{
    public string Name = "Missing name";
    [Header("Model")]
    public GameObject Prefab;
    public GameObject bullet;

    public Vector3 bulletScale = new (1,1,1);
    public Vector3 Rotate = new (0,0,0);

    [Header("Sounds")]
    public AudioClip Sfx_Fire;
    public AudioClip Sfx_Reload;
    public AudioClip Sfx_Empty;

    [Header("Weapon Stats")]
    public bool AOE = false;
        [DrawIf("AOE", true)]
        public int AOERaius = 0;
        [DrawIf("AOE", true)]
        public int AOEExplosiveForceMultiplier = 400;
        [DrawIf("AOE", true)]
        public bool BlowOnImpact = false;
            [DrawIf("BlowOnImpact", false)]   
            public float FuseTimer = 1;
    public int Damage = 1;
    public float BarrelVelocity = 1000;
    public bool ReloadOnPickup = true;
        [DrawIf("ReloadOnPickup",false)]
        public float FireDelay = 0;

    [Header("RidgidBody")]
    public float Mass = 1;
    public float Drag = 1;
    public float AngularDrag = 1;
    public float WaterDrag = 1;
    public float WaterAngularDrag = 1;
    public bool usesGravity = true;

    [Header ("SpringJoint")]
    public bool UsingJoint = false;
        [DrawIf("UsingJoint", true)]
        public bool JointAutoConfigureConnectedAnchor = false;
        [DrawIf("UsingJoint", true)]
        public int JointConnectedMassScale = 1000;
        [DrawIf("UsingJoint", true)]
        public bool JointEnableCollision = true;
        [DrawIf("UsingJoint", true)]
        public int JointSprings = 100;
        [DrawIf("UsingJoint", true)]
        public int JointDamper = 40;
        [DrawIf("UsingJoint", true)]
        public float StartJointDistance = 8;
        [DrawIf("UsingJoint", true)]
        public float MaxJointDistance = 15;

    [Header("Clean Up")]
    public bool SelfDestruct = false;
        [DrawIf("SelfDestruct", true)]
        public float SelfDestructTimer = 5;

}
