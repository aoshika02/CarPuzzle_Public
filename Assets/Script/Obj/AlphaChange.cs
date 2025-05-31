using UnityEngine;

public class AlphaChange : MonoBehaviour
{
    Material _targetMaterial;
    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        Material[] materials = renderer?.materials;
        _targetMaterial = materials.Length > 1 ? materials[1] : materials[0];
    }
    /// <summary>
    /// アルファ変更
    /// </summary>
    /// <param name="alpha"></param>
    public void SetAlpha(float alpha) 
    {
        if (_targetMaterial == null) return;
        Color baseColor = _targetMaterial.GetColor(Shader.PropertyToID("_BaseColor"));
        baseColor.a = alpha; 
        _targetMaterial.SetColor(Shader.PropertyToID("_BaseColor"), baseColor);
    }
}
