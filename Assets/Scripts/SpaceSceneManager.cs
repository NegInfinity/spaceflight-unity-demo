using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpaceBody{
	public DVec3 position = DVec3.zero;
	public SpaceObjectData prefab = null;
	public string name;
	public bool camera = false;
	public bool hidden = false;
	public double radius = 1.0;

	public GameObject[] spawnedObjects = new GameObject[0];

	public void destroyObjects(){
		for(int i = 0; i < spawnedObjects.Length; i++){
			GameObject.Destroy(spawnedObjects[i]);
			spawnedObjects[i] = null;
		} 
	}
}

public class SpaceSceneManager : MonoBehaviour{
	[SerializeField] string[] layerNames = new string[0];

	[SerializeField] DVec3 originPosition = DVec3.zero;
	[SerializeField] double scaleFactor = 8000.0;

	[SerializeField] SpaceBody[] objects = new SpaceBody[0];

	[SerializeField] int[] layerIndexes = new int[0];
	[SerializeField] LayerMask[] layerMasks = new LayerMask[0];

	void Start(){
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
	// Update is called once per frame
	void Update(){
		updateObjects();		
	}

	static void moveToLayer(GameObject obj, int layer){
		if (!obj)
			return;
		obj.layer = layer;
		foreach(Transform cur in obj.transform){
			moveToLayer(cur.gameObject, layer);
		}
	}

	void updateObject(SpaceBody obj){
		var diff = obj.position - originPosition;
		var dist = diff.magnitude;

		int numLayers = layerMasks.Length;

		if (numLayers != obj.spawnedObjects.Length){
			obj.destroyObjects();
			obj.spawnedObjects = new GameObject[numLayers];
		}

		var radius = 1.0;
		if (obj.prefab)
			radius = obj.radius;
		var minDist = dist - radius;
		var maxDist = dist - radius;

		var absDiff = DVec3.abs(diff);
		var absMin = absDiff - new DVec3(radius, radius, radius);
		var absMax = absDiff + new DVec3(radius, radius, radius);

		double currentScale = 1.0;
		double farClip = scaleFactor;
		double nearClip = 0.0;

		for(int curIndex = 0; curIndex < layerMasks.Length; curIndex++){
			var curLayer = layerIndexes[curIndex];		
			var curLayerMask = layerMasks[curIndex];
			var clipped = ((absMax.x < nearClip) && (absMax.y < nearClip) && (absMax.z < nearClip));

			if (curIndex < (layerMasks.Length - 1))
				clipped = clipped || (absMin.x > farClip) || (absMin.y > farClip) || (absMin.z > farClip);

			if (obj.hidden)
				clipped = true;

			if (clipped){
				if (obj.spawnedObjects[curIndex]){
					Destroy(obj.spawnedObjects[curIndex]);
					obj.spawnedObjects[curIndex] = null;
				}
			}
			else{
				if (obj.prefab){
					var prefab = obj.prefab;
					double scale = obj.radius / (prefab.prefabRadius * currentScale);
					var localPos = diff / currentScale;

					if (!obj.spawnedObjects[curIndex]){
						var displayObj = Instantiate<GameObject>(obj.prefab.gameObject);
						moveToLayer(displayObj, curLayer);
						obj.spawnedObjects[curIndex] = displayObj;

						var lights = displayObj.GetComponentsInChildren<Light>();
						for(int i = 0; i < lights.Length; i++){
							var curLight = lights[i];
							if (!curLight)
								continue;
							curLight.cullingMask = curLayerMask;
							curLight.range *= (float)scale;
						}

						displayObj.name = string.Format("{0}: layer {1}", prefab.gameObject.name, curIndex);
					}

					var displayObject = obj.spawnedObjects[curIndex];
					var spawnedData = displayObject.GetComponent<SpawnedSpaceObjectData>();
					if (!spawnedData){
						spawnedData = displayObject.AddComponent<SpawnedSpaceObjectData>();
					}
					spawnedData.modelScale = scale;
					spawnedData.layerIndex = curIndex;
					spawnedData.scaleFactor = currentScale;
					spawnedData.radius = obj.radius;

					displayObject.transform.localScale = new Vector3((float)scale, (float)scale, (float)scale);
					displayObject.transform.position = new Vector3((float)localPos.x, (float)localPos.y, (float)localPos.z);
				}
			}			

			currentScale *= scaleFactor;
			nearClip = farClip;
			farClip *= scaleFactor;
		}
	}

	void updateObjects(){
		for (int i = 0; i < objects.Length; i++){
			 var cur = objects[i];
			 updateObject(cur);
		}
	}
}
