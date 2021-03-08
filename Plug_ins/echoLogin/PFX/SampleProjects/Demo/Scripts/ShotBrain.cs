using UnityEngine;
using System.Collections;

public class ShotBrain : MonoBehaviour 
{
	Transform cachedTras;
	Transform targetTras;
	float speed;

	//==================================================================================================
	public void OnTriggerEnter ()
	{
		Vector3 viewpoint = Camera.main.WorldToViewportPoint ( cachedTras.position );

		gameObject.SetActive(false);

		DemoMain.PlaySFX(6);

		DemoMain.StartShieldHit ( viewpoint.x, viewpoint.y );
	}

	//==================================================================================================
	public void Shoot ( Vector3 istartpos, Vector3 idir, Transform itargetTrans, float ispeed )
	{
		gameObject.SetActive ( true );

		cachedTras = transform;
		cachedTras.position = istartpos;
		cachedTras.rotation = Quaternion.LookRotation ( idir );

		targetTras = itargetTrans;

		speed = ispeed;

	}
	
	//==================================================================================================
	void Update ()
	{
		Quaternion rot;

		cachedTras.Translate ( Vector3.forward * Time.deltaTime * speed );

		rot = Quaternion.LookRotation ( targetTras.position - cachedTras.position );

		cachedTras.rotation = Quaternion.Slerp ( cachedTras.rotation, rot, Time.deltaTime * 0.5f );
	}
}
