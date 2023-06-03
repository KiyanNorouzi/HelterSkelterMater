using UnityEngine;
using System.Collections;

public static class MathFPlus 
{
    public static float DecDeltaTimeToZero(float _timeCounter, float _speed)
    {
        float timeCounter = _timeCounter;
        float speed = _speed;

        timeCounter -= Time.deltaTime * speed;

        if (timeCounter < 0)
            timeCounter = 0;

        return timeCounter;
    }

    public static float DecDeltaTimeToZero(float _timeCounter)
    {
        return DecDeltaTimeToZero(_timeCounter, 1);
    }

    public static float Angle360XY(Vector3 _from, Vector3 _to)
    {
        Vector3 vecA = _from;
        Vector3 vecB = _to;

        float positiveAng = Vector3.Angle(vecA, vecB);

        float crossZ = Vector3.Cross(vecA, vecB).z;

        if (crossZ < 0)
            positiveAng = 360 - positiveAng;

        return positiveAng;
    }

    public static float Angle360XY_FromRight(Vector3 _to)
    {
        Vector3 vecA = Vector3.right;
        Vector3 vecB = _to;


        return Angle360XY(vecA, vecB);
    }

    public static bool IsPointGenerallyInView(Vector3 _basePoint, Vector3 _otherPoint, float _maxDist, Vector3  _viewVector, float _viewHalfAngle)
    {
        Vector3 basePoint = _basePoint;
        Vector3 otherPoint = _otherPoint;
        float maxDist = _maxDist;
        Vector3 viewVector = _viewVector;
        float viewHalfAngle = _viewHalfAngle;

        Vector3 distVec = otherPoint - basePoint;

        if (distVec.magnitude > maxDist)
        {
            return false;
        }

        if (Vector3.Angle(viewVector, distVec) > viewHalfAngle)
        {
            return false;
        }

        return true;
    }
}
