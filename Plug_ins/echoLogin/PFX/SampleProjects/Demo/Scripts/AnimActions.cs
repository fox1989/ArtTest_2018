using UnityEngine;
using System.Collections;

public class AnimActions : MonoBehaviour
{
	private static EchoPFX _fxDistortion;
	private static EchoPFX _fxThermal;
	private static EchoPFX _fxNightVision;

	void Start()
	{
		_fxDistortion 	= new EchoPFX ( "RenderGroup:0","Distortion" );
		_fxThermal 		= new EchoPFX ( "RenderGroup:0","Thermal" );
		_fxNightVision 	= new EchoPFX ( "RenderGroup:0","NightVision" );
	}

	public void FlashScreenWhite()
	{
		_fxDistortion.Start();
	}

	public void DistortionStop()
	{
		_fxDistortion.Stop (3);
	}

	public void Thermal()
	{
		DemoMain.PlaySFX(7);
		_fxThermal.Start();
	}

	public void NightVision()
	{
		DemoMain.PlaySFX(7);
		_fxNightVision.Start();
	}

	public void Proceed()
	{
		DemoMain.PlaySFX(0);
	}

	public void Roger()
	{
		DemoMain.PlaySFX(2);
	}

	public void Never()
	{
		DemoMain.PlaySFX(3);
	}

	public void NeverComingBack()
	{
		DemoMain.PlaySFX(8);
	}

	public void Radiation()
	{
		DemoMain.PlaySFX(1);
	}

	public void Abort()
	{
		DemoMain.PlaySFX(5);
	}

	public void GoingBack()
	{
		DemoMain.PlaySFX(4);
	}

}
