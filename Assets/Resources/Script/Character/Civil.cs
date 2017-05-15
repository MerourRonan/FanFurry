using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Civil : Character, IDamageable {

	public static ILogger logger = Debug.logger;

	public string m_CurrentBehavior;
	public int m_NumberOfMove;
	protected HashSet<Civil> m_CivilNearby;
	public bool m_IsFan;
	protected Coroutine m_FanTransformation;

	public float m_FamePoints=-10;
	public float m_CrisisPoints=0;


	// Use this for initialization
	protected override void Awake () {
		base.Awake ();
		InitScript ();
	}

	void Start()
	{
		InitCivilAi ();
	}

	protected void InitScript()
	{
		m_CivilNearby = new HashSet<Civil> ();
		GameManager.Instance.getCivils ().Add (this);
	}
		
	public void InitCivilAi()
	{
		CustomLogger.debug (this, "InitCivilAi", CustomLogger.civilLog);
		//Debug.Log ("InitCivilAi");
		m_Walk = StartCoroutine(WalkToInterestPoint ());
	}

	public void UpdateCivilAi()
	{
		//Debug.Log ("UpdateCivilAi");
		//StopActions ();
		if (m_NumberOfMove == 0) 
			m_Walk = StartCoroutine (LeaveLevel ());
		else
			m_Walk = StartCoroutine(WalkToInterestPoint ());
	}

	public void ApplyDamage(int damages)
	{
		CustomLogger.debug (this, "ApplyDamage", CustomLogger.civilLog);
		if(m_TakingHit == null)
			m_TakingHit = StartCoroutine (TakingHit ());
		else
			CustomLogger.debug (this, "being hitting", CustomLogger.civilLog);
	}

	public void PushBack(Vector3 bodyGuardPosition)
	{
		CustomLogger.debug (this, "PushBack", CustomLogger.civilLog);
		if (!m_Immune) {
			StartCoroutine(PushingBack(bodyGuardPosition));
		}
		else
			CustomLogger.debug (this, "being push back", CustomLogger.civilLog);
	}

	public IEnumerator PushingBack(Vector3 bodyGuardPosition)
	{
		CustomLogger.debug (this, "PushingBack", CustomLogger.civilLog);
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
		CustomLogger.debug (this, "PushingBack End", CustomLogger.civilLog);

	}

	public override IEnumerator TakingHit()
	{
		CustomLogger.debug (this, "TakingDamage", CustomLogger.civilLog);
		GameManager.Instance.UpdateSerenityFame (m_CrisisPoints, m_FamePoints);
		yield return StartCoroutine(base.TakingHit ());

	}


	// Marche vers un point d'interet (magasin, etc...)
	public IEnumerator WalkToInterestPoint()
	{
		CustomLogger.debug (this, "WalkToInterestPoint", CustomLogger.civilLog);
		m_CurrentBehavior = "Walk";
		Vector3 goal = ComputeInterestPoint (30);
		m_NavAgent.ResetPath ();
		m_NavAgent.SetDestination(goal);
		m_NavAgent.Resume ();
		while(Vector3.Distance(this.transform.position,goal)>1f){
			AlignBody (m_NavAgent.velocity);
			yield return new WaitForFixedUpdate ();
		}
		StopActions ();
		m_NumberOfMove--;
		CustomLogger.debug (this, "number of move remaining = " + m_NumberOfMove, CustomLogger.civilLog);
		TryWaitAndChat ();

	}

	// Marche vers une sortie du niveau
	public IEnumerator LeaveLevel()
	{
		CustomLogger.debug (this, "LeaveLevel", CustomLogger.civilLog);
		m_CurrentBehavior = "Leave";
		List<Transform> civilSpawners = SpawnManager.Instance.GetCivilSpawners ();
		int rand = Random.Range (0, civilSpawners.Count);
		CustomLogger.debug (this, "Exit spawner = "+civilSpawners [rand].name, CustomLogger.civilLog);
		Vector3 goal = civilSpawners [rand].position;
		m_NavAgent.destination = goal;
		m_NavAgent.Resume ();
		while(Vector3.Distance(this.transform.position,goal)>1f){
			AlignBody (m_NavAgent.velocity);
			yield return new WaitForFixedUpdate ();
		}
		StopActions ();
		CustomLogger.debug (this, "Leaving", CustomLogger.civilLog);
		SpawnManager.Instance.RespawnCivils (1);
		Destroy (this.gameObject);
	}

	// Attend et regarde un point d'interet (magazin, etc...)
	public IEnumerator WaitAndLook()
	{
		
		m_CurrentBehavior = "Wait";
		float waitTimer = Random.Range (2f,5f);
		CustomLogger.debug (this, "WaitAndLook, "+waitTimer+" sec", CustomLogger.civilLog);
		while (waitTimer > 0) {
			waitTimer -= Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
		StopActions ();
		UpdateCivilAi ();
	}

	// Attend et discute avec d'autres civils en attente si possible, sinon attente
	public void TryWaitAndChat()
	{
		CustomLogger.debug (this, "CheckWaitAndChat", CustomLogger.civilLog);
		List<Civil> listCivilWaiting = new List<Civil> ();
		Vector3 pointToLook = this.transform.position;
		foreach (Civil civil in m_CivilNearby) {
			if (civil.GetCurrentBehavior () == "Wait") {
				listCivilWaiting.Add (civil);
				pointToLook += civil.transform.position;
			}
		}

		if (listCivilWaiting.Count > 0) {
			float chatDuration = Random.Range (5f, 10f);
			pointToLook = pointToLook / (listCivilWaiting.Count+1);
			m_Wait = this.StartCoroutine (WaitAndChat (chatDuration, pointToLook));
			foreach (Civil civil in listCivilWaiting) {
				civil.StartChatting (chatDuration, pointToLook);
			}
		} else {
			m_Wait = StartCoroutine (WaitAndLook ());
		}
	}

	// Attend et discute avec d'autres civils en attente
	public IEnumerator WaitAndChat(float duration, Vector3 pointToLook)
	{
		CustomLogger.debug (this, "WaitAndChat (" + duration +" sec / " +pointToLook+")", CustomLogger.civilLog);
		m_CurrentBehavior = "Chat";

		float waitTimer = duration;
		while (waitTimer > 0) {
			waitTimer -= Time.deltaTime;
			AlignBody (pointToLook - transform.position);
			yield return new WaitForEndOfFrame ();
		}
		StopActions ();
		UpdateCivilAi ();
	}

	public void StartChatting(float duration, Vector3 pointToLook)
	{
		StopActions ();
		m_Wait = this.StartCoroutine (WaitAndChat (duration, pointToLook));
	}

	public void TriggerFan()
	{
		if (m_IsFan && m_FanTransformation == null && GameParameters.Instance.m_ActiveHiddenFan) {
			StopActions ();
			m_FanTransformation =  StartCoroutine (FanTransformation ());
		}
	}

	public IEnumerator FanTransformation()
	{
		CustomLogger.debug (this, "FanTransformation", CustomLogger.civilLog);
		m_CurrentBehavior = "Transformation";

		float waitTimer = 2;
		while (waitTimer > 0) {
			waitTimer -= Time.deltaTime;
			AlignBody (Vip.Instance.transform.position - this.transform.position);
			transform.localScale = Vector3.one + Vector3.one*0.5f * Mathf.Pow(Mathf.Cos (waitTimer*3),2);
			yield return new WaitForEndOfFrame ();
		}
		StopActions ();
		SpawnManager.Instance.SpawnFan (transform.position, m_Body.rotation);
		SpawnManager.Instance.RespawnCivils (1);
		Destroy (this.gameObject);
	}

	protected void ComputeIsFan()
	{
		int rand = Random.Range (0, 100);
		if (rand < 20)
			m_IsFan = true;	
	}

	public Vector3 ComputeInterestPoint(Vector3 targetPos, float maxDistance)
	{
		Vector3 randomDirection = Random.insideUnitSphere.normalized * maxDistance;
		randomDirection += targetPos;
		randomDirection.y = 0;
		NavMeshHit hit;
		int navLayer = (1<<NavMesh.GetAreaFromName("GameZone"));
		NavMesh.SamplePosition(randomDirection, out hit, maxDistance, navLayer);
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

	public void OnTriggerEnter(Collider collider)
	{
		if (collider.transform.name == "Body") {
			Civil civilScript = collider.transform.parent.GetComponent<Civil> ();
			if (civilScript != null) {
				m_CivilNearby.Add (civilScript);
			}
		}
	}

	public void OnTriggerExit(Collider collider)
	{
		if (collider.transform.name == "Body") {
			Civil civilScript = collider.transform.parent.GetComponent<Civil> ();
			if (civilScript != null) {
				m_CivilNearby.Remove (civilScript);
			}
		}
	}

	public override void StopActions()
	{
		CustomLogger.debug (this, "stop actions", CustomLogger.civilLog);
		base.StopActions ();
	}

	/*** Get ***/
	public string GetCurrentBehavior()
	{
		return m_CurrentBehavior;
	}

	public void SetHiddenFan(bool active)
	{
		CustomLogger.debug (this, "is hiddenfan", CustomLogger.civilLog);
		m_IsFan = active;
	}

}
