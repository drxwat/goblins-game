using UnityEngine;
using System;

[Serializable]
public class Stat
{
    [SerializeField]
    int _value;

    public int value
    {
        get => _value;
        set {
            _value = value;
            onStatChange?.Invoke(this, value);
        }
    }

    [SerializeField]
    int _maxValue = 0;
    public int maxValue
    {
        get => _maxValue;
        set => _maxValue = value;
    }

    public Sprite uiIcon;

    public delegate void StatChangeTrigger(Stat stat, int value);
    public StatChangeTrigger onStatChange;

    public int GetMaxValue()
    {
        return _maxValue;
    }

    public bool hasMaxValue()
    {
        return _maxValue != 0;
    }

}
