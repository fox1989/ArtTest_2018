#if ECHO_PFX_SCANLINE_ON
				fixed vscanval =  (fixed) ( (int)( v.tc1.x * _echoScanLineCountV + ( _Time.y * _echoScanLineScrollV ) ) % 2 );
				_ioRGB.xyz *= lerp ( fixed3(1,1,1), fixed3 ( vscanval, vscanval, vscanval ), _echoScanLineFade );
#endif 


