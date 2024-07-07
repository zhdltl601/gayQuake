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
    public Action<Vector3> OnViewmodelHint;

    //private members
    private Vector3 inputDirection;
    private float lastDirSqr;
    public bool IsHoldingAnyInput { get; private set; }
    private Vector3 forceVec = Vector3.zero;
    private Vector3 re;

    private StateMachine<PlayerStateEnum> playerStateMachine;//will be removed, check Awake() 

    [Header("Debug")]
    [Header("Debug/Viewmodel")]
    public float viewmodelBobbingX;


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

        //calculates if moving or not
        Vector3 currentInput = inputDirection;
        bool isHoldingInput = currentInput.sqrMagnitude > lastDirSqr;
        currentInput = currentInput.sqrMagnitude < 1 ? currentInput : currentInput.normalized;
        isHoldingInput |= currentInput.sqrMagnitude + 0.01f >= 1;

        lastDirSqr = inputDirection.sqrMagnitude;
        IsHoldingAnyInput = isHoldingInput;

        Vector3 a = inputDirection;
        a.y = 0;
        a.Normalize();
        Vector3 tar = IsHoldingAnyInput ? a : Vector3.zero;
        re = Vector3.MoveTowards(re, tar, Time.deltaTime * 6f);
        OnViewmodelHint?.Invoke(re);
    }
    private void FixedUpdate()
    {
        Mov?.Invoke(inputDirection);

        PlayerCamera.SetCameraRotation(xRotation, yRotation);
        PlayerViewmodel.SetViewmodelRotation(xRotation, yRotation);
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
                
                UIManager.Instance.UseDash();
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
        //PlayerViewmodel.SetViewmodelviewmodelHintPosition(viewmodelX, viewmodelY, 0);
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
        //Debug.DrawRay(camTrm.position, dir, Color.yellow, Time.deltaTime);
        bool result = Physics.Raycast(camTrm.position - new Vector3(0, 0.25f, 0), dir, out raycastHit, range, lm_wallrunable);
        dir = -raycastHit.normal;
        dir.Normalize();
        return result;
    }
    #endregion
}
