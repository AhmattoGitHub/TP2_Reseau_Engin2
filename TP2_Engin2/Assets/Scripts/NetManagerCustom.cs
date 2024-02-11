using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetManagerCustom : NetworkManager
{
    public static NetManagerCustom _Instance { get; private set; }
    [field:SerializeField] public Identifier Identifier { get; private set; }



    public GameObject shooterPrefab;
    public GameObject runnerPrefab;
    public GameObject m_platformPrefab;
    public bool spawnRunner = true;
    public bool testing = false;

    [SerializeField] private GameObject m_spawner;


    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this);
        }
        _Instance = this;
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        if (testing)
        {

            
            
            if (spawnRunner)
            {
                //OnServerAddPlayer(conn, runnerPrefab);

                var player = Instantiate(runnerPrefab);

                player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
                NetworkServer.AddPlayerForConnection(conn, player);

                //Identifier.AssignSingleId(player.transform);
            }
            else
            {
                OnServerAddPlayer(conn, shooterPrefab);
            }
            
            
            //if (conn.identity.isLocalPlayer)
            //{
            //    var spawner = Instantiate(m_spawner);
            //    NetworkServer.Spawn(spawner);
            //
            //    m_identifier = spawner.GetComponent<Identifier>();
            //}
            
            
            return;
        }

        if (SceneManager.GetActiveScene().name != "MainLevel")
        {
            
            return;
        }
        if (conn.identity.isLocalPlayer)
        {
            var go = Instantiate(m_platformPrefab);
            NetworkServer.Spawn(go);
        }



        Debug.Log("trying to change :  " + conn.m_name);
        if (conn.m_tag == "Runner")
        {
            NetworkServer.ReplacePlayerForConnection(conn, Instantiate(runnerPrefab), true);
            conn.m_isInMainLevel = true;

        }
        else
        {
            NetworkServer.ReplacePlayerForConnection(conn, Instantiate(shooterPrefab), true);
            conn.m_isInMainLevel = true;
        }

        int mainLevelCounter = 0;
        foreach (var player in LobbyManager.Instance.GetList())
        {
            if (player.m_isInMainLevel)
            {
                mainLevelCounter++;
            }
        }

        if (mainLevelCounter == LobbyManager.Instance.GetList().Count)
        {
            NetworkServer.Destroy(LobbyManager.Instance.gameObject);
        }
    }

    public override void OnValidate()
    {
        base.OnValidate();
        
        if (shooterPrefab != null && !shooterPrefab.TryGetComponent(out NetworkIdentity _))
        {
            Debug.LogError("NetworkManager - Player Prefab must have a NetworkIdentity.");
            shooterPrefab = null;
        }
        if (runnerPrefab != null && !runnerPrefab.TryGetComponent(out NetworkIdentity _))
        {
            Debug.LogError("NetworkManager - Player Prefab must have a NetworkIdentity.");
            runnerPrefab = null;
        }
        
        if (shooterPrefab != null && spawnPrefabs.Contains(shooterPrefab))
        {
            Debug.LogWarning("NetworkManager - Player Prefab doesn't need to be in Spawnable Prefabs list too. Removing it.");
            spawnPrefabs.Remove(shooterPrefab);
        }
        if (runnerPrefab != null && spawnPrefabs.Contains(runnerPrefab))
        {
            Debug.LogWarning("NetworkManager - Player Prefab doesn't need to be in Spawnable Prefabs list too. Removing it.");
            spawnPrefabs.Remove(runnerPrefab);
        }
    }

    public override void RegisterClientMessages()
    {
        base.RegisterClientMessages();

        if (shooterPrefab != null)
            NetworkClient.RegisterPrefab(shooterPrefab);
        if (runnerPrefab != null)
            NetworkClient.RegisterPrefab(runnerPrefab);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        if (testing)
        {
            return;
        }
        LobbyManager.Instance.WaitForConfig();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }


    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("OnserverAddPlayer");
        base.OnServerAddPlayer(conn);
        conn.identity.name = "";
        LobbyManager.Instance.AddToConnections(conn);
    }
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (!testing)
        {
            LobbyManager.Instance.RemoveFromConnections(conn);
        }
        base.OnServerDisconnect(conn);
    }

}
