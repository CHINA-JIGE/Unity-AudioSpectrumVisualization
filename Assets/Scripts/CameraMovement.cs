using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float m_Radius = 75.0f;
    public float m_Height = 60.0f;
    public float m_LookatHeight = 15.0f;
    public float m_Speed = 0.2f;
    private float m_Timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_Timer += Time.deltaTime * m_Speed;
        Camera cam = GetComponent<Camera>();
        cam.transform.position = new Vector3(m_Radius * Mathf.Sin(m_Timer), m_Height, m_Radius * Mathf.Cos(m_Timer));
        cam.transform.LookAt(new Vector3(0, m_LookatHeight, 0));

    }
}
