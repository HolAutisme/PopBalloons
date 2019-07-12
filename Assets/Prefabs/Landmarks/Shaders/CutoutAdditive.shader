Shader "CutoutAdditive" {
	Properties{
		_Color("Main Color", Color) = (1.0,1.0,1.0,1.0)
		_MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
	_Cutoff("Base Alpha cutoff", Range(0,1.0)) = 0.5
	}
		Category{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		SubShader{

		// Set up basic lighting
		Material{
		Diffuse[_Color]
		Ambient[_Color]
	}
		Lighting On

		// Render both front and back facing polygons.
		Cull Off

		// first pass:
		//   render any pixels that are more than [_Cutoff] opaque

		Pass{
		Blend One One
		AlphaTest Greater[_Cutoff]

		SetTexture[_MainTex]{
		constantColor[_Color]
		combine constant * primary QUAD
	}
		SetTexture[_MainTex]{
		combine texture * previous
	}
	}
	}
	}
}