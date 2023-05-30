using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class weaponController : MonoBehaviour
{
    public Camera myCamera;
    public GameObject weapon;
    public GameObject harpoonInstance;
    public GameObject grenadeInstance;
    public bool readyshot = true;

    public int currentWeapon = 0;
    public List<SO_Weapon> weapons;

    public Vector3 pointerOffset;
    private Vector3 renderOffset;
    private PlayerInput input;
    private SpringJoint joint;
    private float MaxJointDistance = 0;

    private float lastShot; 

    private void Start()
    {
        input = GetComponent<PlayerInput>();
    }

    private void OnScroll(InputValue value)
    {
        if (joint)
        {
        joint.maxDistance += (value.Get<float>() / 240);
        joint.maxDistance = Mathf.Clamp(joint.maxDistance, 0f, MaxJointDistance);
        }
    }

    void OnFire()
    {
        if (readyshot) 
        {
            readyshot = false; /// Stops spam until reloaded
            lastShot = Time.time;

            GameObject Bullet = Instantiate(weapons[currentWeapon].bullet, weapon.transform.position + weapon.transform.forward * 2, weapon.transform.rotation);
            Bullet.transform.Rotate(weapons[currentWeapon].Rotate);
            Bullet.GetComponent<Rigidbody>().AddForce(Bullet.transform.forward * weapons[currentWeapon].BarrelVelocity);
            

            if (weapons[currentWeapon].UsingJoint)  /// if weapon uses spring joint
            {
                if (!joint) /// if player is missing spring component
                    joint = gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = weapons[currentWeapon].JointAutoConfigureConnectedAnchor;
                joint.connectedMassScale = weapons[currentWeapon].JointConnectedMassScale;
                joint.enableCollision = weapons[currentWeapon].JointEnableCollision;
                joint.spring = weapons[currentWeapon].JointSprings;
                joint.damper = weapons[currentWeapon].JointDamper;
                joint.maxDistance = weapons[currentWeapon].StartJointDistance;
                joint.connectedBody = Bullet.GetComponent<Rigidbody>();
            }

            if (weapons[currentWeapon].SelfDestruct)
                Destroy(Bullet, weapons[currentWeapon].SelfDestructTimer);

            if (Bullet.GetComponent<harpoonPhysics>()) /// can currently be swaped out for weapons[currentWeapon].AOE == false
            {
                harpoonPhysics BulletScript = Bullet.GetComponent<harpoonPhysics>();
                BulletScript.myPlayer = gameObject;
                BulletScript.Damage = weapons[currentWeapon].Damage;
                BulletScript.SpringJoint = weapons[currentWeapon].UsingJoint;
                
            }
            else if (Bullet.GetComponent<universalGrenade>()) /// can currently be swaped out for weapons[currentWeapon].AOE == true
            {
                universalGrenade BulletScript = Bullet.GetComponent<universalGrenade>();
                BulletScript.scale = weapons[currentWeapon].bulletScale;
                BulletScript.aoeDamage = weapons[currentWeapon].Damage;
                BulletScript.aoeRadius = weapons[currentWeapon].AOERaius;
                if (!weapons[currentWeapon].BlowOnImpact)
                    BulletScript.fuseTime = weapons[currentWeapon].FuseTimer;

            }

        }
            
            ///switch (ammo)
            ///{
            ///    case 1:
            ///        if (!joint)
            ///        {
            ///            joint = gameObject.AddComponent<SpringJoint>();
            ///            joint.autoConfigureConnectedAnchor = false;
            ///            joint.connectedMassScale = 1000;
            ///            joint.enableCollision = true;
            ///            joint.spring = 100;
            ///            joint.damper = 40;
            ///        }
            ///            joint.maxDistance = 8f;
            ///        readyshot = false;
            ///        GameObject bullet = Instantiate(harpoonInstance, weapon.transform.position + weapon.transform.forward * 2, weapon.transform.rotation);
            ///        bullet.transform.Rotate(90, 0, 0);
            ///        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.up * 420f);
            ///        bullet.transform.LookAt(bullet.GetComponent<Rigidbody>().velocity + bullet.transform.position, Vector3.up);
            ///        joint.connectedBody = bullet.GetComponent<Rigidbody>();
            ///        bullet.GetComponent<harpoonPhysics>().myPlayer = gameObject;
            ///        weapon.SetActive(false);
            ///        break;
            ///    case 2:
            ///        GameObject bullet2 = Instantiate(harpoonInstance, weapon.transform.position + weapon.transform.forward * 2, weapon.transform.rotation);
            ///        bullet2.transform.Rotate(90, 0, 0);
            ///        bullet2.GetComponent<Rigidbody>().AddForce(bullet2.transform.up * 420f);
            ///        bullet2.transform.LookAt(bullet2.GetComponent<Rigidbody>().velocity + bullet2.transform.position, Vector3.up);
            ///        bullet2.GetComponent<harpoonPhysics>().myPlayer = gameObject;
            ///        lastShot = Time.time;
            ///        Destroy(bullet2, 8f);
            ///        readyshot = false;
            ///        break;
            ///    case 3:
            ///        GameObject grenade = Instantiate(grenadeInstance, weapon.transform.position + weapon.transform.forward * 2, weapon.transform.rotation);
            ///        grenade.GetComponent<Rigidbody>().AddForce(grenade.transform.forward * 600);
            ///        universalGrenade grenadeScript = grenade.GetComponent<universalGrenade>();
            ///        grenadeScript.scale = new(0.2f, 0.2f, 0.2f);
            ///        grenadeScript.mass = 1;
            ///        grenadeScript.drag = 0.2f;
            ///        grenadeScript.angularDrag = 0.2f;
            ///        grenadeScript.aoeDamage = 1;
            ///        grenadeScript.aoeRadius = 5;
            ///        grenadeScript.fuseTime = 3f;
            ///        lastShot = Time.time;
            ///        readyshot = false;
            ///        break;
            ///    case 4:
            ///
            ///        break;
            ///}
    }

    void OnLook(InputValue value)
    {
        pointerOffset = value.Get<Vector2>();
        renderOffset = new Vector3(Display.main.renderingWidth / 2, Display.main.renderingHeight / 2, 0);
        pointerOffset -= renderOffset;
        pointerOffset /= 25;
    }

    void OnAmmo1(InputValue value) { if (readyshot && weapons.Count >= 1) currentWeapon = 0; }
    void OnAmmo2(InputValue value) { if (readyshot && weapons.Count >= 2) currentWeapon = 1; }
    void OnAmmo3(InputValue value) { if (readyshot && weapons.Count >= 3) currentWeapon = 2; }
    void OnAmmo4(InputValue value) { if (readyshot && weapons.Count >= 4) currentWeapon = 3; }

    void FixedUpdate()
    {
        weapon.transform.LookAt(pointerOffset + transform.position, Vector3.up);
        if (!weapons[currentWeapon].ReloadOnPickup && lastShot <=  Time.time - weapons[currentWeapon].FireDelay)
            readyshot = true;

    }
}
