using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlurData
{
    public float blur_size;
    public int blur_iteration;
    public int blur_down_sample;
    public float blur_spread;
}

[RequireComponent(typeof(Camera))]
public class ScreenBlurEffect : MonoBehaviour
{
    // Ԥ�ȶ���shader��Ⱦ�õ�pass
    const int BLUR_HOR_PASS = 0;
    const int BLUR_VER_PASS = 1;
    bool is_support; // �жϵ�ǰƽ̨�Ƿ�֧��ģ��

    RenderTexture final_blur_rt;
    RenderTexture temp_rt;
    [SerializeField]
    public Material blur_mat; // ģ��������

    // �ⲿ����
    [Range(0, 127)]
    float blur_size = 1.0f; // ģ������ɢ����С
    [Range(1, 10)]
    public int blur_iteration = 4; // ģ��������������
    public float blur_spread = 1; // ģ��ɢֵ
    int cur_iterate_num = 1; // ��ǰ��������
    public int blur_down_sample = 4; // ģ����ʼ����������
    public bool render_blur_screenShot = false; // ģ����ͼִ�п���
    private Action<RenderTexture> blur_callback;

    void Awake()
    {
        is_support = SystemInfo.supportsImageEffects;
    }

    // ģ���������Ҫ����
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (is_support && blur_mat != null && render_blur_screenShot)
        {
// ���ȶ�����Ľ����һ�ν�������Ҳ���ǽ��ͷֱ��ʣ���СRTͼ�Ĵ�С
            int width = src.width / blur_down_sample;
            int height = src.height / blur_down_sample;
            // ����ǰ�����������Ⱦ������������RT��
            final_blur_rt = RenderTexture.GetTemporary(width, height, 0);
            Graphics.Blit(src, final_blur_rt);

            cur_iterate_num = 1; // ��ʼ������
            while (cur_iterate_num <= blur_iteration)
            {
                blur_mat.SetFloat("_BlurSize", (1.0f + cur_iterate_num * blur_spread) * blur_size);  // ����ģ����ɢuvƫ��
                temp_rt = RenderTexture.GetTemporary(width, height, 0);
                // ʹ��blit���������أ���Զ�Ӧ�Ĳ������pass������Ⱦ��������
                Graphics.Blit(final_blur_rt, temp_rt, blur_mat, BLUR_HOR_PASS);
                Graphics.Blit(temp_rt, final_blur_rt, blur_mat, BLUR_VER_PASS);
                RenderTexture.ReleaseTemporary(temp_rt);   // �ͷ���ʱRT
                cur_iterate_num++;
            }
            GetBlurScreenShot();
            Graphics.Blit(src, dest); // ���޸������������
            RenderTexture.ReleaseTemporary(final_blur_rt);  // final_blur_rt�����Ѿ���ɣ����Ի�����
            DisabledBlurRender(); // ��ͼ�߼�ִ����Ϻ�͹رսű�
        }
        else
        {
        Graphics.Blit(src, dest);
        }
    }

    public void EnableBlurRender(BlurData data = null, Action<RenderTexture> callback = null)
    {
        blur_size = data != null ? data.blur_size : 1.0f;
        blur_iteration = data != null ? data.blur_iteration : 4;
        blur_down_sample = data != null ? data.blur_down_sample : 4;
        blur_spread = data != null ? data.blur_spread : 1;
        render_blur_screenShot = true;

        blur_callback = callback;
        this.enabled = true;
    }
    // ������Ⱦ
    public void DisabledBlurRender()
    {
        render_blur_screenShot = false;
        this.enabled = false;
    }
    void GetBlurScreenShot()
    {
        if (blur_callback != null)
        {
            RenderTexture temp_screen_shot = RenderTexture.GetTemporary(final_blur_rt.width, final_blur_rt.height, 0);
            Graphics.Blit(final_blur_rt, temp_screen_shot);
            // ���ô���Ļص�
            blur_callback(temp_screen_shot);
        }
        // ����ִ����񣬶�Ҫ���һ�λص�����
        blur_callback = null;
    }

// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
