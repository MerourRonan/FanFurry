using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

	public static CameraController Instance;
	public List<Transform> m_VisibleTargets;
	protected HashSet<Transform> m_WallNotRendered;
	public float m_CamDistance;
	public float m_CamHeight;

	// Use this for initialization
	void Awake () {
		InitScript ();
	}

	void Update () {
		ResetWallLayer ();
		CheckWallToRender ();
		FollowVip ();
	
	}

	protected void InitScript()
	{
		Instance = this;
		m_WallNotRendered = new HashSet<Transform> ();
		m_VisibleTargets = new List<Transform> ();
	}

	protected void FollowVip()
	{
		Vector3 camPos = Vip.Instance.transform.position;
		camPos.z -=  m_CamDistance;
		camPos.y +=  m_CamHeight;
		transform.position = camPos;

	}

	protected void CheckWallToRender()
	{
//		m_VisibleTargets.Clear ();
//		ConfigureVisibleTargets ();
//		foreach(Transform target in m_VisibleTargets)
//		{
//			RaycastHit hit;
//			LayerMask layerMask = (1 << LayerMask.NameToLayer ("Wall"));
//			Vector3 dir = target.position - transform.position;
//			if (Physics.Raycast (transform.position, dir, out hit, dir.magnitude, layerMask)) {
//				//Debug.Log ("hit : " + hit.collider.transform.name);
//				Transform wall = hit.collider.transform;
//				m_WallNotRendered.Add (wall);
//				Material m = wall.GetComponent<MeshRenderer> ().materials [0];
//				m.SetFloat ("_Mode", 3);
//				m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
//				m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
//				m.DisableKeyword("_ALPHATEST_ON");
//				m.EnableKeyword("_ALPHABLEND_ON");
//				m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
//				m.renderQueue = 3000;
//			}
//		}
		m_VisibleTargets.Clear ();
		ConfigureVisibleTargets ();
		foreach(Transform target in m_VisibleTargets)
		{
			RaycastHit[] hits;
			LayerMask layerMask = (1 << LayerMask.NameToLayer ("Wall"));
			Vector3 dir = target.position - transform.position;
			hits = Physics.RaycastAll (transform.position, dir, dir.magnitude, layerMask);
			for (int i = 0; i < hits.Length; i++) {
				//Debug.Log ("hit : " + hit.collider.transform.name);
				Transform wall = hits[i].collider.transform;
				m_WallNotRendered.Add (wall);
				Material m = wall.GetComponent<MeshRenderer> ().materials [0];
				m.SetFloat ("_Mode", 3);
				m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				m.DisableKeyword("_ALPHATEST_ON");
				m.EnableKeyword("_ALPHABLEND_ON");
				m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				m.renderQueue = 3000;
			}
		}
	}

	private void ResetWallLayer()
	{
		foreach (Transform wall in m_WallNotRendered) {
			Material m = wall.GetComponent<MeshRenderer> ().materials [0];
			m.SetFloat ("_Mode", 0);
			m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			m.DisableKeyword("_ALPHATEST_ON");
			m.DisableKeyword("_ALPHABLEND_ON");
			m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			m.renderQueue = 3000;
		}
		m_WallNotRendered.Clear ();
	}

	public void ConfigureVisibleTargets()
	{
		m_VisibleTargets.Add (Vip.Instance.transform);
		foreach (BodyGuard guard in GameManager.Instance.getBodyGuards()) {
			m_VisibleTargets.Add (guard.transform);
		}
		foreach (Fan fan in GameManager.Instance.getFans()) {
			m_VisibleTargets.Add (fan.transform);
		}
	}
}
