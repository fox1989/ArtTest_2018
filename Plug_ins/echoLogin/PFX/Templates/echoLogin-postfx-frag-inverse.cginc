#ifdef ECHO_PFX_INVERSE_ON
	   			_ioRGB.xyz = lerp ( _ioRGB.xyz, ( fixed3(1,1,1) - _ioRGB.xyz), _echoInverseFade );
#endif

