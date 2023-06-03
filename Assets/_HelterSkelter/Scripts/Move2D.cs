using UnityEngine;
using System.Collections;
//[@RequireComponent(typeof(BoxCollider2D))]



public enum PlayerStateMachine
{
    IDLE_START,
    IDLE_UPDATE,
    IDLE_FINISH,
    MOVEMENT_START,
    MOVEMENT_UPDATE,
    MOVEMENT_FINISH,
    KILL_START,
    KILL_UPDATE,
    KILL_FINISH,
    TRAP_START,
    TRAP_UPDATE,
    TRAP_FINISH,
    HIDDEN_START,
    HIDDEN_UPDATE,
    HIDDEN_FINISH,
    HIDEOUT_START,
    HIDEOUT_UPDATE,
    HIDEOUT_FINISH
}

public class Move2D : MonoBehaviour
{

    public float walkSpeed = 3f;
    
    CharacterController characterController;

    void Start()
    {
       
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Movement();

    }


    void PlayerState()
    {
        


    }




    public void Movement()
    {
        var direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * walkSpeed * Time.deltaTime;

        characterController.Move(new Vector3(direction.x, direction.y));

    }

}
