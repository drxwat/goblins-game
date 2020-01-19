using UnityEngine;

public class UnitsStore : MonoBehaviour
{
    public StoredUnit[] awailableUnits;

    public static UnitsStore instance;
    void Awake()
    {
        instance = this;
    }
}
