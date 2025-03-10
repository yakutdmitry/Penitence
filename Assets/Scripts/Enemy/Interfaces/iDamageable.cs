public interface iDamageable
{
    void TakeDamage(float damageAmount);
    void Die();
    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }
}
