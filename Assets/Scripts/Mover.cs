using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Mover : MonoBehaviour
{
    private Transform m_TargetPos;
    private Ray m_Ray;
    private NavMeshAgent m_Agent;

    // Start is called before the first frame update
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            MoveTo();
        }
    }

    private void MoveTo()
    {
        m_Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(m_Ray, out RaycastHit hit))
        {
            m_Agent.destination = hit.point;
        }
    }
}
