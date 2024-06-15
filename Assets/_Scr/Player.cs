using System;
using System.Collections;
using UnityEngine;
public class Player : MonoBehaviour
{
    [Header("Components")]
    public PlayerCamera playerCamera;
    public PlayerViewmodel playerViewmodel;
    public PlayerController playerController;
    public PlayerAnimator playerAnimator;

    [Header("Camera")]
    [SerializeField] private float xSens;
    [SerializeField] private float ySens;
    private float xRecoil;
    private float yRecoil;
    
    public float xRotation;//임시로 퍼블릭 박은거임..
    public float yRotation;//임시로 퍼블릭 박은거임..
    //private float xFucker;
    //private float yFucker;

    [Header("Movement")]
    [SerializeField] private float speedWalk;
    [SerializeField] private float speedRun;
    public float speedWall;
    public float jumpForce;

    [Header("Movement/Physics")]
    [SerializeField] private float gravity;
    [SerializeField] private float onGroundYVal;
    public float rangeWallRun;

    //dash
    [SerializeField] private AnimationCurve dashCurve;
    [SerializeField] private AnimationCurve wallRunCurve;
    [SerializeField] private AnimationCurve forceVectorCurve;
    private float dashMulti;
    private float forceMulti;

    //private members
    private Vector3 inputDirection;
    private Vector3 forceVec = Vector3.zero;

    public float speed { get; private set; }
    private float yVal;

    private StateMachine<PlayerStateEnum> playerStateMachine;//will be removed, check Awake() 
    public Action<Vector3> Mov;
    public Action OnJump;

    [Header("Debug")]
    [Header("Debug/Viewmodel")]
    [SerializeField] private float viewmodelValue;
    private float viewmodelY = 0;
    private float viewmodelX = 0;
    private bool b_value;
    [Header("Debug/Wallrun")]
    public LayerMask lm_wallrunable;
    private void Awake()
    {
        playerStateMachine = new StateMachine<PlayerStateEnum>();// will change to ref singleton, singleton will automaticaly make instance when not initialized
        playerStateMachine.AddState(PlayerStateEnum.OnGround, new PlayerStateOnGround(this));
        playerStateMachine.AddState(PlayerStateEnum.OnWallrun, new PlayerStateOnWallrun(this));
        playerStateMachine.Initialize(PlayerStateEnum.OnGround);

        playerCamera = GetComponentInChildren<PlayerCamera>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
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
        playerStateMachine.UpdateState();
        PlayerInput();
        PlayerUI.Instance.lists[0].text = playerStateMachine.CurrentState.ToString();
        //PlayerUI.Instance.lists[2].text = playerStateMachine.CurrentState.ToString();
        //if (Input.GetKeyDown(KeyCode.Backspace)) AddMovementImpulse(Vector3.forward, 1, 0);
        //if (Input.GetKeyDown(KeyCode.Mouse0)) playerAnimator.leftArmAnimator.Play("AutoShoot", -1, 0f);
    }
    private void FixedUpdate()
    {
        Mov?.Invoke(inputDirection);

        playerCamera.SetCameraRotation(xRotation, yRotation);
        playerViewmodel.SetViewmodelRotation(xRotation, yRotation);
        //playerViewmodel.SetViewmodelRotation(playerCamera.GetCameraRotTransform().eulerAngles.x, playerCamera.GetCameraRotTransform().eulerAngles.y);
    }
    public void PlayerApplyMovement(Vector3 direction, float speed, float gravityMultiplier = 0, float forceMulti = 1)
    {
        direction *= speed;

        bool isGround = playerController.IsGround;

        if (isGround && yVal < 0) yVal = onGroundYVal;
        else yVal += gravity * gravityMultiplier * Time.deltaTime;

        this.forceMulti -= Time.deltaTime;
        forceVec = Vector3.MoveTowards(forceVec, Vector3.zero, Time.deltaTime * forceMulti);

        direction.y = yVal;
        direction *= Time.deltaTime;

        playerController.Move(direction + forceVec * Time.deltaTime);
    }
    //private void PlayerApplyMovement()
    //{
    //    Vector3 direction = inputDirection;
    //    direction = direction.magnitude < 1 ? direction : direction.normalized; // needs optimazation
    //    direction *= speed;
    //
    //    Transform camTrm = playerCamera.GetCameraRotTransform();
    //
    //    bool CheckWall(out RaycastHit raycastHit, out bool isRight)
    //    {
    //        bool a = false;
    //        isRight = false;
    //        float range = 0.5f;
    //        if(Physics.Raycast(camTrm.position, camTrm.TransformDirection(Vector3.right), out raycastHit, range, lm_wallrunable))
    //        {
    //            //Debug.DrawRay(camTrm.position, camTrm.TransformDirection(Vector3.right) * range, Color.red);
    //            a = true;
    //            isRight = true;
    //        }
    //        else if (Physics.Raycast(camTrm.position, camTrm.TransformDirection(Vector3.left), out raycastHit, range, lm_wallrunable))
    //        {
    //            //Debug.DrawRay(camTrm.position, camTrm.TransformDirection(Vector3.left) * range, Color.red);
    //            a = true;
    //        }
    //        return a;
    //    }
    //    void PlayerAdditionalPhysics()
    //    {
    //        //dash
    //        direction *= dashCurve.Evaluate(dashMulti);
    //
    //        //wallRun
    //        bool isWallRunning = CheckWall(out RaycastHit raycastHit, out bool isRight);
    //        if (isWallRunning)
    //        {
    //            //playerStateMachine.ChangeState2(PlayerStateEnum.OnWallrun);
    //            //Debug.DrawRay(raycastHit.point, raycastHit.normal, Color.yellow); //collider normal
    //
    //            Vector3 forwardVector = Vector3.ProjectOnPlane(inputDirection, raycastHit.normal);
    //            Debug.DrawRay(camTrm.position, inputDirection, Color.red);
    //            Debug.DrawRay(camTrm.position, forwardVector, Color.green);
    //            forwardVector = forwardVector.magnitude < 1 ? forwardVector : forwardVector.normalized; // needs optimazation as well
    //            forwardVector *= speedWall;
    //
    //            direction = forwardVector;
    //        }
    //        //if (b_bufferJump && isGround) PlayerJump(); // jumpbuffer needs implementation
    //    }
    //    PlayerAdditionalPhysics();
    //    bool isGround = playerController.IsGround;
    //
    //    if (isGround && yVal < 0) yVal = onGroundYVal;
    //    else yVal += gravity * Time.deltaTime;
    //    direction.y = yVal;
    //    //PlayerAdditionalRaycast(); //lost
    //    direction *= Time.deltaTime;
    //    playerController.SetVelocity(direction);
    //}
    private void PlayerInput()
    {
        //mouse
        xRotation -= Input.GetAxisRaw("Mouse Y") * ySens + yRecoil;
        xRotation = Mathf.Clamp(xRotation, -89, 89);
        yRotation += Input.GetAxisRaw("Mouse X") * xSens + xRecoil;
        
        //movement
        Transform camTrm = playerCamera.GetCameraRotTransform();
        Vector3 camForward = camTrm.forward;
        camForward.y = 0;
        camForward.Normalize();
        Vector3 camRight = camTrm.right;
        inputDirection = camForward * Input.GetAxis("Vertical") + camRight * Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space)) OnJump?.Invoke();

        void Dash()
        {
            yVal = 0;
            dashMulti = 0.1f;//length of m_dashCurve
            playerAnimator.camAnimator.Play("OnDash", -1, 0);
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

    public void AddRecoil(float xRecoil , float yRecoil)
    {
        this.xRecoil = xRecoil;
        this.yRecoil = yRecoil;
    }
    public void AddForce(Vector3 dir, float speed = 1)
    {
        forceVec += dir * speed;
        forceMulti = 0.5f;//legnth of forceVectorCurve;
    }
    public void SetForce(Vector3 dir, float speed = 1)
    {
        forceVec = dir * speed;
        forceMulti = 0.5f;//legnth of forceVectorCurve;
    }
    #region state

    public void SetYVal(float value)
    {
        yVal = value;
    }
    public void AddYVal(float value)
    {
        yVal += value;
    }
    public float GetDashCurve(float x = -1)
    {
        x = x == -1 ? dashMulti : x;
        return dashCurve.Evaluate(x);
    }
    public float GetWallrunCurve(float x)
    {
        return wallRunCurve.Evaluate(x);
    }
    public float GetForceVectorCurve(float x = -1)
    {
        x = x == -1 ? forceMulti : x;
        return forceVectorCurve.Evaluate(x);
    }
    public bool CheckWall(out RaycastHit raycastHit, out bool isRight)
    {
        Transform camTrm = playerCamera.GetCameraRotTransform();
        float range = rangeWallRun;
        bool result = Physics.Raycast(camTrm.position, camTrm.right, out raycastHit, range, lm_wallrunable);
        isRight = result;
        result = result ? true : Physics.Raycast(camTrm.position, camTrm.TransformDirection(Vector3.left), out raycastHit, range, lm_wallrunable);
        return result;
    }
    public bool CheckWall(out RaycastHit raycastHit, out bool isRight, out Collider col)
    {
        Transform camTrm = playerCamera.GetCameraRotTransform();
        float range = rangeWallRun;
        bool result = Physics.Raycast(camTrm.position, camTrm.right, out raycastHit, range, lm_wallrunable);
        isRight = result;
        result = result ? true : Physics.Raycast(camTrm.position, camTrm.TransformDirection(Vector3.left), out raycastHit, range, lm_wallrunable);

        //angle
        //need optimazation
        Vector3 pForward = playerCamera.GetCameraRotTransform().forward;
        pForward.y = 0;
        pForward.Normalize();
        Vector3 currentDir = -raycastHit.normal;
        float angle = Vector3.Angle(pForward, currentDir);
        bool isOver = angle > 90 + 35 || angle < 90 - 10;
        result &= !isOver;

        col = raycastHit.collider;
        return result;
    }
    public bool CheckWall(out RaycastHit raycastHit, ref Vector3 dir)
    {
        Transform camTrm = playerCamera.GetCameraRotTransform();
        float range = rangeWallRun;
        Debug.DrawRay(camTrm.position, dir, Color.yellow, Time.deltaTime);
        bool result = Physics.Raycast(camTrm.position, dir, out raycastHit, range, lm_wallrunable);
        dir = -raycastHit.normal;
        dir.Normalize();
        return result;
    }
    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerCamera.GetCameraRotTransform().position, rangeWallRun);
    }
}
