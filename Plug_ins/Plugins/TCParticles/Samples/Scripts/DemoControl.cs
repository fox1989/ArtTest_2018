using UnityEngine;

public class DemoControl : MonoBehaviour
{
	private float m_updateInterval = 0.5f;
	private float m_accum;
	private float m_timeleft;
	private int m_frames;

	[SerializeField]
	[HideInInspector]
	private int m_demo;

	private string m_fpsText;

	private TCForce m_forcesExample;

	private static string[] s_levels = {
		"Destruction", "FX1",  "TerrainCollision", "Spooky", "Forces", "Fireflies"
	};


	private static string[] s_levelInfo =
		{
			"Projectiles - Left click to fire. Press 1-3 to cycle weapons",
			"The FX1 Microstar - kindly loaned from HEDRON central requisitions",
			"Dusty moon",
			"Spooky particles - Do not use for a slender game",
			"Forces - Press 1-5 to cycle force types. Left mouse button to interact, Control and scrollwheel to control camera",
			"Fireflies - boids flocking demo"
		};

	void OnGUI()
	{
		float width = Screen.width;
		float height = Screen.height;

		if (m_demo == 4)
		{
			if (m_forcesExample == null)
			{
				GameObject go = GameObject.Find("ForceExampleForce");

				if (go != null)
					m_forcesExample = go.GetComponent<TCForce>();
			}

			if (m_forcesExample != null)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Force type: ", GUILayout.Width(100.0f));
				string forceType = m_forcesExample.forceType.ToString();
				
				if (forceType == "Vector")
					forceType += " (up)";

				if (forceType == "Radial" && m_forcesExample.power < 0)
					forceType += " (inward)";

				GUILayout.Label(forceType);
				GUILayout.EndHorizontal();
			}
		}


		GUI.BeginGroup(new Rect(0.0f, height - 60.0f, width, 60.0f), "", "Box");

		GUI.enabled = m_demo > 0;
		if (GUI.Button(new Rect(10.0f, 10.0f, 100.0f, 40.0f), "Previous demo"))
			PreviousDemo();
		GUI.enabled = true;



		GUI.Label(new Rect(155.0f, 25.0f, 350.0f, 60.0f), s_levelInfo[m_demo]);
		GUI.Label(new Rect(width / 2.0f + 75.0f, 25.0f, 150.0f, 60.0f), "FPS: " + m_fpsText);



		GUI.enabled = m_demo < 5;
		if (GUI.Button(new Rect(width - 10.0f - 150.0f, 10.0f, 100.0f, 40.0f), "Next demo"))
			NextDemo();
		GUI.enabled = true;

		GUI.EndGroup();
	}

	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	void PreviousDemo()
	{
		m_demo--;
		Application.LoadLevel(s_levels[m_demo]);
	}

	void NextDemo()
	{
		m_demo++;
		Application.LoadLevel(s_levels[m_demo]);
	}

	// Update is called once per frame
	void Update()
	{
		m_timeleft -= Time.deltaTime;
		m_accum += Time.timeScale / Time.deltaTime;
		++m_frames;

		// Interval ended - update GUI text and start new interval
		if (m_timeleft <= 0.0)
		{
			// display two fractional digits (f2 format)
			m_fpsText = (m_accum / m_frames).ToString("f2");
			m_timeleft = m_updateInterval;
			m_accum = 0.0f;
			m_frames = 0;
		}
	}
}
