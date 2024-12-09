using UnityEngine;

public class BlinkEffect : MonoBehaviour
{
    public Color startColor = new Color(0, 1, 0, 0.5f); // 绿色，半透明
    public Color endColor = new Color(0, 0, 0, 0.1f);   // 黑色，更透明
    [Range(0, 10)]
    public float speed = 1;

    Renderer ren;

    void Awake()
    {
        ren = GetComponent<Renderer>();
    }

    void Update()
    {
        // 计算颜色渐变，包括透明度
        ren.material.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1));
    }
}
