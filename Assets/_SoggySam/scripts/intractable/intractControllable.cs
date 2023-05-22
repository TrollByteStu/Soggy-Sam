using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class intractControllable : MonoBehaviour
{
    protected PlayerInput myPI;

    public GameObject myPlayer;

    public bool intractLock;

    public TextMeshPro interactText;
    public float textTimer;
    void Start()
    {
        myPI = GetComponent<PlayerInput>();
    }

    void OnInteract()
    {
        intractLock = false;
        myPlayer.GetComponent<PlayerInput>().enabled = true;
        myPlayer.GetComponent<Rigidbody>().isKinematic = false;
        myPlayer.GetComponent<Collider>().enabled = true;
        myPlayer.transform.parent = null;
        myPI.enabled = false;
    }

    private void Update()
    {
        if (textTimer < Time.time)
            interactText.gameObject.SetActive(false);
    }
}
