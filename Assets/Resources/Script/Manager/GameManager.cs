using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;

	public List<BodyGuard> m_BodyGuards;
	public List<Civil> m_Civils;
	public List<Fan> m_Fans;
	protected List<Transform> m_LandMarks;

	public float m_SerenityPoints;
	public float m_FamePoints;
	public int m_FameLevel;
	public float m_ScoreMultiplicator;
	public float m_GameScore;
	protected float m_GameTimer;


	// Use this for initialization
	void Awake () {
		InitScript ();
	}

	// Use this for initialization
	void Start () {
		InitGame ();
		StartCoroutine(SendFanWaves());
	}

	
	// Update is called once per frame
	void Update () {
		UpdateGameTimer ();
		RegenSerenity ();
	}

	private void InitScript()
	{
		Instance = this;
		InitLandMarks ();
	}

	private void InitGame()
	{
		m_GameTimer = GameParameters.Instance.m_GameDurationInSec;
		m_ScoreMultiplicator = GameParameters.Instance.m_ScoreMultiplicatorStart;
		SpawnManager.Instance.InitGame ();
		CameraController.Instance.ConfigureVisibleTargets ();
		m_SerenityPoints = GameParameters.Instance.m_SerenityMax;
		InitUI ();
	}

	private void InitUI()
	{
		UIManager.Instance.UpdateSerenity (m_SerenityPoints);
		UIManager.Instance.ResetFame (GameParameters.Instance.m_FameMax, 0, 1);
		UIManager.Instance.UpdateScore (0);
		UIManager.Instance.UpdateGameTimer (GameParameters.Instance.m_GameDurationInSec);
	}

	private void InitLandMarks()
	{
		m_LandMarks = new List<Transform> ();
		foreach (Transform landMark in GameObject.Find("Level/LandMarks").GetComponentsInChildren<Transform>()) {
			m_LandMarks.Add (landMark);
		}
	}

	private void UpdateGameTimer()
	{
		m_GameTimer -= Time.deltaTime;
		UIManager.Instance.UpdateGameTimer (Mathf.FloorToInt(m_GameTimer));
	}

	private IEnumerator SendFanWaves()
	{
		CustomLogger.debug (this, "SendFanWaves", CustomLogger.gameLog);
		if (GameParameters.Instance.m_ActiveFanWaves == true) {
			yield return new WaitForSeconds (GameParameters.Instance.m_StartWaveTimerSec);
			GameParameters.Instance.m_ActiveHiddenFan = true;
			float timeBetweenWave = 0;
			while (true) {
				timeBetweenWave -= Time.deltaTime;
				if (timeBetweenWave <= 0) {
					CustomLogger.debug (this, "SpawnFanWave", CustomLogger.gameLog);
					SpawnManager.Instance.SpawnFanWave ();
					timeBetweenWave = GameParameters.Instance.m_TimeBetweenWave;
				}
				yield return new WaitForEndOfFrame ();
			}
		}

	}

	private void RegenSerenity()
	{
		m_SerenityPoints = Mathf.Clamp (m_SerenityPoints + GameParameters.Instance.m_SerenityIncreasePerSec*Time.deltaTime, 0, 100);
		UIManager.Instance.UpdateSerenity (m_SerenityPoints);
	}

	private void IncreaseFameLevel()
	{
		CustomLogger.debug (this, "IncreaseFameLevel", CustomLogger.gameLog);
		m_FameLevel++;
		m_FamePoints = 0;
		m_SerenityPoints = GameParameters.Instance.m_SerenityMax;
		m_ScoreMultiplicator *= GameParameters.Instance.m_ScoreMultiplicatorIncrease;
		GameParameters.Instance.IncreaseFameMax ();
		GameParameters.Instance.IncreaseFanPerWave (m_FameLevel);
		UIManager.Instance.ResetFame (GameParameters.Instance.m_FameMax, m_FameLevel, m_ScoreMultiplicator);

	}

	public void UpdateSerenityFame(float crisisPoints,float famePoints)
	{
		m_SerenityPoints -= crisisPoints;
		if (m_SerenityPoints <=0) {
			Debug.Log ("Game Over");
			return;
		}

		m_FamePoints += famePoints;
		if (m_FamePoints >= GameParameters.Instance.m_FameMax) {
			IncreaseFameLevel ();
		}
		UIManager.Instance.UpdateSerenity (m_SerenityPoints);
		UIManager.Instance.UpdateFame (m_FamePoints);
	}

	public void IncreaseScore(float scorePoints)
	{
		m_GameScore += scorePoints * m_ScoreMultiplicator;
		UIManager.Instance.UpdateScore (m_GameScore);
	}



	/**** Get ****/
	public List<BodyGuard> getBodyGuards()
	{
		if (m_BodyGuards == null)
			m_BodyGuards = new List<BodyGuard>();
		return m_BodyGuards;
	}
	public List<Civil> getCivils()
	{
		if (m_Civils == null)
			m_Civils = new List<Civil>();
		return m_Civils;
	}
	public List<Fan> getFans()
	{
		if (m_Fans == null)
			m_Fans = new List<Fan>();
		return m_Fans;
	}
	public List<Transform> getLandMarks()
	{
		return m_LandMarks;
	}
}
