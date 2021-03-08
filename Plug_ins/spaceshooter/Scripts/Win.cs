using UnityEngine;
using System.Collections;

public class Win : MonoBehaviour 
{
	#region Fields
	private int buttonWidth = 200;
	private int buttonHeight = 50;
	
	#endregion
	
	
	#region Functions
	
	void OnGUI()
	{
		
		if (GUI.Button(new Rect((Screen.width / 2) - (buttonWidth / 2),
								   Screen.height / 2 - buttonHeight / 2, buttonWidth, buttonHeight), "You Win!\nPress to Play Again"))
		{
			Player.Score = 0;
			Player.Lives = 3;
			Player.Missed = 0;
			Application.LoadLevel(1);
		}
		
	
	}
	
	#endregion
}
