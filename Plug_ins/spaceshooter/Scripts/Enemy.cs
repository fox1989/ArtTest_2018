using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{
	#region Fields
	public float MinSpeed;
	public float MaxSpeed;
	
	private float currentSpeed;
	private float x, y, z;
	
	#endregion
	
	#region Properties
	
	#endregion
	
	#region Functions
	void Start () 
	{
		SetPositionAndSpeed();
	}
	
	
	void Update () 
	{
		float amtToMove = currentSpeed * Time.deltaTime;
		transform.Translate(Vector3.down * amtToMove);
		
		if (transform.position.y <= -4.5)
		{
			SetPositionAndSpeed();
			Player.Missed++;
		}
	}
	
	 public void SetPositionAndSpeed()
	{
	currentSpeed = Random.RandomRange(MinSpeed, MaxSpeed);
			x = Random.RandomRange(-6f, 6f);
			y = 6.8f;
			z = 0.0f;
		
		transform.position = new Vector3(x, y, z);	
	}
	#endregion
}
