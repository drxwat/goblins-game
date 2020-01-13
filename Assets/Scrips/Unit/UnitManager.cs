﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class UnitManager : MonoBehaviour
{
    public LayerMask walkableLayer;
    public GameObject waypointPrefab;
    public GameObject waypointNextPrefab;
    public static UnitManager instance;

    public delegate void SlectionTrigger(Unit unit);
    public SlectionTrigger onUnitSelection;
    public SlectionTrigger onUnitDeselection;

    public delegate void HoverTrigger(Unit unit);
    public HoverTrigger onUnitHover;
    public HoverTrigger onUnitLeave;

    HashSet<Unit> unitsSet = new HashSet<Unit>();
    Unit selectedUnit;
    Node selectedTarget;
    Path currentPath;
    List<GameObject> pathCurve;
    Camera cam;
    AttackManager attackManager = new AttackManager();

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Unit selection/deselection
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit, 100, walkableLayer))
            {
                return;
            }

            Node hitNode = Grid.instance.NodeFromWorldPosition(hit.point);
            Unit hitUnit = GetUnitFromNode(hitNode);

            if (hitUnit != null)
            {
                if (selectedUnit && selectedUnit != hitUnit)
                {
                    DeselectUnit();
                }
                SelectUnit(hitUnit);
            }
            else if (selectedUnit) 
            {
                DeselectUnit();
            }
        }
        else if (Input.GetMouseButtonDown(1)) // Target selection & movement
        {
            if (selectedUnit == null)
            {
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit, 100, walkableLayer))
            {
                return;
            }

            Node hitNode = Grid.instance.NodeFromWorldPosition(hit.point);
            
            if (selectedTarget == hitNode)
            {
                // Do Action and deselect target
                Unit targetUnit = GetUnitFromNode(hitNode);
                int unitMP = selectedUnit.getStats().movementPoints.value;
                int pathLength = currentPath.waypoints.Length;
                if (targetUnit != null && pathLength - 1 <= unitMP)
                {
                    MoveUnitToTargetAndAttack(currentPath, targetUnit, delegate () {
                        DeselectTarget();
                    });
                }
                else
                {
                    MoveUnitToTarget(pathLength > unitMP ? getPartialPath(currentPath, unitMP): currentPath, delegate () {
                        DeselectTarget();
                    });
                }
            }
            else
            {
                SelectTarget(hitNode);
            }
        }
    }
    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public void AddUnit(Unit unit)
    {
        unitsSet.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        if (Grid.instance.NodeFromWorldPosition(unit.transform.position) == selectedTarget)
        {
            DeselectTarget();
        }
        unitsSet.Remove(unit);
    }

    public Unit GetUnitFromNode(Node node)
    {
        foreach (Unit unit in unitsSet)
        {
            if (Grid.instance.NodeFromWorldPosition(unit.transform.position) == node)
            {
                return unit;
            }
        }
        return null;
    }

    void SelectUnit(Unit unit)
    {
        Debug.Log("Unit Selected" + unit);
        if (selectedUnit != null)
        {
            selectedUnit.ToggleHighlightSelection();
        }
        unit.ToggleHighlightSelection();
        selectedUnit = unit;
        onUnitSelection?.Invoke(unit);
        DeselectTarget();
    }

    void DeselectUnit()
    {
        Debug.Log("Unit Deselected");
        selectedUnit.ToggleHighlightSelection();
        onUnitDeselection?.Invoke(selectedUnit);
        selectedUnit = null;
        DeselectTarget();
    }

    void SelectTarget(Node targetNode)
    {
        selectedTarget = targetNode;

        currentPath = PathFinder.instance.FindPath(selectedUnit.transform.position, selectedTarget.worldPosition);
        if (currentPath.successful)
        {
            Path drawPath = currentPath;
            if (GetUnitFromNode(targetNode) != null)
            {
                drawPath = getPartialPath(drawPath, drawPath.waypoints.Count() - 1);
/*                drawPath = new Path(drawPath.waypoints, drawPath.successful);
                drawPath.waypoints = drawPath.waypoints.Take(drawPath.waypoints.Count() - 1).ToArray();*/
            }
            ErasePathCurve();
            (Path head, Path tail) pathTuple = SplitPathByUnitMP(drawPath, selectedUnit);
            DrawPathCurve(pathTuple.head, waypointPrefab);
            DrawPathCurve(pathTuple.tail, waypointNextPrefab, true);
        }
    }

    void DeselectTarget()
    {
        selectedTarget = null;
        ErasePathCurve();
    }

    void DrawPathCurve(Path path, GameObject wpPrefab, bool add = false)
    {
        if (!add)
        {
            pathCurve = new List<GameObject>();
        }
        foreach (Vector3 point in path.waypoints)
        {
            pathCurve.Add(Instantiate(wpPrefab, point, Quaternion.identity));
        }
    }

    void ErasePathCurve()
    {
        if (pathCurve != null)
        {
            foreach (GameObject waypointObj in pathCurve)
            {
                Destroy(waypointObj);
            }
            pathCurve = null;
        }
    }

    IEnumerator ErasePathCurveOneByOne()
    {
        if (pathCurve != null && pathCurve.Count > 0)
        {
            int currentPathPointIdx = 0;
            Node nextNode = Grid.instance.NodeFromWorldPosition(pathCurve[currentPathPointIdx].transform.position);
            while(true)
            {
                Node unitNode = Grid.instance.NodeFromWorldPosition(selectedUnit.transform.position);
                if (unitNode == nextNode)
                {
                    Destroy(pathCurve[currentPathPointIdx]);
                    currentPathPointIdx++;
                    if (currentPathPointIdx >= pathCurve.Count)
                    {
                        yield break;
                    }
                    nextNode = Grid.instance.NodeFromWorldPosition(pathCurve[currentPathPointIdx].transform.position);
                }
                yield return null;
            }
        }

    }

    void MoveUnitToTarget(Path path, Action onMoveEnd = null)
    {
        selectedUnit.Move(path, onMoveEnd);
        StopCoroutine(ErasePathCurveOneByOne());
        StartCoroutine(ErasePathCurveOneByOne());
    }

    void MoveUnitToTargetAndAttack(Path path, Unit targetUnit, Action onMoveEnd = null)
    {
        Vector3 attackDirection;
        if (path.waypoints.Count() == 1) // Unit already stands near the enemy
        {
            onMoveEnd();
            attackDirection = path.waypoints[0] - selectedUnit.transform.position;
            attackManager.PerformAttack(selectedUnit, targetUnit, attackDirection);
        }
        else if (path.waypoints.Count() > 1) // Unit have to firstly go the enemy
        {
            // remove last waypoint and calculate direction of attack
            attackDirection = path.waypoints[path.waypoints.Count() - 1] - path.waypoints[path.waypoints.Count() - 2];
            path.waypoints = path.waypoints.Take(path.waypoints.Count() - 1).ToArray();
            selectedUnit.Move(path, delegate () {
                onMoveEnd();
                attackManager.PerformAttack(selectedUnit, targetUnit, attackDirection);
            });
            StopCoroutine(ErasePathCurveOneByOne());
            StartCoroutine(ErasePathCurveOneByOne());
        }
    }

    public (Path head, Path tail) SplitPathByUnitMP(Path originalPath, Unit unit)
    {
        int MP = unit.getStats().movementPoints.value;
        if (MP > originalPath.waypoints.Length)
        {
            return (head: originalPath, tail: new Path(new Vector3[0], true));
        }
        return (
            head: new Path(originalPath.waypoints.Take(MP).ToArray(), true), 
            tail: new Path(originalPath.waypoints.Skip(MP).ToArray(), true)
            );
    }

    Path getPartialPath(Path path, int n)
    {
        return new Path(path.waypoints.Take(n).ToArray(), path.successful);
    }
}
