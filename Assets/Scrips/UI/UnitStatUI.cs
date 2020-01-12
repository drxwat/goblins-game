using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitStatUI : MonoBehaviour
{
    [SerializeField]
    private Text statValueText;

    [SerializeField]
    private Image icon;
    
    public void SetValue(int value)
    {
        statValueText.text = value.ToString();
    }

    public void SetValue(int value, int maxValue)
    {
        statValueText.text = value.ToString() + "/" + maxValue.ToString();
    }

    public void SetIcon(Sprite iconSprite)
    {
        icon.sprite = iconSprite;
    }
}
