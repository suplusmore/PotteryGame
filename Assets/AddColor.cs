using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddColor : MonoBehaviour
{
    public Vector3 originPos;//射线起始位置
    public Vector3 direction;//射线方向
    public float maxDistance;//射线最大检测距离
    private Texture2D pottery_tex;//要上色的纹理贴图
    private Texture detail_tex;//花纹
    public Color brush_color;//要刷的颜色
    public Color init_brush_color;
    public GameObject pottery;
    public GameObject brush;
    public GameObject tail;
    public GameObject head;
    public LayerMask PotteryLayerMask = 0;
    private Color[] m_textureColorsStart;
    public int size = 100;
    public bool addcolor;
    // Use this for initialization
    void Start()
    {
        //pottery = getDontDestroyOnLoadGameObjects()[0];
        pottery_tex = pottery.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
        m_textureColorsStart = pottery_tex.GetPixels();
        init_brush_color = brush.GetComponent<MeshRenderer>().material.color;
        pottery.GetComponent<MeshRenderer>().material.EnableKeyword("_DETAIL_MULX2");
        maxDistance = 1.0f;
    }
    // Update is called once per frame
    void Update()
    {
        brush_color = brush.GetComponent<MeshRenderer>().material.color;
        Vector3 tailpos = transform.localToWorldMatrix.MultiplyPoint(tail.transform.localPosition);
        Vector3 headpos = transform.localToWorldMatrix.MultiplyPoint(head.transform.localPosition);
        if(brush_color!=init_brush_color )
            addcolor = true;
        originPos = transform.position;//射线起始位置
        direction = tailpos-headpos;//射线方向
        Vector3 targetPos = originPos + direction * maxDistance;//射线最大检测距离处的点位置，找到起始点与终点，方便画线
        //Debug.Log(direction);
        Ray ray = new Ray(originPos, direction);//创建名为ray的射线
        RaycastHit hit;//碰撞检测信息存储
        if (Physics.Raycast(ray, out hit, maxDistance, PotteryLayerMask)&&(addcolor ==true))
        {//碰撞检测
            pottery.GetComponent<MeshRenderer>().material.mainTexture = GenerateTexture();
            detail_tex  = brush.GetComponent<MeshRenderer>().material.mainTexture;
            pottery.GetComponent<MeshRenderer>().material.SetTexture("_DetailAlbedoMap", detail_tex);
            Debug.DrawLine(originPos, hit.point);//画线显示
            Debug.Log(hit.collider.name);//打印检测到的碰撞体名称
        }
        else
        {
            Debug.Log("no Raycast");
            Debug.DrawLine(originPos, targetPos);//没检测到碰撞体，则以最大检测距离画线
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //还原
            pottery_tex.SetPixels(m_textureColorsStart);
            pottery_tex.Apply();
        }
    }
    Texture2D GenerateTexture()
    {
        // 创建一个 128*128 的二维纹理
        var texture = pottery.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;

        // 定义一个颜色数组
        var colors = new Color[texture.width * texture.height];
        for (int i = 0; i < colors.Length; ++i)
        {
            colors[i] = brush_color;
        }

        // 在纹理左下角 32*32 的范围绘制一块黑色区域
        if (brush.transform.position.y < 0.3)
        {
            texture.SetPixels(0, texture.height * 1 / 2, texture.width, texture.height * 1 / 6 , colors);
        }
        else if (brush.transform.position.y < 0.4)
            texture.SetPixels(0, texture.height * 2 / 3, texture.width, texture.height * 1 / 6 , colors);
        else texture .SetPixels(0, texture.height * 5/6-1, texture.width, texture.height * 1 / 6, colors);


        // Apply 使设置生效
        texture.Apply(false, false);
        return texture;
    }
    private GameObject[] getDontDestroyOnLoadGameObjects()
    {
        var allGameObjects = new List<GameObject>();
        allGameObjects.AddRange(FindObjectsOfType<GameObject>());
        //移除所有场景包含的对象
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            var objs = scene.GetRootGameObjects();
            for (var j = 0; j < objs.Length; j++)
            {
                allGameObjects.Remove(objs[j]);
            }
        }
        //移除父级不为null的对象
        int k = allGameObjects.Count;
        while (--k >= 0)
        {
            if (allGameObjects[k].transform.parent != null)
            {
                allGameObjects.RemoveAt(k);
            }
        }
        return allGameObjects.ToArray();
    }

}
