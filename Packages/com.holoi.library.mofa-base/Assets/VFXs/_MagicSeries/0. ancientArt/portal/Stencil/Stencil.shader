Shader "Custom/Stencil" {
	Properties{
		[IntRange] _StencilRef("Stencil Reference", Range(0, 255)) = 2
	}
		SubShader{
			Tags { "RenderType" = "Opaque" "Queue" = "Geometry-1" }

			Pass {
				ColorMask 0
				ZWrite Off

				Stencil {
					Ref[_StencilRef]
					Comp Always
					Pass Replace
				}
			}
	}
}