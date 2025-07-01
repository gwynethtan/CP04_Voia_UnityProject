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
    private Animator _animator;

    public float runningThreshold = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        if (_navMeshAgent == null)
            Debug.LogError("NavMeshAgent component not attached.");
        if (_animator == null)
            Debug.LogError("Animator component not attached.");
    }

    void Update()
    {
        if (_navMeshAgent != null && _animator != null)
        {
            // Check if the agent has reached the destination
            if (!_navMeshAgent.pathPending &&
                _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance &&
                (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f))
            {
                _animator.SetBool("isRunning", false); // Stop running animation
            }
        }
    }



    public void MoveToDestination()
    {
        if (_destination != null && _navMeshAgent != null)
        {
            Vector3 targetVector = _destination.position; //Gets the position of _destination
            _navMeshAgent.SetDestination(targetVector); //Tells NavMeshAgent to start walking toward that position.
            if (_animator != null)
            {
                _animator.SetBool("isRunning", true);
             
            }
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