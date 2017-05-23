using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Math = System.Math;

public static class Utility{
	public static int beginHash(){
		return 17;
	}

	public static int combineHashValues(int prevHash, int newHash){
		return prevHash * 23 + newHash;
	}

	public static int combineHash<T>(int prevHash, T obj){
		int nextHash = 0;
		if ((object)obj != null)
			nextHash = obj.GetHashCode();
		return combineHashValues(prevHash, nextHash);
	}
}

[System.Serializable]
public struct DQuat{
	public double x;
	public double y;
	public double z;
	public double w;

	public override string ToString(){
		return string.Format("DQuat({0}, {1}, {2}, {3})", x, y, z, w);
	}

	static public DQuat identity{
		get{
			return new DQuat(0.0, 0.0, 0.0, 1.0);
		}
	}

	public DQuat(double x_, double y_, double z_, double w_){
		x = x_;
		y = y_;
		z = z_;
		w = w_;
	}

	public void set(double x_, double y_, double z_, double w_){
		x = x_;
		y = y_;
		z = z_;
		w = w_;
	}

	public static bool operator==(DQuat a, DQuat b){
		return (a.x == b.x) && (a.y == b.y) && (a.z == b.z) && (a.w == b.w);
	}

	public static bool operator!=(DQuat a, DQuat b){
		return !(a == b);
	}

	public static double dot(DQuat a, DQuat b){
		return (a.x * b.x) + (a.y * b.y) + (a.z * b.z) + (a.w * b.w);
	}

	public static DVec3 operator*(DQuat q, DVec3 v){
		var qv = new DQuat(v.x, v.y, v.z, 0.0);
		DQuat result = q * qv * q.conjugate;
		return new DVec3(result.x, result.y, result.z);
	}

	public static DQuat operator*(DQuat q, double f){
		return new DQuat(q.x * f, q.y * f, q.z * f, q.w * f);
	}

	public static DQuat operator/(DQuat q, double f){
		return new DQuat(q.x/f, q.y/f, q.z/f, q.w/f);
	}

	public static DQuat operator+(DQuat a, DQuat b){
		return new DQuat(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
	}

	public static DQuat operator-(DQuat a, DQuat b){
		return new DQuat(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
	}

	public double sqrMagnitude{
		get{
			return dot(this, this);
		}
	}

	public double magnitude{
		get{
			return Math.Sqrt(sqrMagnitude);			
		}
	}

	public DQuat conjugate{
		get{
			return new DQuat(-x, -y, -z, w);
		}
	}

	public DQuat normalized{
		get{
			var l = this.magnitude;
			return new DQuat(x/l, y/l, z/l, w/l);
		}
	}

	public static DQuat fromAxisAngleRad(DVec3 axis, double radAngle){
		axis.normalize();

		double halfAngle = radAngle * 0.5;
		double sin = Math.Sin(halfAngle);
		double cos = Math.Cos(halfAngle);

		return new DQuat(axis.x * sin, axis.y * sin, axis.z * sin, cos);
	}

	public static DQuat fromAxisAngle(DVec3 axis, double angle){
		return fromAxisAngleRad(axis, angle * Math.PI/180.0);
	}

	public override int GetHashCode(){
		int result = Utility.beginHash();
		result = Utility.combineHashValues(result, x.GetHashCode());
		result = Utility.combineHashValues(result, y.GetHashCode());
		result = Utility.combineHashValues(result, z.GetHashCode());
		result = Utility.combineHashValues(result, w.GetHashCode());
		return result;
	}

	public override bool Equals(object other){
		if (! (other is DQuat))
			return false;
		DQuat v = (DQuat)other;

		return v.x.Equals(x) && v.y.Equals(y) && v.z.Equals(z) && v.w.Equals(w);
	}

	public static DQuat operator*(DQuat l, DQuat r){
		return new DQuat(
			l.w*r.x + l.x*r.w + l.y*r.z - l.z*r.y, 
			l.w*r.y + l.y*r.w + l.z*r.x - l.x*r.z, 
			l.w*r.z + l.z*r.w + l.x*r.y - l.y*r.x, 
			l.w*r.w - l.x*r.x - l.y*r.y - l.z*r.z
		);
	}

	public DVec3 left{
		get{
			return this * DVec3.left;
		}
	}
	public DVec3 right{
		get{
			return this * DVec3.right;
		}
	}
	public DVec3 up{
		get{
			return this * DVec3.up;
		}
	}
	public DVec3 down{
		get{
			return this * DVec3.down;
		}
	}
	public DVec3 forward{
		get{
			return this * DVec3.forward;
		}
	}
	public DVec3 back{
		get{
			return this * DVec3.back;
		}
	}

	public static DQuat lerp(DQuat a, DQuat b, double t){
		return new DQuat(
			(b.x - a.x) * t + a.x,
			(b.y - a.y) * t + a.y,
			(b.z - a.z) * t + a.z,
			(b.w - a.w) * t + a.w
		).normalized;
	}

	public static DQuat slerp(DQuat a, DQuat b, double t){
		double dotVal = dot(a, b);
		double angle = Math.Acos(dotVal);

		return (a * Math.Sin(angle * (1.0 - t)) + b*Math.Sin(angle*t))/Math.Sin(angle);
	}

	public Quaternion toQuaternion(){
		return new Quaternion((float)x, (float)y, (float)z, (float)w);
	}
}
