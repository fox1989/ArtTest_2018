// (c) copyright echoLogin LLC 2013. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

//---------------------------------------------------------------
public class EchoPFX
{
	public static List<EchoPFXRenderGroup> 	ergList;  // set by manager on start 
	private List<EchoPFXOption>[] _passOpt = new List<EchoPFXOption>[2]; 
	EchoPFXRenderGroup 						erg;
	EchoPFXEffect      						epe;

	//=========================================================================
	public EchoPFX ( string iergname, string ifxname )
	{
		int loop;

		erg = null;
		epe = null;

		if ( iergname != null && ifxname != null )
		{
			if ( ergList != null )
			{
				for ( loop = 0; loop < ergList.Count; loop++ )
				{
					if ( ergList[loop].name == iergname )
					{
						erg = ergList[loop];
						break;
					}
				}
				
				for ( loop = 0; loop < erg.epeList.Count; loop++ )
				{
					if ( erg.epeList[loop].name == ifxname )
					{
						epe = erg.epeList[loop];
						break;
					}
				}
				// look for special case shockwave option
				_passOpt[0] = epe.passOpt1; 
				_passOpt[1] = epe.passOpt2; 
			}
		}
	}

	//============================================================
	public void ShockWaveCenter ( int ipass, float icenterx, float icentery )
	{
		EchoPFXOption epo;

		epo = epe.OptionGet ( ECHOPFXOPTION.SHOCKWAVE, ipass );

		if ( epo != null )
		{
			epo.shockCenterX = icenterx;
			epo.shockCenterY = icentery;
		}
	}

	//=========================================================================
	public bool OptionTimingSet ( ECHOPFXOPTION ipo, int ipass, float istartDelay, float iattackTime, float isustainTime, float ireleaseTime )
	{
		return ( epe.OptionTimingSet ( ipo, ipass, istartDelay, iattackTime, isustainTime, ireleaseTime ) );
	}

	//=========================================================================
	public void Start()
	{
		erg.epeEchoList.Activate ( epe );
		epe.Start();
	}

	//=========================================================================
	public void Start( float iscale )
	{
		erg.epeEchoList.Activate ( epe );
		epe.Start ( iscale );
	}

	//=========================================================================
	public void Stop()
	{
		epe.Stop();
	}

	//=========================================================================
	public void Stop( float ireleaseTime )
	{
		epe.Stop(ireleaseTime);
	}

}

