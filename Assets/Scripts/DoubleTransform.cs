using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTransform: MonoBehaviour{
	public DVec3 localPosition = DVec3.zero;
	public DQuat localRotation = DQuat.identity;

	DVec3 getPosition(){
		return getParentRotation() * localPosition + getParentPosition();		
		//return DVec3.zero;
	}

	DQuat getRotation(){
		return getParentRotation() * localRotation;
	}

	public bool hasParent(){
		if (!transform.parent)
			return false;
		return transform.parent.GetComponent<DoubleTransform>();
	}

	public DQuat getParentRotation(){
		if (!transform.parent){
			return DQuat.identity;
		}

		var parentTransform = transform.parent.GetComponent<DoubleTransform>();
		if (!parentTransform)
			return DQuat.identity;
		return parentTransform.getRotation();
	}

	public DVec3 getParentPosition(){
		if (!transform.parent){
			return DVec3.zero;
		}

		var parentTransform = transform.parent.GetComponent<DoubleTransform>();
		if (!parentTransform)
			return DVec3.zero;
		return parentTransform.getPosition();
	}

	void setRotation(DQuat rotation){
		localRotation = rotation * getParentRotation().conjugate;
	}

	void setPosition(DVec3 position){
		localPosition = getParentRotation().conjugate * (position - getParentPosition());
	}

	public DVec3 position{
		get{
			return getPosition();
		}
		set{
			setPosition(value);
		}
	}

	public DQuat rotation{
		get{
			return getRotation();
		}
		set{
			setRotation(value);
		}
	}
}
