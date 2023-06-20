using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class transport : intractControllable
{

    private Vector3 moveVector = new Vector3(0, 0, 0);
    private Rigidbody myRB;

    public float _MaxHitPoints = 0;
    public float _HitPoints = 0;
    public float moveSpeed = 0;
    public float invulnerable;
    public float invulnerableTime;

    public void DamageTransport()
    {
        if (CanDamageTransport())
        {
            invulnerable = Time.time + invulnerableTime;
            _HitPoints--;
        }
    }
    public void DamageTransport(int damage)
    {
        if (CanDamageTransport())
        {
            invulnerable = Time.time + invulnerableTime;
            _HitPoints -= damage;
        }
    }

    public bool CanDamageTransport()
    {
        if (invulnerable < Time.time)
            return true;
        else
            return false;
    }

    private void Start()
    {
        myPI = GetComponent<PlayerInput>();
        myRB = GetComponent<Rigidbody>();
    }

    void OnMove(InputValue value)
    {
        moveVector = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        myRB.AddForce(Vector3.right * moveVector.x * Time.deltaTime * moveSpeed * myRB.mass);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
