using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StringBuilder = System.Text.StringBuilder;

public class MathVerification : MonoBehaviour {

	// Use this for initialization
	void Start (){
		runTests();		
	}

	StringBuilder sb = new StringBuilder();

	void compareResults<T1, T2>(T1 a1, T1 b1, T2 a2, T2 b2, System.Func<T1, T1, T1> func1, System.Func<T2, T2, T2> func2){
		T1 c1 = func1(a1, b1);
		T2 c2 = func2(a2, b2);
		sb.AppendFormat("Op1: {0}\nOp2: {1}", c1, c2);
	}

	void runTests(){
		sb.Length = 0;
		var d1 = new DVec3(1.0, 2.0, 3.0);
		var d2 = new DVec3(4.0, 5.0, 6.0);
		var f1 = d1.toVector3();
		var f2 = d2.toVector3();

		Quaternion[] q = {
			new Quaternion(1.0f, 0.0f, 0.0f, 0.0f),
			new Quaternion(0.0f, 1.0f, 0.0f, 0.0f),
			new Quaternion(0.0f, 0.0f, 1.0f, 0.0f),
			new Quaternion(0.0f, 0.0f, 0.0f, 1.0f)
		};

		DQuat[] dq = {
			new DQuat(1.0, 0.0, 0.0, 0.0),
			new DQuat(0.0, 1.0, 0.0, 0.0),
			new DQuat(0.0, 0.0, 1.0, 0.0),
			new DQuat(0.0, 0.0, 0.0, 1.0)
		};

		Vector3[] v= {
			Vector3.left,
			Vector3.right,
			Vector3.up,
			Vector3.down,
			Vector3.forward,
			Vector3.back
		};

		DVec3[] dv= {
			DVec3.left,
			DVec3.right,
			DVec3.up,
			DVec3.down,
			DVec3.forward,
			DVec3.back
		};

		for(int i = 0; i < q.Length; i++){
			for (int j = 0; j < dq.Length; j++){
				sb.AppendFormat("{0} * {1} = {2}\n", q[i], q[j], q[i]*q[j]);
				sb.AppendFormat("{0} * {1} = {2}\n\n", dq[i], dq[j], dq[i]*dq[j]);
			}
		}
		sb.AppendFormat("\n\n");

		for(int i = 0; i < q.Length; i++){
			for (int j = 0; j < v.Length; j++){
				sb.AppendFormat("{0} * {1} = {2}\n", q[i], v[j], q[i]*v[j]);
				sb.AppendFormat("{0} * {1} = {2}\n\n", dq[i], dv[j], dq[i]*dv[j]);

			}
		}
		/*
		for(int i = 0; i < dq.Length; i++){
			for (int j = 0; j < dv.Length; j++){
			}
		}
		*/

		Debug.Log(sb);
	}	
}
