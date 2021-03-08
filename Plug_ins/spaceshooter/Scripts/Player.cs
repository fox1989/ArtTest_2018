using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	
	enum State
	{
		Playing,
		Explosion,
		Invincible
	}
	
	private State state = State.Playing;
	
	
	public float PlayerSpeed;
	public GameObject ProjectilePrefab;
	public GameObject ExplosionPrefab;
	
	public static int Score = 0;
	public static int Lives = 3;
	public static int Missed = 0;
	
	private float shipInvisibleTime = 1.5f;
	private float shipMoveOnToScreenSpeed = 5;
	private float blinkRate = .1f;
	private int numbersOfTimesToBlink = 10;
	private int blinkCount;
	
	// Update is called once per frame
	void Update () 
	{
		
		if (state != State.Explosion)
		{
		
		float amtToMove = Input.GetAxisRaw("Horizontal") * PlayerSpeed * Time.deltaTime;
		
		transform.Translate(Vector3.right * amtToMove);
		
		if (transform.position.x <= -7.3f)
			transform.position = new Vector3(7.3f, transform.position.y, transform.position.z);
		else if (transform.position.x >= 7.3f)
			transform.position = new Vector3(-7.3f, transform.position.y, transform.position.z);
		
		if (Input.GetKeyDown("space"))
		{
			Vector3 position = new Vector3 (transform.position.x, transform.position.y + (transform.localScale.y / 2));
			Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
		}
		
		}
	
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 120, 20), "Score: " + Player.Score.ToString());
		GUI.Label(new Rect(10, 30, 60, 20), "Lives: " + Player.Lives.ToString());
		GUI.Label(new Rect(10, 50, 60, 20), "Missed: " + Player.Missed.ToString());
	}
	
	void OnTriggerEnter(Collider otherObject)
	{
		if (otherObject.tag == "enemy" &&state == State.Playing)  
		{
			Player.Lives--;
			
			Enemy enemy = (Enemy)otherObject.gameObject.GetComponent("Enemy");
			enemy.SetPositionAndSpeed();
			
			
			StartCoroutine(DestroyShip());
				
		}
	}
	
	IEnumerator DestroyShip()
	{
		state = State.Explosion;
		Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
		gameObject.GetComponent<Renderer>().enabled = false;
		transform.position = new Vector3(0f, -5f, transform.position.z);
		yield return new WaitForSeconds(shipInvisibleTime);
		if (Player.Lives > 0)
		{
			gameObject.GetComponent<Renderer>().enabled = true;
			
			while (transform.position.y < -2.5)
			{
				// Move the ship up
				float amtToMove = shipMoveOnToScreenSpeed * Time.deltaTime;
				transform.position = new Vector3(0f, transform.position.y + amtToMove, transform.position.z);
				
				yield return 0;
			}
			
			state = State.Invincible;
			
			while (blinkCount < numbersOfTimesToBlink)
			{
				gameObject.GetComponent<Renderer>().enabled = !gameObject.GetComponent<Renderer>().enabled;
				
				if (gameObject.GetComponent<Renderer>().enabled == true)
					blinkCount++;
				
				yield return new WaitForSeconds(blinkRate);
			}
			blinkCount = 0;
			state = State.Playing;
		}
		
		
		else
			Application.LoadLevel(2);
	}
}
