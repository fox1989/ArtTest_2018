#ifdef ECHO_PFX_MULT_ON
				_ioRGB.xyz *= lerp ( fixed3 ( 1, 1, 1 ), _ioRGB.xyz * _echoMult.xyz, _echoMultFade );
#endif

