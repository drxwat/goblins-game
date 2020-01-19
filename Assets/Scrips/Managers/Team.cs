using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Team
{
    public string name;
    public Color color;

    [HideInInspector]
    public HashSet<Unit> units = new HashSet<Unit>();

    // Units meta for creating real units
    [HideInInspector]
    public List<StoredUnit> storedUnits = new List<StoredUnit>();
}
