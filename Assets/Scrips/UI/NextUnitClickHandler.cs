﻿using UnityEngine;

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
