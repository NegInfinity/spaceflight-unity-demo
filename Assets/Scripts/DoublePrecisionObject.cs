using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DoubleTransform))]
public class DoublePrecisionObject : MonoBehaviour {
	public double radius = 1.0;
	public double prefabRadius = 1.0;
	public GameObject proxyPrefab = null;

	[SerializeField]
	GameObject[] proxies = new GameObject[0];

	public void destroyProxies(){
		for (int i = 0; i < proxies.Length; i++){
			if (proxies[i])
				Destroy(proxies[i]);
			proxies[i] = null;
		}
	}

	static void moveToLayer(GameObject obj, int layer){
		if (!obj)
			return;
		obj.layer = layer;
		foreach(Transform cur in obj.transform){
			moveToLayer(cur.gameObject, layer);
		}
	}

	public void setNumProxies(int newNumProxies){
		if (proxies.Length != newNumProxies){
			destroyProxies();
			proxies = new GameObject[newNumProxies];
		}
	}

	public void updateProxies(DoubleTransformManager manager){
		var origin = manager.origin;
		var dtr = GetComponent<DoubleTransform>();
		if (!dtr){
			Debug.LogWarningFormat("Double precision transform not present on: {0}", gameObject.name);
			return;
		}

		var position = dtr.position;
		var rotation = dtr.rotation;

		var diff = position - origin;
		var dist = diff.magnitude;

		int numLayers = manager.numLevels;

		setNumProxies(numLayers);

		var minDist = dist - radius;
		var maxDist = dist - radius;

		var absDiff = DVec3.abs(diff);
		var absMin = absDiff - new DVec3(radius, radius, radius);
		var absMax = absDiff + new DVec3(radius, radius, radius);

		int numLevels = manager.numLevels;

		for(int levelIndex = 0; levelIndex < numLevels; levelIndex++){
			double nearClip = manager.getNearClip(levelIndex);
			double farClip = manager.getFarClip(levelIndex);
			double levelScale = manager.getScaleFactor(levelIndex);
			var curLayer = manager.getLayerIndex(levelIndex);
			var curLayerMask = manager.getLayerMask(levelIndex);

			var clipped = ((absMax.x < nearClip) && (absMax.y < nearClip) && (absMax.z < nearClip));

			if (levelIndex < (numLevels - 1))
				clipped = clipped || (absMin.x > farClip) || (absMin.y > farClip) || (absMin.z > farClip);

			if (!gameObject.activeInHierarchy)
				clipped = true;

			if (clipped){
				if (proxies[levelIndex]){
					Destroy(proxies[levelIndex]);
					proxies[levelIndex] = null;
				}
			}
			else{
				if (proxyPrefab){
					var prefab = proxyPrefab;
					double scale = radius / (prefabRadius * levelScale);
					var localPos = diff / levelScale;
					var localRot = dtr.rotation;

					if (!proxies[levelIndex]){
						var displayObj = Instantiate<GameObject>(prefab);
						moveToLayer(displayObj, curLayer);
						proxies[levelIndex] = displayObj;

						var lights = displayObj.GetComponentsInChildren<Light>();
						for(int i = 0; i < lights.Length; i++){
							var curLight = lights[i];
							if (!curLight)
								continue;
							curLight.cullingMask = curLayerMask;
							curLight.range *= (float)scale;
						}

						displayObj.name = string.Format("{0}: layer {1}", prefab.gameObject.name, levelIndex);
						displayObj.transform.SetParent(manager.gameObject.transform, false);
					}

					var displayObject = proxies[levelIndex];

					displayObject.transform.localScale = new Vector3((float)scale, (float)scale, (float)scale);
					displayObject.transform.localPosition = localPos.toVector3();
					displayObject.transform.localRotation = localRot.normalized.toQuaternion();
				}
			}			
		}
	}
}
