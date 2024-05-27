using UnityEngine;
public class PlayerController : MonoBehaviour
{
    //private Rigidbody rigidBody;
    private CharacterController characterController;
    public bool IsGround => characterController.isGrounded; // will change later
    private void Awake()
    {
        //rigidBody = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }
    public void SetVelocity(Vector3 velocity)
    {
        characterController.Move(velocity);
    }
}
