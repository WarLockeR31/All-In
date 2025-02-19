using System.Collections;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{

    private UIManager manager;


    [Header("Movement")]
    public float lookSpeed = 2f;
    public float jumpForce = 2f;
    public float doubleJumpForce = 2f;
    public float coyoteTime = 0.2f;
    public float lateralFriction;

    [Header("Dash")]
    public bool canDash = false;
    // Рывок
    public float dashDistance = 10f; // Дальность рывка
    public float dashDuration = 0.5f; // Длительность рывка

    [Header("WallJump")]
    public float wallJumpForce = 7f;
    public float wallCheckDistance = 1f;
    private bool isWallSliding;
    private Vector3 lastWallNormal;

    //Private
    [HideInInspector]
    private bool isDashing = false;
    private CharacterController controller;
    private Vector3 velocity;
    private float verticalVelocity;
    private Vector3 inputDirection;
    private bool isGrounded;
    private bool canDoubleJump;
    private float coyoteTimeCounter;
    private bool activeGrapple;

    private float rotationX = 0f;


    [Header("Walking")]
    public float walkingFriction;
    public float walkingBrakingFriction;
    public float maxWalkSpeed;
    public float acceleration;

    [Header("Falling")]
    public float gravityFalling = 9.81f;
    public float gravityJumping = 4.905f;
    public float fallingFriction;
    public float fallingBrakingFriction;

    [Header("WallSlide")]
    public float wallSlideFriction;
    public float wallSlideBrakingFriction;
    public float wallSlideSpeed = 2f;
    public float wallSlideCoolDown;
    private float wallSlideCDTimer;
    public LayerMask wallLayer; // Указываем в инспекторе, какие слои считаем стенами

    public enum PlayerState
    {
        Idle,
        Walking,
        Falling,
        Dashing,
        WallSliding,
        Hooking
    }

    private Player player;


    private PlayerState currentState;

    void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return; // Если состояние не изменилось, выходим
        currentState = newState;
        Debug.Log("State Changed To: " + currentState);
    }

   

    void Start()
    {
        manager = GetComponent<UIManager>();
        player = GetComponent<Player>();
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Camera.main.transform.localRotation = Quaternion.Euler(0, 0f, 0f);
    }
    


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            manager.ToggleUI();
        }
        if (player.isUIOpen)
        {      
            return;
        }

        Look();

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        inputDirection = (transform.right * moveX + transform.forward * moveZ).normalized;

        isGrounded = controller.isGrounded;
        coyoteTimeCounter = isGrounded ? coyoteTime : coyoteTimeCounter - Time.deltaTime;

        switch (currentState)
        {
            case PlayerState.Idle:
                if (inputDirection.magnitude > 0 || velocity.magnitude > 0) ChangeState(PlayerState.Walking);
                if (!isGrounded) ChangeState(PlayerState.Falling);
                break;

            case PlayerState.Walking:
                if (velocity.magnitude == 0) ChangeState(PlayerState.Idle);
                if (!isGrounded) ChangeState(PlayerState.Falling);
                PhysWalking();
                break;

            case PlayerState.Falling:
                if (isGrounded) ChangeState(PlayerState.Idle);
                PhysFalling();
                break;

            case PlayerState.Dashing:
                // Не реагируем на другие состояния пока идёт рывок
                break;

            case PlayerState.WallSliding:
                if (isGrounded) ChangeState(PlayerState.Idle);
                PhysWallSliding();
                break;

            case PlayerState.Hooking:
                //if (!activeGrapple) ChangeState(PlayerState.Falling);
                break;
        }

        if (player.unlockables[(int)Player.abilities.walljump])
            TryWallSlide();
        
        TryJump();


        if (Input.GetKeyDown(KeyCode.LeftShift)&&player.unlockables[(int)Player.abilities.dash])
        {
                ChangeState(PlayerState.Dashing);
                StartCoroutine(Dash());
        }

        if (wallSlideCDTimer > 0)
            wallSlideCDTimer -= Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        controller.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
    }


    void PhysWalking()
    {
        verticalVelocity = -2f;
        canDoubleJump = true;

        Vector3 lateralVelocity = new Vector3(velocity.x, 0, velocity.z);
        float lateralSpeed = lateralVelocity.magnitude;
        if (lateralSpeed > maxWalkSpeed)
        {

            lateralSpeed = lateralSpeed - walkingFriction * Time.deltaTime;
            if (lateralSpeed < maxWalkSpeed)
                lateralSpeed = maxWalkSpeed;
        }

        if (inputDirection.magnitude == 0)
        {
            lateralSpeed = lateralSpeed - walkingBrakingFriction * Time.deltaTime;
            lateralSpeed = Mathf.Clamp(lateralSpeed, 0, lateralSpeed);
            velocity = lateralSpeed * velocity.normalized;
            return;
        }

        Vector3 Direction = (inputDirection.magnitude == 0) ? velocity.normalized : inputDirection;
        Vector3 newVelocity = lateralSpeed * Direction + Direction * acceleration * Time.deltaTime;


        if (newVelocity.magnitude > maxWalkSpeed)
        {
            if (lateralSpeed > maxWalkSpeed)
            {
                velocity = lateralSpeed * Direction;
            }
            else
            {
                velocity = maxWalkSpeed * Direction;
            }
            return;
        }


        velocity = lateralSpeed * Direction + Direction * acceleration * Time.deltaTime;
    }

    void PhysFalling()
    {
        verticalVelocity -= (verticalVelocity > 0 ? gravityJumping : gravityFalling) * Time.deltaTime;

        Vector3 lateralVelocity = new Vector3(velocity.x, 0, velocity.z);
        float lateralSpeed = lateralVelocity.magnitude;
        if (lateralSpeed > maxWalkSpeed)
        {

            lateralSpeed = lateralSpeed - fallingFriction * Time.deltaTime;
            if (lateralSpeed < maxWalkSpeed)
                lateralSpeed = maxWalkSpeed;
        }

        if (inputDirection.magnitude == 0)
        {
            lateralSpeed = lateralSpeed - fallingBrakingFriction * Time.deltaTime;
            lateralSpeed = Mathf.Clamp(lateralSpeed, 0, lateralSpeed);

            velocity = lateralSpeed * velocity.normalized;
            return;
        }

        Vector3 Direction = (inputDirection.magnitude == 0) ? velocity.normalized : inputDirection;

        Vector3 newVelocity = lateralSpeed * Direction + Direction * acceleration * Time.deltaTime;


        if (newVelocity.magnitude > maxWalkSpeed)
        {
            if (lateralSpeed > maxWalkSpeed)
            {
                velocity = lateralSpeed * Direction;
            }
            else
            {
                velocity = maxWalkSpeed * Direction;
            }
            return;
        }


        velocity = lateralSpeed * Direction + Direction * acceleration * Time.deltaTime;

    }

    void PhysWallSliding()
    {
        verticalVelocity = -wallSlideSpeed;
        canDoubleJump = true;
        velocity = Vector3.zero;
    }

    void TryJump()
    {
        if (!Input.GetButtonDown("Jump"))
            return;

        if (currentState == PlayerState.Idle || currentState == PlayerState.Walking || currentState == PlayerState.WallSliding)
        {
            Jump();
            return;
        }
        if (currentState == PlayerState.Falling && (coyoteTimeCounter > 0 || canDoubleJump) && player.unlockables[(int)Player.abilities.doublejump])
        {
            Jump();
            return;
        }
    }

    void Jump()
    {
        if (currentState == PlayerState.Falling && coyoteTimeCounter <= 0 && canDoubleJump)
        {
            canDoubleJump = false;
            verticalVelocity = doubleJumpForce;
            return;
        }


        if (currentState == PlayerState.WallSliding)
        {
            velocity = lastWallNormal * wallJumpForce;
            wallSlideCDTimer = wallSlideCoolDown;
        }

        verticalVelocity = jumpForce;

        ChangeState(PlayerState.Falling);
    }

    void TryWallSlide()
    {
        bool wallLeft = Physics.Raycast(transform.position, -transform.right, out RaycastHit leftWallHit, wallCheckDistance, wallLayer);
        bool wallRight = Physics.Raycast(transform.position, transform.right, out RaycastHit rightWallHit, wallCheckDistance, wallLayer);

        // Если персонаж касается стены, но не стоит на земле → включаем скольжение
        isWallSliding = (wallLeft || wallRight) && !isGrounded && (wallSlideCDTimer <= 0);

        if (isWallSliding)
        {
            lastWallNormal = wallLeft ? leftWallHit.normal : rightWallHit.normal;
            ChangeState(PlayerState.WallSliding);
        }
    }


    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.Rotate(Vector3.up * mouseX);
        Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }


    // Реализация рывка через корутину
    private IEnumerator Dash()
    {
        if (isDashing)
            yield return null;

        isDashing = true;

        Vector3 dashDirection = inputDirection.normalized;
        float dashSpeed = dashDistance / dashDuration;

        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            velocity = dashDirection * dashSpeed;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        velocity = maxWalkSpeed * velocity.normalized;
        ChangeState(PlayerState.Idle);
        isDashing = false;
    }

    public void SetGrappling(bool newValue)
    {
        activeGrapple = newValue;
        if (activeGrapple)
        {
            ChangeState(PlayerState.Hooking);
        }
        else
        {
            ChangeState(PlayerState.Falling);
            wallSlideCDTimer = wallSlideCoolDown;
        }
    }

    public void SetVelocity(Vector3 newValue)
    {
        velocity = newValue;
    }

    public void SetVerticalVelocity(float newValue)
    {
        verticalVelocity = newValue;
    }
}
