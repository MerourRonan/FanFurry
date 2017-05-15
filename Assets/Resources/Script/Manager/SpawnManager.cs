using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour {

	public static SpawnManager Instance;
	public List<Transform> m_CivilSpawners;
	public List<Transform> m_FanSpawners;

	protected GameObject m_CivilPrefab;
	protected GameObject m_FanPrefab;
	protected GameObject m_BodyGuardPrefab;
	protected GameObject m_VipPrefab;

	protected Transform m_FanParent;
	protected Transform m_CivilParent;
	protected Transform m_BodyGuardParent;
	protected Transform m_VipParent;

	protected int m_CivilID;
	protected int m_FanID;
	protected int m_BodyGuardID;



	// Use this for initialization
	void Awake () {
		InitScript ();
	}
	
	// Update is called once per frame
	void Start () {
		//SpawnCivils ();
	}

	private void InitScript()
	{
		Instance = this;
		InitStandardSpawners ();

		m_CivilPrefab = Resources.Load ("Prefab/Characters/Civil") as GameObject;
		m_FanPrefab = Resources.Load ("Prefab/Characters/Fan") as GameObject;
		m_BodyGuardPrefab = Resources.Load ("Prefab/Characters/BodyGuard") as GameObject;
		m_VipPrefab = Resources.Load ("Prefab/Characters/Vip") as GameObject;

		m_FanParent = GameObject.Find ("Characters/Fans").transform;
		m_CivilParent = GameObject.Find ("Characters/Civils").transform;
		m_BodyGuardParent = GameObject.Find ("Characters/BodyGuards").transform;
		m_VipParent = GameObject.Find ("Characters").transform;

		m_CivilID = 0;
		m_FanID = 0;
		m_BodyGuardID = 1;
	}

	public void InitGame()
	{
		SpawnCivils (GameParameters.Instance.m_NbCivils);
		SpawnBodyGuards (GameParameters.Instance.m_NbGuards);
		if(GameParameters.Instance.m_ActiveVip)
			SpawnVip ();
	}

	private void InitStandardSpawners()
	{
		foreach (Transform spawner in GameObject.Find("Level/Spawners").GetComponentsInChildren<Transform>()) {
			if (spawner.name == "Spawner") {
				GetCivilSpawners ().Add (spawner);
			}
		}
		foreach (Transform spawner in GameObject.Find("Level/Spawners").GetComponentsInChildren<Transform>()) {
			if (spawner.name == "Spawner") {
				GetFanSpawners ().Add (spawner);
			}

		}
	}

	protected void SpawnBodyGuards(int nbGuards)
	{
		for(int iter=0;iter<nbGuards;iter++)
		{
			Transform spawner = GameObject.Find ("Level/Spawners/VipSpawner").transform;
			Vector3 SpawnPos = ComputeRandomPointNextToSpawner (spawner.position,4);
			GameObject guardInstance = Instantiate (m_BodyGuardPrefab, SpawnPos, Quaternion.identity) as GameObject;
			guardInstance.transform.SetParent (m_BodyGuardParent);
			guardInstance.transform.name = "BodyGuard" + m_BodyGuardID.ToString ();
			guardInstance.GetComponent<BodyGuard> ().m_Id = m_BodyGuardID;
			guardInstance.GetComponent<PlayerController> ().m_Id = m_BodyGuardID;
			m_BodyGuardID++;
		}
	}

	protected void SpawnVip()
	{
		Transform spawner = GameObject.Find ("Level/Spawners/VipSpawner").transform;
		Vector3 SpawnPos = spawner.position;
		GameObject vipInstance = Instantiate (m_VipPrefab, SpawnPos, Quaternion.identity) as GameObject;
		vipInstance.transform.SetParent (m_VipParent);
		vipInstance.transform.name = "Vip";
	}

	protected void SpawnCivils(int nbCivils)
	{
		for(int iter=0;iter<nbCivils;iter++)
		{
			float hiddenFanChance = Random.Range (0f, 100f);
			Vector3 SpawnPos = ComputeRandomPointInLevel (30);
			GameObject civilInstance = Instantiate (m_CivilPrefab, SpawnPos, Quaternion.identity) as GameObject;
			civilInstance.transform.SetParent (m_CivilParent);
			civilInstance.transform.name = "Civil" + m_CivilID.ToString ();
			m_CivilID++;
			if (hiddenFanChance <= GameParameters.Instance.m_CivilFanProbability) {
				CustomLogger.debug (this, "hiddenFanChance = "+hiddenFanChance, CustomLogger.civilLog);
				civilInstance.GetComponent<Civil> ().SetHiddenFan (true);
			}
		}
	}

	public void RespawnCivils(int numberToSpawn)
	{
		int rand = Random.Range (0, m_CivilSpawners.Count);
		Transform spawner = m_CivilSpawners [rand];
		CustomLogger.debug (this, "Spawnin Civil" + m_CivilID.ToString ()+" from : " + spawner.name, CustomLogger.spawnerLog);
		for(int iter=0;iter<numberToSpawn;iter++)
		{
			float hiddenFanChance = Random.Range (0f, 100f);
			Vector3 SpawnPos = ComputeRandomPointNextToSpawner (spawner.position,5);
			GameObject civilInstance = Instantiate (m_CivilPrefab, SpawnPos, Quaternion.identity) as GameObject;
			civilInstance.transform.SetParent (m_CivilParent);
			civilInstance.transform.name = "Civil" + m_CivilID.ToString ();
			m_CivilID++;
			if (hiddenFanChance <= GameParameters.Instance.m_CivilFanProbability) {
				CustomLogger.debug (this, "hiddenFanChance = "+hiddenFanChance, CustomLogger.civilLog);
				civilInstance.GetComponent<Civil> ().SetHiddenFan (true);
			}
		}
	}

	public void SpawnFanWave()
	{
		//int spawnerNumber = Random.Range(0,m_FanSpawners.Count);
		CustomLogger.debug (this, "SpawnFanWave", CustomLogger.spawnerLog);
		//Transform spawner = m_FanSpawners[spawnerNumber];
		int nbFans = Random.Range (GameParameters.Instance.m_FanMinPerWave, GameParameters.Instance.m_FanMaxPerWave + 1);
		for(int iter=0;iter<nbFans;iter++)
		{
			int spawnerNumber = Random.Range(0,m_FanSpawners.Count);
			Transform spawner = m_FanSpawners[spawnerNumber];
			Vector3 spawnPos = ComputeRandomPointNextToSpawner (spawner.position, 5);
			GameObject fanInstance = Instantiate (m_FanPrefab, spawnPos, Quaternion.identity) as GameObject;
			fanInstance.transform.SetParent (m_FanParent);
			fanInstance.transform.name = "Fan" + m_FanID.ToString ();
			m_FanID++;
		}
	}


	public void SpawnFan(Vector3 spawnPos, Quaternion rotation)
	{
		GameObject fanInstance = Instantiate (m_FanPrefab, spawnPos, Quaternion.identity) as GameObject;
		fanInstance.transform.SetParent (m_FanParent);
		fanInstance.transform.name = "Fan" + m_FanID.ToString ();
		m_FanID++;
		fanInstance.GetComponent<Fan> ().GetBody ().rotation = rotation;
	}

	protected Vector3 ComputeRandomPointNextToSpawner(Vector3 spawnerPosition, float maxDistance)
	{
		Vector3 randomDirection = Random.insideUnitSphere * maxDistance;
		randomDirection += spawnerPosition;
		randomDirection.y = 0;
		NavMeshHit hit;
		int navLayer = (1<<NavMesh.GetAreaFromName("SpawnZone"));
		NavMesh.SamplePosition(randomDirection, out hit, maxDistance, navLayer);
		if (hit.position.magnitude != Mathf.Infinity)
			return hit.position;
		else
			return spawnerPosition;
	}

	protected Vector3 ComputeRandomPointInLevel(float maxDistance)
	{
		Vector3 randomDirection = Random.insideUnitSphere * maxDistance;
		randomDirection.y = 0;
		NavMeshHit hit;
		int navLayer = (1<<NavMesh.GetAreaFromName("GameZone"));
		NavMesh.SamplePosition(randomDirection, out hit, maxDistance, navLayer);
		if (hit.position.magnitude != Mathf.Infinity)
			return hit.position;
		else
			return Vector3.zero;
	}

	public List<Transform> GetCivilSpawners()
	{
		if (m_CivilSpawners == null)
			m_CivilSpawners = new List<Transform> ();
		return m_CivilSpawners;
	}
	public List<Transform> GetFanSpawners()
	{
		if (m_FanSpawners == null)
			m_FanSpawners = new List<Transform> ();
		return m_FanSpawners;
	}


}
