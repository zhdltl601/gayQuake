using UnityEngine;
public class PlayerController : MonoBehaviour
{
    //private Rigidbody rigidBody;
    private CharacterController characterController;
    public bool IsGround => characterController.isGrounded;
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
