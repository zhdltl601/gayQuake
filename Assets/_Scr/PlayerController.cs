using UnityEngine;
public class PlayerController : MonoBehaviour
{
    //private Rigidbody rigidBody;
    private CharacterController characterController;
    public bool IsGround => characterController.isGrounded; // will change later
    //public bool IsOnWall => 
    private void Awake()
    {
        //rigidBody = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }
    public void Move(Vector3 velocitiy)
    {
        characterController.Move(velocitiy);
    }
}
