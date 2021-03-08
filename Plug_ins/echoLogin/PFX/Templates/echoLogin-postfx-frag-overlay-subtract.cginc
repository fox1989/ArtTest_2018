#ifdef ECHO_PFX_OVERLAY_SUB_ON
				fixed3 uocolor 	= tex2D ( _OverlayTexSub, ECHODEF_OVERLAY_SUB_TC ).xyz;
				// subtract
				_ioRGB.xyz 		= lerp ( _ioRGB.xyz, _ioRGB.xyz - uocolor.xyz, _echoOverlaySubFade );
#endif
