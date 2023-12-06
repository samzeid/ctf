Shader "Unlit/TargetShader"
{
     Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ColorMask RGB

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma exclude_renderers gles xbox360 ps3
            ENDCG

            SetTexture[_MainTex]
            {
                combine texture
            }
        }
    }
}
