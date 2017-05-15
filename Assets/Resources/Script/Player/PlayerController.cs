using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public int m_Id;
	protected BodyGuard m_BodyGuardCharacter;
	public float m_AttackSpeed=0.3f;
	public float m_DashCoolDown=0.5f;

	void Awake () {
		m_BodyGuardCharacter = transform.GetComponent<BodyGuard> ();

	}
	
	// Update is called once per frame
	void Start () {
		StartCoroutine (PlayerAttack ());
		StartCoroutine (PlayerMove ());
		StartCoroutine (PlayerDash ());
		StartCoroutine (PlayerPushBack ());
	}

	void FixedUpdate()
	{
		PlayerPause ();
	}

	public IEnumerator PlayerMove()
	{
		while (true) {
			Vector3 moveDir = Vector3.zero;
			moveDir += Vector3.right * Input.GetAxis ("Horizontal_"+m_Id.ToString());
			moveDir += Vector3.back * Input.GetAxis ("Vertical_"+m_Id.ToString());
			moveDir = Vector3.ClampMagnitude (moveDir, 1);
			m_BodyGuardCharacter.Move (moveDir);

			yield return new WaitForFixedUpdate();
		}
	}
	public IEnumerator PlayerAttack()
	{
		while (true) {
			if (Input.GetAxis ("Attack_"+m_Id) > 0) {
				//Debug.Log ("input attack : " + Input.GetAxis ("A"));
				m_BodyGuardCharacter.Attack ();
				yield return new WaitForSeconds (m_AttackSpeed);
			}
			yield return new WaitForFixedUpdate();
		}
	}
	public IEnumerator PlayerPushBack()
	{
		while (true) {
			if (Input.GetAxis ("Push_"+m_Id) > 0) {
				Debug.Log ("input attack : X_0");
				m_BodyGuardCharacter.PushBack ();
				yield return new WaitForSeconds (1);
			}
			yield return new WaitForFixedUpdate();
		}
	}
	public IEnumerator PlayerDash()
	{
		while (true) {
			if (Input.GetAxis ("Dash_"+m_Id) > 0) {
				//Debug.Log ("input attack : " + Input.GetAxis ("A"));
				m_BodyGuardCharacter.Dash ();
				yield return new WaitForSeconds (m_DashCoolDown);
			}
			yield return new WaitForFixedUpdate();
		}
	}

	public void PlayerPause()
	{
		if (Input.GetAxis ("Start_"+m_Id) > 0.1f) {
			Debug.Log (" start = " + Input.GetAxis ("Start_" + m_Id));
			if (Time.timeScale == 1) {
				MenuManager.Instance.PauseGame (m_Id);
			} else {
				MenuManager.Instance.ResumeGame ();
			}
		}
	}
}
