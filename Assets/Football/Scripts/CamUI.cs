using UnityEngine;

public class CamUI : MonoBehaviour
{
    RenderTexture texture;

    void Start()
    {
        texture = GetComponent<Camera>().targetTexture;
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(Screen.width - 320, 0, 320, 172), texture);
    }
}
