Shader "Custom/AlphaBlendTransition" {
	Properties{
		_Blend("Blend", Range(0, 1)) = 0.0
		_BaseTexture("Base Texture", 2D) = "white" {}
		_OverlayTexture("Texture 2 with alpha", 2D) = "" {}

	}
		SubShader{
		Tags {
		"Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
				Pass {
					SetTexture[_BaseTexture]
					SetTexture[_OverlayTexture] {
						ConstantColor(0,0,0,[_Blend])
						combine texture Lerp(constant) previous
					}
				}
	}
}