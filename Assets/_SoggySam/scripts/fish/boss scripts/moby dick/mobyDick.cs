using _SoggySam.scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobyDick : WaterStateHelper
{
    private fishBuoyancy[] _Floaters;

    public int _PickedName = 0;
    public List<string> _names;
    public float _MaxHitPoints;
    public float _HitPoints;
    [Tooltip("needs to be more than 0 so grenade do to much damage")]
    public float _InvulnerableTime = 0.1f;
    private float _Invulnerable = 0;
    public float _Speed;
    public float _Turn; // turnspeed not used yet
    public bool _Dead;
    private bool _FirstFrameOnDeath = true;
    public GameObject _myPlayer;
    public Animator _Animator;

    private float _EndOfMoveTime;
    private Rigidbody _myRB;
    public int _CurrentMove = 0;

    private float _RunAwayTime = 0;

    private bool _Push = false;
    private float _PushTimer = 0;

    private bool _ChargeUp = true;
    private bool _PlayedOnce = false;

    private float _ZOffset = 0;
    private float _POM = 1; // why did i call it POM wtf does POM  Player Offset Mark???
    public float _FlipCount = 0;


    void Start()
    {
        _Floaters = transform.GetComponentsInChildren<fishBuoyancy>();
        if (_myPlayer == null)
            _myPlayer = GameManager.Instance.player;
        _myRB = GetComponent<Rigidbody>();
        PickAName();
        GameManager.Instance._HudManager.BossHealthBar(name, _HitPoints, _MaxHitPoints);
    }

    void FixedUpdate()
    {
        AnimatorUpdate();
        if (_HitPoints < 1 && !_Dead)
        {
            _Dead = true;
            _myRB.useGravity = false;
            _Animator.SetBool("Dead", true);
            transform.LookAt(new Vector3 (transform.forward.x + transform.position.x, transform.position.y, transform.position.z),Vector3.up);
        }
        if (!_Dead)
        {
            var mouth = Physics.OverlapBox(transform.forward * 6 + transform.up + transform.position, Vector3.one * 3);
            foreach (Collider collider in mouth)
            {
                if (collider.tag == "Player")
                {
                    GameManager.Instance.stats.DamagePlayer();
                }
                else if (collider.GetComponent<transport>())
                {
                    collider.GetComponent<transport>().DamageTransport();
                }
            }

            switch (_CurrentMove)
            {

                case -1:
                    SwimAway();
                    break;
                case 0:
                    ChoosingNextMove();
                    break;
                case 1:
                    Charge();
                    break;
                case 2:
                    BellyFlop();
                    break;
                case 3:
                    Background();
                    break;
            }

            transform.position = new Vector3(transform.position.x, transform.position.y, _ZOffset);
        }
        else
        {
            Dead();
        }
    }
    
    void Dead()
    {
        foreach (fishBuoyancy Floater in _Floaters)
            Floater.dead();
        if (_FirstFrameOnDeath)
            _FirstFrameOnDeath = false;
        GameManager.Instance._EntityManager._Bosses.Remove(gameObject);
        GameManager.Instance._HudManager.BossDeath();

    }
    
    private void PickAName()
    {
        _PickedName = Random.Range(0, _names.Count - 1);
        name = _names[_PickedName];
    }

    public void DamageMoby(float _Damage) // custom Damage number
    {
        if (CanDamageMoby())
        {
            _HitPoints -= _Damage;
            GameManager.Instance._HudManager.BossHealthBar(name,_HitPoints, _MaxHitPoints);
            if (Random.Range(1, 5) <= _Damage) // picks and plays damage animation
            {
                _Animator.SetTrigger("Damage");
                _Animator.SetInteger("DamageAnim", Random.Range(1,2));
            }

            if (Random.Range(1, 10) <= _Damage) //moby retreats
            {
                _CurrentMove = -1;
                _RunAwayTime = Time.time;
            }
                _Invulnerable = Time.time + _InvulnerableTime;
        }
    }    

    public void DamageMoby() // 1 Damage
    {
        if (CanDamageMoby())
        {
            _HitPoints--;
            GameManager.Instance._HudManager.BossHealthBar(name, _HitPoints, _MaxHitPoints);
            if (Random.Range(1, 5) <= 1) // picks and plays damage animation
            {
                _Animator.SetTrigger("Damage");
                _Animator.SetInteger("DamageAnim", Random.Range(1, 2));
            }

            if (Random.Range(1, 10) <= 1) // moby retreats
            {
                _CurrentMove = -1;
                _RunAwayTime = Time.time;
            }
                _Invulnerable = Time.time + _InvulnerableTime;
        }
    }

    private bool CanDamageMoby() // tests if moby is invulnerable
    {
        if (_Invulnerable <= Time.time)
            return true;
        else
            return false;
    }

    private void AnimatorUpdate()
    {
        _Animator.SetFloat("Swim", _myRB.velocity.magnitude);
    }

    private void ChoosingNextMove()
    {
        if (InWater) Move();
        if (_EndOfMoveTime + Random.Range(5f, 30f) <= Time.time)
            _CurrentMove = Random.Range(1, 3);
    }

    private void SwimAway() // retreat. cant be picked by ChoosingNextMove 
    {
        if (_ChargeUp && InWater) // swim away
        {
            transform.LookAt(_myPlayer.transform.position + (Vector3.up * 10f));
            transform.Rotate(Vector3.up, 180);
            _myRB.AddForce(2 * _Speed * Time.fixedDeltaTime * transform.forward);
        }

        if (_RunAwayTime + 5 < Time.time)
            _CurrentMove = 0;
    }

    private void Move()
    {
            _myRB.AddForce(_Speed * Time.fixedDeltaTime * transform.forward);   
    }

    private void Charge()
    {
        if (_ChargeUp && InWater) // swim away
        {
            transform.LookAt(_myPlayer.transform.position + (Vector3.up * 10f));
            transform.Rotate(Vector3.up,180);
            _myRB.AddForce(2 * _Speed * Time.fixedDeltaTime * transform.forward);
        }
        else if (!_ChargeUp && InWater) // swim towards
        {
            transform.LookAt(_myPlayer.transform);
            _myRB.AddForce(3 * _Speed * Time.fixedDeltaTime * transform.forward);
        }

        if (100f <= Vector3.Distance(_myPlayer.transform.position, transform.position) && _ChargeUp) // swim 100 units away 
        {
            _ChargeUp = false;
            _Animator.SetBool("Charging", true);
        }
        else if (10f >= Vector3.Distance(_myPlayer.transform.position, transform.position) && !_ChargeUp && !_Push) // end of move
        {
            _Animator.SetTrigger("Bite");
            _Push = true;
            _PushTimer = Time.time;
        }
        else if (20f >= Vector3.Distance(_myPlayer.transform.position, transform.position) && !_ChargeUp && !_Push) // end of move
        {
            _Animator.SetTrigger("Charge");
            _Animator.SetBool("Charging", false);
        }

        if (_Push && _PushTimer + 2f < Time.time && InWater)
        {
            _Push = false;
            _ChargeUp = true;
            transform.Rotate(Vector3.up, 180);
            _CurrentMove = 0;
            _EndOfMoveTime = Time.time;
        }
        else if (InWater)
        {
            _myRB.AddForce(2 * _Speed * Time.fixedDeltaTime * transform.forward);
        }

    }

    private void BellyFlop()
    {
        if (100f <= Vector3.Distance(_myPlayer.transform.position, transform.position) && _ChargeUp) // swim 100 units away 
        {
            _ChargeUp = false;
        }
        else if (10f >= Vector3.Distance(_myPlayer.transform.position, transform.position) && !_ChargeUp) // end of move
        {
            _ChargeUp = true;
            _Animator.SetTrigger("Bite");
            _myRB.AddForce(5 * _Speed * Time.fixedDeltaTime * transform.forward);
            _CurrentMove = 0;
            _EndOfMoveTime = Time.time; 
            _PlayedOnce = false;

        }
        else if (InWater && _PlayedOnce)
        {
            _ChargeUp = true;
            _myRB.AddForce(3 * _Speed * Time.fixedDeltaTime * transform.forward);
            _CurrentMove = 0;
            _EndOfMoveTime = Time.time;
            _PlayedOnce = false;
        }

        if (_ChargeUp && InWater) // swim away
        {
            transform.LookAt(_myPlayer.transform.position + (Vector3.up * 70f));
            transform.Rotate(Vector3.up, 180);
            _myRB.AddForce(2 * _Speed * Time.fixedDeltaTime * transform.forward);
        }
        else if (!_ChargeUp && InWater) // swim towards
        {
            transform.LookAt(_myPlayer.transform.position + (Vector3.up * 150f));
            _myRB.AddForce(10 * _Speed * Time.fixedDeltaTime * transform.forward);
        }
        else if (!_ChargeUp && !InWater && !_PlayedOnce)
        {
            _Animator.SetTrigger("Jump");
            _PlayedOnce = true;
        }

        if (_myRB.velocity.y < 0 && !InWater)
        {
            _Animator.SetTrigger("BodySlam");
            transform.LookAt(_myPlayer.transform);
            
        }

    }
    
    private void Background() // swim in the background
    {
        if (_ChargeUp && InWater) // swim away
        {
            transform.LookAt(_myPlayer.transform.position + (Vector3.up * 10f));
            transform.Rotate(Vector3.up, 180);
            _myRB.AddForce(2 * _Speed * Time.fixedDeltaTime * transform.forward);
        }

        if (100f <= Vector3.Distance(_myPlayer.transform.position, transform.position) && _ChargeUp) // swim 100 units away 
        {
            _ChargeUp = false;
            _ZOffset = 20;
        }

        if (!_ChargeUp)
        {
            transform.LookAt(_myPlayer.transform.position + new Vector3(80f * _POM, -15f, _ZOffset), Vector3.up);
            _myRB.AddForce(2 * _Speed * Time.fixedDeltaTime * transform.forward);
        }

        if (_FlipCount > 3 && !_ChargeUp)
        {
            _ZOffset = 0;
            _CurrentMove = 1;
        }
        else if (5f >= Vector3.Distance(_myPlayer.transform.position + new Vector3(80f * _POM ,-15f ,_ZOffset), transform.position) && !_ChargeUp)
        {
            _POM *= -1f;
            _FlipCount++;
        }



    }

    protected override void OnEnterWater()
    {
        _Animator.SetBool("InWater", true);
        _myRB.drag = 5;
        _myRB.angularDrag = 2f;
        _myRB.useGravity = false;
    }

    protected override void OnExitWater()
    {
        _Animator.SetBool("InWater", false);
        _myRB.drag = 0.5f;
        _myRB.angularDrag = 0.5f;
        _myRB.useGravity = true;
    }

}
