using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vip : Character {

	public static Vip Instance;
	protected List<Fan> m_FansToSign;

	protected Coroutine m_SignAutograph;

	public float m_WaitDurationMin=3;
	public float m_WaitDurationMax=6;
	public float m_FamePoints=0;
	public float m_CrisisPoints=0;

	// Use this for initialization
	protected override void Awake () {
		base.Awake ();
		InitScript ();
	}

	void Start()
	{
		//UpdateVipAi ();
		m_Walk = StartCoroutine (Walk ());
	}

	private void InitScript()
	{
		Instance = this;
		m_FansToSign = new List<Fan> ();
	}

	public IEnumerator Walk()
	{
		CustomLogger.debug (this, "Walk", CustomLogger.vipLog);
		Vector3 goal = ComputeInterestPoint (10);
		m_NavAgent.ResetPath ();
		m_NavAgent.SetDestination(goal);
		m_NavAgent.Resume ();
		while(Vector3.Distance(this.transform.position,goal)>1f)
		{
//			CustomLogger.debug (this, "m_NavAgent.velocity = "+m_NavAgent.velocity, CustomLogger.vipLog);
//			CustomLogger.debug (this, "goal distance = "+Vector3.Distance(this.transform.position,goal), CustomLogger.vipLog);
//			CustomLogger.debug (this, "transform.position = "+transform.position, CustomLogger.vipLog);
//			CustomLogger.debug (this, "goal.position = "+goal, CustomLogger.vipLog);
//			CustomLogger.debug (this, "nav mesh goal  = "+m_NavAgent.pathEndPosition, CustomLogger.vipLog);
			AlignBody (m_NavAgent.velocity);
			yield return new WaitForFixedUpdate ();
		}
		StopActions();
		m_Wait = StartCoroutine (Wait ());
	}

	public IEnumerator Wait()
	{
		
		float waitTimer = Random.Range (m_WaitDurationMin,m_WaitDurationMax);
		CustomLogger.debug (this, "Wait ("+waitTimer+")", CustomLogger.vipLog);
		while (waitTimer > 0) {
			waitTimer -= Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
		StopActions ();
		m_Walk = StartCoroutine (Walk ());
	}

	public IEnumerator SignAutograph()
	{
		CustomLogger.debug (this, "SignAutograph", CustomLogger.vipLog);
		m_Immune = true;
		m_NavAgent.Stop ();
		List<Fan> fans = new List<Fan> (m_FansToSign);
		foreach (Fan fan in fans) {
			float waitTimer = 0.5f;
			while (waitTimer > 0) {
				waitTimer -= Time.deltaTime;
				AlignBody (fan.transform.position-this.transform.position,360);
				yield return new WaitForEndOfFrame ();
			}
			m_FansToSign.Remove (fan);
			Destroy (fan.gameObject);
		}
		m_SignAutograph = null;
		//StopActions ();
		CustomLogger.debug (this, "remaining fan to sign = "+m_FansToSign.Count, CustomLogger.vipLog);
		if (m_FansToSign.Count > 0) {
			m_SignAutograph = StartCoroutine (SignAutograph ());
		} else {
			m_NavAgent.Resume ();
			m_Immune = false;
			//m_Walk = StartCoroutine (Walk ());
		}
	}

	public void AddFanToSign(Fan fan)
	{
		CustomLogger.debug (this, "AddFanToSign ("+fan.transform.name+")", CustomLogger.vipLog);
		m_FansToSign.Add (fan);
		if (m_SignAutograph == null) {
			//StopActions ();
			m_SignAutograph = StartCoroutine (SignAutograph ());
		}
	}

	public Vector3 ComputeRandomPointAroundTarget(Vector3 targetPos, float maxDistance)
	{
		Vector3 randomDirection = Random.insideUnitSphere.normalized * maxDistance;
		randomDirection += targetPos;
		randomDirection.y = 0;
		NavMeshHit hit;
		int navLayer = (1<<NavMesh.GetAreaFromName("GameZone"));
		NavMesh.SamplePosition(randomDirection, out hit, maxDistance,navLayer);
		Vector3 goal = hit.position;
		return goal;
	}

	public Vector3 ComputeInterestPoint(float maxDistance)
	{
		List<Transform> landMarks = GameManager.Instance.getLandMarks();
		int rand = Random.Range (0, landMarks.Count);
		Transform landMarkDestination = landMarks [rand];
		Vector3 randomDirection = Random.insideUnitSphere.normalized * maxDistance;
		randomDirection += landMarkDestination.position;
		randomDirection.y = 0;
		NavMeshHit hit;
		int navLayer = (1<<NavMesh.GetAreaFromName("GameZone"));
		NavMesh.SamplePosition(randomDirection, out hit, maxDistance, navLayer);
		Vector3 goal = hit.position;
		return goal;
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.transform.name == "Body") {
			Civil civilScript = collider.transform.parent.GetComponent<Civil> ();
			if (civilScript != null) {
				civilScript.TriggerFan ();
			}
		}
	}

	public override void StopActions()
	{
		CustomLogger.debug (this, "stop actions", CustomLogger.vipLog);
		base.StopActions ();
		m_SignAutograph = null;
	}

}
