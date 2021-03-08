#ifdef ECHO_PFX_OVERLAY_OVR_ON
			fixed4 svcolor 		= tex2D ( _OverlayTexOvr, ECHODEF_OVERLAY_OVR_TC );
			fixed3 svoverlay 	= lerp ( 2.0 * _ioRGB.xyz * svcolor.xyz, 1.0 - 2.0 * ( 1.0 - _ioRGB.xyz)*( 1.0 - svcolor.xyz), clamp ( sign ( _ioRGB.xyz - fixed3(0.5,0.5,0.5) ), 0.0, 1.0 ) );
			_ioRGB.xyz 			= lerp ( _ioRGB.xyz, svoverlay.xyz, svcolor.a * _echoOverlayOvrFade );
		
//			fixed3 svoverlay 	= lerp ( _ioRGB.xyz * svcolor.xyz, svcolor.xyz + _ioRGB.xyz - _ioRGB.xyz * svcolor.xyz, svmix );
//			_ioRGB.xyz 			= lerp ( _ioRGB.xyz, svoverlay.xyz, svcolor.a * _echoOverlayOvrFade );
#endif

