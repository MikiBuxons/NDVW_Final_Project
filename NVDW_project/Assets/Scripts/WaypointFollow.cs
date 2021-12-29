using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class WaypointFollow : MonoBehaviour
{
  [SerializeField] private GameObject[] waypoints;
    private int currentWaypointIndex = 0;
    public bool hitPlayer = false;
    [SerializeField] private float speed = 2f;

    private void FixedUpdate()
    {
        if (Vector2.Distance(waypoints[currentWaypointIndex].transform.position, transform.position) < .1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
        }

        if (!hitPlayer)
        {
            transform.position = Vector2.MoveTowards(transform.position,
                waypoints[currentWaypointIndex].transform.position,
                Time.deltaTime * speed);
            
        }
        hitPlayer = false;

    }
}
