using UnityEngine;

public class SetMainCamera : MonoBehaviour
{
    public Camera newMainCamera;

    void Start()
    {
        if (newMainCamera != null)
        {
            Camera.main.tag = "Untagged"; // ȡ����ǰ MainCamera �ı�ǩ
            newMainCamera.tag = "MainCamera"; // �����������Ϊ MainCamera
        }
    }
}
