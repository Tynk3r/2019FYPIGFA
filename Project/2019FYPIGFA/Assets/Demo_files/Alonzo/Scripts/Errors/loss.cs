using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loss : MonoBehaviour
{
    private Renderer m_renderer;
    // Start is called before the first frame update
    void Start()
    {
        m_renderer = GetComponent<Renderer>();
        if (null != m_renderer)
            Debug.Log("Gotem");
    }

    // Update is called once per frame
    void Update()
    {
        if (m_renderer.material.color.a != 0)
        {
            Color newColor = m_renderer.material.color;
            newColor.a = Mathf.Max(0f, newColor.a - Time.deltaTime);
            m_renderer.material.color = newColor;
            Debug.Log(m_renderer.material.color.a);
        }
    }
}
