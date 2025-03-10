using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iTriggerCheckable
{
    bool IsAggroed { get; set; }
    bool IsWithinStrikingDistance { get; set; }

    void SetAggroedBool(bool value);
    void SetStrikingDistanceBool(bool value);
}
