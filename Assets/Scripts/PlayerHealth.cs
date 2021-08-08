using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] float hp = 500f;
    [SerializeField] float currentHp;
    [SerializeField] float droneProjectileDamage = 1f;
    [SerializeField] float healthRegenDelay = 10f;
    [SerializeField] float healthRegenRate = 5f;

    [SerializeField] PlayerUI ui;

    public float _healthFraction => currentHp / hp;

    float timeLastDamaged = 0f;

    private void Start()
    {
        currentHp = hp;
        ui.UpdateHealth(_healthFraction);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) OnDamage(9);
        if (Input.GetKeyDown(KeyCode.C)) ChangeHealth(9);

        if (Time.realtimeSinceStartup - timeLastDamaged > healthRegenDelay)
        {
            ChangeHealth(healthRegenRate * Time.deltaTime);
        }
    }

    public void OnDamage(float damage)
    {
        timeLastDamaged = Time.realtimeSinceStartup;

        float amount = Mathf.Abs(damage);

        ChangeHealth(-amount);
    }

    public IEnumerator OnDamageCo(float damage)
    {
        yield return null;
    }

    public void ChangeHealth(float health)
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Drone Projectile"))
        {
            OnDamage(droneProjectileDamage);
        }
    }
}
