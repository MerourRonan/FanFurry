using UnityEngine;
using System.Collections;

public class Fan : Character,IDamageable {

	public int m_HeartPoint=2;
	public float m_VipInteractionDistance=2;

	public float m_ScorePoints;
	public float m_FamePoints;
	public float m_CrisisPoints;

	// Use this for initialization
	protected override void Awake () {
		base.Awake ();
		InitScript ();
	}

	void Start()
	{
		InitFanAi ();
	}

	void OnDestroy()
	{
		GameManager.Instance.getFans ().Remove (this);
	}

	protected void InitScript()
	{
		GameManager.Instance.getFans ().Add (this);
	}

	public void InitFanAi()
	{
		CustomLogger.debug (this, "InitFanAi", CustomLogger.fanLog);
		StopActions ();
		m_Walk = StartCoroutine (Walk ());
	}

	public void ApplyDamage(int damages)
	{
		CustomLogger.debug (this, "ApplyDamage", CustomLogger.fanLog);
		if (!m_Immune) {
			m_HeartPoint--;
			if (m_HeartPoint > 0) {
				m_TakingHit = StartCoroutine (TakingHit ());
			}
			else {
				StopActions ();
				StartCoroutine (Dying ());
			}
		}
		else
			CustomLogger.debug (this, "being hitting", CustomLogger.fanLog);
	}

	public void PushBack(Vector3 bodyGuardPosition)
	{
		CustomLogger.debug (this, "PushBack", CustomLogger.fanLog);
		if (!m_Immune) {
			StartCoroutine(PushingBack(bodyGuardPosition));
		}
		else
			CustomLogger.debug (this, "being push back", CustomLogger.fanLog);
	}

	public IEnumerator PushingBack(Vector3 bodyGuardPosition)
	{
		CustomLogger.debug (this, "PushingBack", CustomLogger.fanLog);
		m_NavAgent.Stop ();
		float pushDuration = 0.2f;
		Vector3 pushDir = (transform.position - bodyGuardPosition).normalized;
		while (pushDuration > 0) {
			Vector3 moveVector = pushDir * 20* Time.fixedDeltaTime;
			m_NavAgent.Move (moveVector);
			pushDuration -= Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
		m_NavAgent.Resume ();
		CustomLogger.debug (this, "PushingBack End", CustomLogger.fanLog);

	}

	public override IEnumerator TakingHit()
	{
		CustomLogger.debug (this, "TakingDamage", CustomLogger.fanLog);
		yield return StartCoroutine(base.TakingHit ());
	}
	public IEnumerator Dying()
	{
		CustomLogger.debug (this, "Dying", CustomLogger.fanLog);
		GameManager.Instance.IncreaseScore (m_ScorePoints);
		m_Immune = true;
		float deathDuration = 0.5f;
		Material matBody = m_Body.GetComponent<MeshRenderer> ().materials [0];
		Material matHead = m_Head.GetComponent<MeshRenderer> ().materials [0];
		matBody.color = new Color (1, 0, 0, 1);
		matHead.color = new Color (1, 0, 0, 1);
		while (transform.localScale.x >0) {
			Vector3 newScale = transform.localScale;
			newScale -= Vector3.one * Time.deltaTime / deathDuration;
			transform.localScale = newScale;
			yield return new WaitForEndOfFrame ();
		}
		Destroy (this.gameObject);
	}

	public IEnumerator Walk()
	{
		CustomLogger.debug (this, "Walk", CustomLogger.fanLog);
		m_NavAgent.destination = Vip.Instance.transform.position;
		m_NavAgent.Resume ();
		while (Vector3.Distance(Vip.Instance.transform.position,this.transform.position) >= m_VipInteractionDistance) {
			Vector3 goal = Vip.Instance.transform.position;
			goal.y = transform.position.y;
			m_NavAgent.destination = goal;
			AlignBody (m_NavAgent.velocity);
			yield return new WaitForFixedUpdate ();
		}
		StopActions ();
		DemandAutograph ();
	}

	public IEnumerator WaitAutograph()
	{
		CustomLogger.debug (this, "WaitAutograph", CustomLogger.fanLog);
		Vector3 alignDir = Vip.Instance.transform.position - this.transform.position;
		while (true) {
			AlignBody (alignDir);
			yield return new WaitForFixedUpdate ();
		}
	}

	public void DemandAutograph()
	{
		CustomLogger.debug (this, "DemandAutograph", CustomLogger.fanLog);
		m_Immune = true;
		Vip.Instance.AddFanToSign (this);
		m_Wait = StartCoroutine (WaitAutograph ());
		GameManager.Instance.UpdateSerenityFame (m_CrisisPoints, m_FamePoints);
	}

	public override void StopActions()
	{
		CustomLogger.debug (this, "StopActions", CustomLogger.fanLog);
		base.StopActions ();
	}
}
