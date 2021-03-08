// (c) copyright echoLogin LLC 2013. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EchoPFXAreaEffect 
{
	public float duration;   // < 0 one frame, 0 == forever, > 0 time
	public int   durType;
	public float time;       // time elapst
	public float intensity; // 0.0-1.0
	public int   opt;
	public int x1;
	public int y1;
	public int x2;
	public int y2;
	public static EchoList<EchoPFXAreaEffect> objs;
	
	//=========================================================================
	public static void Init( int inumobjs )
	{
		objs = new EchoList<EchoPFXAreaEffect>(inumobjs);
	}

	//=========================================================================
	public static EchoPFXAreaEffect GetFirst()
	{
		return ( objs.GetFirstActive() );
	}

	//=========================================================================
	public static EchoPFXAreaEffect GetNext( EchoPFXAreaEffect icurrent )
	{
		return ( objs.GetNext ( icurrent ) );
	}

	//=========================================================================
	public static EchoPFXAreaEffect GetFree ()
	{
		return objs.GetFree();
	}

	//=========================================================================
	public static void Active2Inactive ( EchoPFXAreaEffect ieobj )
	{
		objs.Deactivate ( ieobj );
	}
};
