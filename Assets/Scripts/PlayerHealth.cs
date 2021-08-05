using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] int hp = 100;
    [SerializeField] int currentHp;

    [SerializeField] PlayerUI ui;

    public float _healthFraction => (float)currentHp / hp;

    private void Start()
    {
        currentHp = hp;
        ui.UpdateHealth(_healthFraction);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) OnDamage(9);
        if (Input.GetKeyDown(KeyCode.C)) ChangeHealth(9);
    }

    public void OnDamage(float damage)
    {
        int amount = Mathf.Abs(Mathf.RoundToInt(damage));

        ChangeHealth(-amount);
    }

    public IEnumerator OnDamageCo(float damage)
    {
        yield return null;
    }

    public void ChangeHealth(int health)
    {
        currentHp += health;
        if (currentHp > hp) currentHp = hp;
        if (currentHp < 0) {
           currentHp = 0; 
           PauseMenu pauseMenu = (PauseMenu)FindObjectOfType(typeof(PauseMenu));
           pauseMenu.ReloadGame();
        }

        ui.UpdateHealth(_healthFraction);
    }
}
