using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	
	#region Fields
	private string InstructionText = "Instructions:\nPress Left and Right Arrows to move.\nPress Spacebar to fire.";
	private int buttonWidth = 200;
	private int buttonHeight = 50;
	
	#endregion
	
	#region Properties
	
	#endregion
	
	
	#region Functions
	
	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 250, 200), InstructionText);
		if (GUI.Button(new Rect((Screen.width / 2) - (buttonWidth / 2),
								   Screen.height / 2 - buttonHeight / 2, buttonWidth, buttonHeight), "Start Game"))
		{
			Application.LoadLevel(1);
		}
		
	
	}
	
	#endregion
	
}

