using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Slider healthBar;

    public void UpdateHealth(float healthFraction)
    {
        healthBar.value = healthFraction;
    }
}
