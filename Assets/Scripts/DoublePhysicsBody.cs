using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DoubleTransform))]
public class DoublePhysicsBody: MonoBehaviour{
	public double mass = 1.0;
	public bool createGravity = false;
	public bool receiveGravity = true;
	public DVec3 velocity = DVec3.zero;

	public DVec3 accumulatedForce = DVec3.zero;
	public DVec3 lastAcceleration = DVec3.zero;
	public double lastLinearVel = 0.0;
	public double lastLinearAccel = 0.0;
	public double lastLinearForce = 0.0;

	public static readonly double gravitationalConstant = 6.67E-11;

	public static DVec3 computeGravity(DoublePhysicsBody src, DoublePhysicsBody dst){
		var srcComp = src.GetComponent<DoubleTransform>();
		var dstComp = dst.GetComponent<DoubleTransform>();

		var srcPos = srcComp.position;
		var dstPos = dstComp.position;

		var diff = dstPos - srcPos;
		var r2 = DVec3.dot(diff, diff);

		double fMagnitude = gravitationalConstant * src.mass * dst.mass / r2;

		var result = diff.normalized * fMagnitude;
		return result;
	}

	public void updatePositions(double deltaT){
		var a = getAcceleration();
		lastAcceleration = a;
		lastLinearVel = velocity.magnitude;
		lastLinearAccel = a.magnitude;
		lastLinearForce = accumulatedForce.magnitude;

		var dtr = GetComponent<DoubleTransform>();

		var nextPos = dtr.position + a * deltaT * deltaT * 0.5 + velocity * deltaT;
		var nextVelocity = velocity + a * deltaT;
		velocity = nextVelocity;
		dtr.position = nextPos;
	}

	public DVec3 getAcceleration(){
		return accumulatedForce/mass;
	}

	public void resetForces(){
		accumulatedForce = DVec3.zero;
	}

	public void addForce(DVec3 force){
		accumulatedForce += force;
	}


}
