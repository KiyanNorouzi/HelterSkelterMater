using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DoorStates
{
    DOOR_CLOSE_INIT,
    DOOR_CLOSE_UPDATE,
    DOOR_OPEN_INIT,
    DOOR_OPEN_UPDATE,
}

public class Door : PixelObj
{
    [HideInInspector]
    public DoorStates doorState = DoorStates.DOOR_CLOSE_INIT;
    public Collider2D mainCollider;
    bool isDoorOpen = false;

    [HideInInspector]
    public string animNameCloseDoor = "CloseDoor";

    [HideInInspector]
    public string animNameOpenDoor = "OpenDoor";

    void Update()
    {
        #region States

        #region DOOR_OPEN_INIT

        if (IsState(DoorStates.DOOR_OPEN_INIT))
        {
            anim2DController.PlayAnim(animNameOpenDoor);
            mainCollider.gameObject.SetActive(false);
            isDoorOpen = true;

            SetState(DoorStates.DOOR_OPEN_UPDATE);
        }

        #endregion

        #region DOOR_OPEN_UPDATE

        if (IsState(DoorStates.DOOR_OPEN_UPDATE))
        {

        }

        #endregion

        #region DOOR_ClOSE_INIT

        if (IsState(DoorStates.DOOR_CLOSE_INIT))
        {
            anim2DController.PlayAnim(animNameCloseDoor);
            mainCollider.gameObject.SetActive(true);
            isDoorOpen = false;

            SetState(DoorStates.DOOR_CLOSE_UPDATE);
        }

        #endregion

        #region DOOR_CLOSE_UPDATE

        if (IsState(DoorStates.DOOR_CLOSE_UPDATE))
        {

        }

        #endregion

        #endregion
    }

    public void SetState(DoorStates _doorState)
    {
        doorState = _doorState;
    }

    public bool IsState(DoorStates _doorState)
    {
        return doorState == _doorState;
    }

}
