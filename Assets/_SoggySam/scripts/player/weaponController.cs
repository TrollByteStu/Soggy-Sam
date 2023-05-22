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

    public int ammo = 1;
    public List<int> weapons;

    public Vector3 pointerOffset;
    private Vector3 renderOffset;
    private PlayerInput input;
    private SpringJoint joint;

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
        joint.maxDistance = Mathf.Clamp(joint.maxDistance, 0f, 15f);
        }
    }

    void OnFire()
    {
        if (readyshot)
            switch (ammo)
            {
                case 1:
                    if (!joint)
                    {
                        joint = gameObject.AddComponent<SpringJoint>();
                        joint.autoConfigureConnectedAnchor = false;
                        joint.connectedMassScale = 1000;
                        joint.enableCollision = true;
                        joint.spring = 100;
                        joint.damper = 40;
                    }
                        joint.maxDistance = 8f;
                    readyshot = false;
                    GameObject bullet = Instantiate(harpoonInstance, weapon.transform.position + weapon.transform.forward * 2, weapon.transform.rotation);
                    bullet.transform.Rotate(90, 0, 0);
                    bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.up * 420f);
                    bullet.transform.LookAt(bullet.GetComponent<Rigidbody>().velocity + bullet.transform.position, Vector3.up);
                    joint.connectedBody = bullet.GetComponent<Rigidbody>();
                    bullet.GetComponent<harpoonPhysics>().myPlayer = gameObject;
                    weapon.SetActive(false);
                    break;
                case 2:
                    GameObject bullet2 = Instantiate(harpoonInstance, weapon.transform.position + weapon.transform.forward * 2, weapon.transform.rotation);
                    bullet2.transform.Rotate(90, 0, 0);
                    bullet2.GetComponent<Rigidbody>().AddForce(bullet2.transform.up * 420f);
                    bullet2.transform.LookAt(bullet2.GetComponent<Rigidbody>().velocity + bullet2.transform.position, Vector3.up);
                    bullet2.GetComponent<harpoonPhysics>().myPlayer = gameObject;
                    lastShot = Time.time;
                    Destroy(bullet2, 8f);
                    break;
                case 3:
                    GameObject grenade = Instantiate(grenadeInstance, weapon.transform.position + weapon.transform.forward * 2, weapon.transform.rotation);
                    grenade.GetComponent<Rigidbody>().AddForce(grenade.transform.forward * 200);
                    universalGrenade grenadeScript = grenade.GetComponent<universalGrenade>();
                    grenadeScript.scale = new(0.2f, 0.2f, 0.2f);
                    grenadeScript.mass = 1;
                    grenadeScript.drag = 0.2f;
                    grenadeScript.angularDrag = 0.2f;
                    grenadeScript.aoeDamage = 1;
                    grenadeScript.aoeRadius = 5;
                    grenadeScript.fuseTime = 3f;
                    lastShot = Time.time;

                    break;
                case 4:
                    break;
            }
    }

    void OnLook(InputValue value)
    {
        pointerOffset = value.Get<Vector2>();
        renderOffset = new Vector3(Display.main.renderingWidth / 2, Display.main.renderingHeight / 2, 0);
        pointerOffset -= renderOffset;
        pointerOffset = pointerOffset / 25;
    }

    void OnAmmo1(InputValue value) { if (readyshot && weapons.Count > 1) ammo = weapons[0]; }
    void OnAmmo2(InputValue value) { if (readyshot && weapons.Count > 2) ammo = weapons[1]; }
    void OnAmmo3(InputValue value) { if (readyshot && weapons.Count > 3) ammo = weapons[2]; }
    void OnAmmo4(InputValue value) { if (readyshot && weapons.Count > 4) ammo = weapons[3]; }

    void FixedUpdate()
    {
        weapon.transform.LookAt(pointerOffset + transform.position, Vector3.up);
        if (!readyshot && ammo == 2 && lastShot + 0.2f <= Time.time)
            readyshot = true;
        if (!readyshot && ammo == 3 && lastShot + 0.5f <= Time.time)
            readyshot = true;
    }
}
