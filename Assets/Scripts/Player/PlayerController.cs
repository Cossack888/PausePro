using System.Diagnostics.Contracts;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private IMovement currentMovement;
    private Rigidbody rb;
    private PlayerAction action;
    public delegate void Landing();
    public event Landing OnLand;
    [Header("Camera Settings")]
    [SerializeField] private float mouseSentitvityX;
    [SerializeField] private float mouseSentitvityY;
    [SerializeField] private float smoothingFactor = 0.2f;
    [SerializeField] private Camera normalCam;
    [SerializeField] private Camera ghostCam;
    private float prevMouseX;
    private float prevMouseY;
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
    [Header("Combat Settings")]
    [SerializeField] Animator leftHand;
    [SerializeField] Animator rightHand;
    private RegularMovement regularMovement;
    private Jumping jumping;
    private Somersault somersault;
    private WallRun wallRun;
    private Dash dash;
    private Crouching crouching;
    private Attacking attacking;
    private GhostForm ghostForm;
    private float cameraPitch;
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
    public float MouseSensitivityX => mouseSentitvityX;
    public float MouseSensitivityY => mouseSentitvityY;
    public Mesh Sphere => sphere;
    public Mesh Capsule => capsule;
    public Transform GroundCheck => groundCheck;
    public RegularMovement RegularMovement => regularMovement;
    public Jumping Jumping => jumping;
    public Somersault Somersault => somersault;
    public WallRun WallRun => wallRun;
    public Dash Dash => dash;
    public Crouching Crouching => crouching;
    public Attacking Attacking => attacking;
    public Transform Cam => cam;
    public Camera NormalCam => normalCam;
    public Camera GhostCam => ghostCam;
    public GhostForm GhostForm => ghostForm;
    public Animator LeftHand => leftHand;
    public Animator RightHand => rightHand;
    public LayerMask GhostInteractionLayer => ghostInteractionLayer;
    public float GhostInteractionDistance => ghostInteractionDistance;
    public IMovement CurrentMovement => currentMovement;
    void Start()
    {
        SwitchCamera(NormalCam);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        action = GetComponent<PlayerAction>();
        regularMovement = new RegularMovement(rb, transform, this, action);
        jumping = new Jumping(rb, transform, this, action);
        somersault = new Somersault(rb, transform, this, action);
        wallRun = new WallRun(rb, transform, this, action);
        dash = new Dash(rb, transform, this, action);
        crouching = new Crouching(rb, transform, this, action);
        attacking = new Attacking(rb, transform, this, action);
        ghostForm = new GhostForm(rb, transform, this, action);
        SetMovement(regularMovement);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            OnLand?.Invoke();
        }
    }
    void Update()
    {
        currentMovement?.UpdateMovement();
    }
    private void FixedUpdate()
    {
        currentMovement?.FixedUpdateMovement();
    }
    private void LateUpdate()
    {
        UpdateCamera();
    }
    public void SwitchCamera(Camera cam)
    {
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
        transform.Rotate(Vector3.up * smoothMouseX * MouseSensitivityX);
        cameraPitch -= smoothMouseY * MouseSensitivityY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        cam.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        prevMouseX = smoothMouseX;
        prevMouseY = smoothMouseY;
    }

}
