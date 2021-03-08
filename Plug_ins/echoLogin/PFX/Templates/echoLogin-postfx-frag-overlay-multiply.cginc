#ifdef ECHO_PFX_OVERLAY_MUL_ON
				fixed3 mocolor 	= tex2D ( _OverlayTexMul, ECHODEF_OVERLAY_MUL_TC ).xyz;
				// multiply
				_ioRGB.xyz 		= lerp ( _ioRGB.xyz, _ioRGB.xyz * mocolor, _echoOverlayMulFade );
#endif
