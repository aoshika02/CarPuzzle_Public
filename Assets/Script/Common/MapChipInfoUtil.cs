using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapChipInfoUtil : MonoBehaviour
{
    private static List<(RoadType roadType, List<DirectionType> directionType)> _activeDatas = new List<(RoadType, List<DirectionType>)>
    {
        (RoadType.UpDown,   new List<DirectionType>{DirectionType.Up,   DirectionType.Down}),
        (RoadType.LeftRight,new List<DirectionType>{DirectionType.Left, DirectionType.Right}),
        (RoadType.UpLeft,   new List<DirectionType>{DirectionType.Up,   DirectionType.Left}),
        (RoadType.UpRight,  new List<DirectionType>{DirectionType.Up,   DirectionType.Right}),
        (RoadType.LeftDown, new List<DirectionType>{DirectionType.Left, DirectionType.Down}),
        (RoadType.RightDown,new List<DirectionType>{DirectionType.Right,DirectionType.Down}),
        (RoadType.TUp,      new List<DirectionType>{DirectionType.Up,   DirectionType.Left, DirectionType.Right,}),
        (RoadType.TDown,    new List<DirectionType>{DirectionType.Left, DirectionType.Right,DirectionType.Down,  }),
        (RoadType.TLeft,    new List<DirectionType>{ DirectionType.Up,  DirectionType.Left, DirectionType.Down,  }),
        (RoadType.TRight,   new List<DirectionType>{DirectionType.Up ,  DirectionType.Right,DirectionType.Down,  })
    };
    private static List<(RoadType roadType, List<ConectData> notTarget)> _conectRoadType = new List<(RoadType, List<ConectData>)>
    {
        (RoadType.UpDown,   new List<ConectData>
        {
            (new ConectData
            {
                DirectionType = DirectionType.Up,
                ConectRoadTypes = new List < RoadType > { RoadType.LeftRight,RoadType.UpLeft,RoadType.UpRight,RoadType.TUp}
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Left,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Right,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Down,
                ConectRoadTypes = new List < RoadType > { RoadType.LeftRight, RoadType.LeftDown,RoadType.RightDown,RoadType.TDown}
            }),
        }),
        (RoadType.LeftRight,new List<ConectData>
        {
           (new ConectData
            {
                DirectionType = DirectionType.Up,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Left,
                ConectRoadTypes = new List < RoadType > { RoadType.UpDown,RoadType.UpLeft,RoadType.LeftDown,RoadType.TLeft}
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Right,
                ConectRoadTypes = new List < RoadType > { RoadType.UpDown,RoadType.UpRight,RoadType.RightDown,RoadType.TRight}
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Down,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
        }),
        (RoadType.UpLeft,   new List<ConectData>
        {
            (new ConectData
            {
                DirectionType = DirectionType.Up,
                ConectRoadTypes = new List < RoadType > { RoadType.LeftRight, RoadType.UpLeft, RoadType.UpRight, RoadType.TUp }
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Left,
                ConectRoadTypes = new List < RoadType > { RoadType.UpDown, RoadType.UpLeft, RoadType.LeftDown, RoadType.TLeft }
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Right,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Down,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
        }),
        (RoadType.UpRight,  new List<ConectData>
        {
            (new ConectData
            {
                DirectionType = DirectionType.Up,
                ConectRoadTypes = new List < RoadType > { RoadType.LeftRight, RoadType.UpLeft, RoadType.UpRight, RoadType.TUp }
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Left,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Right,
                ConectRoadTypes = new List < RoadType > { RoadType.UpDown, RoadType.UpRight, RoadType.RightDown, RoadType.TRight }
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Down,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
        }),
        (RoadType.LeftDown, new List<ConectData>
        {
            (new ConectData
            {
                DirectionType = DirectionType.Up,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Left,
                ConectRoadTypes = new List < RoadType > { RoadType.UpDown, RoadType.UpLeft, RoadType.LeftDown, RoadType.TLeft }
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Right,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Down,
                ConectRoadTypes = new List < RoadType > { RoadType.LeftRight, RoadType.LeftDown, RoadType.RightDown, RoadType.TDown }
            }),
        }),
        (RoadType.RightDown,new List<ConectData>
        {
             (new ConectData
            {
                DirectionType = DirectionType.Up,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Left,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Right,
                ConectRoadTypes = new List < RoadType > { RoadType.UpDown, RoadType.UpRight, RoadType.RightDown, RoadType.TRight }
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Down,
                ConectRoadTypes = new List < RoadType > { RoadType.LeftRight, RoadType.LeftDown, RoadType.RightDown, RoadType.TDown }
            }),
        }),
        (RoadType.TUp,      new List<ConectData>
        {
            (new ConectData
            {
                DirectionType = DirectionType.Up,
                ConectRoadTypes = new List < RoadType > { RoadType.LeftRight, RoadType.UpLeft, RoadType.UpRight, RoadType.TUp }
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Left,
                ConectRoadTypes = new List < RoadType > { RoadType.UpDown, RoadType.UpLeft, RoadType.LeftDown, RoadType.TLeft }
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Right,
                ConectRoadTypes = new List < RoadType > { RoadType.UpDown, RoadType.UpRight, RoadType.RightDown, RoadType.TRight }
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Down,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
        }),
        (RoadType.TDown,    new List<ConectData>
        {
            (new ConectData
            {
                DirectionType = DirectionType.Up,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Left,
                ConectRoadTypes = new List < RoadType > { RoadType.UpDown, RoadType.UpLeft, RoadType.LeftDown, RoadType.TLeft }
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Right,
                ConectRoadTypes = new List < RoadType > { RoadType.UpDown, RoadType.UpRight, RoadType.RightDown, RoadType.TRight }
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Down,
                ConectRoadTypes = new List < RoadType > { RoadType.LeftRight, RoadType.LeftDown, RoadType.RightDown, RoadType.TDown }
            }),
        }),
        (RoadType.TLeft,    new List<ConectData>
        {
             (new ConectData
            {
                DirectionType = DirectionType.Up,
                ConectRoadTypes = new List < RoadType > { RoadType.LeftRight, RoadType.UpLeft, RoadType.UpRight, RoadType.TUp }
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Left,
                ConectRoadTypes = new List < RoadType > { RoadType.UpDown, RoadType.UpLeft, RoadType.LeftDown, RoadType.TLeft }
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Right,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Down,
                ConectRoadTypes = new List < RoadType > { RoadType.LeftRight, RoadType.LeftDown, RoadType.RightDown, RoadType.TDown }
            }),
        }),
        (RoadType.TRight,   new List<ConectData>
        {
             (new ConectData
            {
                DirectionType = DirectionType.Up,
                ConectRoadTypes = new List < RoadType > { RoadType.LeftRight, RoadType.UpLeft, RoadType.UpRight, RoadType.TUp }
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Left,
                ConectRoadTypes = new List < RoadType > { RoadType.None}
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Right,
                ConectRoadTypes = new List < RoadType > { RoadType.UpDown, RoadType.UpRight, RoadType.RightDown, RoadType.TRight }
            }),
            (new ConectData
            {
                DirectionType = DirectionType.Down,
                ConectRoadTypes = new List < RoadType > { RoadType.LeftRight, RoadType.LeftDown, RoadType.RightDown, RoadType.TDown }
            }),
        })
    };
    private class ConectData
    {
        public DirectionType DirectionType;
        public List<RoadType> ConectRoadTypes;
    }
    /// <summary>
    /// 道の属性と方向から通れるか判定
    /// </summary>
    /// <param name="roadType"></param>
    /// <param name="directionType"></param>
    /// <returns></returns>
    public static bool ActiveDecision(RoadType roadType, DirectionType directionType)
    {
        return GetActiveDirectionType(roadType).Any(x => x == directionType);
    }
    /// <summary>
    /// 通れる方向を取得
    /// </summary>
    /// <param name="roadType"></param>
    /// <returns></returns>
    public static List<DirectionType> GetActiveDirectionType(RoadType roadType)
    {
        return _activeDatas.FirstOrDefault(x => x.roadType == roadType).directionType;
    }
    /// <summary>
    /// 道の属性と方向からつながる方向を取得
    /// </summary>
    /// <param name="roadType"></param>
    /// <param name="directionType"></param>
    /// <returns></returns>
    public static List<RoadType> GetNotConectType(RoadType roadType, DirectionType directionType)
    {
        return _conectRoadType
            .Where(rt => rt.roadType == roadType)
            .SelectMany(nt => nt.notTarget)
            .Where(dt => dt.DirectionType == directionType)
            .SelectMany(crt => crt.ConectRoadTypes)
            .ToList();
    }
}
