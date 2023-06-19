using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerStats : MonoBehaviour
{
    public float _CurrentHealth = 3f;
    public float _MaxHealth = 3f;
    public float invulnerable = 0;
    public float invulnerableTime = 2;
    public SO_Item_Inventory inventory;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<predatorFish>())
        {
            var fish = collision.gameObject.GetComponent<predatorFish>();
            if (fish.dead == true)
            {
                inventory.addItem(fish.SoLoot, 1);
                if (fish.harpooned == true)
                {
                    GetComponent<weaponController>()._Readyshot = true;
                    GetComponent<weaponController>()._Weapon.SetActive(true);
                }
                Destroy(collision.gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (_CurrentHealth < 1f)
            SceneManager.LoadScene(0);
    }
}
