using UnityEngine;

public class GoalFlagObj : FlagObj
{
    [SerializeField] private AlphaChange _alphaChange;
    public void SetMaterialAlhpa(float alpha)
    {
        _alphaChange?.SetAlpha(alpha);
    }
}
