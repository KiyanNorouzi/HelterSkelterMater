using UnityEngine;
using System.Collections;

public static class St_ExtraMath
{
    public static float DecByDeltaTimeToZero(float _val, float _speed)
    {
        float result = _val;
        float speed = _speed;

        result -= Time.deltaTime * speed;

        result = Mathf.Clamp(result, 0, float.MaxValue);

        return result;
    }

    public static float DecByDeltaTimeToZero(float _val)
    {
        return DecByDeltaTimeToZero(_val, 1);
    }

    public static Rect SumRects(Rect _rectA, Rect _rectB)
    {
        return new Rect(_rectA.xMin + _rectB.xMin, _rectA.yMin + _rectB.yMin, _rectA.width + _rectB.width, _rectA.height + _rectB.height);
    }

    public static float Dist(float _valA, float _valB)
    {
        return Mathf.Abs(_valA - _valB);
    }

}
