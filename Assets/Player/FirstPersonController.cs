using System.Collections;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;
    public float jumpForce = 2f;
    public float doubleJumpForce = 2f;
    public float coyoteTime = 0.2f;
    public float hookSpeed = 10f;
    public float gravityFalling = 9.81f;
    public float gravityJumping = 4.905f;

    // Рывок
    public float dashDistance = 10f; // Дальность рывка
    public float dashDuration = 0.5f; // Длительность рывка
    
    private bool isDashing = false;
    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 inputMovement;
    private float ySpeed = 0f;
    private bool isGrounded;
    private bool canDoubleJump;
    private float coyoteTimeCounter;

    private float rotationX = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Camera.main.transform.localRotation = Quaternion.Euler(0, 0f, 0f);
    }

    void Update()
    {
        

        //Input movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        inputMovement = transform.right * moveX + transform.forward * moveZ;


        //Vertical movement
        VerticalMovement();
        

        controller.Move(inputMovement * moveSpeed * Time.deltaTime);
        controller.Move(new Vector3(0f, isDashing ? 0 : ySpeed, 0f) * Time.deltaTime);


        Look();

        //if (Input.GetButtonDown("Fire1") && !isHooking)
        //{
        //    StartCoroutine(HookShot());
        //}

        if (Input.GetButtonDown("Fire2"))
        {
            StartCoroutine(Dash());
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

    //// Простая реализация крюка-кошки
    //private IEnumerator HookShot()
    //{
    //    isHooking = true;
    //    hookDirection = Camera.main.transform.forward;

    //    float elapsedTime = 0f;
    //    Vector3 startPosition = transform.position;

    //    while (elapsedTime < 1f)
    //    {
    //        transform.position = Vector3.Lerp(startPosition, startPosition + hookDirection * 10f, elapsedTime);
    //        elapsedTime += Time.deltaTime * hookSpeed;
    //        yield return null;
    //    }

    //    isHooking = false;
    //}

    void VerticalMovement()
    {
        isGrounded = controller.isGrounded;

        coyoteTimeCounter = isGrounded ? coyoteTime : coyoteTimeCounter - Time.deltaTime;

        if (isDashing)
        {
            ySpeed = -2f;
            return;
        }

        if (isGrounded)
        {
            ySpeed = -2f;
            canDoubleJump = true;
        }
        else
        {
            ySpeed -= (controller.velocity.y > 0f ? gravityJumping : gravityFalling) * Time.deltaTime;
        }

        if (coyoteTimeCounter > 0 && Input.GetButtonDown("Jump"))
        {
            ySpeed = Mathf.Sqrt(jumpForce);
        }
        else if (Input.GetButtonDown("Jump") && canDoubleJump)
        {
            ySpeed = Mathf.Sqrt(doubleJumpForce);
            canDoubleJump = false;
        }
    }

    // Реализация рывка через корутину
    private IEnumerator Dash()
    {
        if (isDashing)
            yield return null;

        isDashing = true;

        Vector3 dashDirection = inputMovement.normalized;
        float dashSpeed = dashDistance / dashDuration;

        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }
}
