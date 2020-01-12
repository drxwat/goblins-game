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

    private void Awake()
    {
        helth.value = helth.maxValue;
        movementPoints.value = movementPoints.maxValue;

        attack.maxValue = 0;
        deffence.maxValue = 0;

        elements = new Stat[] { attack, damage, deffence, helth, movementPoints };
    }

}
