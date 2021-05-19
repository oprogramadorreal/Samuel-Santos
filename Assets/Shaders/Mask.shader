// Based on http://wiki.unity3d.com/index.php?title=DepthMask
Shader "Custom/Mask"
{
    SubShader
    {
        Tags { "Queue" = "Transparent+1" }

        // Don't draw in the RGBA channels; just the depth buffer
        ColorMask 0
        ZWrite On
        ZTest Always
        Alphatest LEqual 1.5
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        // Do nothing specific in the pass:
        Pass {}
    }
}
