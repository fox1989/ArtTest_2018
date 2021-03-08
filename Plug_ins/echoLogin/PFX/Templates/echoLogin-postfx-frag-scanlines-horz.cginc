#if ECHO_PFX_SCANLINE_ON
				fixed scanval =  (fixed) ( (int)( v.tc1.y * _echoScanLineCountH + ( _Time.y * _echoScanLineScrollH ) ) % 2 );
				_ioRGB.xyz *= lerp ( fixed3(1,1,1), fixed3 ( scanval, scanval, scanval ), _echoScanLineFade );
#endif 


