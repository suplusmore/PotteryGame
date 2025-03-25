using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        var material = GetComponent<MeshRenderer>().material;

        material.mainTexture = GenerateTexture();
    }


    Texture2D GenerateTexture()
    {
        // 创建一个 128*128 的二维纹理
        var texture = new Texture2D(128, 128, TextureFormat.ARGB32, false);

        // 定义一个颜色数组
        var colors = new Color[32 * 32];
        for (int i = 0; i < colors.Length; ++i)
        {
            colors[i] = new Color(0, 0, 0, 1);
        }

        // 在纹理左下角 32*32 的范围绘制一块黑色区域
        texture.SetPixels(0, 0, 31, 31, colors);

        // Apply 使设置生效
        texture.Apply(false, false);
        return texture;
    }

}
