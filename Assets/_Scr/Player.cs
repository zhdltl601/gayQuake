using System;
using System.Collections;
using UnityEngine;
public class Player : MonoBehaviour
{
    public PlayerCamera PlayerCamera { get; private set; }
    public PlayerViewmodel PlayerViewmodel { get; private set; }
    public PlayerController PlayerController { get; private set; }
    public PlayerAnimator PlayerAnimator { get; private set; }

    [Header("Camera")]
    [SerializeField] private float xSens;
    [SerializeField] private float ySens;
    private float xRecoil;
    private float yRecoil;
    private float xRotation;
    private float yRotation;

    [Header("Movement")]
    [SerializeField] private float speedWalk;
    [SerializeField] private float delayDash = 1;
    public float jumpForce;
    public float Speed { get; private set; }

    [Header("Wallrun")]
    public float speedWall;
    public float allowedWallrunAngleMin;
    public float allowedWallrunAngleMax;
    public float wallrunViewmodelAngle;
    public LayerMask lm_wallrunable;

    [Header("Movement/Physics")]
    [SerializeField] private float gravity = -9.81f;
    public float gravityMultiOnGround = 1;
    public float gravityMultiOnWall = 0.4f;
    [SerializeField] private float onGroundYVal;
    public float rangeWallrunCheck;

    [Header("Movement/Curves")]
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve speedStopCurve;
    [SerializeField] private AnimationCurve wallRunCurve;
    [SerializeField] private AnimationCurve dashCurve;
    [SerializeField] private AnimationCurve forceVectorCurve;
    private float dashMulti;
    private float forceMulti;
    private float yVal;

    [Header("Actions")]
    public Action<Vector3> Mov;
    public Action OnJump;
    public Action OnDash;

    //private members
    private Vector3 inputDirection;
    private Vector3 forceVec = Vector3.zero;

    private StateMachine<PlayerStateEnum> playerStateMachine;//will be removed, check Awake() 

    [Header("Debug")]
    [Header("Debug/Viewmodel")]
    [SerializeField] private float viewmodelBobbingX;
    private void Awake()
    {
        playerStateMachine = new StateMachine<PlayerStateEnum>();// will change to ref singleton, singleton will automaticaly make instance when not initialized
        playerStateMachine.AddState(PlayerStateEnum.OnGround, new PlayerStateOnGround(this));
        playerStateMachine.AddState(PlayerStateEnum.OnWallrun, new PlayerStateOnWallrun(this));
        playerStateMachine.Initialize(PlayerStateEnum.OnGround);

        PlayerCamera = GetComponentInChildren<PlayerCamera>();
        PlayerAnimator = GetComponentInChildren<PlayerAnimator>();
        PlayerViewmodel = GetComponentInChildren<PlayerViewmodel>();
        PlayerController = GetComponentInChildren<PlayerController>();
    }


    private void Start()
    {
        //player Looks down when start?
        //PlayerCamera.SetCameraPosition(0, 0, 0);
        //PlayerCamera.SetCameraRotation(0, 0, 0);
        xRotation = 0;
        yRotation = 0;

        Speed = speedWalk;
    }
    private void Update()
    {
        playerStateMachine.UpdateState();
        PlayerInput();
        delayDash += Time.deltaTime;
        delayDash = delayDash > 0.7f ? 0.7f : delayDash;
        //PlayerUI.Instance.lists[0].text = playerStateMachine.CurrentState.ToString();
        //PlayerUI.Instance.lists[2].text = playerStateMachine.CurrentState.ToString();
        //if (Input.GetKeyDown(KeyCode.Backspace)) AddMovementImpulse(Vector3.forward, 1, 0);
        //if (Input.GetKeyDown(KeyCode.Mouse0)) PlayerAnimator.leftArmAnimator.Play("AutoShoot", -1, 0f);
    }
    private void FixedUpdate()
    {
        Mov?.Invoke(inputDirection);

        PlayerCamera.SetCameraRotation(xRotation, yRotation);
        PlayerViewmodel.SetViewmodelRotation(xRotation, yRotation);
        Vector3 a = inputDirection;
        a.y = 0;
        a.Normalize();
        PlayerViewmodel.SetViewmodelHintPosition(a.x * viewmodelBobbingX, 0, a.z * viewmodelBobbingX);
        PlayerViewmodel.UpdateViewmodel();
    }
    public void PlayerApplyMovement(Vector3 direction, float speed, float gravityMultiplier = 0, float forceMulti = 1)
    {
        direction *= speed;

        bool isGround = PlayerController.IsGround;

        if (isGround && yVal < 0) yVal = onGroundYVal;
        else yVal += gravity * gravityMultiplier * Time.deltaTime;

        this.forceMulti -= Time.deltaTime;
        forceVec = Vector3.MoveTowards(forceVec, Vector3.zero, Time.deltaTime * forceMulti);

        direction.y = yVal;
        direction *= Time.deltaTime;

        PlayerController.Move(direction + forceVec * Time.deltaTime);
    }
    //private void PlayerApplyMovement()
    //{
    //    Vector3 direction = inputDirection;
    //    direction = direction.magnitude < 1 ? direction : direction.normalized; // needs optimazation
    //    direction *= speed;
    //
    //    Transform camTrm = PlayerCamera.GetCameraRotTransform();
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
    //    bool isGround = PlayerController.IsGround;
    //
    //    if (isGround && yVal < 0) yVal = onGroundYVal;
    //    else yVal += gravity * Time.deltaTime;
    //    direction.y = yVal;
    //    //PlayerAdditionalRaycast(); //lost
    //    direction *= Time.deltaTime;
    //    PlayerController.SetVelocity(direction);
    //}
    private void PlayerInput()
    {
        //mouse
        xRotation -= Input.GetAxisRaw("Mouse Y") * ySens + yRecoil;
        xRotation = Mathf.Clamp(xRotation, -89, 89);
        yRotation += Input.GetAxisRaw("Mouse X") * xSens + xRecoil;
        
        //movement
        Transform camTrm = PlayerCamera.GetCameraRotTransform();
        Vector3 camForward = camTrm.forward;
        camForward.y = 0;
        camForward.Normalize();
        Vector3 camRight = camTrm.right;
        inputDirection = camForward * Input.GetAxis("Vertical") + camRight * Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space)) OnJump?.Invoke();

        void Dash()
        {
            if(delayDash == 0.7f)
            {
                delayDash = 0;
                yVal = 0;
                dashMulti = 0.09f;//length of m_dashCurve
                OnDash?.Invoke();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftShift)) Dash(); 
        if (dashMulti > 0) dashMulti -= Time.deltaTime; // will change later 
        else dashMulti = 0;

        #region debug
        //b_value = Input.GetKeyDown(KeyCode.C);
        //float amount = viewmodelBobbingX;
        //if (Input.GetKey(KeyCode.I)) viewmodelY += Time.deltaTime * amount;
        //if (Input.GetKey(KeyCode.K)) viewmodelY -= Time.deltaTime * amount;
        //if (Input.GetKey(KeyCode.J)) viewmodelX -= Time.deltaTime * amount;
        //if (Input.GetKey(KeyCode.L)) viewmodelX += Time.deltaTime * amount;
        //PlayerViewmodel.SetViewmodelHintPosition(viewmodelX, viewmodelY, 0);
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
    public void SetYVal(float value)
    {
        yVal = value;
    }
    public void AddYVal(float value)
    {
        yVal += value;
    }
    #region state
    public float GetSpeedCurve(float x)
    {
        return speedCurve.Evaluate(x);
    }
    public float GetSpeedStopCurve(float value)
    {
        return speedStopCurve.Evaluate(value);
    }

    public float GetWallrunCurve(float x)
    {
        return wallRunCurve.Evaluate(x);
    }
    public float GetDashCurve(float x = -1)
    {
        x = x == -1 ? dashMulti : x;
        return dashCurve.Evaluate(x);
    }
    public float GetForceVectorCurve(float x = -1)
    {
        x = x == -1 ? forceMulti : x;
        return forceVectorCurve.Evaluate(x);
    }
    public bool CheckWall(out RaycastHit raycastHit, out bool isRight)
    {
        Transform camTrm = PlayerCamera.GetCameraRotTransform();
        float range = rangeWallrunCheck;
        bool result = Physics.Raycast(camTrm.position - new Vector3(0, 0.25f, 0), camTrm.right, out raycastHit, range, lm_wallrunable);
        isRight = result;
        result = result ? true : Physics.Raycast(camTrm.position - new Vector3(0, 0.25f, 0), camTrm.TransformDirection(Vector3.left), out raycastHit, range, lm_wallrunable);
        return result;
    }
    public bool CheckWall(out RaycastHit raycastHit, out bool isRight, out Collider col)
    {
        Transform camTrm = PlayerCamera.GetCameraRotTransform();
        float range = rangeWallrunCheck;
        bool result = Physics.Raycast(camTrm.position - new Vector3(0, 0.25f, 0), camTrm.right, out raycastHit, range, lm_wallrunable);
        isRight = result;
        result = result ? true : Physics.Raycast(camTrm.position, camTrm.TransformDirection(Vector3.left), out raycastHit, range, lm_wallrunable);

        //angle
        //need optimazation
        Vector3 pForward = PlayerCamera.GetCameraRotTransform().forward;
        pForward.y = 0;
        pForward.Normalize();
        Vector3 currentDir = -raycastHit.normal;
        float angle = Vector3.Angle(pForward, currentDir);//need optimazation
        bool isOver = angle > 90 + allowedWallrunAngleMax || angle < 90 - allowedWallrunAngleMin;
        result &= !isOver;

        col = raycastHit.collider;
        return result;
    }
    public bool CheckWall(out RaycastHit raycastHit, ref Vector3 dir)
    {
        Transform camTrm = PlayerCamera.GetCameraRotTransform();
        float range = rangeWallrunCheck * 2;
        Debug.DrawRay(camTrm.position, dir, Color.yellow, Time.deltaTime);
        bool result = Physics.Raycast(camTrm.position - new Vector3(0, 0.25f, 0), dir, out raycastHit, range, lm_wallrunable);
        dir = -raycastHit.normal;
        dir.Normalize();
        return result;
    }
    #endregion
}
