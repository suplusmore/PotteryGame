using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;

// RequireComponent用于确保脚本绑定的物体上必须包含这几个组件，如果这些组件缺失的话，Unity编辑器会自动添加它们
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class potterymesh : MonoBehaviour
{
    public LeapServiceProvider LeapServiceProvider; // 与LeapMotion设备交互获取手势数据的接口
    private Hand _hand; // 表示用户的手部数据

    // 初始化变量
    public int Details = 40; // 表示网格的细节程度，每一层的定点数量
    public int LayerCount = 20; // 模型的层数
    public float LayerHeight = 0.1f; // 每层高度
    public float Radius = 1.0f; // 半径
    public float Thickness = 0.1f; // 陶瓷壁厚度
    public float MinRadius = 0.5f; // 变形过程中的最小半径
    public float MaxRadius = 1.5f; // 变形过程中的最大半径
    [Space]
    public int InfluenceLayer = 8; // 定义每次受到手势操作影响的层数范围
    public float TouchPower = 0.005f; // 手势对模型变形的影响强度
    [Space]
    public Material Potteing = null; // 模型材质
    public LayerMask PotteryLayerMask = 0; // 指定与模型交互的层掩码，可能会涉及碰撞检测之类的

    private Mesh theMesh = null; // 陶瓷模型的网格数据

    // 模型的最小和最大内外半径平方和，用于后续碰撞检测和变形判断
    private float SqrMinOuterRadius = 0;
    private float SqrMaxOuterRadius = 0;
    private float SqrMinInnerRadius = 0;
    private float SqrMaxInnerRadius = 0;
    // 
    private int SplitIndex = 0;


    private void Awake()
    {
        // 计算最大最小内外半径平方值
        SqrMaxOuterRadius = MaxRadius * MaxRadius;
        SqrMinOuterRadius = MinRadius * MinRadius;
        float inner = MaxRadius - Thickness;
        SqrMaxInnerRadius = inner * inner;
        inner = MinRadius - Thickness;
        SqrMinInnerRadius = inner * inner;

        // 生成初始陶瓷模型
        CreateMesh();
    }

    public void CreateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();// 存储顶点数据
        List<int> triangles = new List<int>(); // 存储三角面片
        List<Vector2> uvs = new List<Vector2>(); // 存储网格的UV坐标数据用于纹理映射
        float deltaAngle = (float)(360f / (float)Details) * Mathf.Deg2Rad; // 计算每两个相邻顶点之间的角度增量（360°/一圈有多少顶点）

        // 分别生成陶瓷的底部、外部、顶部、内部、内底部
        CreateBottom(deltaAngle, vertices, triangles, uvs);
        CreateOuter(deltaAngle, vertices, triangles, uvs);

        SplitIndex = vertices.Count;

        CreateTop(deltaAngle, vertices, triangles, uvs);
        CreateInner(deltaAngle, vertices, triangles, uvs);
        CreateInnerBottom(deltaAngle, vertices, triangles, uvs);

        // 创建并设置网格
        theMesh = new Mesh();
        theMesh.name = "Pottery";
        theMesh.vertices = vertices.ToArray();
        theMesh.triangles = triangles.ToArray();
        theMesh.uv = uvs.ToArray();
        // 重新计算网格的边界和法线，保证渲染和碰撞检测的准确性
        theMesh.RecalculateBounds();
        theMesh.RecalculateNormals();
        SmoothNormals();

        // 将生成的网格数据和材质赋给对应的组件，用于碰撞检测和渲染
        GetComponent<MeshFilter>().mesh = theMesh;
        GetComponent<MeshCollider>().sharedMesh = theMesh;
        GetComponent<MeshRenderer>().material = Potteing;
    }

    private void CreateBottom(float deltaAngle, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        // 添加中心点
        vertices.Add(Vector3.zero);
        uvs.Add(new Vector2(0.25f, 0.25f));

        int index = 1;

        for (int i = 0; i < Details; ++i)
        {
            float angle = (float)i * deltaAngle;
            float cosAngle = Mathf.Cos(angle);
            float sinAngle = Mathf.Sin(angle);

            Vector3 v = new Vector3(Radius * cosAngle, 0, Radius * sinAngle);
            vertices.Add(v);

            triangles.Add(index);
            triangles.Add((index >= Details) ? 1 : index + 1); // 连接最后一个顶点和第一个顶点，形成闭合的圆形
            triangles.Add(0);

            // 为当前顶点分配uv坐标，每个顶点的uv坐标是以(0.25,0.25)为圆心的一个圆
            Vector2 u = new Vector2(0.25f + 0.5f * cosAngle, 0.25f + 0.5f * sinAngle);
            uvs.Add(u);

            ++index;
        }
    }

    private void CreateOuter(float deltaAngle, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        // 遍历每一层建模
        for (int layer = 0; layer <= LayerCount; ++layer)
        {
            float height = layer * LayerHeight; // 计算当前层的高度
            int vIndex = vertices.Count;
            int lastIndex = vIndex - Details - 1;
            int vIndexAddOne = vIndex + 1;
            int lastIndexAddOne = lastIndex + 1;

            float v = (((float)layer) / ((float)LayerCount)) * 0.4f + 0.5f;// v在(0.5,0.9)之间

            // 生成每一层的顶点和三角形
            for (int i = 0; i <= Details; ++i)
            {
                float angle = (i == Details) ? 0 : i * deltaAngle;
                float cosAngle = Mathf.Cos(angle);
                float sinAngle = Mathf.Sin(angle);

                Vector3 vo = new Vector3(Radius * cosAngle, height, Radius * sinAngle);
                vertices.Add(vo);

                // 生成两个三角形面片拼接成一个四边形，四边形中有两个顶点是被复用的
                if (layer > 0 && i < Details)
                {
                    triangles.Add(vIndex + i);
                    triangles.Add(vIndexAddOne + i);
                    triangles.Add(lastIndex + i);

                    triangles.Add(lastIndex + i);
                    triangles.Add(vIndexAddOne + i);
                    triangles.Add(lastIndexAddOne + i);
                }

                float u = ((float)i) / ((float)Details); // u在(0,1)之间
                Vector2 uv = new Vector2(u, v);
                uvs.Add(uv);
            }
        }
    }

    // 陶瓷模型最顶上的两层
    private void CreateTop(float deltaAngle, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        float inner = Radius - Thickness;

        for (int h = 0; h < 2; ++h)
        {
            float height = (LayerCount - h) * LayerHeight;

            int vIndex = vertices.Count; // 本层顶点索引
            int lastIndex = vIndex - Details - 1; // 上层顶点索引
            int vIndexAddOne = vIndex + 1;
            int lastIndexAddOne = lastIndex + 1;

            float v = 0.95f + h * 0.05f;

            for (int i = 0; i <= Details; ++i)
            {
                float angle = (i == Details) ? 0 : i * deltaAngle;
                float cosAngle = Mathf.Cos(angle);
                float sinAngle = Mathf.Sin(angle);

                Vector3 vo = new Vector3(inner * cosAngle, height, inner * sinAngle);
                vertices.Add(vo);

                if (i < Details)
                {
                    triangles.Add(vIndex + i);
                    triangles.Add(vIndexAddOne + i);
                    triangles.Add(lastIndex + i);

                    triangles.Add(lastIndex + i);
                    triangles.Add(vIndexAddOne + i);
                    triangles.Add(lastIndexAddOne + i);
                }

                float u = ((float)i) / ((float)Details);
                Vector2 uv = new Vector2(u, v);
                uvs.Add(uv);
            }
        }
    }

    // 建立陶瓷侧壁内侧
    private void CreateInner(float deltaAngle, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        // 与建立Outer类似，建立Innerface
        float inner = Radius - Thickness;
        int count = LayerCount - 1;
        for (int layer = 0; layer < count; ++layer)
        {
            float height = (LayerCount - layer - 1) * LayerHeight;
            int vIndex = vertices.Count;
            int lastIndex = vIndex - Details - 1;
            int vIndexAddOne = vIndex + 1;
            int lastIndexAddOne = lastIndex + 1;

            float v = 0.5f - ((((float)layer) / ((float)count)) * 0.5f); //(0,0.5)之间

            for (int i = 0; i <= Details; ++i)
            {
                float angle = (i == Details) ? 0 : i * deltaAngle;
                float cosAngle = Mathf.Cos(angle);
                float sinAngle = Mathf.Sin(angle);

                Vector3 vo = new Vector3(inner * cosAngle, height, inner * sinAngle);
                vertices.Add(vo);

                if (layer > 0 && i < Details)
                {
                    triangles.Add(vIndex + i);
                    triangles.Add(vIndexAddOne + i);
                    triangles.Add(lastIndex + i);

                    triangles.Add(lastIndex + i);
                    triangles.Add(vIndexAddOne + i);
                    triangles.Add(lastIndexAddOne + i);
                }

                float u = ((float)i) / ((float)Details) * 0.5f + 0.5f; //(0.5,1)之间
                Vector2 uv = new Vector2(u, v);
                uvs.Add(uv);
            }
        }
    }
    // 创建内底
    private void CreateInnerBottom(float deltaAngle, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        int index = vertices.Count;
        float inner = Radius - Thickness;

        for (int i = 0; i < Details; ++i)
        {
            float angle = (float)i * deltaAngle;
            float cosAngle = Mathf.Cos(angle);
            float sinAngle = Mathf.Sin(angle);

            Vector3 v = new Vector3(inner * cosAngle, LayerHeight, inner * sinAngle);
            vertices.Add(v);

            triangles.Add(index + Details);
            triangles.Add((i >= Details - 1) ? index : index + i + 1);
            triangles.Add(index + i);

            Vector2 u = new Vector2(0.75f + 0.5f * cosAngle, 0.25f + 0.5f * sinAngle);
            uvs.Add(u);
        }

        vertices.Add(new Vector3(0, LayerHeight, 0));
        uvs.Add(new Vector2(0.75f, 0.25f));
    }

    private Vector3 targetWorldPos = Vector3.zero;
    private bool isShaping = false;
    private bool rIsPinching = false;
    private bool lIsPinching = false;
    private Vector3 lastScreenPos = Vector3.zero;

    // 识别用户的手势动作并根据手势动作对陶瓷模型进行形状调整
    private void Update()
    {
        // LeapMotion提供的Hands类接口，获取手部数据
        Hand leftHand = Hands.Left;
        Hand rightHand = Hands.Right;
        // 获取左右手的食指数据
        Finger rIndex = rightHand.GetIndex();
        Finger lIndex = leftHand.GetIndex();

        // 获取左右手手指的数据，并将其从世界坐标系转换到屏幕坐标系
        Vector3 rIndexPos = new Vector3(rIndex.TipPosition.x, rIndex.TipPosition.y, rIndex.TipPosition.z);
        Vector3 lIndexPos = new Vector3(lIndex.TipPosition.x, lIndex.TipPosition.y, lIndex.TipPosition.z);
        Vector3 rscreenPos = Camera.main.WorldToScreenPoint(rIndexPos);
        Vector3 lscreenPos = Camera.main.WorldToScreenPoint(lIndexPos);

        // 检测右手操作
        if (!rIsPinching && rightHand.IsPinching()) // 如果右手处于捏合状态，rIsPinching用于保证捏合的过程中不一直进行碰撞检测，而是捏一次检测一次
        {
            rIsPinching = true; // 表示当前正在捏合过程中
            lastScreenPos = rscreenPos;
            Debug.Log(lastScreenPos);
            // 从右手食指屏幕坐标发出一条射线，检测是否与陶瓷模型相交
            if (Physics.Raycast(Camera.main.ScreenPointToRay(lastScreenPos), out RaycastHit hit, 1000f, PotteryLayerMask))
            {
                // 记录交点以进行形状调整
                targetWorldPos = hit.point;
                isShaping = true;
            }
            else
                isShaping = false;

        }
        else if (isShaping && rightHand.IsPinching()) // 在捏合过程中进行形状调整
        {
            Vector3 currPos = rscreenPos; // 当前右手食指在屏幕中的位置
            ShapeIt(currPos - lastScreenPos, true); // 根据（每次更新时食指的位置-未捏合时/上一次的手指位置）来对陶瓷模型进行形变
            lastScreenPos = currPos; // 更新上一次手指位置
        }
        else if (!rightHand.IsPinching()) // 单次捏合结束，重置rIsPinching
        {
            rIsPinching = false;
        }

        // 左手捏合与右手逻辑相同
        if (!lIsPinching && leftHand.IsPinching())
        {
            lIsPinching = true;
            lastScreenPos = lscreenPos;
            Debug.Log(lastScreenPos);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(lastScreenPos), out RaycastHit hit, 1000f, PotteryLayerMask))
            {
                targetWorldPos = hit.point;
                isShaping = true;
            }
            else
                isShaping = false;

        }
        else if (isShaping && leftHand.IsPinching())
        {
            Vector3 currPos = lscreenPos;
            ShapeIt(currPos - lastScreenPos, false);
            lastScreenPos = currPos;
        }
        else if (!leftHand.IsPinching())
        {
            lIsPinching = false;
        }

        //mouse
        //if (Input.touchCount == 1)
        //{
        //    Touch touch = Input.GetTouch(0);
        //    if (touch.phase == TouchPhase.Began)
        //    {
        //        if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out RaycastHit hit, 1000f, PotteryLayerMask))
        //        {
        //            targetWorldPos = hit.point;
        //            isShaping = true;
        //        }
        //        else
        //            isShaping = false;
        //    }
        //    else if (isShaping && (touch.phase == TouchPhase.Moved))
        //    {
        //        ShapeIt(touch.deltaPosition);
        //    }
        //    else if (touch.phase == TouchPhase.Ended)
        //        isShaping = false;
        //}
        //else
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        lastScreenPos = Input.mousePosition;
        //        if (Physics.Raycast(Camera.main.ScreenPointToRay(lastScreenPos), out RaycastHit hit, 1000f, PotteryLayerMask))
        //        {
        //            targetWorldPos = hit.point;
        //            isShaping = true;
        //        }
        //        else
        //            isShaping = false;
        //    }
        //    else if (isShaping && Input.GetMouseButton(0))
        //    {
        //        Vector3 currPos = Input.mousePosition;
        //        ShapeIt(currPos - lastScreenPos);
        //        lastScreenPos = currPos;
        //    }
        //    else if (Input.GetMouseButtonUp(0))
        //        isShaping = false;
        //}


        //ShowNormals();
    }

    /*
    private void ShowNormals()
    {
        Vector3[] positions = theMesh.vertices;
        Vector3 [] normals = theMesh.normals;

        int step = Details + 1;
        int start = step;
        int end = start + (LayerCount + 3 ) * step;
        
        for( int i = start; i < end; i += step )
        {
            Vector3 dir = transform.TransformDirection(normals[i]);
            Vector3 pos = transform.TransformPoint(positions[i]);
            Debug.DrawRay(pos, dir, Color.red );
            int index = i + Details;
            dir = transform.TransformDirection(normals[index]);
            pos = transform.TransformPoint(positions[index]);
            Debug.DrawRay(pos, dir, Color.blue);
        }
    }
    */

    private bool IsInRight()
    {
        Transform camera = Camera.main.transform;
        // 获得交点与陶瓷模型在相机空间的局部坐标系
        Vector3 target = camera.InverseTransformPoint(targetWorldPos); // targetWoldPos为手指与陶瓷触碰的交点
        Vector3 pottery = camera.InverseTransformPoint(transform.position); // transform.position为陶瓷模型的世界坐标

        return target.x > pottery.x; // 确定调整的方向是内缩还是外扩
    }

    // 调整陶瓷模型的形状：修改网格顶点位置或缩放模型
    private void ShapeIt(Vector3 deltaPos ,bool isHorizontal)
    {
        bool bHorizontal = false;
        bool bVertical = false;

        float dirRate = 0, scale = 0.005f;


        if (isHorizontal) // 调整水平方向
        {
            // 判断是否超出正常范围（去除一些可能抖动的情况）
            if (deltaPos.x > 0.01f) // 向右移动
            {
                dirRate = IsInRight() ? 1f : -1f;
                bHorizontal = true;
            }
            else if (deltaPos.x < -0.01f) // 向左移动
            {
                dirRate = IsInRight() ? -1f : 1f;
                bHorizontal = true;
            }
        }
        else // 调整垂直方向
        {
            if (deltaPos.y > 0.02f)
            {
                scale = 0.002f;
                bVertical = true;
            }
            else if (deltaPos.y < -0.02f)
            {
                scale = -0.002f;
                bVertical = true;
            }
        }

        //if (deltaPos.x > 0.01f)
        //{
        //    dirRate = IsInRight() ? 1f : -1f;
        //    bHorizontal = true;
        //}
        //else if (deltaPos.x < -0.01f)
        //{
        //    dirRate = IsInRight() ? -1f : 1f;
        //    bHorizontal = true;
        //}

        //if (deltaPos.y > 0.02f)
        //{
        //    scale = 0.002f;
        //    bVertical = true;
        //}
        //else if (deltaPos.y < -0.02f)
        //{
        //    scale = -0.002f;
        //    bVertical = true;
        //}

        if (bHorizontal) // 进行水平方向的变形
        {
            Vector3 targetPos = transform.InverseTransformPoint(targetWorldPos); // 将陶瓷从世界坐标系转换到相机坐标系中
            Vector3[] vertices = theMesh.vertices;
            int maxVerticesIndex = vertices.Length - 1;
            float Range = InfluenceLayer * LayerHeight; // 每次调整影响8层节点

            for (int i = 1; i < maxVerticesIndex; ++i) // 遍历所有的顶点
            {
                float max, min;
                // 更新变化的最大值和最小值
                if (i < SplitIndex) // 如果是底部、外部顶点
                {
                    max = SqrMaxOuterRadius;
                    min = SqrMinOuterRadius;
                }
                else // 如果是内部、顶部顶点
                {
                    max = SqrMaxInnerRadius;
                    min = SqrMinInnerRadius;
                }

                float dis = Mathf.Abs(targetPos.y - vertices[i].y);
                if (dis < Range) // 如果当前变化在影响范围内
                {
                    // 计算当前顶点水平方向的向量（x和z方向）
                    Vector3 dir = vertices[i];
                    dir.y = 0;
                    // 调整顶点位置
                    if ((dirRate > 0 && dir.sqrMagnitude < max) || (dirRate < 0 && dir.sqrMagnitude > min))
                        // 更新后的顶点坐标 = 原坐标+顶点单位向量*位移方向*位移强度*衰减范围（y方向变化越大，横向变化就得越小，因为陶瓷体积是一定的）
                        vertices[i] += dir.normalized * dirRate * TouchPower * (1f - dis / Range);
                }
            }

            // 重新计算边界和法线
            theMesh.vertices = vertices;
            theMesh.RecalculateBounds();
            theMesh.RecalculateNormals();
            SmoothNormals();
        }

        // 垂直方向调整：直接进行缩放
        if (bVertical)
        {
            Vector3 sc = transform.localScale;
            sc.y = Mathf.Clamp(sc.y + scale, 0.5f, 1.5f); // 限制缩放范围在0.5到1.5之间
            transform.localScale = sc;
        }
    }

    private void SmoothNormals()
    {
        // 获取网格的所有法线
        Vector3[] normals = theMesh.normals;

        // 定义平滑的范围，start和end分别是平滑的起始、结束顶点索引
        int step = Details + 1;
        int start = step;
        int end = start + (LayerCount + 3) * step;

        // 一层一层进行遍历
        for (int i = start; i < end; i += step)
        {
            // 当前层和下一层对应顶点的索引
            int index1 = i;
            int index2 = i + Details;

            // 将二者对应顶点的法线平均一下然后归一化
            Vector3 normal = ((normals[index1] + normals[index2]) / 2f).normalized;
            normals[index1] = normal;
            normals[index2] = normal;
        }

        theMesh.normals = normals;
    }
}

