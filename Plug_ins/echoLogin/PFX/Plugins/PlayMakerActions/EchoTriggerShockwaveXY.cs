#if false
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("echoLogin PFX")]
	[Tooltip("Trigger PostFX.")]
	public class EchoTriggerShockwaveXY : FsmStateAction
	{
		[RequiredField]
        [Tooltip("Render Group Name")]
		public FsmString renderGroupName;
		
		[RequiredField]
		[Tooltip("Effect Name")]
		public FsmString effectName;

		[RequiredField]
		[Tooltip("Effect Time Scale")]
		public FsmFloat effectTimeScale = 1.0f;

		[RequiredField]
		[Tooltip("Center X of Shockwave")]
		public FsmFloat effectCenterX = 0;

		[RequiredField]
		[Tooltip("Center Y of Shockwave")]
		public FsmFloat effectCenterY = 0;

		private EchoPFX _fxID;

		public override void Awake ()
		{
			_fxID = new EchoPFX ( renderGroupName.Value, effectName.Value );
		}

		public override void Reset()
		{
			renderGroupName = "";
			effectName = "";
			effectTimeScale = 1.0f;
		}

		public override void OnEnter()
		{
//			_fxID.ShockWaveCenter ( 0, effectCenterX.Value / (float)Screen.width, effectCenterY.Value / (float)Screen.height );
			_fxID.ShockWaveCenter ( 0, effectCenterX.Value , effectCenterY.Value  );
			_fxID.Start (effectTimeScale.Value);
			Finish();
		}
	}
}
#endif
