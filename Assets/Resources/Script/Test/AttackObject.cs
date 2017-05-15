using UnityEngine;
using System.Collections;

public class AttackObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Destroy (this.gameObject,0.2f);
	}
}
