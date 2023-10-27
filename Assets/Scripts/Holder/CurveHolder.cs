using UnityEngine;
using System.Collections;

public class CurveHolder : MonoBehaviour
{
    static CurveHolder _instance;

    public AnimationCurve _linearRising;
    public AnimationCurve _acceleratedRising;
    public AnimationCurve _deAcceleratedRising;
    public AnimationCurve _tangentRising;

    public static AnimationCurve _LinearRising { get { return _instance._linearRising; } }
    public static AnimationCurve _AcceleratedRising { get { return _instance._acceleratedRising; } }
    public static AnimationCurve _DeAcceleratedRising { get { return _instance._deAcceleratedRising; } }
    public static AnimationCurve _TangentRising { get { return _instance._tangentRising; } }

    public static void Initialize()
    {
        _instance = FindObjectOfType<CurveHolder>();
    }
}
