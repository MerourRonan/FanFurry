using UnityEngine;
using System.Collections;

public class GameParameters : MonoBehaviour {

	public static GameParameters Instance;

	public float m_SheepFanProba=60;
	public float m_MouseFanProba=35;
	public float m_LionFanProba=20;
	public float m_StalkerFanProba=5;
	public float m_FishFanProba=20;
	public float m_RoosterFanProba=20;

	// Spawn Parameters
	public bool m_ActiveVip=true;
	public bool m_ActiveHiddenFan=false;
	public bool m_ActiveFanWaves=true;
	public float m_StartWaveTimerSec=10;
	public int m_NbGuards;
	public int m_NbCivils=25;
	public int m_CivilFanProbability=15;


	// General Parameters
	public int m_GameDurationInSec=180;

	// Crisis Parameters
	public float m_SerenityMax=100;
	public float m_SerenityIncreasePerSec=0.5f;

	// Fame Parameters
	public float m_FameMax=100;
	public float m_FameMaxPerLevel=25;

	// Fan Wave Parameters
	public float m_TimeBetweenWave=3;
	public int m_FanMinPerWave=1;
	public int m_FanMaxPerWave=1;

	// Score Parameters
	public float m_ScoreMultiplicatorStart = 1;
	public float m_ScoreMultiplicatorIncrease = 1.5f;

	void Awake()
	{
		Instance = this;
		m_NbGuards = Input.GetJoystickNames ().Length;
		Debug.Log ("nb guard = " + m_NbGuards);
	}

	public void IncreaseFameMax()
	{
		m_FameMax += m_FameMaxPerLevel;
	}
	public void IncreaseFanPerWave(int fameLevel)
	{
		if (fameLevel <= 6) {
			if (fameLevel % 2 > 0) {
				m_FanMaxPerWave++;
			} else {
				m_FanMinPerWave = m_FanMaxPerWave;
			}
		} else {
			m_FanMaxPerWave++;
		}
	}

}
