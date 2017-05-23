using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {
	public float rotationSpeed = 90.0f;
	// Update is called once per frame
	void Update(){
		double rotAngle = (double)Time.deltaTime * rotationSpeed;
		double yaw = 0.0;
		double pitch = 0.0;
		double roll = 0.0;
		bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ||
			Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

		if (Input.GetKey(KeyCode.LeftArrow)){
			if (shift)
				roll -= rotAngle;
			else
				yaw -= rotAngle;
		}
		if (Input.GetKey(KeyCode.RightArrow)){
			if (shift)
				roll += rotAngle;
			else
				yaw += rotAngle;
		}
		Debug.Log(shift);
		if (Input.GetKey(KeyCode.UpArrow)){
			pitch -= rotAngle;
		}
		if (Input.GetKey(KeyCode.DownArrow)){
			pitch += rotAngle;
		}

		if ((yaw == 0.0) && (pitch == 0.0) && (roll == 0.0))
			return;

		var dtr = GetComponent<DoubleTransform>();
		if (!dtr){
			transform.RotateAround(transform.position, transform.up, (float)yaw);
			transform.RotateAround(transform.position, transform.right, (float)pitch);
			return;
		}

		if (yaw != 0.0){
			rotateAroundAxis(dtr, DVec3.up, yaw);
		}
		if (pitch != 0.0){
			rotateAroundAxis(dtr, DVec3.right, pitch);
		}
		if (roll != 0.0){
			rotateAroundAxis(dtr, DVec3.forward, roll);
		}
		//var rot2 = DQuat.fromAxisAngle((dtr.rotation * DVec3.right).normalized, (double)vRot).normalized;
	}

	static void rotateAroundAxis(DoubleTransform dtr, DVec3 localAxis, double angle){
		var axis = (dtr.localRotation * localAxis).normalized;
		var rot = DQuat.fromAxisAngle(axis, angle).normalized;
		dtr.localRotation = (rot * dtr.localRotation).normalized;
	}
}
