using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	public static UIManager Instance;

	protected Slider m_SerenitySlider;
	protected Slider m_FameSlider;
	protected Text m_SerenityRatio;
	protected Text m_FameRatio;
	protected Text m_FameLevel;
	protected Text m_GameTimer;
	protected Text m_ScorePoints;
	protected Text m_ScoreMultiplicator;

	void Awake()
	{
		InitScript ();
	}

	public void InitScript()
	{
		Instance = this;
		m_SerenitySlider = GameObject.Find ("UI/BarsPanel/SerenityPanel/SerenitySlider").GetComponent<Slider> ();
		m_FameSlider = GameObject.Find ("UI/BarsPanel/FamePanel/FameSlider").GetComponent<Slider> ();
		m_SerenityRatio = GameObject.Find ("UI/BarsPanel/SerenityPanel/SerenitySlider/SerenityRatio").GetComponent<Text> ();
		m_FameRatio = GameObject.Find ("UI/BarsPanel/FamePanel/FameSlider/FameRatio").GetComponent<Text> ();
		m_FameLevel = GameObject.Find ("UI/BarsPanel/FamePanel/FameSlider/FameLevel").GetComponent<Text> ();
		m_GameTimer = GameObject.Find ("UI/GameTimePanel/GameTimer").GetComponent<Text> ();
		m_ScorePoints = GameObject.Find ("UI/ScorePanel/ScorePoints").GetComponent<Text> ();
		m_ScoreMultiplicator = GameObject.Find ("UI/ScorePanel/ScoreMultiplicator").GetComponent<Text> ();
	}

	public void UpdateSerenity(float currentSerenity)
	{
		m_SerenitySlider.value = Mathf.FloorToInt(currentSerenity);
		m_SerenityRatio.text = m_SerenitySlider.value.ToString() + " / " + m_SerenitySlider.maxValue.ToString();
	}
	public void UpdateFame(float currentFame)
	{
		m_FameSlider.value = currentFame;
		m_FameRatio.text = m_FameSlider.value.ToString() + " / " + m_FameSlider.maxValue.ToString();
	}
	public void ResetFame(float maxFame, int fameLevel, float scoreMultiplicator)
	{
		m_FameSlider.maxValue = maxFame;
		m_FameSlider.value = 0;
		m_FameRatio.text = m_FameSlider.value.ToString() + " / " + m_FameSlider.maxValue.ToString();
		m_FameLevel.text = fameLevel.ToString();
		m_ScoreMultiplicator.text = "x"+scoreMultiplicator.ToString ("0.0");
	}
	public void UpdateScore(float scorePoints)
	{
		m_ScorePoints.text = scorePoints.ToString();
	}
	public void UpdateGameTimer(int time)
	{
		int minutes = Mathf.FloorToInt(time / 60);
		int secondes = time - 60 * minutes;
		m_GameTimer.text = minutes.ToString("00") + ":" + secondes.ToString ("00");
	}

}
