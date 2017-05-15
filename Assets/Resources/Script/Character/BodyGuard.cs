using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyGuard : Character{

	public int m_Id;
	protected List<Transform> m_CharactersinRange;
	protected Coroutine m_Attack;
	protected Coroutine m_Dash;
	public float m_DashSpeed=20;
	public float m_DashDuration=0.1f;

	// Test
	GameObject m_AttackPrefab;

	// Use this for initialization
	protected override void Awake () {
		base.Awake ();
		InitScript ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	private void InitScript()
	{
		GameManager.Instance.getBodyGuards ().Add (this);
		m_CharactersinRange = new List<Transform> ();
		// Test
		m_AttackPrefab = Resources.Load ("Prefab/Test/Attack") as GameObject;
	}

	public void Move(Vector3 moveDirection)
	{
		if (m_Attack == null && m_Dash == null) {
			Vector3 moveVector = moveDirection * m_NavAgent.speed * Time.fixedDeltaTime;
			m_NavAgent.Move (moveVector);
			AlignBody (moveVector, m_NavAgent.angularSpeed);
		}
	}
	public void Dash()
	{
		CustomLogger.debug (this, "Dash",CustomLogger.guardLog);
		m_Dash = StartCoroutine (Dashing ());
	}
	public IEnumerator Dashing()
	{
		float dashDuration = m_DashDuration;
		while (dashDuration > 0) {
			Debug.Log ("Dash");
			Vector3 moveVector = m_Body.forward * m_DashSpeed* Time.fixedDeltaTime;
			m_NavAgent.Move (moveVector);
			dashDuration -= Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
		StopActions ();
	}

	public void PushBack()
	{
		CustomLogger.debug (this, "PushBack",CustomLogger.guardLog);
		float halfAttackAngle = 90;
		List<Transform> targets = new List<Transform> ();
		foreach (Transform character in m_CharactersinRange) {

			if (character == null) {
				continue;
			}

			Vector3 lookDir = m_Body.forward;
			lookDir.y = 0;
			lookDir = lookDir.normalized;
			Vector3 toTargetDir = (character.transform.position - this.transform.position);
			toTargetDir.y = 0;
			toTargetDir = toTargetDir.normalized;
			float dotProd = Vector3.Dot (lookDir,toTargetDir);
			if (dotProd >= 0 && halfAttackAngle >= (90 - 90 * dotProd))
				targets.Add (character);
		}
		foreach (Transform target in targets) {
			CustomLogger.debug (this, "pushing : " + target.transform.name,CustomLogger.guardLog);
			if (target == null) {
				continue;
			}
			target.transform.GetComponent<IDamageable> ().PushBack (this.transform.position);
		}
	}

	public void Attack()
	{
		CustomLogger.debug (this, "Attack",CustomLogger.guardLog);
		m_Attack = StartCoroutine (Attacking ());
		float halfAttackAngle = 90;
		List<Transform> targets = new List<Transform> ();
		foreach (Transform character in m_CharactersinRange) {

			if (character == null) {
				continue;
			}

			Vector3 lookDir = m_Body.forward;
			lookDir.y = 0;
			lookDir = lookDir.normalized;
			Vector3 toTargetDir = (character.transform.position - this.transform.position);
			toTargetDir.y = 0;
			toTargetDir = toTargetDir.normalized;
			float dotProd = Vector3.Dot (lookDir,toTargetDir);
			if (dotProd >= 0 && halfAttackAngle >= (90 - 90 * dotProd))
				targets.Add (character);
		}
		foreach (Transform target in targets) {
			CustomLogger.debug (this, "hitting : " + target.transform.name,CustomLogger.guardLog);
			if (target == null) {
				continue;
			}
			target.transform.GetComponent<IDamageable> ().ApplyDamage (1);

		}
	}

	public IEnumerator Attacking()
	{
		CustomLogger.debug (this, "Attacking",CustomLogger.guardLog);
		Vector3 spawnPos = this.transform.position + m_Body.forward;
		Instantiate (m_AttackPrefab, spawnPos, Quaternion.identity);

		yield return new WaitForSeconds (0.2f);
		StopActions ();
	}

	public override void StopActions()
	{
		CustomLogger.debug (this, "stop actions", CustomLogger.guardLog);
		base.StopActions ();
		m_Attack = null;
		m_Dash = null;
	}

	protected void OnTriggerEnter(Collider collider)
	{
		CustomLogger.debug (this, "OnTriggerEnter, collider = " + collider.transform.name,CustomLogger.guardLog);
		if (collider.transform.name == "Body" && collider.transform.parent.GetComponent<IDamageable> () != null) {
			Transform character = collider.transform.parent;
			if (character != null) {
				CustomLogger.debug (this, "adding character : " + character.name,CustomLogger.guardLog);
				m_CharactersinRange.Add (character);
			}
		}
	}
	protected void OnTriggerExit(Collider collider)
	{
		CustomLogger.debug (this, "OnTriggerExit, collider = " + collider.transform.name,CustomLogger.guardLog);
		if (collider.transform.name == "Body" && collider.transform.parent.GetComponent<IDamageable> () != null) {
			Transform character = collider.transform.parent;
			if (character != null) {
				CustomLogger.debug (this, "removing character : " + character.name, CustomLogger.guardLog);
				m_CharactersinRange.Remove (character);
			}
		}
	}
}
