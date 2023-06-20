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

    public GameManager myGM;
    public gameStatePopUp myGameStateUI;

    public void FixedUpdate()
    {
        _HealthText.text = GameManager.Instance.stats._CurrentHealth + " / " + GameManager.Instance.stats._MaxHealth;
        _HealthBar.transform.localScale =new (1f * (GameManager.Instance.stats._CurrentHealth / GameManager.Instance.stats._MaxHealth),1,1);
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
