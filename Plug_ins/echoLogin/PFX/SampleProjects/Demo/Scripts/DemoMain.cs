using UnityEngine;
using System.Collections;

public class DemoMain : MonoBehaviour 
{
	public static EchoPFX   		_shieldHit;
	public static EchoPFX   		_shockWave;
	private static AudioSource[] 	_sfxSource;
	private static AudioClip[]      _sounds;
	private static int             _sfxIndex = 0;

	public AudioClip 		DemoSong;
	public GameObject 		shotPrefab;
	public AudioClip[]      soundsLocal;
	private AudioSource 	_musicSource;
	private GameObject[] 	_shotsArray;
	private ShotBrain[]     _shotsScript;
	private int          	_shotsArrayIndex = 0;
	private float 			_shootTime = -1;

	//==================================================================================================
	void Start () 
	{
		_sounds = soundsLocal;

		_sfxSource = new AudioSource[4];
		for ( int loop = 0; loop < 4; loop++ )
		{
			_sfxSource[loop] 						= gameObject.AddComponent <AudioSource>( ) as AudioSource;
			_sfxSource[loop].volume	 				= 1;
			_sfxSource[loop].loop 					= false;
			_sfxSource[loop].ignoreListenerVolume 	= true;
		}
		
		_shotsArray 	= new GameObject[4];
		_shotsScript 	= new ShotBrain[4];

		for ( int loop = 0; loop < 4; loop++ )
		{
			_shotsArray[loop] 	= Instantiate(shotPrefab) as GameObject;
			_shotsScript[loop] 	= _shotsArray[loop].GetComponent<ShotBrain>();
		}

		_shieldHit = new EchoPFX ( "RenderGroup:0", "ShieldHit" );
		_shockWave = new EchoPFX ( "RenderGroup:0", "Shockwave" );

		_musicSource 						= gameObject.AddComponent <AudioSource>( ) as AudioSource;
		_musicSource.clip 					= DemoSong;
		_musicSource.volume 				= 1;
		_musicSource.ignoreListenerVolume 	= true;
		_musicSource.loop 					= true;
		_musicSource.Play();

	}

	//==================================================================================================
	public static void PlaySFX ( int index )
	{
		_sfxSource[_sfxIndex].clip = _sounds[index];
		_sfxSource[_sfxIndex].Play();

		_sfxIndex = ( _sfxIndex + 1 ) % 4;
	}

	//==================================================================================================
	public static void StartShieldHit( float ivx, float ivy )
	{
		_shockWave.ShockWaveCenter ( 0, ivx, ivy );
		_shockWave.Start();
		_shieldHit.Start();
	}

	//==================================================================================================
	void Update () 
	{
		_shootTime += Time.deltaTime;

		if ( _shootTime > 3.0f )
		{
			_shootTime = 0;

			_shotsScript[_shotsArrayIndex].Shoot ( transform.position, new Vector3(Random.Range ( 0.08f, -0.08f ), Random.Range ( 0.0f, 0.2f ),-1), Camera.main.transform, 8.5f );

			_shotsArrayIndex = ( _shotsArrayIndex + 1 ) % 4;
		}
	}
}
