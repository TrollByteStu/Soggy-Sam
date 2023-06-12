using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveMission : MonoBehaviour
{
    public SO_Mission mission;
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.missionManager.RemoveMission(mission);
    }
}
