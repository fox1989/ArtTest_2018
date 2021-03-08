#pragma strict

var rainObj : GameObject;
var rainCloseObj : GameObject;

function Start () 
{

}

function Update () 
{
	if(rainObj)
	{
		rainObj.transform.position.x = gameObject.transform.position.x;
		rainObj.transform.position.z = gameObject.transform.position.z;
	}
	
	if(rainCloseObj)
	{
		rainCloseObj.transform.position.x = gameObject.transform.position.x;
		rainCloseObj.transform.position.z = gameObject.transform.position.z;
	}
}