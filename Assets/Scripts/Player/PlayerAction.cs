using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    private PlayerBindings bindings;
    public delegate void PlayerActionPerformed();
    public event PlayerActionPerformed OnJumpGlobal;
    public event PlayerActionPerformed OnParkourGlobal;
    public event PlayerActionPerformed OnDashGlobal;
    public event PlayerActionPerformed OnExitGlobal;
    public event PlayerActionPerformed OnGhostGlobal;
    public event PlayerActionPerformed OnShootGlobal;
    public event PlayerActionPerformed OnAttackGlobal;
    public event PlayerActionPerformed OnInteractGlobal;
    private Vector2 movementVector;
    private float mouseX;
    private float mouseY;
    private float scrollAmount;
    private bool isSprinting;
    private bool isCrouching;
    public Vector2 Movement => movementVector;
    public float ScrollAmount => scrollAmount;
    public bool IsSprinting => isSprinting;
    public bool IsCrouching => isCrouching;
    public float MouseX => mouseX;
    public float MouseY => mouseY;
    private void Awake()
    {
        bindings = new PlayerBindings();
        bindings.Player.Enable();
        bindings.Player.Jump.performed += OnJump;
        bindings.Player.Parkour.performed += OnParkour;
        bindings.Player.Scroll.performed += OnScroll;
        bindings.Player.Sprint.performed += OnSprint;
        bindings.Player.Sprint.canceled += OnSprint;
        bindings.Player.Dash.performed += OnDash;
        bindings.Player.Crouch.performed += OnCrouch;
        bindings.Player.Crouch.canceled += OnCrouch;
        bindings.Player.Exit.performed += OnExit;
        bindings.Player.GhostForm.performed += OnGhost;
        bindings.Player.Shoot.performed += OnShoot;
        bindings.Player.Attack.performed += OnAttack;
        bindings.Player.Interact.performed += OnInteract;
    }
    public void OnDisable()
    {
        bindings.Player.Jump.performed -= OnJump;
        bindings.Player.Parkour.performed -= OnParkour;
        bindings.Player.Scroll.performed -= OnScroll;
        bindings.Player.Sprint.performed -= OnSprint;
        bindings.Player.Sprint.canceled -= OnSprint;
        bindings.Player.Dash.performed -= OnDash;
        bindings.Player.Crouch.performed -= OnCrouch;
        bindings.Player.Crouch.canceled -= OnCrouch;
        bindings.Player.Exit.performed -= OnExit;
        bindings.Player.GhostForm.performed -= OnGhost;
        bindings.Player.Shoot.performed -= OnShoot;
        bindings.Player.Attack.performed -= OnAttack;
        bindings.Player.Interact.performed -= OnInteract;
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) { OnJumpGlobal?.Invoke(); }
    }
    public void OnParkour(InputAction.CallbackContext context)
    {
        if (context.performed) { OnParkourGlobal?.Invoke(); }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed) { OnDashGlobal?.Invoke(); }
    }
    public void OnExit(InputAction.CallbackContext context)
    {
        if (context.performed) { OnExitGlobal?.Invoke(); }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed) { isCrouching = true; }
        else
        {
            isCrouching = false;
        }
    }
    public void OnGhost(InputAction.CallbackContext context)
    {
        if (context.performed) { OnGhostGlobal?.Invoke(); }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed) { OnAttackGlobal?.Invoke(); }
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) { OnInteractGlobal?.Invoke(); }
    }

    private void OnScroll(InputAction.CallbackContext context)
    {
        scrollAmount += Mathf.Clamp(context.ReadValue<float>(), -1, 1);
    }
    private void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed) { OnShootGlobal?.Invoke(); }
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (movementVector != Vector2.zero)
            {
                isSprinting = true;
            }
        }
        else if (context.canceled)
        {
            isSprinting = false;
        }
    }
    public void FixedUpdate()
    {
        movementVector = bindings.Player.Movement.ReadValue<Vector2>();
    }
    private void Update()
    {

        if (movementVector != Vector2.zero && bindings.Player.Sprint.ReadValue<float>() > 0)
        {
            isSprinting = true;
        }
        else if (movementVector == Vector2.zero || bindings.Player.Sprint.ReadValue<float>() == 0)
        {
            isSprinting = false;
        }
    }
    private void LateUpdate()
    {
        mouseX = bindings.Player.MouseX.ReadValue<float>();
        mouseY = bindings.Player.MouseY.ReadValue<float>();
    }

}
