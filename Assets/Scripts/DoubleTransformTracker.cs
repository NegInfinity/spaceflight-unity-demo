using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTransformTracker : MonoBehaviour {
	[SerializeField] DoubleTransformManager manager = null;
	[SerializeField] DoubleTransform transformObj = null;

	void LateUpdate(){
		if (!manager)
			return;
		transform.position = manager.transform.position;
		if (!transformObj)
			return;
		transform.rotation = manager.transform.rotation * transformObj.rotation.normalized.toQuaternion();
	}
}
