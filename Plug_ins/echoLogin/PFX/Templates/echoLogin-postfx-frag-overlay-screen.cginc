#ifdef ECHO_PFX_OVERLAY_SCR_ON
				fixed3 socolor 	= tex2D ( _OverlayTexScr, ECHODEF_OVERLAY_SCR_TC ).xyz;
				// screen
				_ioRGB.xyz 		= lerp ( _ioRGB.xyz, socolor.xyz + _ioRGB.xyz - _ioRGB.xyz * socolor.xyz, _echoOverlayScrFade );
#endif
