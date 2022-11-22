Shader "Custom/AreaShader"
{
    Properties
    {
        _ExteriorTex ("Exterior (RGB)", 2D) = "white" {}
        _InteriorTex ("Interior (RGB)", 2D) = "white" {}
        _Progress ("Progress", Range (0, 1)) = 0.0
    }
    
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade
        
        sampler2D _ExteriorTex;
        sampler2D _InteriorTex;
        float _Progress;

        struct Input
        {
            float2 uv_ExteriorTex;
            float2 uv_InteriorTex;
        };
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float realProgress = lerp(0.5, 1.0, _Progress);
            float2 uv = (IN.uv_InteriorTex - 0.5) / realProgress + 0.5;
            // get color of the base texture
            float4 base = tex2D (_ExteriorTex, IN.uv_ExteriorTex);
            float4 overlay = tex2D (_InteriorTex, uv);
            
            // set the output for now
            o.Albedo = base.rgb * overlay.rgb;
            o.Alpha = base.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
