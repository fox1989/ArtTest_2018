#ifdef ECHO_PFX_NOISE_ON
				fixed3 scolor 	= tex2D ( _NoiseTex, v.tc1.zw ).xyz;
				_ioRGB.xyz 		= lerp ( _ioRGB.xyz, scolor.xyz, _echoNoiseFade );
#endif

