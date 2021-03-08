#ifdef ECHO_PFX_OVERLAY_ADD_ON
				fixed3 aocolor 	= tex2D ( _OverlayTexAdd, ECHODEF_OVERLAY_ADD_TC ).xyz;
				_ioRGB.xyz 		= _ioRGB.xyz += lerp ( fixed3(0,0,0), aocolor.xyz, _echoOverlayAddFade );
#endif