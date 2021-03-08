#ifdef ECHO_PFX_RAMP_ON
				fixed  inten 	= ( _ioRGB.x + _ioRGB.y + _ioRGB.z ) * 0.33333;
    			fixed3 tcolor 	= tex2D ( _echoRampTex, fixed2 ( inten, inten ) ).xyz;
				_ioRGB 			= lerp ( _ioRGB, tcolor, _echoRampFade );
#endif
