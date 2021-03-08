#ifdef ECHO_PFX_OVERLAY_ON
				fixed4 ocolor 	= tex2D ( _OverlayTex, ECHODEF_OVERLAY_TC );
				_ioRGB.xyz 		= lerp ( _ioRGB.xyz, ocolor.xyz, _echoOverlayFade * ocolor.w );
#endif
