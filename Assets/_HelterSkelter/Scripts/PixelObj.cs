using UnityEngine;
using System.Collections;

public class PixelObj : MonoBehaviour
{
    public bool changeSpriteDepthByYPos = true;
    public Anim2DController anim2DController;
    Vector3 oldPos = new Vector3(-1000000, -1000000, -1000000);
    Vector3 trKhurd = Vector3.zero;

    int spriteOrderOffset = 0;

    [HideInInspector]
    public Side side = Side.RIGHT;

    float sideAngle_RU = 45f;
    float sideAngle_LU = 135f;
    float sideAngle_LD = 225f;
    float sideAngle_RD = 315f;


    // <<< For test

    Player asPlayer = null;

    float playerSikimKhiariSideAngle_R_U = 22.5f;
    float playerSikimKhiariSideAngle_RU_U = 67.5f;
    float playerSikimKhiariSideAngle_LU_U = 112.5f;
    float playerSikimKhiariSideAngle_L_U = 157.5f;
    float playerSikimKhiariSideAngle_L_D = 202.5f;
    float playerSikimKhiariSideAngle_LD_D = 247.5f;
    float playerSikimKhiariSideAngle_RD_D = 292.5f;
    float playerSikimKhiariSideAngle_R_D = 337.5f;

    //~

    void Awake()
    {
        PixObj_Awake();
    }

    // Update is called once per frame
    void Update()
    {
        PixObj_Update();
    }


    public virtual void PixObj_Awake()
    {
        //<<< For test

        asPlayer = GetComponent<Player>();

        //~

        oldPos = transform.position;
        SetViewSpriteDepthByY();
    }

    public virtual void PixObj_Update()
    {
        //transform.position += new Vector3(-0.08f, 0.0f, 0.0f);

        if (changeSpriteDepthByYPos)
        {
            if (oldPos.y != transform.position.y)
            {
                SetViewSpriteDepthByY();
            }
        }

        if (oldPos != transform.position)
        {
            Vector3 snappedPos = GetSnappedVec(transform.position);

            trKhurd += transform.position - snappedPos;

            transform.position = snappedPos;

            transform.position += GetKhurdVal();

            oldPos = transform.position;
        }
    }

    public Vector3 GetSnappedVec(Vector3 _vec)
    {
        Vector3 pos = _vec;
        return new Vector3((int)pos.x, (int)pos.y, (int)pos.z);
    }

    public Vector3 GetKhurdVal()
    {
        Vector3 result = Vector3.zero;

        if (trKhurd.x >= 1)
        {
            result.x = 1;
            trKhurd.x -= 1;
        }

        if (trKhurd.x <= -1)
        {
            result.x = -1;
            trKhurd.x += 1;
        }

        //

        if (trKhurd.y >= 1)
        {
            result.y = 1;
            trKhurd.y -= 1;
        }

        if (trKhurd.y <= -1)
        {
            result.y = -1;
            trKhurd.y += 1;
        }

        //

        if (trKhurd.z >= 1)
        {
            result.z = 1;
            trKhurd.z -= 1;
        }

        if (trKhurd.z <= -1)
        {
            result.z = -1;
            trKhurd.z += 1;
        }

        return result;
    }

    public virtual void SetSide(Side _side)
    {
        side = _side;

        switch (side)
        {

        }
    }

    public string GetAnimNameForCurSide(string _baseAnimName)
    {
        string result = _baseAnimName;

        switch (side)
        {
            case Side.UP:
                result += "_Up";
                break;

            case Side.DOWN:
                result += "_Down";
                break;

            case Side.LEFT:
                result += "_Left";
                break;

            case Side.RIGHT:
                result += "_Right";
                break;

            case Side.RIGHT_UP:
                result += "_RightUp";
                break;

            case Side.RIGHT_DOWN:
                result += "_RightDown";
                break;

            case Side.LEFT_UP:
                result += "_LeftUp";
                break;

            case Side.LEFT_DOWN:
                result += "_LeftDown";
                break;
        }

        return result;
    }

    public void SetSideByVec(Vector3 _vec)
    {
        Vector3 vec = _vec;

        float ang360 = MathFPlus.Angle360XY_FromRight(vec);

        Side si = GetSideByAngle(ang360);

        SetSide(si);
    }

    public Side GetSideByAngle(float _ang360) //<<< For test
    {
        float ang = _ang360;

        if (asPlayer != null)
        {
            if ((ang >= 0 && ang < playerSikimKhiariSideAngle_R_U) || (ang >= playerSikimKhiariSideAngle_R_D && ang <= 360))
            {
                return Side.RIGHT;
            }

            if (ang >= playerSikimKhiariSideAngle_R_U && ang < playerSikimKhiariSideAngle_RU_U)
            {
                return Side.RIGHT_UP;
            }

            if (ang >= playerSikimKhiariSideAngle_RU_U && ang < playerSikimKhiariSideAngle_LU_U)
            {
                return Side.UP;
            }

            if (ang >= playerSikimKhiariSideAngle_LU_U && ang < playerSikimKhiariSideAngle_L_U)
            {
                return Side.LEFT_UP;
            }

            if (ang >= playerSikimKhiariSideAngle_L_U && ang < playerSikimKhiariSideAngle_L_D)
            {
                return Side.LEFT;
            }

            if (ang >= playerSikimKhiariSideAngle_L_D && ang < playerSikimKhiariSideAngle_LD_D)
            {
                return Side.LEFT_DOWN;
            }

            if (ang >= playerSikimKhiariSideAngle_LD_D && ang < playerSikimKhiariSideAngle_RD_D)
            {
                return Side.DOWN;
            }

            if (ang >= playerSikimKhiariSideAngle_RD_D && ang < playerSikimKhiariSideAngle_R_D)
            {
                return Side.RIGHT_DOWN;
            }
        }
        else
        {
            if ((ang >= 0 && ang < sideAngle_RU) || (ang >= sideAngle_RD && ang <= 360))
            {
                return Side.RIGHT;
            }

            if (ang >= sideAngle_RU && ang < sideAngle_LU)
            {
                return Side.UP;
            }

            if (ang >= sideAngle_LU && ang < sideAngle_LD)
            {
                return Side.LEFT;
            }

            if (ang >= sideAngle_LD && ang < sideAngle_RD)
            {
                return Side.DOWN;
            }
        }

        return Side.RIGHT;
    }

    public void PlayAnimForCurSide(string _baseAnimName)
    {
        PlayAnimForCurSide(_baseAnimName, 1);
    }

    public void PlayAnimForCurSide(string _baseAnimName, float _animSpeed)
    {
        string anim = GetAnimNameForCurSide(_baseAnimName);
        float animSpeed = _animSpeed;

        anim2DController.PlayAnim(anim, animSpeed);
    }

    public void SetVisible(bool _val)
    {
        anim2DController.viewFrame.gameObject.SetActive(_val);
    }

    public void SetViewSpriteDepthByY()
    {
        SetViewSpriteDepth((int)(-transform.position.y), spriteOrderOffset);
    }

    public void SetViewSpriteDepth(int _val, int _offset)
    {
        anim2DController.viewFrame.sortingOrder = _val + _offset;
    }
}
