using UnityEngine;

public class SetMainCamera : MonoBehaviour
{
    public Camera newMainCamera;

    void Start()
    {
        if (newMainCamera != null)
        {
            Camera.main.tag = "Untagged"; // 取消当前 MainCamera 的标签
            newMainCamera.tag = "MainCamera"; // 设置新摄像机为 MainCamera
        }
    }
}
