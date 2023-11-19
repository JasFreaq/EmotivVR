using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_playerSpeed = 10f;

    private Rigidbody m_rigidbody;

    private Queue<float> m_inputQueue = new Queue<float>();
    
    private float m_lastPlayerInput;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //float input = 0f;

        //if (m_inputQueue.Count > 0)
        //{
        //    float currentPlayerInput = m_inputQueue.Dequeue();

        //    input = Mathf.Lerp(m_lastPlayerInput, currentPlayerInput, Time.deltaTime);
        //}

        //transform.Translate(Vector3.forward * m_playerSpeed * input);
        
        if (m_inputQueue.Count > 0)
        {
            float input = m_inputQueue.Dequeue();
            
            m_rigidbody.velocity = Vector3.forward * m_playerSpeed * input * Time.deltaTime;
        }
        else
        {
            m_rigidbody.velocity = Vector3.forward * Mathf.Lerp(m_rigidbody.velocity.magnitude, 0f, Time.deltaTime);
        }
    }

    public void EnqueueInput(float input)
    {
        m_inputQueue.Enqueue(input);
    }
}
