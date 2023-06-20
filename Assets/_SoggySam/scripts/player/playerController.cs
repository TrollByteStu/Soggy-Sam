using System.Collections;
using System.Collections.Generic;
using _SoggySam.scripts.GameManager;
using _SoggySam.scripts.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;

public class playerController : WaterStateHelper
{
    public Animator animator;
    public GameObject myAvatar;
    public SoundManager soundManager;
    protected Rigidbody myRB;
    protected CapsuleCollider myCC;
    protected PlayerInput myPI;

    protected Vector3 moveVector = new Vector3(0, 0, 0);
    protected float jump;

    protected RaycastHit groundRay;
    protected RaycastHit interactRay;



    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        myCC = GetComponent<CapsuleCollider>();
        myPI = GetComponent<PlayerInput>();
    }

    void OnMove(InputValue value)
    {
        moveVector = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        jump = value.Get<float>();
    }

    void OnInteract()
    {
        Physics.Raycast(transform.position, transform.forward, out interactRay, 10);
        if (interactRay.collider != null && myPI != null)
        {
            if (interactRay.collider.tag == "Interactable" && !interactRay.collider.transform.parent.GetComponent<intractPickUp>())
            {
                transform.parent = interactRay.collider.transform.parent;
                myPI.enabled = false;
                myRB.isKinematic = true;
                myCC.enabled = false;
                GameObject temp = interactRay.collider.transform.parent.gameObject;
                temp.GetComponent<intractControllable>().myPlayer = gameObject;
                temp.GetComponent<intractControllable>().intractLock = true;
                temp.GetComponent<PlayerInput>().enabled = true;
                GameManager.Instance._MainCameraScript._TransportOffset = temp.GetComponent<intractControllable>()._TransportOffset;
            }
            else if (interactRay.collider.tag == "Interactable"  && interactRay.collider.transform.parent.GetComponent<intractPickUp>())
            {

            }
        }
    }

    protected void InteractText()
    {
        if (Physics.Raycast(transform.position, transform.forward, out interactRay, 10))
        {
            Debug.DrawRay(transform.position, transform.forward);
            if (interactRay.collider.CompareTag("Interactable"))
            {
                GameObject temp = interactRay.collider.transform.parent.gameObject;
                if (temp.GetComponent<transport>())
                {
                    temp.GetComponent<transport>().interactText.gameObject.SetActive(true);
                    temp.GetComponent<transport>().textTimer = 1 + Time.time;
                }
                if (temp.GetComponent<cannon>())
                {
                    temp.GetComponent<cannon>().interactText.gameObject.SetActive(true);
                    temp.GetComponent<cannon>().textTimer = 1 + Time.time;
                }
                if (temp.GetComponent<intractPickUp>())
                {
                    temp.GetComponent<intractPickUp>().interactText.gameObject.SetActive(true);
                    temp.GetComponent<intractPickUp>().textTimer = 1 + Time.time;
                }
            }
        }
    }
}
