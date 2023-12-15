using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class TrackableJSON
{
    public List<PosDataPoint> dataRecord;
}

[Serializable]
public class PosDataPoint
{
    public float[] dataPoint;
}
