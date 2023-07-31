using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class HudManager : MonoBehaviour
{

    public Transform _HealthBar;
    public TMP_Text _HealthText;

    public GameObject _Menu;

    public gameStatePopUp myGameStateUI;

    public Transform _Boss;
    public Transform _BossHealthBar;
    public TMP_Text _BossHealthText;
    public TMP_Text _BossName;

    public void FixedUpdate()
    {
    }

    public void BossHealthBar(string Name, float CurrentHP, float MaxHP)
    {
        _BossName.text = Name;
        _BossHealthBar.localScale = new(Mathf.Clamp((CurrentHP / MaxHP), 0, 1), 1, 1);
        _BossHealthText.text = CurrentHP + " / " + MaxHP;
    }

    public void BossDeath() // play boss bar disapear animation
    {
        _Boss.gameObject.SetActive(false);
    }

    public void PlayerHealthBar(float CurrentHP , float MaxHP)
    {
        _HealthText.text = CurrentHP + " / " + MaxHP;
        _HealthBar.localScale = new( Mathf.Clamp((CurrentHP / MaxHP),0,1), 1, 1);
    }

    void OnEscape(InputValue value)
    {
        if (_Menu.activeSelf == false)
        {
            _Menu.SetActive(true);
            GameManager.Instance.player.GetComponent<PlayerInput>().enabled = false;
            Time.timeScale = 0;
        }
        else
        {
            _Menu.SetActive(false);
            GameManager.Instance.player.GetComponent<PlayerInput>().enabled = true;
            Time.timeScale = 1;
        }
    }
}
