Shader "Custom/RadioEffectShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        [PerRenderData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Mask ("Mask (R)", 2D) = "white" {}
        _Lines ("Lines", float) = 5.0
        _Speed ("Speed", float) = 0.3
    }
    
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade
        
        sampler2D _MainTex;
        sampler2D _Mask;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Mask;
        };

        fixed4 _Color;
        float _Lines;
        float _Speed;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // displace the uvMask
            const float2 maskUv = float2 (IN.uv_Mask.x, IN.uv_Mask.y + _Time.y * _Speed);
            
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            fixed4 mask = tex2D (_Mask, maskUv * _Lines);
            
            o.Albedo = c.rgb;
            o.Alpha = c.a * mask.r;
            o.Emission = _Color;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
