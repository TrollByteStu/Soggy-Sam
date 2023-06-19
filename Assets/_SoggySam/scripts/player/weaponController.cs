using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class weaponController : MonoBehaviour
{
    public Camera _MyCamera;
    public GameObject _Weapon;
    public GameObject _EquipedWeapon;
    public GameObject _HarpoonInstance;
    public GameObject _GrenadeInstance;
    public bool _Readyshot = true;

    public int _CurrentWeapon = 0;
    public List<SO_Weapon> _Weapons;

    public Vector3 _PointerOffset;
    private Vector3 _RenderOffset;
    private PlayerInput _Input;
    private SpringJoint _Joint;

    private float lastShot; 

    private void Start()
    {
        if (_MyCamera == null)
            _MyCamera = GameManager.Instance._MainCamera;
        if (_EquipedWeapon == null)
        {
            WeaponChange(_CurrentWeapon);
        }
        _Input = GetComponent<PlayerInput>();
    }

    private void OnScroll(InputValue value)
    {
        if (_Joint)
        {
            _Joint.maxDistance += (value.Get<float>() / 240);
            _Joint.maxDistance = Mathf.Clamp(_Joint.maxDistance, 0f, _Weapons[_CurrentWeapon].MaxJointDistance);
        }
    }

    void WeaponChange(int _WeaponChange)
    {
        _CurrentWeapon = _WeaponChange;
        Destroy(_EquipedWeapon);
        _EquipedWeapon = Instantiate(_Weapons[_WeaponChange].Prefab);
        _EquipedWeapon.transform.parent = _Weapon.transform;
        _EquipedWeapon.transform.localPosition = Vector3.zero;
        _EquipedWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
        _EquipedWeapon.transform.localPosition = Vector3.forward / 2;

    }

    void OnFire()
    {
        if (_Readyshot) 
        {
            _Readyshot = false; /// Stops spam until reloaded
            lastShot = Time.time;

            GameObject Bullet = Instantiate(_Weapons[_CurrentWeapon].bullet, _Weapon.transform.position + _Weapon.transform.forward * 2, _Weapon.transform.rotation);
            Bullet.transform.Rotate(_Weapons[_CurrentWeapon].Rotate);
            Bullet.GetComponent<Rigidbody>().AddForce(Bullet.transform.forward * _Weapons[_CurrentWeapon].BarrelVelocity);
            

            if (_Weapons[_CurrentWeapon].UsingJoint)  /// if weapon uses spring joint
            {
                if (!_Joint) /// if player is missing spring component
                    _Joint = gameObject.AddComponent<SpringJoint>();
                _Joint.autoConfigureConnectedAnchor = _Weapons[_CurrentWeapon].JointAutoConfigureConnectedAnchor;
                _Joint.connectedMassScale = _Weapons[_CurrentWeapon].JointConnectedMassScale;
                _Joint.enableCollision = _Weapons[_CurrentWeapon].JointEnableCollision;
                _Joint.spring = _Weapons[_CurrentWeapon].JointSprings;
                _Joint.damper = _Weapons[_CurrentWeapon].JointDamper;
                _Joint.maxDistance = _Weapons[_CurrentWeapon].StartJointDistance;
                _Joint.connectedBody = Bullet.GetComponent<Rigidbody>();
            }

            if (_Weapons[_CurrentWeapon].SelfDestruct)
                Destroy(Bullet, _Weapons[_CurrentWeapon].SelfDestructTimer);

            if (Bullet.GetComponent<harpoonPhysics>()) /// can currently be swaped out for weapons[currentWeapon].AOE == false
            {
                harpoonPhysics BulletScript = Bullet.GetComponent<harpoonPhysics>();
                BulletScript.scale = _Weapons[_CurrentWeapon].bulletScale;
                BulletScript.myPlayer = gameObject;
                BulletScript.Damage = _Weapons[_CurrentWeapon].Damage;
                BulletScript.SpringJoint = _Weapons[_CurrentWeapon].UsingJoint;
                BulletScript.mass = _Weapons[_CurrentWeapon].Mass;
                BulletScript.drag = _Weapons[_CurrentWeapon].Drag;
                BulletScript.angularDrag = _Weapons[_CurrentWeapon].AngularDrag;
                BulletScript.WaterDrag = _Weapons[_CurrentWeapon].WaterDrag;
                BulletScript.WaterAngularDrag = _Weapons[_CurrentWeapon].WaterAngularDrag;
            }
            else if (Bullet.GetComponent<universalGrenade>()) /// can currently be swaped out for weapons[currentWeapon].AOE == true
            {
                universalGrenade BulletScript = Bullet.GetComponent<universalGrenade>();
                BulletScript.scale = _Weapons[_CurrentWeapon].bulletScale;
                BulletScript.aoeDamage = _Weapons[_CurrentWeapon].Damage;
                BulletScript.aoeRadius = _Weapons[_CurrentWeapon].AOERaius;
                BulletScript.mass = _Weapons[_CurrentWeapon].Mass;
                BulletScript.drag = _Weapons[_CurrentWeapon].Drag;
                BulletScript.angularDrag = _Weapons[_CurrentWeapon].AngularDrag;
                BulletScript.WaterDrag = _Weapons[_CurrentWeapon].WaterDrag;
                BulletScript.WaterAngularDrag = _Weapons[_CurrentWeapon].WaterAngularDrag;
                if (!_Weapons[_CurrentWeapon].BlowOnImpact)
                    BulletScript.fuseTime = _Weapons[_CurrentWeapon].FuseTimer;

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
        _PointerOffset = value.Get<Vector2>();
        _RenderOffset = new Vector3(Display.main.renderingWidth / 2, Display.main.renderingHeight / 2, 0);
        _PointerOffset -= _RenderOffset;
        _PointerOffset /= 25;
    }

    void OnAmmo1(InputValue value) { if (_Readyshot && _Weapons.Count >= 1) WeaponChange(0); }
    void OnAmmo2(InputValue value) { if (_Readyshot && _Weapons.Count >= 2) WeaponChange(1); }
    void OnAmmo3(InputValue value) { if (_Readyshot && _Weapons.Count >= 3) WeaponChange(2); }
    void OnAmmo4(InputValue value) { if (_Readyshot && _Weapons.Count >= 4) WeaponChange(3); }

    void FixedUpdate()
    {
        _Weapon.transform.LookAt(_PointerOffset + transform.position, Vector3.up);
        if (!_Weapons[_CurrentWeapon].ReloadOnPickup && lastShot <=  Time.time - _Weapons[_CurrentWeapon].FireDelay)
            _Readyshot = true;

    }
}
