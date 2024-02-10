using Mirror;
using UnityEngine;

public class NetworkSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject m_platformPrefab = null;    
    [SerializeField] private GameObject m_verticalMapBoundsTrigger = null;
    [SerializeField] private GameObject m_winBoundTrigger = null;    

    [SerializeField] private GameObject m_matchManagerPrefab;
    private NetworkMatchManager m_matchManagerInstance;

    public override void OnStartServer()
    {
        GameObject platformInstance = Instantiate(m_platformPrefab);
        NetworkServer.Spawn(platformInstance);

        if(isServer)
        {
            if (m_matchManagerInstance == null)
            {
                GameObject matchManagerObj = Instantiate(m_matchManagerPrefab);
                m_matchManagerInstance = matchManagerObj.GetComponent<NetworkMatchManager>();
                NetworkServer.Spawn(matchManagerObj);
            }

        }

        GameObject verticalBoundTrigger = Instantiate(m_verticalMapBoundsTrigger);
        var boundsDetection = verticalBoundTrigger.GetComponent<BoundsDetection>();
        if (boundsDetection != null) 
        {
            boundsDetection.TriggerType = E_TriggerTypes.OutOfVerticalMapBounds;
        }
        NetworkServer.Spawn(verticalBoundTrigger);        
    }

}
