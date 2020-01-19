using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextUnitClickHandler : MonoBehaviour
{
    BattleManager battleManager;
    void Start()
    {
        battleManager = BattleManager.instance;
    }

    public void OnClick()
    {
        battleManager.SelectNextUnit();
    }
}
