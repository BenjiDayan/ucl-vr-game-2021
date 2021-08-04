using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI enemyHealth;
    public void UpdateHealth(float healthFraction)
    {
        healthBar.value = healthFraction;
    }

    public void UpdateEnemyHealth(float health) {
        enemyHealth.text = health.ToString();
    }



}
