using System;
using System.Collections.Generic;
using UnityEngine;

public class MapChipData : MonoBehaviour
{
    public List<TargetPosData> TargetPosDatas = new List<TargetPosData>();
}
[Serializable]
public class TargetPosData
{
    public DirectionType DirectionType;
    public Transform Target;
}
