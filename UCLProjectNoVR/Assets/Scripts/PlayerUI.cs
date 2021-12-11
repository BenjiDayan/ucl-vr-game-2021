using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Slider healthBar;
    [SerializeField] Slider enemyHealthBar;
    [SerializeField] Slider completenessBar;
    [SerializeField] TextMeshProUGUI enemyHealth;
    [SerializeField] TextMeshProUGUI ammo;

    public void UpdateHealth(float healthFraction)
    {
        healthBar.value = healthFraction;
    }

    public void UpdateEnemyHealthBar(float fraction)
    {
        enemyHealthBar.value = fraction;
    }

    public void UpdateCompleteness(float fraction)
    {
        completenessBar.value = fraction;
    }

    public void UpdateEnemyHealth(float health) {
        enemyHealth.text = health.ToString();
    }

    public void UpdateAmmo(string ammoInfo)
    {
        ammo.text = ammoInfo;
    }



}
