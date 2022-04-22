using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class MoveAgent : MonoBehaviour
{
    public List<Transform> wayPoints;
    public int nextIdx = 0;
    private NavMeshAgent agent;

    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 4.0f;

    private Transform enemyTr;

    private float damping = 1.0f;

    private bool _patrolling;

    public float speed
    {
        get { return agent.velocity.magnitude; }
    }

    public bool patrolling
    {
        get
        {
            return _patrolling;
        }
        set
        {
            _patrolling = value;
            if (_patrolling)
            {
                agent.speed = patrolSpeed;
                damping = 1.0f;
                MoveWayPoint();
            }
        }
    }

    private Vector3 _traceTarget;

    public Vector3 traceTarget
    {
        get 
        { 
            return _traceTarget; 
        }
        set 
        {
            _traceTarget = value;
            agent.speed = traceSpeed;
            damping = 7.0f;
            TraceTarget(_traceTarget);
        }
    }

    private void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) return;

        agent.destination = pos;
        agent.isStopped = false;
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }
    

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyTr = GetComponent<Transform>();
        agent.autoBraking = false;
        agent.speed = patrolSpeed;
        agent.updateRotation = false;
        var group = GameObject.Find("WayPointGroup");
        if(group!=null)
        {
            group.GetComponentsInChildren<Transform>(wayPoints);
            wayPoints.RemoveAt(0);

            nextIdx = UnityEngine.Random.Range(0, wayPoints.Count);
        }

        MoveWayPoint();
    }

    private void MoveWayPoint()
    {
        if(agent.isPathStale)
        {
            return;
        }

        agent.destination = wayPoints[nextIdx].position;
        agent.isStopped = false;
    }


    // Update is called once per frame
    void Update()
    {
        if(agent.isStopped==false)
        {
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }




        if (!_patrolling) return;

        if(agent.velocity.sqrMagnitude>=0.2f*0.2f && agent.remainingDistance<0.5f)
        {
            //nextIdx = ++nextIdx % wayPoints.Count;
            nextIdx = UnityEngine.Random.Range(0, wayPoints.Count);
            MoveWayPoint();
        }


    }



}
