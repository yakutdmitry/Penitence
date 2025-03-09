using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iDamageable
{
    void Damage(float damage¿mount);

    void Die();

    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }
}
