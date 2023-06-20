using _SoggySam.scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobyDick : WaterStateHelper
{
    public float _MaxHitPoints;
    public float _HitPoints;
    public float _Speed;
    public float _Turn;
    public bool _Dead;
    public GameObject _myPlayer;
    public Animator _Animator;

    private float _EndOfMoveTime;
    private Rigidbody _myRB;
    public int _CurrentMove = 0;

    private bool _Push = false;
    private float _PushTimer = 0;

    private bool _ChargeUp = true;
    private bool _PlayedOnce = false;

    void Start()
    {
        if (_myPlayer == null)
            _myPlayer = GameManager.Instance.player;
        _myRB = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        AnimatorUpdate();
        if (_HitPoints < 0)
            _Dead = true;
        if (!_Dead)
        {
            var mouth = Physics.OverlapBox(transform.forward * 6 + transform.up + transform.position, Vector3.one * 3);
            foreach (Collider collider in mouth)
            {
                if (collider.tag == "Player")
                {
                    GameManager.Instance.stats.DamagePlayer();
                }
            }

            switch (_CurrentMove)
            {
                case 0:
                    ChoosingNextMove();
                    break;
                case 1:
                    Charge();
                    break;
                case 2:
                    BellyFlop();
                    break;
            }
        }   
    }
    private void AnimatorUpdate()
    {
        _Animator.SetFloat("Swim", _myRB.velocity.magnitude);
    }

    private void ChoosingNextMove()
    {
        if (InWater) Move();
        if (_EndOfMoveTime + Random.Range(5f, 30f) <= Time.time)
            _CurrentMove = Random.Range(1, 2);
    }

    private void Move()
    {
        if (InWater && !_Dead)
        {
            _myRB.AddForce(_Speed * Time.fixedDeltaTime * transform.forward);
        }
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
        }
        else if (10f >= Vector3.Distance(_myPlayer.transform.position, transform.position) && !_ChargeUp && !_Push) // end of move
        {
            _Animator.SetTrigger("Bite");
            _Push = true;
            _PushTimer = Time.time;
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

        }
        else if (InWater && _PlayedOnce)
        {
            _ChargeUp = true;
            _myRB.AddForce(3 * _Speed * Time.fixedDeltaTime * transform.forward);
            _CurrentMove = 0;
            _EndOfMoveTime = Time.time;
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
