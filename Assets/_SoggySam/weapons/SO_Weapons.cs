using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "weapon",menuName ="ScriptableObjects/Weapon", order = 2)]
public class SO_Weapons : ScriptableObject
{
    public string Name = "Missing name";

    public GameObject Prefab;

    public Vector3 ModelScale = new (1,1,1);

    public bool AOE = false;

    public int AOEDamage = 1;

    public int AOERaius = 0;

    public int Damage = 1;

    public float fireDelay = 0;

    public Vector3 Rotate = new (0,0,0);

    public bool UsingJoint = false;
        public bool JointAutoConfigureConnectedAnchor = false;
        public int JointConnectedMassScale = 1000;
        public bool JointEnableCollision = true;
        public int JointSprings = 100;
        public int JointDamper = 40;
        public float MaxJointDistance = 8;

}
