using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Mover : MonoBehaviour
{
    private Transform m_TargetPos;
    private NavMeshAgent m_Agent;

    // Start is called before the first frame update
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) 
        {
            MoveTo();
        }

        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        float speed = localVelocity.z;

        GetComponent<Animator>().SetFloat("forwardSpeed", speed);
    }

    private void MoveTo()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            m_Agent.destination = hit.point;
        }
    }
}
