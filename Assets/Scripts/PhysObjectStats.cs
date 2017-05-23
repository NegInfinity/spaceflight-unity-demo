using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysObjectStats: MonoBehaviour{
	[SerializeField] DoublePhysicsBody trackedObject = null;
	[SerializeField] DoublePhysicsBody relativeObject = null;
	System.Text.StringBuilder sb = new System.Text.StringBuilder();

	// Update is called once per frame
	void Update(){
		if (!trackedObject)
			return;

		var dtr = trackedObject.GetComponent<DoubleTransform>();
		if (!dtr)
			return;
		sb.Length = 0;
		sb.AppendFormat("Object:\n\t{0}\n", dtr.gameObject.name);

		var manager = trackedObject.GetComponentInParent<DoubleTransformManager>();
		if (manager){
			sb.AppendFormat("Timescale: {0}\n", manager.timeScale);
		}

		sb.AppendFormat("Mass:\n\t{0}\n", trackedObject.mass);
		sb.AppendFormat("Absolute Velocity:\n\t{0}\n", trackedObject.velocity);
		sb.AppendFormat("Absolute Linear Velocity:\n\t{0}\n", trackedObject.velocity.magnitude);
		if (relativeObject){
			var relVelocity = trackedObject.velocity - relativeObject.velocity;
			sb.AppendFormat("Relative Velocity ({1}):\n\t{0}\n", relVelocity, relativeObject.gameObject.name);
			sb.AppendFormat("Relative Linear Velocity ({1}):\n\t{0}\n", relVelocity.magnitude, relativeObject.gameObject.name);					
		}
		sb.AppendFormat("Absolute Accel:\n\t{0}\n", trackedObject.lastAcceleration);
		sb.AppendFormat("Absolute Linear Accel:\n\t{0}\n", trackedObject.lastAcceleration.magnitude);
		sb.AppendFormat("Coordinates:\n\t{0}\n", dtr.position);
		if (relativeObject){
			var relDtr = relativeObject.GetComponent<DoubleTransform>();
			if (relDtr){
				var relPos = dtr.position - relDtr.position;
				sb.AppendFormat("Relative Coordinates ({1}):\n\t{0}\n", relPos, relativeObject.gameObject.name);
			}
		}

		var text = GetComponent<UnityEngine.UI.Text>();
		if (!text)
			return;
		text.text = sb.ToString();
	}
}
