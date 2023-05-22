using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class cameraController : MonoBehaviour
{
    public Vector3 offset;
    public GameObject myPlayer;
    public Vector3 pointerOffset;
    private Vector3 renderOffset;

    void OnLook(InputValue value)
    {
        pointerOffset = value.Get<Vector2>();
        renderOffset = new Vector3(Display.main.renderingWidth / 2, Display.main.renderingHeight / 2, 0);
        pointerOffset -= renderOffset;
        pointerOffset = pointerOffset / 180;

    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position,myPlayer.transform.position + offset + pointerOffset, Time.deltaTime * 3);
    }
}
