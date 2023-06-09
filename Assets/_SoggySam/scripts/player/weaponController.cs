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

    // Sounds
    [SerializeField] private AudioSource WeaponAudio;
    private AudioClip Sfx_Weapon_Fire;
    private AudioClip Sfx_Weapon_Reload;
    private AudioClip Sfx_Weapon_Empty;

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

        //change sound
        Sfx_Weapon_Fire = _Weapons[_WeaponChange].Sfx_Fire;
        Sfx_Weapon_Reload = _Weapons[_WeaponChange].Sfx_Reload;
        Sfx_Weapon_Empty = _Weapons[_WeaponChange].Sfx_Empty;
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

            //play sound
            WeaponAudio.clip = Sfx_Weapon_Fire;
            WeaponAudio.Play();

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
                BulletScript.ReloadOnPickup = _Weapons[_CurrentWeapon].ReloadOnPickup;
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

        } else { // no ready shot
            // play sound
            WeaponAudio.clip = Sfx_Weapon_Empty;
            WeaponAudio.Play();
        }
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
