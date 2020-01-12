using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    public float speed = .1f;
    public float rotationSpeed = 1f;
    public Sprite portrait;

    Vector3[] pathWaypoints;
    int currentWaypointIndes;
    Animator animator;
    Projector projector;
    UnitStats stats;

    public delegate void OnStatsChange(UnitStats stats);
    public OnStatsChange onStatChange;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        projector = GetComponentInChildren<Projector>();
        stats = GetComponent<UnitStats>();

        // setting grid position of unit and placing unit to grid center
        transform.position = Grid.instance.NodeFromWorldPosition(transform.position).worldPosition;
        // registring unit in manager
        UnitManager.instance.AddUnit(this);
    }

    public void Move(Path path, Action onEndCallback = null)
    {
        if (path.successful)
        {
            pathWaypoints = path.waypoints;
            StopCoroutine(FollowPath(onEndCallback));
            StartCoroutine(FollowPath(onEndCallback));
        }
    }

    public void Attack(Vector3 direction, Action callback = null)
    {
        transform.rotation = Quaternion.LookRotation(direction);
        animator.SetTrigger("isAttacking");
    }

    public void BeAttacked(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(-direction);
    }

    IEnumerator FollowPath(Action onEndCallback = null)
    {
        Vector3 currentWaypoint = pathWaypoints[0];
        Vector3 lookDirection = currentWaypoint - transform.position;

        currentWaypointIndes = 0;

        while(true)
        {
            if (transform.position == currentWaypoint)
            {
                currentWaypointIndes++;
                if (currentWaypointIndes >= pathWaypoints.Length)
                {
                    animator.SetFloat("speedPercent", 0);
                    onEndCallback?.Invoke();
                    yield break;
                }
                currentWaypoint = pathWaypoints[currentWaypointIndes];
                lookDirection = currentWaypoint - transform.position;
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            Vector3 newLookDirection  = Vector3.RotateTowards(transform.forward, lookDirection, rotationSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newLookDirection);
            animator.SetFloat("speedPercent", 1);
            yield return null;
        }
    }

    public void ToggleHighlightSelection()
    {
        projector.enabled = !projector.enabled;
    }

    public UnitStats getStats()
    {
        return stats;
    }

    public void takeDamage(int damage)
    {
        damage = Mathf.Min(stats.helth.value, damage);
/*
        stats.helth.SetValue*/
    }

}
