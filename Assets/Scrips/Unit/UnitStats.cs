using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public Stat attack;
    public Stat damage;
    public Stat deffence;
    public Stat helth;
    public Stat movementPoints;

    public Stat[] elements;

    public delegate void StatsChangeTrigger(Stat stat, int value);
    public StatsChangeTrigger onStatChange; // broadcasts particular stat change

    private void Awake()
    {
        helth.value = helth.maxValue;
        movementPoints.value = movementPoints.maxValue;

        attack.maxValue = 0;
        deffence.maxValue = 0;

        elements = new Stat[] { attack, damage, deffence, helth, movementPoints };

        foreach(Stat stat in elements)
        {
            stat.onStatChange += BroadcastStatChange;
        }
    }

    void BroadcastStatChange(Stat stat, int value)
    {
        onStatChange?.Invoke(stat, value);
    }
}
