/*
 * Author: Ong Xi Yi Verlaine
 * Date: 23/6/2025
 * Description: Handles the wolf animations in the book
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMove : MonoBehaviour
{
    /// <summary>
    /// The primary target destination Transform that the NPC will move to.
    /// </summary>
    [SerializeField]
    private Transform _destination;

    /// <summary>
    /// A secondary destination Transform for the NPC (e.g., running away).
    /// </summary>
    public Transform destination2;

    /// <summary>
    /// The NavMeshAgent component used to control NPC navigation.
    /// </summary>
    private NavMeshAgent _navMeshAgent;

    /// <summary>
    /// The Animator component used to control NPC animations.
    /// </summary>
    private Animator _animator;

    /// <summary>
    /// Threshold used to determine when the NPC should be considered "running."
    /// </summary>
    public float runningThreshold = 0.1f;

    /// <summary>
    /// Called once at the start. Initializes NavMeshAgent and Animator references.
    /// </summary>
    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        if (_navMeshAgent == null)
            Debug.LogError("NavMeshAgent component not attached.");
        if (_animator == null)
            Debug.LogError("Animator component not attached.");
    }

    /// <summary>
    /// Called every frame. Checks if the NPC has reached the destination and stops running if so.
    /// </summary>
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


    /// <summary>
    /// Moves the NPC towards the currently set destination
    /// </summary>
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

    /// <summary>
    /// Moves the NPC to the secondary destination (destination2).
    /// </summary>
    public void MoveToDestination2()
    {
        SetNewDestination(destination2);
        
    }

    /// <summary>
    /// Sets a new destination for the NPC and makes it move there.
    /// </summary>
    /// <param name="runningAway">The new target Transform.</param>
    public void SetNewDestination(Transform runningAway)
    {
        _destination = runningAway;
        MoveToDestination();
    }
}