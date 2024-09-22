using System.Diagnostics.Contracts;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private IMovement currentMovement;
    private Rigidbody rb;
    private PlayerAction action;
    private GameObject playerBody;
    public delegate void Landing();
    public event Landing OnLand;
    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivityX;
    [SerializeField] private float mouseSensitivityY;
    [SerializeField] private float smoothingFactor = 0.2f;
    [SerializeField] private Camera normalCam;
    [SerializeField] private Camera ghostCam;
    private float prevMouseX;
    private float prevMouseY;
    private bool cameraLocked = true;
    private float unlockTime = 1f;
    private float timer = 0f;
    [Header("Player Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float somersaultDuration;
    [SerializeField] private float wallRunDuration;
    [SerializeField] private float airControlFactor;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float dashForce;
    [SerializeField] private float rollSpeed;
    [Header("Ground Settings")]
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    public float currentY;
    [Header("WallRun Settings")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float wallDistance;
    [Header("Crouch Settings")]
    [SerializeField] private Mesh sphere;
    [SerializeField] private Mesh capsule;
    [Header("Ghost Settings")]
    [SerializeField] private float ghostInteractionDistance;
    [SerializeField] private LayerMask ghostInteractionLayer;
    [SerializeField] GameObject playerPrefab;
    [Header("Combat Settings")]
    [SerializeField] Animator leftHand;
    [SerializeField] Animator rightHand;
    [SerializeField] Animator ghostLeftHand;
    [SerializeField] Animator ghostRightHand;
    [SerializeField] Slider sensitivitySliderX;
    [SerializeField] Slider sensitivitySliderY;
    [SerializeField] float defaultSensitivityX = 2f;
    [SerializeField] float defaultSensitivityY = 2f;
    private RegularMovement regularMovement;
    private Jumping jumping;
    private WallRun wallRun;
    private Crouching crouching;
    private CrouchJump crouchJump;
    private GhostForm ghostForm;
    private float cameraPitch;
    public bool smoothingEnabled = true;
    private Transform cam;
    public float GroundDistance => groundDistance;
    public float WallDistance => wallDistance;
    public LayerMask GroundMask => groundMask;
    public LayerMask WallMask => wallMask;
    public float RunSpeed => runSpeed;
    public float WalkSpeed => walkSpeed;
    public float JumpForce => jumpForce;
    public float RotationSpeed => rotationSpeed;
    public float SomersaultDuration => somersaultDuration;
    public float WallRunDuration => wallRunDuration;
    public float AirControlFactor => airControlFactor;
    public float JumpHeight => jumpHeight;
    public float DashForce => dashForce;
    public float RollSpeed => rollSpeed;
    public float MouseSensitivityX => mouseSensitivityX;
    public float MouseSensitivityY => mouseSensitivityY;
    public Mesh Sphere => sphere;
    public Mesh Capsule => capsule;
    public Transform GroundCheck => groundCheck;
    public RegularMovement RegularMovement => regularMovement;
    public Jumping Jumping => jumping;
    public WallRun WallRun => wallRun;
    public Crouching Crouching => crouching;
    public CrouchJump CrouchJump => crouchJump;

    public Transform Cam => cam;
    public Camera NormalCam => normalCam;
    public Camera GhostCam => ghostCam;
    public GhostForm GhostForm => ghostForm;
    public Animator LeftHand => leftHand;
    public Animator RightHand => rightHand;
    public Animator GhostRightHand => ghostRightHand;
    public Animator GhostLeftHand => ghostLeftHand;
    public LayerMask GhostInteractionLayer => ghostInteractionLayer;
    public float GhostInteractionDistance => ghostInteractionDistance;
    public GameObject PlayerPrefab => playerPrefab;
    public IMovement CurrentMovement => currentMovement;
    void Start()
    {
        SwitchCamera(NormalCam);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraLocked = true;
        cameraPitch = 0f;
        cam.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        rb = GetComponent<Rigidbody>();
        action = GetComponent<PlayerAction>();
        regularMovement = new RegularMovement(rb, transform, this, action);
        jumping = new Jumping(rb, transform, this, action);
        wallRun = new WallRun(rb, transform, this, action);
        crouching = new Crouching(rb, transform, this, action);
        crouchJump = new CrouchJump(rb, transform, this, action);
        ghostForm = new GhostForm(rb, transform, this, action);
        SetMovement(regularMovement);
        SetMouseSensitivity();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            OnLand?.Invoke();
        }
    }
    private void SetMouseSensitivity()
    {
        sensitivitySliderX.minValue = 0.1f;
        sensitivitySliderX.maxValue = 10f;
        sensitivitySliderY.minValue = 0.1f;
        sensitivitySliderY.maxValue = 10f;
        sensitivitySliderX.value = defaultSensitivityX;
        sensitivitySliderY.value = defaultSensitivityY;
        mouseSensitivityX = defaultSensitivityX;
        mouseSensitivityY = defaultSensitivityY;
        sensitivitySliderX.onValueChanged.AddListener(OnSensitivityXChanged);
        sensitivitySliderY.onValueChanged.AddListener(OnSensitivityYChanged);
    }
    public void OnSensitivityXChanged(float newValue)
    {
        mouseSensitivityX = newValue;
    }
    public void OnSensitivityYChanged(float newValue)
    {
        mouseSensitivityY = newValue;
    }
    public void CreatePlayerBody(Vector3 pos)
    {
        playerBody = Instantiate(playerPrefab, pos, Quaternion.identity);
    }
    public void DestroyPlayerBody()
    {
        Destroy(playerBody);
    }
    public void SmoothingOnOff()
    {
        smoothingEnabled = !smoothingEnabled;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (ghostLeftHand.isActiveAndEnabled)
            {
                ghostLeftHand.SetTrigger("spell");
            }
            else if (leftHand.isActiveAndEnabled)
            {
                leftHand.SetTrigger("spell");
            }
        }
        if (Input.GetMouseButtonDown(2))
        {
            if (ghostLeftHand.isActiveAndEnabled)
            {
                ghostLeftHand.SetTrigger("push");
            }
            else if (leftHand.isActiveAndEnabled)
            {
                leftHand.SetTrigger("push");
            }
        }
        currentMovement?.UpdateMovement();
    }
    private void FixedUpdate()
    {
        currentMovement?.FixedUpdateMovement();
        if (cameraLocked)
        {
            timer += Time.deltaTime;
            if (timer >= unlockTime)
            {
                cameraLocked = false;
            }
            return;
        }
        UpdateCamera();
    }
    private void LateUpdate()
    {

    }
    public void SwitchCamera(Camera cam)
    {
        foreach (Camera camera in GameObject.FindObjectsOfType<Camera>())
        {
            camera.GetComponent<AudioListener>().enabled = false;
        }

        if (cam.GetComponent<AudioListener>().enabled == false)
        {
            cam.GetComponent<AudioListener>().enabled |= true;
        }
        this.cam = cam.transform;
    }

    public void SetMovement(IMovement newMovement)
    {
        currentMovement?.ExitMovement();
        currentMovement = newMovement;
        currentMovement?.EnterMovement();
        Debug.Log(currentMovement.ToString());
    }
    private void UpdateCamera()
    {
        float smoothMouseX = Mathf.Lerp(prevMouseX, action.MouseX, smoothingFactor);
        float smoothMouseY = Mathf.Lerp(prevMouseY, action.MouseY, smoothingFactor);

        if (!smoothingEnabled)
        {
            smoothMouseX = action.MouseX;
            smoothMouseY = action.MouseY;
        }
        Vector3 currentEuler = transform.eulerAngles;
        currentEuler.y += smoothMouseX * MouseSensitivityX;
        transform.eulerAngles = currentEuler;
        cameraPitch -= smoothMouseY * MouseSensitivityY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        cam.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);

        prevMouseX = smoothMouseX;
        prevMouseY = smoothMouseY;
    }


}
