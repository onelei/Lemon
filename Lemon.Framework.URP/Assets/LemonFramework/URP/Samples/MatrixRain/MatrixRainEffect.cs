using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MatrixRainEffect : MonoBehaviour
{
    public Texture2D mainTexture;
    public Texture2D rainDropTexture;
    public float speed = 0.1f;
    public float intensity = 1.0f;
    public Color color = Color.green;
    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.SetTexture("_MainTex", mainTexture);
        mat.SetTexture("_RainDropTex", rainDropTexture);
        mat.SetFloat("_Speed", speed);
        mat.SetFloat("_Intensity", intensity);
        mat.SetColor("_Color", color);
    }

    void Update()
    {
        mat.SetFloat("_Speed", speed);
        // 动态更新可以在这里添加
    }
}