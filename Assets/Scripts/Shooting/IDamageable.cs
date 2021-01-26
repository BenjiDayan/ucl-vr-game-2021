using System.Collections;

// Classes must inherit from this to be damaged by projectiles
public interface IDamageable
{
    void OnDamage(float damage);
    IEnumerator OnDamageCo(float damage);
}
