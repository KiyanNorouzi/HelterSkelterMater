using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorTrigger : MonoBehaviour
{
    public Door owner;

    [HideInInspector]
    public List<Character> curInsideCharacters = new List<Character>();

    void OnTriggerEnter2D(Collider2D col)
    {
        Character character = col.GetComponent<CharacterPublicTrigger>().owner;

        if (!curInsideCharacters.Contains(character))
        {
            curInsideCharacters.Add(character);
        }

        RefreshDoorState();
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Character character = col.GetComponent<CharacterPublicTrigger>().owner;

        if (curInsideCharacters.Contains(character))
        {
            curInsideCharacters.Remove(character);
        }

        RefreshDoorState();
    }

    void RefreshDoorState()
    {
        if (curInsideCharacters.Count > 0)
        {
            if (!owner.IsState(DoorStates.DOOR_OPEN_UPDATE))
                owner.SetState(DoorStates.DOOR_OPEN_INIT);
        }
        else
        {
            if (!owner.IsState(DoorStates.DOOR_CLOSE_UPDATE))
                owner.SetState(DoorStates.DOOR_CLOSE_INIT);
        }
    }
}
