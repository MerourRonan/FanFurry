using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

	public static MainMenuManager Instance;
	protected EventSystem m_EventSystem;

	protected Transform m_PausePanel;
	protected Button m_NewGameButton;
	protected Button m_ExitGameButton;

	public int m_PlayerPauseID;
	public int m_ButtonSelectedNumber;
	public Button[] m_Buttons;
	public bool m_VerticaleInUsed;

	// Use this for initialization
	void Awake () {
		InitScript ();
	}

	void Start()
	{
		m_ButtonSelectedNumber = 0;
		m_Buttons [m_ButtonSelectedNumber].Select ();
	}

	void Update()
	{
		MenuListener ();
	}

	void InitScript () {
		Instance = this;
		m_EventSystem = GameObject.Find ("EventSystem").GetComponent<EventSystem>();
		m_NewGameButton = GameObject.Find ("UI/Background/ButtonPanel/NewGameButton").GetComponent<Button> ();
		m_ExitGameButton = GameObject.Find ("UI/Background/ButtonPanel/ExitGameButton").GetComponent<Button> ();
		m_Buttons = new Button[2]{m_NewGameButton,m_ExitGameButton };
	}

	protected void MenuListener()
	{
			// Next or Previous Button
			if (Input.GetAxisRaw ("Vertical_" + m_PlayerPauseID.ToString ()) > 0 && !m_VerticaleInUsed) {
				Debug.Log ("vertical = "+Input.GetAxisRaw ("Vertical_" + m_PlayerPauseID.ToString ()));
				SelectNextButton ();
				m_VerticaleInUsed = true;
			} else if (Input.GetAxisRaw ("Vertical_" + m_PlayerPauseID.ToString ()) < 0 && !m_VerticaleInUsed) {
				Debug.Log ("vertical = "+Input.GetAxisRaw ("Vertical_" + m_PlayerPauseID.ToString ()));
				SelectPreviousButton ();
				m_VerticaleInUsed = true;
			} else if(Input.GetAxisRaw ("Vertical_" + m_PlayerPauseID.ToString ())==0 && m_VerticaleInUsed) {
				Debug.Log ("vertical not used");
				m_VerticaleInUsed = false;
			}

			//Valid Button
			if (Input.GetAxisRaw ("Attack_" + m_PlayerPauseID.ToString ()) > 0) {
				m_Buttons [m_ButtonSelectedNumber].onClick.Invoke ();
			}
	}

	public void NewGame()
	{
		SceneManager.LoadScene ("scene0");
	}

	public void ExitGame()
	{
		Application.Quit ();
	}

	protected void SelectNextButton()
	{
		m_ButtonSelectedNumber++;
		if (m_ButtonSelectedNumber >= m_Buttons.Length) {
			m_ButtonSelectedNumber = 0;
		}
		m_Buttons [m_ButtonSelectedNumber].Select ();
	}
	protected void SelectPreviousButton()
	{
		m_ButtonSelectedNumber--;
		if (m_ButtonSelectedNumber <0) {
			m_ButtonSelectedNumber = m_Buttons.Length-1;
		}
		m_Buttons [m_ButtonSelectedNumber].Select ();
	}
}
