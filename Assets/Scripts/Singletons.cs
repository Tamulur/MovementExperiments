using UnityEngine;
using System.Collections;

public class Singletons
{


			static Player _player;
	public static Player player {
		get { if ( _player == null )
			_player = GameObject.FindObjectOfType<Player>();
			return _player; }
	}
	
	
			static GUIManager _guiManager;
	public static GUIManager guiManager {
		get { if ( _guiManager == null )
					_guiManager = GameObject.FindObjectOfType<GUIManager>();
				return _guiManager; }
	}
	
			static TimeManager _timeManager;
	public static TimeManager timeManager {
		get { if ( _timeManager == null )
			_timeManager = GameObject.FindObjectOfType<TimeManager>();
			return _timeManager; }
	}
	
	
}
