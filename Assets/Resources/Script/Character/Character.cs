using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	// Attributes
	protected Rigidbody m_RigidBody;
	protected Transform m_Body;
	protected Transform m_Head;
	protected Collider m_BodyCollider;
	protected NavMeshAgent m_NavAgent;

	protected Coroutine m_Walk;
	protected Coroutine m_Wait;
	protected Coroutine m_TakingHit;

	protected bool m_Immune;
	public float m_Speed=2;
	public float m_StunDuration=0.5f;

	protected virtual void Awake () {
		m_RigidBody = transform.GetComponent<Rigidbody> ();
		m_Body = transform.Find ("Body");
		m_Head = m_Body.Find ("Head");
		m_BodyCollider = m_Body.GetComponent<Collider> ();
		m_NavAgent = transform.GetComponent<NavMeshAgent> ();
		m_NavAgent.speed = m_Speed;
	}

	public virtual void StopActions()
	{
		StopAllCoroutines ();
		m_Wait = null;
		m_Walk = null;
		m_NavAgent.Stop ();
	}

	public virtual IEnumerator TakingHit()
	{
		m_Immune = true;
		m_NavAgent.Stop ();
		Material matBody = m_Body.GetComponent<MeshRenderer> ().materials [0];
		Material matHead = m_Head.GetComponent<MeshRenderer> ().materials [0];
		Color baseColor = matBody.color;
		matBody.color = new Color (1, 0, 0, 1);
		matHead.color = new Color (1, 0, 0, 1);
		yield return new WaitForSeconds (m_StunDuration);
		matBody.color = baseColor;
		matHead.color = baseColor;
		m_NavAgent.Resume ();
		m_TakingHit = null;
		m_Immune = false;
	}

	protected void AlignBody()
	{
		if (m_RigidBody.velocity != Vector3.zero) {
			m_Body.rotation = Quaternion.RotateTowards (m_Body.rotation, Quaternion.LookRotation (m_RigidBody.velocity), 180 * Time.fixedDeltaTime);
		}
	}
	protected void AlignBody(Vector3 direction)
	{
		if(direction != Vector3.zero)
			m_Body.rotation = Quaternion.RotateTowards (m_Body.rotation, Quaternion.LookRotation (direction), 180 * Time.fixedDeltaTime);
	}
	protected void AlignBody(Vector3 direction, float rotationSpeed)
	{
		if(direction != Vector3.zero)
			m_Body.rotation = Quaternion.RotateTowards (m_Body.rotation, Quaternion.LookRotation (direction), rotationSpeed * Time.fixedDeltaTime);
	}

	public Transform GetBody()
	{
		return m_Body;
	}
}
