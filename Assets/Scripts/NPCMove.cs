using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMove : MonoBehaviour
{
    [SerializeField]
    private Transform _destination;
    public Transform destination2;

    private NavMeshAgent _navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        if (_navMeshAgent == null)
        {
            Debug.Log("NavMeshAgent component not attached.");
        }
    }



    public void MoveToDestination()
    {
        if (_destination != null && _navMeshAgent != null)
        {
            Vector3 targetVector = _destination.position; //Gets the position of _destination
            _navMeshAgent.SetDestination(targetVector); //Tells NavMeshAgent to start walking toward that position.
        }
        else
        {
            Debug.LogWarning("Destination or NavMeshAgent missing.");
        }
    }

    public void MoveToDestination2()
    {
        SetNewDestination(destination2);
    }

    public void SetNewDestination(Transform runningAway)
    {
        _destination = runningAway;
        MoveToDestination();
    }
}