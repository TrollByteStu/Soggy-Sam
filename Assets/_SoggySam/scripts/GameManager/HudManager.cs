using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudManager : MonoBehaviour
{

    public Transform _HealthBar;
    public TMP_Text _HealthText;

    public gameStatePopUp myGameStateUI;

    public void FixedUpdate()
    {
        _HealthText.text = GameManager.Instance.stats._CurrentHealth + " / " + GameManager.Instance.stats._MaxHealth;
        _HealthBar.transform.localScale =new (1f * (GameManager.Instance.stats._CurrentHealth / GameManager.Instance.stats._MaxHealth),1,1);
    }
}
