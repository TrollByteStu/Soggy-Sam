using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMission : MonoBehaviour
{
    public SO_Mission mission;
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.missionManager.AddMission(mission);
    }
}
