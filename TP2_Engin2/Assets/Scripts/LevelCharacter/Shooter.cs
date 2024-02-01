using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum EProjectileType
{
    Bullet,
    Bomb
};

public class Shooter : NetworkBehaviour
{
    [SerializeField] private GameObject m_bulletPrefab;
    [SerializeField] private GameObject m_bombPrefab;
    [SerializeField] private Camera m_camera;

    private EProjectileType m_currentProjectile;


    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = m_camera.nearClipPlane - 5;
            Vector3 screenPosition = m_camera.ScreenToWorldPoint(mousePosition);
            Vector3 direction = (transform.position - screenPosition).normalized;

            switch (m_currentProjectile)
            {
                case EProjectileType.Bullet:
                    CMD_ShootBullet(direction);
                    break;
                case EProjectileType.Bomb:
                    CMD_ShootBomb(direction);
                    break;
                default:
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_currentProjectile = EProjectileType.Bullet;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_currentProjectile = EProjectileType.Bomb;
        }
    }

    [Command(requiresAuthority = false)]
    public void CMD_ShootBullet(Vector3 direction)
    {
        var bullet = Instantiate(m_bulletPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(bullet);

        bullet.GetComponent<Bullet>().CMD_Shoot(direction);
    }

    [Command(requiresAuthority = false)]
    public void CMD_ShootBomb(Vector3 direction)
    {
        var bomb = Instantiate(m_bombPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(bomb);

        bomb.GetComponent<Bomb>().CMD_Shoot(direction);
    }

}
