using UnityEngine;
public class Player : MonoBehaviour
{
    [Header("Components")]
    public PlayerCamera playerCamera;
    public PlayerViewmodel playerViewmodel;
    public PlayerController playerController;

    [Header("Mouse")]
    [SerializeField] private float xSens;
    [SerializeField] private float ySens;
    private float xRotation;
    private float yRotation;

    [Header("Movement")]
    [SerializeField] private float speedWalk;
    [SerializeField] private float speedRun;
    [SerializeField] private float jumpForce;

    [SerializeField] private float onGroundYVal;
    private Vector3 direction;
    private float speed;
    private float yVal;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<PlayerCamera>();
        playerController = GetComponentInChildren<PlayerController>();
    }
    private void Start()
    {
        //player Looks down when start?
        playerCamera.SetCameraPosition(0, 0, 0);
        playerCamera.SetCameraRotation(0, 0, 0);
        xRotation = 0;
        yRotation = 0;

        speed = speedWalk;
    }
    private void Update()
    {
        PlayerInput();
    }
    private void FixedUpdate()
    {
        PlayerApplyMovement();
        playerCamera.SetCameraRotation(xRotation, yRotation);
    }
    private void PlayerInput()
    {
        //mouse
        xRotation -= Input.GetAxisRaw("Mouse Y") * ySens;
        xRotation = Mathf.Clamp(xRotation, -89, 89);
        yRotation += Input.GetAxisRaw("Mouse X") * xSens;

        //movement
        Vector3 camForward = playerCamera.GetCameraRotTransform().forward;
        camForward.y = 0;
        camForward.Normalize();
        Vector3 camRight = playerCamera.GetCameraRotTransform().right;
        direction = camForward * Input.GetAxis("Vertical") + camRight * Input.GetAxis("Horizontal");
        void Jump()
        {
            yVal = jumpForce;
        }
        if (Input.GetKeyDown(KeyCode.Space) && playerController.IsGround) Jump(); // will be removed check PlayerApplyMovement();
    }
    private void PlayerApplyMovement()
    {
        direction *= speed;
        void PlayerAdditionalPhysics()
        {
            bool isGround = playerController.IsGround;//isGround will change later
            //if (b_bufferJump && isGround) PlayerJump(); // jumpbuffer needs implementation

            if (isGround && yVal < 0) yVal = onGroundYVal;
            else yVal += -9.81f * Time.deltaTime;
            direction.y = yVal;
            //PlayerAdditionalRaycast();
        }
        PlayerAdditionalPhysics();
        direction *= Time.deltaTime;
        playerController.SetVelocity(direction);
    }
}
