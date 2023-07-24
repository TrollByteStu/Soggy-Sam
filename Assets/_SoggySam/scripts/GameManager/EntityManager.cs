using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public List<GameObject> _Bosses;
    public List<GameObject> _Fishes;

    private void Start()
    {
        var array = GameObject.FindGameObjectsWithTag("Boss");
        _Bosses.AddRange(array);
        array = GameObject.FindGameObjectsWithTag("Fish");
        _Fishes.AddRange(array);
    }
}
