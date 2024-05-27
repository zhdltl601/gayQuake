using UnityEngine;
public class Player : MonoBehaviour
{
    [Header("Components")]
    public PlayerCamera playerCamera;
    public PlayerViewmodel playerViewmodel;
    public PlayerController playerController;

    [Header("Camera")]
    [SerializeField] private float xSens;
    [SerializeField] private float ySens;
    private float xRotation;
    private float yRotation;
    //private float xFucker;
    //private float yFucker;

    [Header("Movement")]
    [SerializeField] private float speedWalk;
    [SerializeField] private float speedRun;
    [SerializeField] private float speedWall;
    [SerializeField] private float jumpForce;

    [Header("Movement/Physics")]
    [SerializeField] private float gravity;
    [SerializeField] private float onGroundYVal;
    private float gravityMultiplier = 1;

    //dash
    [SerializeField] private AnimationCurve dashCurve;
    private float dashMulti;

    //private members
    private Vector3 inputDirection;
    private float speed;
    private float yVal;

    [Header("Debug")]
    [Header("Debug/Viewmodel")]
    [SerializeField] private float viewmodelValue;
    private float viewmodelY = 0;
    private float viewmodelX = 0;
    private bool b_value;
    [Header("Debug/Wallrun")]
    [SerializeField] private LayerMask lm_wallrunable;
    private void Awake()
    {
        playerCamera = GetComponentInChildren<PlayerCamera>();
        playerViewmodel = GetComponentInChildren<PlayerViewmodel>();
        playerController = GetComponentInChildren<PlayerController>();
    }
    private void Start()
    {
        //player Looks down when start?
        //playerCamera.SetCameraPosition(0, 0, 0);
        //playerCamera.SetCameraRotation(0, 0, 0);
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
        //...
        PlayerApplyMovement();
        playerCamera.SetCameraRotation(xRotation, yRotation);
        playerViewmodel.SetViewmodelRotation(xRotation, yRotation);
    }
    private void PlayerApplyMovement()
    {
        Vector3 direction = inputDirection;
        direction = direction.magnitude < 1 ? direction : direction.normalized; // needs optimazation
        direction *= speed;
        Transform camTrm = playerCamera.GetCameraRotTransform();

        void PlayerAdditionalPhysics()
        {
            //dash
            direction *= dashCurve.Evaluate(dashMulti);

            //wallRun
            gravityMultiplier = 1;
            bool isWallRunning = Physics.Raycast(camTrm.position, camTrm.TransformDirection(Vector3.right), out RaycastHit raycastHit, 1, lm_wallrunable);
            if (isWallRunning)
            {
                gravityMultiplier = 0;
                //Debug.DrawRay(raycastHit.point, raycastHit.normal, Color.yellow); //collider normal

                Vector3 forwardVector = Vector3.ProjectOnPlane(inputDirection, raycastHit.normal);
                forwardVector = forwardVector.magnitude < 1 ? forwardVector : forwardVector.normalized; // needs optimazation as well
                forwardVector *= speedWall;

                Debug.DrawRay(camTrm.position, inputDirection, Color.red);
                Debug.DrawRay(camTrm.position, forwardVector, Color.green);
                direction = forwardVector;
            }


            //if (b_bufferJump && isGround) PlayerJump(); // jumpbuffer needs implementation

            bool isGround = playerController.IsGround;

            if (isGround && yVal < 0) yVal = onGroundYVal;
            else yVal += gravity * Time.deltaTime;
            yVal *= gravityMultiplier;   //
            direction.y = yVal;
            //PlayerAdditionalRaycast(); //lost
        }
        PlayerAdditionalPhysics();
        direction *= Time.deltaTime;
        playerController.SetVelocity(direction);
    }
    private void PlayerInput()
    {
        //mouse
        xRotation -= Input.GetAxisRaw("Mouse Y") * ySens;
        xRotation = Mathf.Clamp(xRotation, -89, 89);
        yRotation += Input.GetAxisRaw("Mouse X") * xSens;

        //movement
        Transform camTrm = playerCamera.GetCameraRotTransform();
        Vector3 camForward = camTrm.forward;
        camForward.y = 0;
        camForward.Normalize();
        Vector3 camRight = camTrm.right;
        inputDirection = camForward * Input.GetAxis("Vertical") + camRight * Input.GetAxis("Horizontal");

        void Jump()
        {
            //will change to set b_buffer, check PlayerAdditionalPhysics();
            yVal = jumpForce;
        }
        if (Input.GetKeyDown(KeyCode.Space) && playerController.IsGround) Jump();

        void Dash()
        {
            yVal = 0;
            dashMulti = 0.1f;//length of m_dashCurve
        }
        if (Input.GetKeyDown(KeyCode.LeftShift)) Dash(); 
        if (dashMulti > 0) dashMulti -= Time.deltaTime; // will change later
        else dashMulti = 0;

        #region debug
        b_value = Input.GetKeyDown(KeyCode.C);
        float amount = viewmodelValue;
        if (Input.GetKey(KeyCode.I)) viewmodelY += Time.deltaTime * amount;
        if (Input.GetKey(KeyCode.K)) viewmodelY -= Time.deltaTime * amount;
        if (Input.GetKey(KeyCode.J)) viewmodelX -= Time.deltaTime * amount;
        if (Input.GetKey(KeyCode.L)) viewmodelX += Time.deltaTime * amount;
        playerViewmodel.SetViewmodelHintPosition(viewmodelX, viewmodelY, 0);
        #endregion
    }
    
}
