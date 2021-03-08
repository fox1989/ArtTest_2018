#pragma strict

var playerObj : GameObject;
var mainLight : GameObject;

private var nextStrikeTimer : float = 0;

private var skyboxLightTimer : float = 0;

//-----------------------------------------------------

function Start () 
{
	nextStrikeTimer = Random.Range(2.0, 5.0);
}

//-----------------------------------------------------

function Update () 
{
	// strike timer
	if(nextStrikeTimer > 0)
	{
		nextStrikeTimer -= Time.deltaTime;
		if(nextStrikeTimer <= 0)
		{
			CreateNewLighting();
		}
	}
	
	// restore skybox tint timer
	if(skyboxLightTimer > 0)
	{
 		skyboxLightTimer -= Time.deltaTime;
 		if(skyboxLightTimer <= 0)
 		{
 			skyboxLightTimer = 0;
 			RenderSettings.skybox.SetColor("_Tint", Color(0.5,0.5,0.5));
 		}
	}
}

//-----------------------------------------------------------------------

function CreateNewLighting()
{
	// random position for the strike
	transform.rotation = playerObj.transform.rotation;	
	transform.position = playerObj.transform.position + // camera position
							Vector3(0, Random.Range(20, 30), 0) + // up in the air
							playerObj.transform.forward * Random.Range(50, 80) + // in front of the camera
							playerObj.transform.right * Random.Range(-20, 20); // random placement horizontally
										
	// change light intensity
	mainLight.animation.Rewind("light_intensity");
	mainLight.animation.Play("light_intensity");
		
	// play fade animation
	animation.Rewind("lighting");
	animation.Play("lighting");
	
	// light the skybox
	skyboxLightTimer = 0.1;
	RenderSettings.skybox.SetColor("_Tint", Color(1.0,1.0,1.0));
	
	// prepare next strike
	var rand:int = Random.Range(0,100);
	if(rand < 50)
	{
		// short amount of time to next strike
		nextStrikeTimer = Random.Range(0.1, 0.3);
	}
	else
	{
		// normal amount of time to next strike
		nextStrikeTimer = Random.Range(3.0, 6.0);
	}
}

//-----------------------------------------------------