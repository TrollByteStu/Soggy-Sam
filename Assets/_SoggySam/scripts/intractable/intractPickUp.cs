using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class intractPickUp : MonoBehaviour
{
    public TextMeshPro interactText;
    public float textTimer;

    void OnInteract()
    {

    }

    private void Update()
    {
        if (textTimer < Time.time)
            interactText.gameObject.SetActive(false);
    }
}
