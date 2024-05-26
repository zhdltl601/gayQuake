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

    //dash
    [SerializeField] private AnimationCurve dashCurve;
    private float dashMulti;

    private Vector3 inputDirection;
    private float speed;
    private float yVal;

    [Header("Debug")]
    [SerializeField] private float viewmodelValue;
    private float viewmodelY = 0;
    private float viewmodelX = 0;
    private bool b_value;
    [SerializeField] private LayerMask lm;
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
        //direction = direction.magnitude < 1 ? direction : direction.normalized; needs optimazation
        direction *= speed;
        void PlayerAdditionalPhysics()
        {
            //dash
            direction *= dashCurve.Evaluate(dashMulti);

            //wallRun
            bool isWallRunning = Physics.Raycast(playerCamera.GetCameraRotTransform().position, playerCamera.GetCameraRotTransform().TransformDirection(Vector3.left), out RaycastHit raycastHit, 2, lm);
            //PlayerUI.Instance.lists[0].text = isWallRunning.ToString();
            if (isWallRunning)
            {
                Debug.DrawRay(raycastHit.point, raycastHit.normal, Color.red);
                //bool r = true;
                //direction = 
            }


            //if (b_bufferJump && isGround) PlayerJump(); // jumpbuffer needs implementation

            bool isGround = playerController.IsGround;//isGround will change later

            if (isGround && yVal < 0) yVal = onGroundYVal;
            else yVal += gravity * Time.deltaTime;
            direction.y = yVal;
            //PlayerAdditionalRaycast();
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
        Vector3 camForward = playerCamera.GetCameraRotTransform().forward;
        camForward.y = 0;
        camForward.Normalize();
        Vector3 camRight = playerCamera.GetCameraRotTransform().right;
        inputDirection = camForward * Input.GetAxis("Vertical") + camRight * Input.GetAxis("Horizontal");

        void Jump()
        {
            yVal = jumpForce;
        }
        if (Input.GetKeyDown(KeyCode.Space) && playerController.IsGround) Jump(); // will be removed check PlayerApplyMovement();
        void Dash()
        {
            yVal = 0;
            dashMulti = 0.1f;//length of m_dashCurve
        }
        if (dashMulti > 0) dashMulti -= Time.deltaTime; // will change later
        else dashMulti = 0;
        if (Input.GetKeyDown(KeyCode.LeftShift)) Dash(); 



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
