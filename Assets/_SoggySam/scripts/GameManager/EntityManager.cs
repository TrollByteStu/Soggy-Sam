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

    private void FixedUpdate()
    {
        if (_Bosses.Count > 0)
        {
            if (_Bosses.Count >= 2) // if there are 2 or more bosses sort by who is closes to player
                _Bosses.Sort((t2, t1) => Vector3.Distance(GameManager.Instance.player.transform.position, t2.transform.position).CompareTo(Vector3.Distance(GameManager.Instance.player.transform.position, t1.transform.position)));

            if (Vector3.Distance(_Bosses[0].transform.position, GameManager.Instance.player.transform.position) <= 100)
                GameManager.Instance._HudManager._Boss.gameObject.SetActive(true);
        }
    }
}
