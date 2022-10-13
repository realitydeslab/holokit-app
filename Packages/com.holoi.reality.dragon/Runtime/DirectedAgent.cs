using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Holoi.Reality.Dragon
{
    public class DirectedAgent : MonoBehaviour
    {
        [SerializeField] Transform _target;

        private NavMeshAgent agent;

        // Use this for initialization
        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            //MoveToLocation();
        }

        private void Update()
        {
            MoveToLocation();

            //if(Vector3.Distance(transform.position, _target.position) < 0.1f)
            //{
            //    agent.isStopped = true;
            //}
            //else
            //{

            //}

            //if (agent.isStopped)
            //{
            //    if (Vector3.Distance(transform.position, _target.position) > 0.1f)
            //    {
            //        MoveToLocation();
            //    }
            //}
        }

        public void MoveToLocation()
        {
            agent.destination = _target.position;
            agent.isStopped = false;
        }
    }
}
