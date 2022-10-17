Shader "Unlit/test"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100


        HLSLINCLUDE
        #include "Packages/com.unity.render-pipeline.universal/ShaderLibrary/Core.hlsl"

        // we should declare the properties again here
        // and hlsl ask us using CBUFFER when declare properties
        CBUFFER_START(UnityPerMaterial)
        float4 _BaseColor;

        CBUFFER_END
        ENDHLSL
        Pass
        {
           
        }
    }
}
