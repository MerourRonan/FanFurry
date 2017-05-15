using UnityEngine;
using System.Collections;

public interface IDamageable {

	void ApplyDamage(int damage);
	void PushBack(Vector3 bodyguardPosition);
}
