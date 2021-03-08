#ifdef ECHO_PFX_CORRECT_ON
				_ioRGB = lerp ( _ioRGB, fixed3 ( tex2D ( _ColorCorrectTex, _ioRGB.xx ).x, tex2D ( _ColorCorrectTex, _ioRGB.yy ).y, tex2D ( _ColorCorrectTex, _ioRGB.zz ).z ), _echoCorrectFade );
#endif


