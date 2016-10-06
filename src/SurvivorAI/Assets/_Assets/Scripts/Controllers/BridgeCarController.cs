using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BridgeCarController : MonoBehaviour
{
    private TextMesh m_text;
    private Rigidbody m_rb;
    private float m_lastX = int.MinValue;
    public float Speed = 0.1f;
    public float MaxZRotation = .5f;

    void Start()
    {
        m_text = GetComponentInChildren<TextMesh>();
        m_rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        var rz = transform.rotation.z;
        var x = transform.position.x;

        if (m_rb.useGravity && rz >= (-1 * MaxZRotation) && rz <= MaxZRotation && m_lastX != x)
        {
            transform.position += new Vector3(Speed, 0, 0);
        }
        else
        {
            m_rb.useGravity = false;
            m_rb.constraints = RigidbodyConstraints.FreezeAll;
            m_rb.Sleep();
        }

        m_lastX = x;
        m_text.text = m_lastX.ToString("N0");

    }

    public void InvertSpeed()
    {
        Speed *= -1;
    }
}
