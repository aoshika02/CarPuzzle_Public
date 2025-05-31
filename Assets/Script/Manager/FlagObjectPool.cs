using System.Collections.Generic;
using UnityEngine;

public class FlagObjectPool : SingletonMonoBehaviour<FlagObjectPool>
{
    [SerializeField] private GameObject _flagObj;
    [SerializeField] private GameObject _goalFlag;
    private List<FlagObj> _flagObjs = new List<FlagObj>();
    private List<GoalFlagObj> _goalFlagObjs = new List<GoalFlagObj>();

    protected override void Awake()
    {
        if (CheckInstance() == false) return;
        for (int i = 0; i < 5; i++)
        {
            CreateFlagObj();
        }
    }
    /// <summary>
    /// フラッグオブジェト取得
    /// </summary>
    /// <returns></returns>
    public FlagObj GetFlagObj() 
    {
        FlagObj flagObj = null;
        foreach (var fo in _flagObjs)
        {
            if (fo.IsUse == true) continue;
            flagObj = fo;
        }
        if (flagObj == null) 
        {
            flagObj = CreateFlagObj();
        }
        flagObj.IsUse = true;
        return flagObj;
    }
    /// <summary>
    /// ゴールフラッグ取得
    /// </summary>
    /// <returns></returns>
    public GoalFlagObj GetGoalFlag() 
    {
        if(_goalFlagObjs.Count == 0) 
        {
            return CreateGoalFlagObj();
        }
        return _goalFlagObjs[0];
    }
    /// <summary>
    /// フラッグの生成
    /// </summary>
    /// <returns></returns>
    public FlagObj CreateFlagObj() 
    {
        GameObject obj = Instantiate(_flagObj, transform);
        FlagObj flagObj= obj.GetComponent<FlagObj>();
        flagObj.IsUse = false;
        flagObj.Deactive();
        _flagObjs.Add(flagObj);
        return flagObj;
    }
    /// <summary>
    /// ゴールフラッグの生成
    /// </summary>
    /// <returns></returns>
    public GoalFlagObj CreateGoalFlagObj()
    {
        GameObject obj = Instantiate(_goalFlag, transform);
        GoalFlagObj flagObj = obj.GetComponent<GoalFlagObj>();
        _goalFlagObjs.Add(flagObj);
        return flagObj;
    }
    /// <summary>
    /// フラッグオブジェト返還
    /// </summary>
    /// <param name="flagObj"></param>
    public void ReleaseFlagObj(FlagObj flagObj)
    {
        flagObj.IsUse = false;
    }
}
