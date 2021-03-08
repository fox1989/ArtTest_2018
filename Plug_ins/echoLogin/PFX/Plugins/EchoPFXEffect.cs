// (c) copyright echoLogin LLC 2013. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class EchoPFXEffect
{
	public List<EchoPFXOption> 		passOpt1	= new List<EchoPFXOption>();
	public List<EchoPFXOption> 		passOpt2	= new List<EchoPFXOption>();
	public List<EchoPFXOption>[]	passOpt 	= new List<EchoPFXOption>[2]; 
	public bool[]                  	passActive 	= new bool[2];
	public string           		name;
	public bool             		active;
	public int                      optionsTotal;
	public int                      optionsOff;
	private EchoPFXRenderGroup 		_erg;
	private int                     _pass_high;
	private int                     _pass_low;
	private Dictionary<ECHOPFXOPTION,EchoPFXOption> _pass1Dict = new Dictionary<ECHOPFXOPTION,EchoPFXOption>();
	private Dictionary<ECHOPFXOPTION,EchoPFXOption> _pass2Dict = new Dictionary<ECHOPFXOPTION,EchoPFXOption>();
	private Dictionary<ECHOPFXOPTION,EchoPFXOption>[] _passDict;


	//=========================================================================
	public EchoPFXEffect()
	{
		active 		= false;
		name 		= "";
				
		passOpt[0] = passOpt1;
		passOpt[1] = passOpt2;
	}

	//=========================================================================
	public EchoPFXOption OptionGet ( ECHOPFXOPTION ipo, int ipass )
	{
		return ( _passDict[ipass][ipo] );
	}

	//=========================================================================
	public bool OptionTimingSet ( ECHOPFXOPTION ipo, int ipass, float istartDelay, float iattackTime, float isustainTime, float ireleaseTime )
	{
		EchoPFXOption epo;
		bool rc = false;

		epo = _passDict[ipass][ipo];
		if ( epo != null )
		{
			epo.DefaultTimingSet ( istartDelay, iattackTime, isustainTime, ireleaseTime );
			rc = true;
		}

		return ( rc );
	}
	
	//=========================================================================
	public void Start()
	{
		int index;

		optionsOff 		= 0;

		for ( int pass = _pass_low; pass < _pass_high; pass++ )
		{
			for ( index = 0; index < passOpt[pass].Count; index++ )
			{
				passOpt[pass][index].Start();
			}
		}
	}

	//=========================================================================
	public void Start( float iscale )
	{
		int index;

		optionsOff 		= 0;

		for ( int pass = _pass_low; pass < _pass_high; pass++ )
		{
			for ( index = 0; index < passOpt[pass].Count; index++ )
			{
				passOpt[pass][index].Start( iscale );
			}
		}
	}

	//=========================================================================
	public void Stop()
	{
		int index;
		
		for ( int pass = _pass_low; pass < _pass_high; pass++ )
		{
			for ( index = 0; index < passOpt[pass].Count; index++ )
			{
				passOpt[pass][index].Stop();
			}
		}
	}

	//=========================================================================
	public void Stop( float ireleaseTime )
	{
		int index;
		
		for ( int pass = _pass_low; pass < _pass_high; pass++ )
		{
			for ( index = 0; index < passOpt[pass].Count; index++ )
			{
				passOpt[pass][index].Stop( ireleaseTime );
			}
		}
	}

	//=========================================================================
	public void RemoveOptionOfType ( ECHOPFXOPTION iopt, int ipass )
	{
		int loop;

		passOpt[0] = passOpt1;
		passOpt[1] = passOpt2;

		for ( loop = passOpt[ipass].Count-1; loop >= 0; loop-- )
		{
			if ( passOpt[ipass][loop].optType == iopt )
			{
				passOpt[ipass].Remove ( passOpt[ipass][loop] );
			}
		}
	}

	//=========================================================================
	private int FindPassOptIndex ( int ipass, EchoPFXRenderGroup ierg, ECHOPFXOPTION iopt )
	{
		if ( ipass == 0 )
		{
			for ( int loop = 0; loop < ierg.possibleOpts1.Count; loop++ )
			{
				if ( ierg.possibleOpts1[loop].type == iopt )
				{
					return ( loop );
				}
			}
		}
		else
		{
			for ( int loop = 0; loop < ierg.possibleOpts2.Count; loop++ )
			{
				if ( ierg.possibleOpts2[loop].type == iopt )
				{
					return ( loop );
				}
			}
		}
		
		return ( -1 );
	}
	
	//=========================================================================
	public int  ValidateOptions ( EchoPFXRenderGroup ierg )
	{
		int total = 0;
		int loop;
		int pass;

		_erg = ierg;

		_passDict = new Dictionary<ECHOPFXOPTION,EchoPFXOption>[2];
		_passDict[0] = _pass1Dict;
		_passDict[1] = _pass2Dict;

		passOpt[0] = passOpt1;
		passOpt[1] = passOpt2;

		optionsOff 		= 0;
		optionsTotal 	= passOpt1.Count + passOpt2.Count;

		for ( pass = 0; pass < 2; pass++ )
		{
			for ( loop = 0; loop < passOpt[pass].Count; loop++ )
			{
				passOpt[pass][loop].passOrder =  FindPassOptIndex ( pass, _erg, passOpt[pass][loop].optType );
			}

			passOpt[pass].Sort ( delegate ( EchoPFXOption e1, EchoPFXOption e2 ) 
			{
				return ( e1.passOrder.CompareTo ( e2.passOrder ) );
			});
		}

		_passDict[0].Clear();
		_passDict[1].Clear();

		for ( pass = 0; pass < 2; pass++ )
		{
			for ( loop = 0; loop < passOpt[pass].Count; loop++ )
			{
				_passDict[pass].Add ( passOpt[pass][loop].optType, passOpt[pass][loop] );
			}
		}

		if ( passActive == null || passActive.Length < 1 )
			passActive 		= new bool[2];

		total = 0;
		if ( passOpt[0].Count > 0 )
		{
			passActive[0] 	= true;
			total++;
		}

		if ( passOpt[1].Count > 0 )
		{
			passActive[1] = true;
			total++;
		}

		if ( total == 2 )
		{
			_pass_low = 0;
			_pass_high = 2;
		}
		else
		{
			if ( passOpt[0].Count > 0 )
			{
				_pass_low = 0;
				_pass_high = 1;
			}
			else
			{
				_pass_low = 1;
				_pass_high = 2;
			}
		}

		return ( total );
	}

	//=========================================================================
	public int EffectPassCount ( int ipass )
	{
		return ( passOpt[ipass].Count );
	}

	//=========================================================================
	public void CopyTo ( EchoPFXEffect iepe )
	{
		int loop;
		EchoPFXOption epo;
		
		iepe.name 		= name;
		iepe.active	= active;

		for ( int pass = 0; pass < 2; pass++ )
		{
			for ( loop = 0; loop < passOpt[pass].Count; loop++ )
			{
				epo = passOpt[pass][loop];
				iepe.passOpt[pass][loop].CopyTo ( epo );
			}
		}
	}

};

