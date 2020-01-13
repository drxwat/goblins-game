using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInformationUI : MonoBehaviour
{
    public Image portrait;
    public GameObject statsParent;
    public GameObject statPrefab;
    public enum ShowOn { Selection, Hover };
    public ShowOn showOn = ShowOn.Selection;

    Dictionary<Stat, UnitStatUI> statMapping; // mapping of stats of the selected unit to UI the stats

    UnitManager unitManager;

    // Start is called before the first frame update
    void Start()
    {
        transform.gameObject.SetActive(false);
        ClearUnitStats();
        portrait.sprite = null;

        unitManager = UnitManager.instance;
        if (showOn == ShowOn.Selection)
        {
            unitManager.onUnitSelection += DisplayUnitInformation;
            unitManager.onUnitDeselection += ClearUnitInformation;
        } else
        {
            unitManager.onUnitHover += DisplayUnitInformation;
            unitManager.onUnitLeave += ClearUnitInformation;
            unitManager.onUnitSelection += ClearUnitInformation;
        }
    }

    void DisplayUnitInformation(Unit unit)
    {
        portrait.sprite = unit.portrait;
        DisplayUnitStats(unit.getStats());
        unit.onStatsChange += DisplayUnitStats;
        transform.gameObject.SetActive(true);
    }

    void ClearUnitInformation(Unit unit)
    {
        portrait.sprite = null;
        unit.onStatsChange -= DisplayUnitStats;
        transform.gameObject.SetActive(false);
    }

    void DisplayUnitStats(UnitStats stats)
    {
        ClearUnitStats();
        statMapping = new Dictionary<Stat, UnitStatUI>();
        foreach (Stat stat in stats.elements)
        {
            GameObject statGO = Instantiate(statPrefab);
            statGO.transform.SetParent(statsParent.transform, false);

            UnitStatUI unitStatUI = statGO.GetComponent<UnitStatUI>();
            if (stat.hasMaxValue())
            {
                unitStatUI.SetValue(stat.value, stat.maxValue);
            } else
            {
                unitStatUI.SetValue(stat.value);
            }
            unitStatUI.SetIcon(stat.uiIcon);

            statMapping.Add(stat, unitStatUI);
        }
    }

    void ClearUnitStats()
    {
        foreach (Transform child in statsParent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
