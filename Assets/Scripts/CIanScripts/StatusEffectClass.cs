using System.Collections;
using UnityEngine;

public class StatusEffectClass
{
    public string EffectName { get; set; }
    public float Duration { get; set; }
    public float TickRate { get; set; }
    public float DamagePerTick { get; set; }
    public float SlowPercentage { get; set; }

    public StatusEffectClass(string name, float duration, float tickRate, float damagePerTick = 0f, float slowPercentage = 0f)
    {
        EffectName = name;
        Duration = duration;
        TickRate = tickRate;
        DamagePerTick = damagePerTick;
        SlowPercentage = slowPercentage;
    }
}
