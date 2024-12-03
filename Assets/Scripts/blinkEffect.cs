using UnityEngine;

public class BlinkEffect : MonoBehaviour
{
    public Color startColor = new Color(0, 1, 0, 0.5f); // ��ɫ����͸��
    public Color endColor = new Color(0, 0, 0, 0.1f);   // ��ɫ����͸��
    [Range(0, 10)]
    public float speed = 1;

    Renderer ren;

    void Awake()
    {
        ren = GetComponent<Renderer>();
    }

    void Update()
    {
        // ������ɫ���䣬����͸����
        ren.material.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1));
    }
}
