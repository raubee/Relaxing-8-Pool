// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Sprites/Ball"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _SpFactor ("Pixel snap", Float) = 128.
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFragN
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
            #include "UnityLightingCommon.cginc"

        float _SpFactor;

        // Expensive for low-end devices but it's fun to have shiny realtime spec these pool balls
        fixed4 SpriteFragN(v2f IN) : SV_Target
		{
		    fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
            fixed3 norm = float3(IN.texcoord.x, IN.texcoord.y, length(IN.texcoord - float2(0.5, 0.5))) - 0.5;
            norm = normalize(norm);
            fixed3 l = normalize(_WorldSpaceLightPos0.xyz);
            fixed3 v = normalize(float3(0.2, 0.2, -1.)); // Simulated faked view vector
            fixed3 h = normalize(l + v);
            fixed sp = pow(max(0, dot(norm, h)), _SpFactor);
            fixed df = max(0, dot(norm, l));
            c.rgb = c * df + sp * _LightColor0;
		    c.rgb *= c.a;
		    return c;
		}
        
        ENDCG

        }
    }
}
