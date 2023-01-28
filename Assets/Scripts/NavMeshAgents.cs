using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NavMeshAgents : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private PlayerController playerController;
    [SerializeField] private Transform destination;
    private Vector3 lastPos;
    private void Start() {
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        playerController = GetComponentInParent<PlayerController>();
        destination = transform.parent;
    }
    private void Update() {
        navMeshAgent.SetDestination(destination.localPosition);
    }
}
