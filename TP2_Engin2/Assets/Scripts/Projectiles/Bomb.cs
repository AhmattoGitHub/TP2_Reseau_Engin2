using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


/*
 
Could be fun if bombs go faster and in straight line
but stick to whatever they hit

Potentially:
    OnCollisionEnter
        velocity = 0
        become child object of what you hit
???
 
 */




public class Bomb : NetworkBehaviour
{
    [SerializeField] private Rigidbody m_rb;
    [SerializeField] private float m_projectileSpeed = 10;
    [SerializeField] private float m_explosionForce = 10;
    [SerializeField] private float m_explosionRadius = 10;
    [SerializeField] private float m_height = 20;
    [SerializeField] private float m_explosionTimer = 5;

    private float m_timer = 0;
    //private const float EXPLOSION_TIMER = 5;


    void Start()
    {
        m_timer = m_explosionTimer;

    }

    void Update()
    {
        if (!isServer)
        {
            return;
        }

        HandleTimer();
    }

    [Server]
    private void HandleTimer()
    {
        if (m_timer < 0)
        {
            //Debug.Log("explode");
            CMD_Explode();
            return;
        }
        m_timer -= Time.deltaTime;
    }

    [Command(requiresAuthority = false)]
    private void CMD_Explode()
    {
        var surroundingObjects = Physics.OverlapSphere(transform.position, m_explosionRadius);

        foreach (var obj in surroundingObjects)
        {
            // Needs to affect only characterPlayers
            // Basic implementation would be to check tag
            // if (obj.gameObject.tag != "CharacterPlayer") continue;

            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null || rb == m_rb)
            {
                continue;
            }


            rb.AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius, m_height, ForceMode.Impulse);
        }

        NetworkServer.Destroy(gameObject);
    }

    [Command(requiresAuthority = false)]
    public void CMD_Shoot(Vector3 direction)
    {
        m_rb.AddForce(direction * m_projectileSpeed, ForceMode.Impulse);
    }

}