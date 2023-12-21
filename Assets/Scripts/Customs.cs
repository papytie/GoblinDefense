using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Customs
{
    public static float GetDist(Vector3 _position, Vector3 _target)
    {
        return Vector3.Distance(_position, _target);
    }
}
