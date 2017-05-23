using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Math = System.Math;

public class DoubleTransformManager : MonoBehaviour {
	[SerializeField] string[] layerNames = new string[0];
	[SerializeField] DVec3 originPosition = DVec3.zero;
	[SerializeField] DoubleTransform originObject = null;
	public double timeScale = 50.0;
	public double scaleFactor = 8000.0;
	public double displayLevel = 0.0;

	public DVec3 origin{
		get{
			if (originObject)
				return originObject.position;
			return originPosition;
		}
	}

	[System.Serializable]
	public struct PhysBodyRecord{
		public DoublePhysicsBody phys;
		public DoubleTransform dtr;

		public PhysBodyRecord(DoublePhysicsBody phys_, DoubleTransform dtr_){
			phys = phys_;
			dtr = dtr_;
		}
	}

	[SerializeField] List<PhysBodyRecord> physRecords = new List<PhysBodyRecord>();

	public int numLevels{
		get{
			return layerNames.Length;
		}
	}

	public double getFarClip(int level){
		return getScaleFactor(level + 1);
	}

	public double getNearClip(int level){
		if (level <= 0)
			return 0.0;
		return getScaleFactor(level);
	}

	public double getScaleFactor(int level){
		level = Mathf.Clamp(level, 0, numLevels - 1);
		double result = 1.0;
		for (int i = 0; i < level; i++){
			result *= scaleFactor;
		}
		return result;
	}

	public int getLayerIndex(int level){
		return layerIndexes[level];
	}
	public int getLayerMask(int level){
		return layerMasks[level];
	}
	public string getLayerName(int level){
		return layerNames[level];
	}

	[SerializeField] int[] layerIndexes = new int[0];
	[SerializeField] LayerMask[] layerMasks = new LayerMask[0];


	void buildLayerIndexes(){
		layerIndexes = new int[layerNames.Length];
		layerMasks = new LayerMask[layerNames.Length];
		for(int i = 0; i < layerNames.Length; i++){
			var curName = layerNames[i];
			var curLayer = LayerMask.NameToLayer(curName);
			var curMask = LayerMask.GetMask(curName);
			layerIndexes[i] = curLayer;
			layerMasks[i] = curMask;
		}
	}

	void Start(){
		buildLayerIndexes();
	}
	// Update is called once per frame
	void Update(){
		updateKeys();
		updateObjects();		
		updatePhysics(timeScale * (double)Time.deltaTime);
	}

	void collectPhysicsObjects(Transform curRoot, List<PhysBodyRecord>records){
		for (int i = 0; i < curRoot.childCount; i++){
			var cur = curRoot.GetChild(i);
			if (!cur)
				continue;
			if (!cur.gameObject.activeInHierarchy)
				continue;

			var phys = cur.GetComponent<DoublePhysicsBody>();
			if (phys){
				var dtr = cur.GetComponent<DoubleTransform>();
				if (dtr)
					records.Add(new PhysBodyRecord(phys, dtr));
			}
			collectPhysicsObjects(cur, records);
		}
	}

	void updatePhysics(double dt){
		physRecords.Clear();
		collectPhysicsObjects(transform, physRecords);

		for (int i = 0; i < physRecords.Count; i++){
			var cur = physRecords[i];
			cur.phys.resetForces();
		}

		for (int i = 0; i < physRecords.Count; i++){
			var src = physRecords[i];
			if (!src.phys.createGravity)
				continue;
			for (int j = 0; j < physRecords.Count; j++){
				if (i == j)
					continue;
				var dst = physRecords[j];
				var force = DoublePhysicsBody.computeGravity(src.phys, dst.phys);
				if (src.phys.receiveGravity)
					src.phys.addForce(force);
				if (dst.phys.receiveGravity)
					dst.phys.addForce(-force);
			}
		}

		for(int i = 0; i < physRecords.Count; i++){
			var cur = physRecords[i];
			cur.phys.updatePositions(dt);
		}
	}

	void updateObject(Transform cur){
		if (!cur)
			return;
		var renderObj = cur.GetComponent<DoublePrecisionObject>();
		if (!renderObj)
			return;
		renderObj.updateProxies(this);			
		for(int i = 0; i < cur.childCount; i++){
			updateObject(cur.GetChild(i));
		}
	}

	void updateObjects(){
		for (int i = 0; i < transform.childCount; i++){
			updateObject(transform.GetChild(i));
		}
	}

	double getScaleFactor(double level){
		return Math.Pow(scaleFactor, level);
	}

	public Vector3 getLocalPos(DVec3 pos, double level){
		var diff = pos - origin;
		var scaleFactor = getScaleFactor(level);
		var localDPos = diff/scaleFactor;
		return localDPos.toVector3();
	}
			
	public double getLocalRadius(double radius, double level){
		var scaleFactor = getScaleFactor(level);
		return radius/scaleFactor;
	}

	public Vector3 getLocalPos(DVec3 pos, int level){
		var diff = pos - origin;
		var scaleFactor = getScaleFactor(level);
		var localDPos = diff/scaleFactor;
		return localDPos.toVector3();
	}
			
	public double getLocalRadius(double radius, int level){
		var scaleFactor = getScaleFactor(level);
		return radius/scaleFactor;
	}

	void drawTransformGizmos(Transform cur){
		var planetColor = Color.white;
		var velocityColor = Color.red;

		if (!cur)
			return;
		if (!cur.gameObject.activeInHierarchy)
			return;

		var dtr = cur.GetComponent<DoubleTransform>();
		if (!dtr)
			return;

		var renderObj = cur.GetComponent<DoublePrecisionObject>();
		var objPos = dtr.position;
		var localPos = getLocalPos(dtr.position, displayLevel);

		Gizmos.color = planetColor;
		var localParentPos = getLocalPos(dtr.getParentPosition(), displayLevel);
		Gizmos.DrawLine(localParentPos, localPos);
		//Gizmos.DrawLine(Vector3.zero, localPos);
		double sphereRadius = 1.0/getScaleFactor(displayLevel);
		if (renderObj){
			sphereRadius = getLocalRadius(renderObj.radius, displayLevel);
			Gizmos.DrawWireSphere(localPos, (float)sphereRadius);
		}

		if (!dtr.hasParent()){
			var c = Gizmos.color;
			c.a *= 0.5f;
			Gizmos.color = c;
			Gizmos.DrawLine(transform.position, localPos);
		}

		var rotation = dtr.rotation;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(localPos, getLocalPos(dtr.position + rotation * DVec3.right * sphereRadius, displayLevel));
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(localPos, getLocalPos(dtr.position + rotation * DVec3.forward * sphereRadius, displayLevel));
		Gizmos.color = Color.green;
			Gizmos.DrawLine(localPos, getLocalPos(dtr.position + rotation * DVec3.up * sphereRadius, displayLevel));

		var phys = dtr.GetComponent<DoublePhysicsBody>();
		if (phys){
			var velocityVec = phys.velocity;
			var velocityPos = objPos + velocityVec * 10.0;

			Gizmos.color = velocityColor;
			Gizmos.DrawLine(localPos, getLocalPos(velocityPos, displayLevel));
		}

		for (int i = 0; i < cur.transform.childCount; i++){
			drawTransformGizmos(cur.GetChild(i));
		}
	}

	void OnDrawGizmos(){
		Gizmos.matrix = transform.localToWorldMatrix;
		var planetColor = Color.white;
		var velocityColor = Color.red;
		for (int i = 0; i < transform.childCount; i++){
			drawTransformGizmos(transform.GetChild(i));
		}
		Gizmos.matrix = Matrix4x4.identity;
	}

	void updateKeys(){
		if (Input.GetKeyDown(KeyCode.KeypadPlus)){
			if (timeScale < 1000000.0)
				timeScale *= 10.0;
		}
		if (Input.GetKeyDown(KeyCode.KeypadMinus)){
			if (timeScale > 1.0)
				timeScale /= 10.0;
		}
	}
}
