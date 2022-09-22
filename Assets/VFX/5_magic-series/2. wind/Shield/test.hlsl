uniform vector HitPosition[20];
uniform vector HitSize[20];

float MyFunction_float(float AffectorAmount, float HitFadeDistance,float HitFadePower, float HitSperate,float HitNoise, Texture2D RampTex,SamplerState sampler_BaseMap, vector WorldPosition)
{
	float hit_result;

	for ( int i = 0;  i < AffectorAmount;  i++)
	{
		float distance_mask = distance(HitPosition[i], WorldPosition);
		float hit_range = clamp((distance_mask - HitSize[i] + HitNoise)/ HitSperate,1,0);
		float2 ramp_uv = float2(hit_range,0.5);
		float hit_wave = SAMPLE_TEXTURE2D(RampTex,sampler_BaseMap ,ramp_uv).r;
		float hit_fade = clamp((1.0 - distance_mask / HitFadeDistance) * HitFadePower,0,1);

		hit_result = hit_result + hit_fade * hit_wave;
	}

	return hit_result;
	// return saturate(hit_result);
}
