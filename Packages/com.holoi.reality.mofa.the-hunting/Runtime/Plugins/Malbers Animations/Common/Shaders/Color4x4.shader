// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Malbers/Color4x4"
{
	Properties
	{
		_Tint("Tint", Color) = (1,1,1,1)
		[Header(Albedo (A Gradient))]_Color1("Color 1", Color) = (1,0.1544118,0.1544118,0.291)
		_Color2("Color 2", Color) = (1,0.1544118,0.8017241,0.253)
		_Color3("Color 3", Color) = (0.2535501,0.1544118,1,0.541)
		_Color4("Color 4", Color) = (0.1544118,0.5451319,1,0.253)
		[Space(10)]_Color5("Color 5", Color) = (0.9533468,1,0.1544118,0.553)
		_Color6("Color 6", Color) = (0.2720588,0.1294625,0,0.097)
		_Color7("Color 7", Color) = (0.1544118,0.6151115,1,0.178)
		_Color8("Color 8", Color) = (0.4849697,0.5008695,0.5073529,0.078)
		[Space(10)]_Color9("Color 9", Color) = (0.3164301,0,0.7058823,0.134)
		_Color10("Color 10", Color) = (0.362069,0.4411765,0,0.759)
		_Color11("Color 11", Color) = (0.6691177,0.6691177,0.6691177,0.647)
		_Color12("Color 12", Color) = (0.5073529,0.1574544,0,0.128)
		[Space(10)]_Color13("Color 13", Color) = (1,0.5586207,0,0.272)
		_Color14("Color 14", Color) = (0,0.8025862,0.875,0.047)
		_Color15("Color 15", Color) = (1,0,0,0.391)
		_Color16("Color 16", Color) = (0.4080882,0.75,0.4811866,0.134)
		[Header(Metallic(R) Rough(G) Emmission(B))]_MRE1("MRE 1", Color) = (0,1,0,0)
		_MRE2("MRE 2", Color) = (0,1,0,0)
		_MRE3("MRE 3", Color) = (0,1,0,0)
		_MRE4("MRE 4", Color) = (0,1,0,0)
		[Space(10)]_MRE5("MRE 5", Color) = (0,1,0,0)
		_MRE6("MRE 6", Color) = (0,1,0,0)
		_MRE7("MRE 7", Color) = (0,1,0,0)
		_MRE8("MRE 8", Color) = (0,1,0,0)
		[Space(10)]_MRE9("MRE 9", Color) = (0,1,0,0)
		_MRE10("MRE 10", Color) = (0,1,0,0)
		_MRE11("MRE 11", Color) = (0,1,0,0)
		_MRE12("MRE 12", Color) = (0,1,0,0)
		[Space(10)]_MRE13("MRE 13", Color) = (0,1,0,0)
		_MRE14("MRE 14", Color) = (0,1,0,0)
		_MRE15("MRE 15", Color) = (0,1,0,0)
		_MRE16("MRE 16", Color) = (0,1,0,0)
		[Header(Emmision)]_EmissionPower("Emission Power", Float) = 1
		[SingleLineTexture][Header(Gradient)]_Gradient("Gradient", 2D) = "white" {}
		_GradientIntensity("Gradient Intensity", Range( 0 , 1)) = 1
		_GradientColor("Gradient Color", Color) = (0,0,0,0)
		_GradientScale("Gradient Scale", Float) = 1
		_GradientOffset("Gradient Offset", Float) = 0
		_GradientPower("Gradient Power", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 5.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Gradient;
		uniform float4 _GradientColor;
		uniform float _GradientIntensity;
		uniform float _GradientScale;
		uniform float _GradientOffset;
		uniform float _GradientPower;
		uniform float4 _Color1;
		uniform float4 _Color2;
		uniform float4 _Color3;
		uniform float4 _Color4;
		uniform float4 _Color5;
		uniform float4 _Color6;
		uniform float4 _Color7;
		uniform float4 _Color8;
		uniform float4 _Color9;
		uniform float4 _Color10;
		uniform float4 _Color11;
		uniform float4 _Color12;
		uniform float4 _Color13;
		uniform float4 _Color14;
		uniform float4 _Color15;
		uniform float4 _Color16;
		uniform float4 _Tint;
		uniform float _EmissionPower;
		uniform float4 _MRE1;
		uniform float4 _MRE2;
		uniform float4 _MRE3;
		uniform float4 _MRE4;
		uniform float4 _MRE5;
		uniform float4 _MRE6;
		uniform float4 _MRE7;
		uniform float4 _MRE8;
		uniform float4 _MRE9;
		uniform float4 _MRE10;
		uniform float4 _MRE11;
		uniform float4 _MRE12;
		uniform float4 _MRE13;
		uniform float4 _MRE14;
		uniform float4 _MRE15;
		uniform float4 _MRE16;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord256 = i.uv_texcoord * float2( 1,4 );
			float4 temp_cast_0 = (_GradientPower).xxxx;
			float temp_output_3_0_g801 = 1.0;
			float temp_output_7_0_g801 = 4.0;
			float temp_output_9_0_g801 = 4.0;
			float temp_output_8_0_g801 = 4.0;
			float temp_output_3_0_g796 = 2.0;
			float temp_output_7_0_g796 = 4.0;
			float temp_output_9_0_g796 = 4.0;
			float temp_output_8_0_g796 = 4.0;
			float temp_output_3_0_g798 = 3.0;
			float temp_output_7_0_g798 = 4.0;
			float temp_output_9_0_g798 = 4.0;
			float temp_output_8_0_g798 = 4.0;
			float temp_output_3_0_g797 = 4.0;
			float temp_output_7_0_g797 = 4.0;
			float temp_output_9_0_g797 = 4.0;
			float temp_output_8_0_g797 = 4.0;
			float temp_output_3_0_g791 = 1.0;
			float temp_output_7_0_g791 = 4.0;
			float temp_output_9_0_g791 = 3.0;
			float temp_output_8_0_g791 = 4.0;
			float temp_output_3_0_g794 = 2.0;
			float temp_output_7_0_g794 = 4.0;
			float temp_output_9_0_g794 = 3.0;
			float temp_output_8_0_g794 = 4.0;
			float temp_output_3_0_g790 = 3.0;
			float temp_output_7_0_g790 = 4.0;
			float temp_output_9_0_g790 = 3.0;
			float temp_output_8_0_g790 = 4.0;
			float temp_output_3_0_g784 = 4.0;
			float temp_output_7_0_g784 = 4.0;
			float temp_output_9_0_g784 = 3.0;
			float temp_output_8_0_g784 = 4.0;
			float temp_output_3_0_g795 = 1.0;
			float temp_output_7_0_g795 = 4.0;
			float temp_output_9_0_g795 = 2.0;
			float temp_output_8_0_g795 = 4.0;
			float temp_output_3_0_g800 = 2.0;
			float temp_output_7_0_g800 = 4.0;
			float temp_output_9_0_g800 = 2.0;
			float temp_output_8_0_g800 = 4.0;
			float temp_output_3_0_g799 = 3.0;
			float temp_output_7_0_g799 = 4.0;
			float temp_output_9_0_g799 = 2.0;
			float temp_output_8_0_g799 = 4.0;
			float temp_output_3_0_g793 = 4.0;
			float temp_output_7_0_g793 = 4.0;
			float temp_output_9_0_g793 = 2.0;
			float temp_output_8_0_g793 = 4.0;
			float temp_output_3_0_g786 = 1.0;
			float temp_output_7_0_g786 = 4.0;
			float temp_output_9_0_g786 = 1.0;
			float temp_output_8_0_g786 = 4.0;
			float temp_output_3_0_g792 = 2.0;
			float temp_output_7_0_g792 = 4.0;
			float temp_output_9_0_g792 = 1.0;
			float temp_output_8_0_g792 = 4.0;
			float temp_output_3_0_g789 = 3.0;
			float temp_output_7_0_g789 = 4.0;
			float temp_output_9_0_g789 = 1.0;
			float temp_output_8_0_g789 = 4.0;
			float temp_output_3_0_g787 = 4.0;
			float temp_output_7_0_g787 = 4.0;
			float temp_output_9_0_g787 = 1.0;
			float temp_output_8_0_g787 = 4.0;
			float4 temp_output_329_0 = ( ( ( _Color1 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g801 - 1.0 ) / temp_output_7_0_g801 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g801 / temp_output_7_0_g801 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g801 - 1.0 ) / temp_output_8_0_g801 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g801 / temp_output_8_0_g801 ) ) * 1.0 ) ) ) ) + ( _Color2 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g796 - 1.0 ) / temp_output_7_0_g796 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g796 / temp_output_7_0_g796 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g796 - 1.0 ) / temp_output_8_0_g796 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g796 / temp_output_8_0_g796 ) ) * 1.0 ) ) ) ) + ( _Color3 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g798 - 1.0 ) / temp_output_7_0_g798 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g798 / temp_output_7_0_g798 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g798 - 1.0 ) / temp_output_8_0_g798 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g798 / temp_output_8_0_g798 ) ) * 1.0 ) ) ) ) + ( _Color4 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g797 - 1.0 ) / temp_output_7_0_g797 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g797 / temp_output_7_0_g797 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g797 - 1.0 ) / temp_output_8_0_g797 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g797 / temp_output_8_0_g797 ) ) * 1.0 ) ) ) ) ) + ( ( _Color5 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g791 - 1.0 ) / temp_output_7_0_g791 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g791 / temp_output_7_0_g791 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g791 - 1.0 ) / temp_output_8_0_g791 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g791 / temp_output_8_0_g791 ) ) * 1.0 ) ) ) ) + ( _Color6 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g794 - 1.0 ) / temp_output_7_0_g794 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g794 / temp_output_7_0_g794 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g794 - 1.0 ) / temp_output_8_0_g794 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g794 / temp_output_8_0_g794 ) ) * 1.0 ) ) ) ) + ( _Color7 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g790 - 1.0 ) / temp_output_7_0_g790 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g790 / temp_output_7_0_g790 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g790 - 1.0 ) / temp_output_8_0_g790 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g790 / temp_output_8_0_g790 ) ) * 1.0 ) ) ) ) + ( _Color8 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g784 - 1.0 ) / temp_output_7_0_g784 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g784 / temp_output_7_0_g784 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g784 - 1.0 ) / temp_output_8_0_g784 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g784 / temp_output_8_0_g784 ) ) * 1.0 ) ) ) ) ) + ( ( _Color9 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g795 - 1.0 ) / temp_output_7_0_g795 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g795 / temp_output_7_0_g795 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g795 - 1.0 ) / temp_output_8_0_g795 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g795 / temp_output_8_0_g795 ) ) * 1.0 ) ) ) ) + ( _Color10 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g800 - 1.0 ) / temp_output_7_0_g800 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g800 / temp_output_7_0_g800 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g800 - 1.0 ) / temp_output_8_0_g800 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g800 / temp_output_8_0_g800 ) ) * 1.0 ) ) ) ) + ( _Color11 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g799 - 1.0 ) / temp_output_7_0_g799 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g799 / temp_output_7_0_g799 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g799 - 1.0 ) / temp_output_8_0_g799 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g799 / temp_output_8_0_g799 ) ) * 1.0 ) ) ) ) + ( _Color12 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g793 - 1.0 ) / temp_output_7_0_g793 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g793 / temp_output_7_0_g793 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g793 - 1.0 ) / temp_output_8_0_g793 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g793 / temp_output_8_0_g793 ) ) * 1.0 ) ) ) ) ) + ( ( _Color13 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g786 - 1.0 ) / temp_output_7_0_g786 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g786 / temp_output_7_0_g786 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g786 - 1.0 ) / temp_output_8_0_g786 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g786 / temp_output_8_0_g786 ) ) * 1.0 ) ) ) ) + ( _Color14 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g792 - 1.0 ) / temp_output_7_0_g792 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g792 / temp_output_7_0_g792 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g792 - 1.0 ) / temp_output_8_0_g792 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g792 / temp_output_8_0_g792 ) ) * 1.0 ) ) ) ) + ( _Color15 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g789 - 1.0 ) / temp_output_7_0_g789 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g789 / temp_output_7_0_g789 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g789 - 1.0 ) / temp_output_8_0_g789 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g789 / temp_output_8_0_g789 ) ) * 1.0 ) ) ) ) + ( _Color16 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g787 - 1.0 ) / temp_output_7_0_g787 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g787 / temp_output_7_0_g787 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g787 - 1.0 ) / temp_output_8_0_g787 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g787 / temp_output_8_0_g787 ) ) * 1.0 ) ) ) ) ) );
			float4 clampResult348 = clamp( ( pow( (( ( tex2D( _Gradient, uv_TexCoord256 ) + _GradientColor ) + ( 1.0 - _GradientIntensity ) )*_GradientScale + _GradientOffset) , temp_cast_0 ) + ( 1.0 - (temp_output_329_0).a ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			o.Albedo = ( clampResult348 * temp_output_329_0 * _Tint ).rgb;
			float temp_output_3_0_g812 = 1.0;
			float temp_output_7_0_g812 = 4.0;
			float temp_output_9_0_g812 = 4.0;
			float temp_output_8_0_g812 = 4.0;
			float temp_output_3_0_g807 = 2.0;
			float temp_output_7_0_g807 = 4.0;
			float temp_output_9_0_g807 = 4.0;
			float temp_output_8_0_g807 = 4.0;
			float temp_output_3_0_g811 = 3.0;
			float temp_output_7_0_g811 = 4.0;
			float temp_output_9_0_g811 = 4.0;
			float temp_output_8_0_g811 = 4.0;
			float temp_output_3_0_g815 = 4.0;
			float temp_output_7_0_g815 = 4.0;
			float temp_output_9_0_g815 = 4.0;
			float temp_output_8_0_g815 = 4.0;
			float temp_output_3_0_g809 = 1.0;
			float temp_output_7_0_g809 = 4.0;
			float temp_output_9_0_g809 = 3.0;
			float temp_output_8_0_g809 = 4.0;
			float temp_output_3_0_g810 = 2.0;
			float temp_output_7_0_g810 = 4.0;
			float temp_output_9_0_g810 = 3.0;
			float temp_output_8_0_g810 = 4.0;
			float temp_output_3_0_g808 = 3.0;
			float temp_output_7_0_g808 = 4.0;
			float temp_output_9_0_g808 = 3.0;
			float temp_output_8_0_g808 = 4.0;
			float temp_output_3_0_g813 = 4.0;
			float temp_output_7_0_g813 = 4.0;
			float temp_output_9_0_g813 = 3.0;
			float temp_output_8_0_g813 = 4.0;
			float temp_output_3_0_g822 = 1.0;
			float temp_output_7_0_g822 = 4.0;
			float temp_output_9_0_g822 = 2.0;
			float temp_output_8_0_g822 = 4.0;
			float temp_output_3_0_g818 = 2.0;
			float temp_output_7_0_g818 = 4.0;
			float temp_output_9_0_g818 = 2.0;
			float temp_output_8_0_g818 = 4.0;
			float temp_output_3_0_g817 = 3.0;
			float temp_output_7_0_g817 = 4.0;
			float temp_output_9_0_g817 = 2.0;
			float temp_output_8_0_g817 = 4.0;
			float temp_output_3_0_g820 = 4.0;
			float temp_output_7_0_g820 = 4.0;
			float temp_output_9_0_g820 = 2.0;
			float temp_output_8_0_g820 = 4.0;
			float temp_output_3_0_g821 = 1.0;
			float temp_output_7_0_g821 = 4.0;
			float temp_output_9_0_g821 = 1.0;
			float temp_output_8_0_g821 = 4.0;
			float temp_output_3_0_g819 = 2.0;
			float temp_output_7_0_g819 = 4.0;
			float temp_output_9_0_g819 = 1.0;
			float temp_output_8_0_g819 = 4.0;
			float temp_output_3_0_g814 = 3.0;
			float temp_output_7_0_g814 = 4.0;
			float temp_output_9_0_g814 = 1.0;
			float temp_output_8_0_g814 = 4.0;
			float temp_output_3_0_g816 = 4.0;
			float temp_output_7_0_g816 = 4.0;
			float temp_output_9_0_g816 = 1.0;
			float temp_output_8_0_g816 = 4.0;
			float4 temp_output_344_0 = ( ( ( _MRE1 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g812 - 1.0 ) / temp_output_7_0_g812 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g812 / temp_output_7_0_g812 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g812 - 1.0 ) / temp_output_8_0_g812 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g812 / temp_output_8_0_g812 ) ) * 1.0 ) ) ) ) + ( _MRE2 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g807 - 1.0 ) / temp_output_7_0_g807 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g807 / temp_output_7_0_g807 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g807 - 1.0 ) / temp_output_8_0_g807 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g807 / temp_output_8_0_g807 ) ) * 1.0 ) ) ) ) + ( _MRE3 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g811 - 1.0 ) / temp_output_7_0_g811 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g811 / temp_output_7_0_g811 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g811 - 1.0 ) / temp_output_8_0_g811 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g811 / temp_output_8_0_g811 ) ) * 1.0 ) ) ) ) + ( _MRE4 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g815 - 1.0 ) / temp_output_7_0_g815 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g815 / temp_output_7_0_g815 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g815 - 1.0 ) / temp_output_8_0_g815 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g815 / temp_output_8_0_g815 ) ) * 1.0 ) ) ) ) ) + ( ( _MRE5 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g809 - 1.0 ) / temp_output_7_0_g809 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g809 / temp_output_7_0_g809 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g809 - 1.0 ) / temp_output_8_0_g809 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g809 / temp_output_8_0_g809 ) ) * 1.0 ) ) ) ) + ( _MRE6 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g810 - 1.0 ) / temp_output_7_0_g810 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g810 / temp_output_7_0_g810 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g810 - 1.0 ) / temp_output_8_0_g810 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g810 / temp_output_8_0_g810 ) ) * 1.0 ) ) ) ) + ( _MRE7 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g808 - 1.0 ) / temp_output_7_0_g808 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g808 / temp_output_7_0_g808 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g808 - 1.0 ) / temp_output_8_0_g808 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g808 / temp_output_8_0_g808 ) ) * 1.0 ) ) ) ) + ( _MRE8 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g813 - 1.0 ) / temp_output_7_0_g813 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g813 / temp_output_7_0_g813 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g813 - 1.0 ) / temp_output_8_0_g813 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g813 / temp_output_8_0_g813 ) ) * 1.0 ) ) ) ) ) + ( ( _MRE9 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g822 - 1.0 ) / temp_output_7_0_g822 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g822 / temp_output_7_0_g822 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g822 - 1.0 ) / temp_output_8_0_g822 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g822 / temp_output_8_0_g822 ) ) * 1.0 ) ) ) ) + ( _MRE10 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g818 - 1.0 ) / temp_output_7_0_g818 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g818 / temp_output_7_0_g818 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g818 - 1.0 ) / temp_output_8_0_g818 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g818 / temp_output_8_0_g818 ) ) * 1.0 ) ) ) ) + ( _MRE11 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g817 - 1.0 ) / temp_output_7_0_g817 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g817 / temp_output_7_0_g817 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g817 - 1.0 ) / temp_output_8_0_g817 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g817 / temp_output_8_0_g817 ) ) * 1.0 ) ) ) ) + ( _MRE12 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g820 - 1.0 ) / temp_output_7_0_g820 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g820 / temp_output_7_0_g820 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g820 - 1.0 ) / temp_output_8_0_g820 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g820 / temp_output_8_0_g820 ) ) * 1.0 ) ) ) ) ) + ( ( _MRE13 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g821 - 1.0 ) / temp_output_7_0_g821 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g821 / temp_output_7_0_g821 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g821 - 1.0 ) / temp_output_8_0_g821 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g821 / temp_output_8_0_g821 ) ) * 1.0 ) ) ) ) + ( _MRE14 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g819 - 1.0 ) / temp_output_7_0_g819 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g819 / temp_output_7_0_g819 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g819 - 1.0 ) / temp_output_8_0_g819 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g819 / temp_output_8_0_g819 ) ) * 1.0 ) ) ) ) + ( _MRE15 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g814 - 1.0 ) / temp_output_7_0_g814 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g814 / temp_output_7_0_g814 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g814 - 1.0 ) / temp_output_8_0_g814 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g814 / temp_output_8_0_g814 ) ) * 1.0 ) ) ) ) + ( _MRE16 * ( ( ( 1.0 - step( i.uv_texcoord.x , ( ( temp_output_3_0_g816 - 1.0 ) / temp_output_7_0_g816 ) ) ) * ( step( i.uv_texcoord.x , ( temp_output_3_0_g816 / temp_output_7_0_g816 ) ) * 1.0 ) ) * ( ( 1.0 - step( i.uv_texcoord.y , ( ( temp_output_9_0_g816 - 1.0 ) / temp_output_8_0_g816 ) ) ) * ( step( i.uv_texcoord.y , ( temp_output_9_0_g816 / temp_output_8_0_g816 ) ) * 1.0 ) ) ) ) ) );
			o.Emission = ( temp_output_329_0 * ( _EmissionPower * (temp_output_344_0).b ) ).rgb;
			o.Metallic = (temp_output_344_0).r;
			o.Smoothness = ( 1.0 - (temp_output_344_0).g );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-96;161;1600;777;-434.0838;546.4365;1.653484;True;False
Node;AmplifyShaderEditor.CommentaryNode;366;535.9316,-229.9137;Inherit;False;2485.59;618.0679;Gradiendt;16;256;270;259;264;292;289;295;326;325;338;341;340;342;343;347;348;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;361;-759.6837,1065.365;Inherit;False;570.2268;892.4404;Row 4;8;273;283;287;266;293;282;258;260;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;360;-1403.199,1034.025;Inherit;False;564.674;897.8647;Row 3;8;277;276;281;279;269;263;268;274;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;359;-1403.314,83.42817;Inherit;False;578.765;900.6964;Row 1;8;288;286;285;290;271;261;275;265;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;358;-754.3921,79.81281;Inherit;False;570.5063;932.3542;Row 2;8;284;291;280;278;262;267;272;257;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;266;-707.4464,1526.003;Float;False;Property;_Color15;Color 15;15;0;Create;True;0;0;0;False;0;False;1,0,0,0.391;0.2841758,0.4203553,0.5849056,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;263;-1349.037,1505.674;Float;False;Property;_Color11;Color 11;11;0;Create;True;0;0;0;False;0;False;0.6691177,0.6691177,0.6691177,0.647;0.427451,0.6941177,1,0.291;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;269;-1353.199,1714.419;Float;False;Property;_Color12;Color 12;12;0;Create;True;0;0;0;False;0;False;0.5073529,0.1574544,0,0.128;0.1425774,0.1937784,0.3396226,0.128;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;271;-1360.466,759.4822;Float;False;Property;_Color4;Color 4;4;0;Create;True;0;0;0;False;0;False;0.1544118,0.5451319,1,0.253;0.5943396,0.4843941,0.2495104,0.8941177;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;260;-711.3392,1318.291;Float;False;Property;_Color14;Color 14;14;0;Create;True;0;0;0;False;0;False;0,0.8025862,0.875,0.047;0.3411764,0.4078431,0.4823529,0.047;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;258;-697.8932,1107.106;Float;False;Property;_Color13;Color 13;13;0;Create;True;0;0;0;False;1;Space(10);False;1,0.5586207,0,0.272;0.6037736,0.4733819,0.2591669,0.272;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;257;-722.6031,131.1891;Float;False;Property;_Color5;Color 5;5;0;Create;True;0;0;0;False;1;Space(10);False;0.9533468,1,0.1544118,0.553;0.7924528,0.6783531,0.3551085,0.684;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;273;-700.0488,1737.502;Float;False;Property;_Color16;Color 16;16;0;Create;True;0;0;0;False;0;False;0.4080882,0.75,0.4811866,0.134;0.4196078,0.1679361,0.1372549,0.1294118;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;275;-1349.185,341.859;Float;False;Property;_Color2;Color 2;2;0;Create;True;0;0;0;False;0;False;1,0.1544118,0.8017241,0.253;0.1735509,0.1759028,0.2205881,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;265;-1345.374,133.4282;Float;False;Property;_Color1;Color 1;1;0;Create;True;0;0;0;False;1;Header(Albedo (A Gradient));False;1,0.1544118,0.1544118,0.291;0.1174795,0.1211977,0.1509433,0.378;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;267;-736.2856,561.0974;Float;False;Property;_Color7;Color 7;7;0;Create;True;0;0;0;False;0;False;0.1544118,0.6151115,1,0.178;0.5,0.3857653,0.1627358,0.6745098;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;262;-723.931,778.1015;Float;False;Property;_Color8;Color 8;8;0;Create;True;0;0;0;False;0;False;0.4849697,0.5008695,0.5073529,0.078;0.8301887,0.5880298,0.2388749,0.7450981;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;274;-1344.989,1084.025;Float;False;Property;_Color9;Color 9;9;0;Create;True;0;0;0;False;1;Space(10);False;0.3164301,0,0.7058823,0.134;0.2459991,0.4415089,0.6691177,0.134;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;268;-1347.424,1291.078;Float;False;Property;_Color10;Color 10;10;0;Create;True;0;0;0;False;0;False;0.362069,0.4411765,0,0.759;0.3020112,0.31177,0.4191176,0.291;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;261;-1354.927,549.5719;Float;False;Property;_Color3;Color 3;3;0;Create;True;0;0;0;False;0;False;0.2535501,0.1544118,1,0.541;0.4694286,0.6760256,0.7264151,0.4666667;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;272;-727.791,345.1252;Float;False;Property;_Color6;Color 6;6;0;Create;True;0;0;0;False;0;False;0.2720588,0.1294625,0,0.097;0.9333334,0.7058823,0.2196078,0.682353;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;256;585.9316,-153.5259;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,4;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;279;-1109.104,1084.737;Inherit;True;ColorShartSlot;-1;;795;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;285;-1109.17,343.947;Inherit;True;ColorShartSlot;-1;;796;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;288;-1118.527,760.4057;Inherit;True;ColorShartSlot;-1;;797;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;286;-1114.912,551.6597;Inherit;True;ColorShartSlot;-1;;798;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;364;-1367.103,2769.069;Inherit;False;467.2708;742.0146;Comment;8;332;298;327;301;310;320;331;306;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;363;-893.7528,2026.784;Inherit;False;466.332;731.8296;Comment;8;318;312;311;334;304;319;322;309;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;362;-1382.415,2014.24;Inherit;False;460.2247;730.0024;Comment;8;321;314;323;308;333;305;324;313;;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;281;-1111.539,1291.79;Inherit;True;ColorShartSlot;-1;;800;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;290;-1105.359,135.516;Inherit;True;ColorShartSlot;-1;;801;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;280;-479.5176,345.8368;Inherit;True;ColorShartSlot;-1;;794;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;276;-1113.152,1506.385;Inherit;True;ColorShartSlot;-1;;799;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;277;-1115.39,1715.131;Inherit;True;ColorShartSlot;-1;;793;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;283;-472.4226,1740.968;Inherit;True;ColorShartSlot;-1;;787;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;259;830.0755,-179.9137;Inherit;True;Property;_Gradient;Gradient;34;1;[SingleLineTexture];Create;True;0;0;0;False;1;Header(Gradient);False;-1;0f424a347039ef447a763d3d4b4782b0;0f424a347039ef447a763d3d4b4782b0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;365;-888.8892,2792.783;Inherit;False;475.3564;744.249;Comment;8;330;300;316;294;297;317;315;307;;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;278;-474.3295,131.9009;Inherit;True;ColorShartSlot;-1;;791;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;291;-488.0124,561.8089;Inherit;True;ColorShartSlot;-1;;790;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;287;-470.1839,1530.844;Inherit;True;ColorShartSlot;-1;;789;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;293;-473.0185,1107.818;Inherit;True;ColorShartSlot;-1;;786;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;284;-491.627,781.5659;Inherit;True;ColorShartSlot;-1;;784;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;264;844.9053,47.40545;Float;False;Property;_GradientColor;Gradient Color;36;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;282;-474.0769,1320.972;Inherit;True;ColorShartSlot;-1;;792;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;270;816.5891,230.7126;Float;False;Property;_GradientIntensity;Gradient Intensity;35;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;311;-869.2375,2410.001;Float;False;Property;_MRE7;MRE 7;23;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;298;-1349.235,3336.623;Float;False;Property;_MRE12;MRE 12;28;0;Create;True;0;0;0;False;0;False;0,1,0,0;0.6941177,0.4627448,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;302;-54.34647,165.9581;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;305;-1363.989,2223.768;Float;False;Property;_MRE2;MRE 2;18;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;303;-48.29223,531.2434;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;308;-1363.03,2395.617;Float;False;Property;_MRE3;MRE 3;19;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;312;-875.2134,2580.248;Float;False;Property;_MRE8;MRE 8;24;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;299;-82.66957,1179.32;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;296;-91.42815,1577.66;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;313;-1365.449,2057.625;Float;False;Property;_MRE1;MRE 1;17;0;Create;True;0;0;0;False;1;Header(Metallic(R) Rough(G) Emmission(B));False;0,1,0,0;0.7647059,0.8941177,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;309;-874.2711,2070.23;Float;False;Property;_MRE5;MRE 5;21;0;Create;True;0;0;0;False;1;Space(10);False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;314;-1360.115,2564.694;Float;False;Property;_MRE4;MRE 4;20;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;289;1152.971,-154.4466;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;307;-865.3842,2837.277;Float;False;Property;_MRE13;MRE 13;29;0;Create;True;0;0;0;False;1;Space(10);False;0,1,0,0;0.4745098,0.4666665,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;304;-873.5535,2240.293;Float;False;Property;_MRE6;MRE 6;22;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;294;-869.8051,3185.809;Float;False;Property;_MRE15;MRE 15;31;0;Create;True;0;0;0;False;0;False;0,1,0,0;0.6745098,0.737255,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;301;-1354.192,3164.495;Float;False;Property;_MRE11;MRE 11;27;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;306;-1356.103,2816.068;Float;False;Property;_MRE9;MRE 9;25;0;Create;True;0;0;0;False;1;Space(10);False;0,1,0,0;0.490196,0.1921568,0.4549019,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;297;-863.7613,3009.661;Float;False;Property;_MRE14;MRE 14;30;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;300;-864.2806,3358.549;Float;False;Property;_MRE16;MRE 16;32;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;292;1127.454,149.8354;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;310;-1352.963,2989.84;Float;False;Property;_MRE10;MRE 10;26;0;Create;True;0;0;0;False;0;False;0,1,0,0;0.7098039,0,0.3254901,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;320;-1150.589,2985.199;Inherit;False;ColorShartSlot;-1;;818;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;2;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;317;-653.8816,3010.149;Inherit;False;ColorShartSlot;-1;;819;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;2;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;329;1974.697,794.1887;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;326;1351.87,149.2626;Float;False;Property;_GradientOffset;Gradient Offset;38;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;331;-1149.852,2816.534;Inherit;False;ColorShartSlot;-1;;822;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;1;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;325;1345.052,56.51933;Float;False;Property;_GradientScale;Gradient Scale;37;0;Create;True;0;0;0;False;0;False;1;0.89;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;327;-1143.916,3167.113;Inherit;False;ColorShartSlot;-1;;817;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;3;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;315;-653.6279,2841.871;Inherit;False;ColorShartSlot;-1;;821;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;1;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;330;-648.4551,3359.667;Inherit;False;ColorShartSlot;-1;;816;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;4;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;332;-1138.915,3333.611;Inherit;False;ColorShartSlot;-1;;820;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;4;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;316;-649.3379,3180.557;Inherit;False;ColorShartSlot;-1;;814;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;3;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;318;-670.3987,2573.107;Inherit;False;ColorShartSlot;-1;;813;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;4;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;324;-1153.393,2059.979;Inherit;False;ColorShartSlot;-1;;812;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;1;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;323;-1159.554,2399.387;Inherit;False;ColorShartSlot;-1;;811;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;3;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;319;-667.2534,2239.941;Inherit;False;ColorShartSlot;-1;;810;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;2;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;322;-668.5063,2071.817;Inherit;False;ColorShartSlot;-1;;809;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;1;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;334;-666.5795,2410.909;Inherit;False;ColorShartSlot;-1;;808;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;3;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;333;-1162.682,2228.395;Inherit;False;ColorShartSlot;-1;;807;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;2;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;295;1338.27,-165.6665;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;321;-1159.586,2570.461;Inherit;False;ColorShartSlot;-1;;815;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;4;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;340;2017.051,104.5139;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;341;1803.245,-133.4551;Inherit;True;3;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;338;1356.361,245.6984;Float;False;Property;_GradientPower;Gradient Power;39;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;337;-336.3689,2047.395;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;335;-341.6371,3294.449;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;336;-313.9554,2830.794;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;339;-362.0783,2545.693;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;342;2110.79,-134.1479;Inherit;True;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;343;2239.635,102.8223;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;344;1530.555,1323.444;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;345;1605.128,1601.318;Inherit;False;Property;_EmissionPower;Emission Power;33;0;Create;True;0;0;0;False;1;Header(Emmision);False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;346;1589.24,1688.918;Inherit;False;False;False;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;347;2403.432,-135.4491;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;348;2669.966,-138.4495;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;349;2205.509,1022.676;Inherit;True;False;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;357;2246.987,613.945;Float;False;Property;_Tint;Tint;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;350;1814.294,1610.711;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;355;2250.938,1415.226;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;353;2507.258,795.3847;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;354;1974.363,1019.803;Inherit;True;True;False;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;351;2447.02,1017.897;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3722.291,1294.892;Float;False;True;-1;7;ASEMaterialInspector;0;0;Standard;Malbers/Color4x4;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.1;True;True;0;False;Opaque;;Geometry;ForwardOnly;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;279;38;274;0
WireConnection;285;38;275;0
WireConnection;288;38;271;0
WireConnection;286;38;261;0
WireConnection;281;38;268;0
WireConnection;290;38;265;0
WireConnection;280;38;272;0
WireConnection;276;38;263;0
WireConnection;277;38;269;0
WireConnection;283;38;273;0
WireConnection;259;1;256;0
WireConnection;278;38;257;0
WireConnection;291;38;267;0
WireConnection;287;38;266;0
WireConnection;293;38;258;0
WireConnection;284;38;262;0
WireConnection;282;38;260;0
WireConnection;302;0;290;0
WireConnection;302;1;285;0
WireConnection;302;2;286;0
WireConnection;302;3;288;0
WireConnection;303;0;278;0
WireConnection;303;1;280;0
WireConnection;303;2;291;0
WireConnection;303;3;284;0
WireConnection;299;0;279;0
WireConnection;299;1;281;0
WireConnection;299;2;276;0
WireConnection;299;3;277;0
WireConnection;296;0;293;0
WireConnection;296;1;282;0
WireConnection;296;2;287;0
WireConnection;296;3;283;0
WireConnection;289;0;259;0
WireConnection;289;1;264;0
WireConnection;292;0;270;0
WireConnection;320;38;310;0
WireConnection;317;38;297;0
WireConnection;329;0;302;0
WireConnection;329;1;303;0
WireConnection;329;2;299;0
WireConnection;329;3;296;0
WireConnection;331;38;306;0
WireConnection;327;38;301;0
WireConnection;315;38;307;0
WireConnection;330;38;300;0
WireConnection;332;38;298;0
WireConnection;316;38;294;0
WireConnection;318;38;312;0
WireConnection;324;38;313;0
WireConnection;323;38;308;0
WireConnection;319;38;304;0
WireConnection;322;38;309;0
WireConnection;334;38;311;0
WireConnection;333;38;305;0
WireConnection;295;0;289;0
WireConnection;295;1;292;0
WireConnection;321;38;314;0
WireConnection;340;0;329;0
WireConnection;341;0;295;0
WireConnection;341;1;325;0
WireConnection;341;2;326;0
WireConnection;337;0;324;0
WireConnection;337;1;333;0
WireConnection;337;2;323;0
WireConnection;337;3;321;0
WireConnection;335;0;315;0
WireConnection;335;1;317;0
WireConnection;335;2;316;0
WireConnection;335;3;330;0
WireConnection;336;0;331;0
WireConnection;336;1;320;0
WireConnection;336;2;327;0
WireConnection;336;3;332;0
WireConnection;339;0;322;0
WireConnection;339;1;319;0
WireConnection;339;2;334;0
WireConnection;339;3;318;0
WireConnection;342;0;341;0
WireConnection;342;1;338;0
WireConnection;343;0;340;0
WireConnection;344;0;337;0
WireConnection;344;1;339;0
WireConnection;344;2;336;0
WireConnection;344;3;335;0
WireConnection;346;0;344;0
WireConnection;347;0;342;0
WireConnection;347;1;343;0
WireConnection;348;0;347;0
WireConnection;349;0;344;0
WireConnection;350;0;345;0
WireConnection;350;1;346;0
WireConnection;355;0;349;0
WireConnection;353;0;348;0
WireConnection;353;1;329;0
WireConnection;353;2;357;0
WireConnection;354;0;344;0
WireConnection;351;0;329;0
WireConnection;351;1;350;0
WireConnection;0;0;353;0
WireConnection;0;2;351;0
WireConnection;0;3;354;0
WireConnection;0;4;355;0
ASEEND*/
//CHKSM=48C16ADEE5CBAE96C24DA5621D4569F16FDB26D2