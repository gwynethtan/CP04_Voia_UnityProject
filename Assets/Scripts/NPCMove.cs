using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class NPCMove : MonoBehaviour
{
    [SerializeField]
    private Transform _destination;

    private NavMeshAgent _navMeshAgent;  

    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        if (_navMeshAgent == null)
        {
            Debug.Log("NavMeshAgent component not attached.");
        }
        else
        {
            SetDestination();
        }
    }

    public void SetDestination() 
    {
        if (_destination != null)
        {
            Vector3 targetVector = _destination.position; //Gets the position of _destination
            _navMeshAgent.SetDestination(targetVector); //Tells NavMeshAgent to start walking toward that position.
        }
    }
}
