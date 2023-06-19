using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobyDick : MonoBehaviour
{
    public float _Hitpoints;
    public float _Speed;
    public float _Turn;
    public bool _Dead;
    public GameObject _myPlayer;

    private float _EndOfMoveTime;
    private Rigidbody _myRB;
    public int _CurrentMove = 0;
    private bool _InWater;

    private bool _ChargeUp = true;
    void Start()
    {
        if (_myPlayer == null)
            _myPlayer = GameManager.Instance.player;
        _myRB = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        switch (_CurrentMove)
        {
            case 0:
                ChoosingNextMove();
                break;
            case 1:
                if (_InWater) Charge();
                break;
            case 2:
                BellyFlop();
                break;
        }
        
    }
    void ChoosingNextMove()
    {
        if (_InWater) Move();
        if (_EndOfMoveTime + Random.Range(5f, 30f) <= Time.time)
            _CurrentMove = Random.Range(1, 2);


    }

    private void Move()
    {
        if (_InWater && !_Dead)
        {
            _myRB.AddForce(_Speed * Time.fixedDeltaTime * transform.forward);
        }
    }

    private void Charge()
    {
        if (_ChargeUp) // swim away
        {
            transform.LookAt(_myPlayer.transform.position + (Vector3.up * 10f));
            transform.Rotate(Vector3.up,180);
            _myRB.AddForce(2 * _Speed * Time.fixedDeltaTime * transform.forward);
        }
        else if (!_ChargeUp) // swim towards
        {
            transform.LookAt(_myPlayer.transform);
            _myRB.AddForce(3 * _Speed * Time.fixedDeltaTime * transform.forward);
        }
        if (100f <= Vector3.Distance(_myPlayer.transform.position, transform.position) && _ChargeUp) // swim 100 units away 
        {
            _ChargeUp = false;
        }
        else if (10f >= Vector3.Distance(_myPlayer.transform.position, transform.position) && !_ChargeUp) // end of move
        {
            _ChargeUp = true;
            transform.Rotate(Vector3.up, 180);
            _CurrentMove = 0;
            _EndOfMoveTime = Time.time;
            
        }

    }

    private void BellyFlop()
    {
        if (_ChargeUp && _InWater) // swim away
        {
            transform.LookAt(_myPlayer.transform.position + (Vector3.up * 70f));
            transform.Rotate(Vector3.up, 180);
            _myRB.AddForce(2 * _Speed * Time.fixedDeltaTime * transform.forward);
        }
        else if (!_ChargeUp && _InWater) // swim towards
        {
            transform.LookAt(_myPlayer.transform.position + (Vector3.up * 150f));
            _myRB.AddForce(10 * _Speed * Time.fixedDeltaTime * transform.forward);
        }

        if (_myRB.velocity.y < 0 && !_InWater)
        {
            transform.LookAt(_myPlayer.transform);
        }

        if (100f <= Vector3.Distance(_myPlayer.transform.position, transform.position) && _ChargeUp) // swim 100 units away 
        {
            _ChargeUp = false;
        }
        else if (10f >= Vector3.Distance(_myPlayer.transform.position, transform.position) && !_ChargeUp) // end of move
        {
            _ChargeUp = true;
            _myRB.AddForce(5 * _Speed * Time.fixedDeltaTime * transform.forward);
            _CurrentMove = 0;
            _EndOfMoveTime = Time.time;

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            _myRB.drag = 5;
            _myRB.angularDrag = 2f;
            _myRB.useGravity = false;
            _InWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            _myRB.drag = 0.5f;
            _myRB.angularDrag = 0.5f;
            _myRB.useGravity = true;
            _InWater = false;
        }
    }
}
