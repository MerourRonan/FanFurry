using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	public static MenuManager Instance;
	protected EventSystem m_EventSystem;

	protected Transform m_PausePanel;
	protected Button m_ResumeButton;
	protected Button m_RestartButton;
	protected Button m_MainMenuButton;
	protected Button m_ExitGameButton;

	public int m_PlayerPauseID;
	public bool m_PauseMenuEnable;
	public int m_ButtonSelectedNumber;
	protected Button[] m_Buttons;
	public bool m_VerticaleInUsed;

	// Use this for initialization
	void Awake () {
		InitScript ();
	}

	void Update()
	{
		MenuListener ();
	}

	void InitScript () {
		Instance = this;
		m_EventSystem = GameObject.Find ("EventSystem").GetComponent<EventSystem>();
		m_PausePanel = GameObject.Find ("UI/PausePanel").transform;
		m_ResumeButton = GameObject.Find ("UI/PausePanel/Background/ResumeButton").GetComponent<Button> ();
		m_RestartButton = GameObject.Find ("UI/PausePanel/Background/RestartButton").GetComponent<Button> ();
		m_MainMenuButton = GameObject.Find ("UI/PausePanel/Background/MainMenuButton").GetComponent<Button> ();
		m_ExitGameButton = GameObject.Find ("UI/PausePanel/Background/ExitGameButton").GetComponent<Button> ();
		m_Buttons = new Button[4]{m_ResumeButton,m_RestartButton,m_MainMenuButton,m_ExitGameButton };
		ResumeGame ();
	}

	protected void MenuListener()
	{
		if (m_PauseMenuEnable) {
			Debug.Log ("vertical = "+Input.GetAxisRaw ("Vertical_" + m_PlayerPauseID.ToString ()));
			// Next or Previous Button
			if (Input.GetAxisRaw ("Vertical_" + m_PlayerPauseID.ToString ()) > 0 && !m_VerticaleInUsed) {
				SelectNextButton ();
				m_VerticaleInUsed = true;
			} else if (Input.GetAxisRaw ("Vertical_" + m_PlayerPauseID.ToString ()) < 0 && !m_VerticaleInUsed) {
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
			if (Input.GetAxisRaw ("Push_" + m_PlayerPauseID.ToString ()) > 0) {
				ResumeGame ();
			}
		}
	}

	public void PauseGame(int playerID)
	{
		CustomLogger.debug (this, "PauseGame", CustomLogger.gameLog);
		Time.timeScale = 0;
		m_PauseMenuEnable = true;
		m_PlayerPauseID = playerID;
		m_PausePanel.gameObject.SetActive (true);
		m_ButtonSelectedNumber = 0;
		m_Buttons [m_ButtonSelectedNumber].Select ();
		//StartCoroutine (MenuListener ());
	}
	public void ResumeGame()
	{
		CustomLogger.debug (this, "ResumeGame", CustomLogger.gameLog);
		m_EventSystem.SetSelectedGameObject (null);
		m_PausePanel.gameObject.SetActive (false);
		m_PauseMenuEnable = false;
		Time.timeScale = 1;
	}

	public void RestartGame()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void ExitGame()
	{
		Application.Quit ();
	}

	public void ExitToMainMenu()
	{
		SceneManager.LoadScene ("sceneMainMenu");
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
