#ifdef ECHO_PFX_GREYSCALE_ON
				fixed greycolor = ( _ioRGB.x + _ioRGB.y + _ioRGB.z ) * 0.3333;
	   			_ioRGB.xyz = lerp ( _ioRGB.xyz, fixed3 ( greycolor, greycolor, greycolor ), _echoGreyFade );
#endif
