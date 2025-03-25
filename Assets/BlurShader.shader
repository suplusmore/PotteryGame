Shader "Unlit/BlurShader"
{
Properties
    { 
        _MainTex("Texture", 2D) = "white" {} 
        _BlurSize("BlurSize", Range(0, 127)) = 1.0 
    }
SubShader
        { 
            CGINCLUDE
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                half4 _MainTex_TexelSize;
                fixed _BlurSize;

                struct v2f {
    
float4 pos : SV_POSITION;
half2 uv[5] : TEXCOORD0;
};

 
                v2f vert_hor(appdata_img v)
                {
 
       v2f o;
       o.pos = UnityObjectToClipPos(v.vertex);
       half2 uv = v.texcoord;


       o.uv[0] = uv;
       o.uv[1] = uv + half2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
       o.uv[2] = uv - half2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;

       o.uv[3] = uv + half2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
       o.uv[4] = uv - half2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;

       return o;
   }

 
                v2f vert_ver(appdata_img v)
                {
       v2f o;
       o.pos = UnityObjectToClipPos(v.vertex);
       half2 uv = v.texcoord;

 
       o.uv[0] = uv;
       o.uv[1] = uv + half2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
       o.uv[2] = uv - half2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;

       o.uv[3] = uv + half2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
       o.uv[4] = uv - half2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;

       return o;
   }

                fixed4 frag(v2f i) : SV_TARGET
                {
                    half weight[3] = {0.4026, 0.2442, 0.0545};
                    fixed3 color = tex2D(_MainTex, i.uv[0]).rgb * weight[0];
                
                    color += tex2D(_MainTex, i.uv[1]).rgb * weight[1];
                    color += tex2D(_MainTex, i.uv[2]).rgb * weight[1];
                    color += tex2D(_MainTex, i.uv[3]).rgb * weight[2];
                    color += tex2D(_MainTex, i.uv[4]).rgb * weight[2];

                return fixed4(color, 1.0);
            }
        ENDCG

        Cull Off
        ZWrite Off
        Pass 
        {
   Name "BLUR_HORIZONTAL"
   CGPROGRAM
   #pragma vertex vert_hor
   #pragma fragment frag
   ENDCG
}

Pass // 1:处理垂直模糊
{
    Name "BLUR_VERTICAL"
    CGPROGRAM
    #pragma vertex vert_ver
    #pragma fragment frag
    ENDCG
}
        }

            Fallback Off
}