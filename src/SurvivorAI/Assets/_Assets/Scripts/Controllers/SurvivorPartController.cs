using UnityEngine;
using System.Collections;

public class SurvivorPartController : MonoBehaviour
{
    private Rigidbody m_rb;

    void Start() {
        m_rb = GetComponentInChildren<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            var survivor = GetSurvivor();
            ChallengeControllerBase.Current.RegisterSurvivorReachFloor(survivor.name);
        }
        else if (collision.collider.CompareTag("DeadZone"))
        {
            var survivor = GetSurvivor();
            Debug.LogFormat("Survivor '{0}' fall in dead zone.", survivor.name);
            ChallengeControllerBase.Current.RegisterSurvivorFallInDeadZone(survivor.name);
        }
    }

    private void Update()
    {
        if (!m_rb.useGravity || m_rb.IsSleeping())
        {
            var survivor = GetSurvivor();
            ChallengeControllerBase.Current.RegisterSurvivorStop(survivor.name);	
        }
    }

    private Transform GetSurvivor()
    {
        return transform.parent ?? transform;
    }
}
