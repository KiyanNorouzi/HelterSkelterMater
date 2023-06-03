using UnityEngine;
using System.Collections;

public class Sikim : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D[] asasas = new RaycastHit2D[100];

        int hitCount = Physics2D.RaycastNonAlloc(transform.position, Vector3.right, asasas, 1000, MapManager.Instance.rayCastLayer_Corpse);

        //print("sikaram");
    }
}
