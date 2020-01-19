using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public List<Team> teams = new List<Team>();

    [HideInInspector]
    public int turnNumber = 1;

    public static BattleManager instance;

    Team currentTeam;

    Grid grid;
    UnitsStore unitsStore;
    UnitManager unitManager;
    CameraMotor cameraMotor;
    TurnStartText turnStartText;

    System.Random random = new System.Random();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        grid = Grid.instance;
        unitsStore = UnitsStore.instance;
        unitManager = UnitManager.instance;
        cameraMotor = Camera.main.GetComponent<CameraMotor>();
        turnStartText = GameObject.FindObjectOfType<TurnStartText>();

        currentTeam = teams[0]; // attacker moves first
        ShowTeamTurnUI();

        // assuming for now that there are two teams only and they have random units
        // TODO: Teams should be inserted from outside of the script (probably by UI on previous screen)
        int unitsN = 5;
        foreach (Team t in teams)
        {
            for (int i = 0; i < unitsN; i++)
            {
                int rndUnitIndex = random.Next(unitsStore.awailableUnits.Length);
                t.storedUnits.Add(unitsStore.awailableUnits[rndUnitIndex]);
            }
        }

        int attackerNodeX = grid.gridSizeX / 2 - teams[0].storedUnits.Count / 2;
        int attackerNodeY = 1;
        PlaceTeamUnits(teams[0], attackerNodeX, attackerNodeY);

        int defenderNodeX = grid.gridSizeX / 2 - teams[1].storedUnits.Count / 2;
        int defenderNodeY = grid.gridSizeY - 2;
        PlaceTeamUnits(teams[1], defenderNodeX, defenderNodeY);
        foreach(Unit u in teams[1].units)
        {
            u.RotateTo(u.transform.position);
        }
    }

    public Team GetCurrentTeam()
    {
        return currentTeam;
    }

    public void EndTurn()
    {
        foreach(Unit u in currentTeam.units)
        {
            if (!u.isDead)
            {
                u.ResetActivity();
            }
        }

        SelectNextTeam();
        ShowTeamTurnUI();
        FocusCameraOnTeam(currentTeam);

        if (unitManager.GetSelectedUnit() != null)
        {
            unitManager.DeselectUnit();
        }
        turnNumber++;
    }

    public void SelectNextUnit()
    {
        List<Unit> units = currentTeam.units.ToList();
        Unit selectedUnit = unitManager.GetSelectedUnit();
        Unit nextUnit = null;
        if (selectedUnit != null && currentTeam.units.Contains(selectedUnit))
        {
            int selectedIndex = units.FindIndex(u => u == selectedUnit);
            if (selectedIndex + 1 != units.Count)
            {
                // Search in Tail
                nextUnit = GetNextActiveUnit(units, selectedIndex + 1, units.Count);
            }
            
            if (nextUnit == null)
            {
                // Search in Head
                nextUnit = GetNextActiveUnit(units, 0, selectedIndex);
            }
            
        } else
        {
            // Serach in All Team
            nextUnit = GetNextActiveUnit(units, 0, units.Count);
        }

        if (nextUnit == null)
        {
            Debug.Log("No one active unit found");
            return;
        }

        unitManager.SelectUnit(nextUnit);
        cameraMotor.MoveTo(nextUnit.transform.position); // Focus the selected unit
    }

    public bool AreUnitsAllies(Unit a, Unit b)
    {
        foreach(Team t in teams)
        {
            // not optimal but short :D
            if (t.units.Contains(a) && t.units.Contains(b))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsCurrentTeamUnit(Unit unit)
    {
        return currentTeam.units.Contains(unit);
    }

    void PlaceTeamUnits(Team team, int nodeX, int nodeY)
    {
        for(int i = 0; i < team.storedUnits.Count; i++)
        {
            Node node = grid.grid[nodeX + i, nodeY];
            StoredUnit storedUnit = team.storedUnits[i];
            GameObject obj = Instantiate(storedUnit.unitPrefab, node.worldPosition, Quaternion.identity);

            Unit unit = obj.GetComponent<Unit>();
            team.units.Add(unit);
        }
    }

    void SelectNextTeam()
    {
        int currentTeamIndex = teams.FindIndex(t => t == currentTeam);
        if (currentTeamIndex == teams.Count - 1)
        {
            currentTeam = teams[0];
        }
        else
        {
            currentTeam = teams[currentTeamIndex + 1];
        }
    }

    Unit GetNextActiveUnit(List<Unit> units, int startIndex, int EndIndex)
    {
        for (int i = startIndex; i < EndIndex; i++)
        {
            if (units[i].HasActivity())
            {
                return units[i];
            }
        }
        return null;
    }

    void ShowTeamTurnUI()
    {
        turnStartText.ShowText(currentTeam.name + " Turn", 3f);
    }

    void FocusCameraOnTeam(Team team)
    {
        Vector3[] teamPositions = team.units.Where(u => !u.isDead).Select(u => u.transform.position).ToArray();
        Vector3 sum = Vector3.zero;
        foreach (Vector3 pos in teamPositions)
        {
            sum += pos;
        }
        Vector3 centroid = sum / teamPositions.Length;
        cameraMotor.MoveTo(centroid);
    }

}
