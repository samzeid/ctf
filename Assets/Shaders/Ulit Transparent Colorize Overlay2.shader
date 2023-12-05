Shader "Unlit/Transparent Colorize Overlay2" {
	Properties {
        _Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
    }
    Category {
       Lighting On
       ZWrite Off
	   ZTest Always
       Cull Back
       Blend SrcAlpha OneMinusSrcAlpha
       Tags {"Queue"="Transparent+1"}
       SubShader {
            Material {
               Emission [_Color]
            }
            Pass {
               SetTexture [_MainTex] {
               		constantColor [_Color]
                    Combine Texture * Primary, Texture * constant
                }
            }
        } 
    }
}
