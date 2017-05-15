using UnityEngine;
using System.Collections;

public class CustomLogger : MonoBehaviour {

	public static bool civilLog = false;
	public static bool fanLog = false;
	public static bool vipLog = false;
	public static bool guardLog = false;
	public static bool spawnerLog = false;
	public static bool gameLog = true;

	public static void debug(MonoBehaviour script, string message,bool active)
	{
		if (active) {
			Debug.Log (script.transform.name+" : "+ message);
		}
	}

	public static void debugError(MonoBehaviour script, string message)
	{
		Debug.LogError (script.transform.name+" : "+ message);
	}
}
