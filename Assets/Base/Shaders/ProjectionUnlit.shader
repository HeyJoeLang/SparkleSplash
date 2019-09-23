Shader "Projector/Unlit" {
	Properties{
		  _Color("Main Color", Color) = (1,1,1,1)
	   _ShadowTex("Cookie", 2D) = "" { TexGen ObjectLinear }
	   _FalloffTex("FallOff", 2D) = "" { TexGen ObjectLinear }
	}
		Subshader{
		   Pass {
			  ZWrite off
			  Fog { Color(0, 0, 0) }
			  Color[_Color]
			  ColorMask RGB
			  Blend One OneMinusSrcAlpha
			  Offset -1, -1
			  SetTexture[_ShadowTex] {
				 combine texture * primary, texture
				 Matrix[_Projector]
			  }
			  SetTexture[_FalloffTex] {
				 constantColor(0,0,0,1)
				 combine previous lerp(texture) constant
				 Matrix[_ProjectorClip]
			  }
		   }
	}
}