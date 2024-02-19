using UnityEngine;

public class LocalLevelPlayerController : MonoBehaviour
{
    private NetworkPlatformManager m_platformController = null;
    private NetworkLevelPlayerController m_networkComponent;

    [SerializeField] private DisplayProjectileCooldown m_cooldownDisplay;
    [SerializeField] private GameObject m_go;
    [SerializeField] private Camera m_camera;
    [SerializeField] private Camera m_cinematicCamera;
    [SerializeField] private Transform m_objectToLookAt;
    [SerializeField] private Vector2 m_verticalLimits;
    [SerializeField] private float m_startDistance = 5.0f;
    [SerializeField] private float m_lerpF = 0.1f;
    [SerializeField] private float m_rotationSpeed = 2.0f;
    [SerializeField] private float m_moveSpeed = 0.1f;
    [SerializeField] private float m_edgeDistance = 50.0f;
    
    private float m_lerpedAngleX;
    private float m_lerpedInputY;

    private bool m_inputPaused = false;
    private bool m_controllingPlatform = false;

    private bool m_isInNonGameplay = true;

    private void Start()
    {
        m_cinematicCamera.enabled = true;
        m_camera.enabled = false;



        var platformController = NetworkPlatformManager._Instance?.GetComponent<NetworkPlatformManager>();
        if (platformController == null)
        {
            Debug.LogError("No platform controller in scene");
            m_controllingPlatform = false;
        }
        else
        {
            m_networkComponent.SetPlatformController(platformController);
            m_controllingPlatform = true;
        }
        

    }

    private void Update()
    {
        if (m_isInNonGameplay)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.P))    //For easier testing    //Feature ?
        {
            m_inputPaused = !m_inputPaused;
        }

        if (m_inputPaused == false)
        {
            MoveHorizontally();
            MoveVertically();
        }

        if (m_controllingPlatform)
        {
            SendInputsToNetworkComponent();
        }

    }

    private void MoveHorizontally()
    {
        float currentAngleX = 0;

        if (Input.mousePosition.x >= Screen.width - m_edgeDistance)
        {
            currentAngleX -= m_rotationSpeed;
        }
        if (Input.mousePosition.x <= 0 + m_edgeDistance)
        {
            currentAngleX += m_rotationSpeed;
        }

        m_lerpedAngleX = Mathf.Lerp(m_lerpedAngleX, currentAngleX, m_lerpF);
        m_go.transform.RotateAround(m_objectToLookAt.position, m_objectToLookAt.up, m_lerpedAngleX);
    }

    private void MoveVertically()
    {
        float inputY = 0;

        if (Input.mousePosition.y >= Screen.height - m_edgeDistance && 
            transform.position.y <= m_verticalLimits.y)
        {
            inputY += m_moveSpeed;
        }
        if (Input.mousePosition.y <= 0 + m_edgeDistance && 
            transform.position.y >= m_verticalLimits.x)
        {
            inputY -= m_moveSpeed;
        }

        m_lerpedInputY = Mathf.Lerp(m_lerpedInputY, inputY, m_lerpF);
        m_go.transform.position += new Vector3(0, m_lerpedInputY, 0);
    }

    private void SendInputsToNetworkComponent()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.red);

        Vector3 localInput = GetLocalInput();

        Debug.DrawRay(transform.position + new Vector3(0, 1, 0), localInput * 5, Color.green);

        //Debug.Log("send inputs from local");        
        m_networkComponent.CMD_SendInputs(localInput);
    }

    private Vector3 GetLocalInput()
    {
        Vector3 localInput = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            localInput += m_camera.transform.forward;
        if (Input.GetKey(KeyCode.S))
            localInput -= m_camera.transform.forward;
        if (Input.GetKey(KeyCode.D))
            localInput += m_camera.transform.right;
        if (Input.GetKey(KeyCode.A))
            localInput -= m_camera.transform.right;

        if (localInput == Vector3.zero)
        {
            //Debug.Log("Local Inputs = Zero");
            return Vector3.zero;
        }

        return localInput;
    }

    public void SetCamera(Camera camera)
    {
        m_camera = camera;
    }

    public void SetNetworkController(NetworkLevelPlayerController controller)
    {
        m_networkComponent = controller;
    }

    public void SetParentGo(GameObject go)
    {
        m_go = go;
    }

    public void SetIsInNonGameplay(bool value)
    {
        if (value == false)
        {
            m_camera.enabled = true;
            m_cinematicCamera.enabled = false;
        }

        m_isInNonGameplay = value;
    }

    public void SetCinematicCamera(GameObject go)
    {
        m_cinematicCamera = go.GetComponent<Camera>();
    }

    public void SetCooldownDisplay(/*DisplayProjectileCooldown DisplayScript,*/ Shooter shooterScript)
    {
        //m_cooldownDisplay = DisplayScript;
        m_cooldownDisplay.SetShooterScript(shooterScript);
    }
}
