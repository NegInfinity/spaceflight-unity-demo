using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DVec3{
	public double x;
	public double y;
	public double z;

	static public DVec3 zero{
		get{
			return new DVec3(0.0, 0.0, 0.0);
		}
	}

	public static DVec3 back{
		get{
			return new DVec3(0.0, 0.0, -1.0);
		}
	}
	public static DVec3 forward{
		get{
			return new DVec3(0.0, 0.0, 1.0);
		}
	}
	public static DVec3 up{
		get{
			return new DVec3(0.0, 1.0, 0.0);
		}
	}
	public static DVec3 down{
		get{
			return new DVec3(0.0, -1.0, 0.0);
		}
	}
	public static DVec3 left{
		get{
			return new DVec3(-1.0, 0.0, 0.0);
		}
	}
	public static DVec3 right{
		get{
			return new DVec3(1.0, 0.0, 0.0);
		}
	}

	public DVec3(double x_, double y_, double z_){
		x = x_;
		y = y_;
		z = z_;
	}

	public double magnitude{
		get{
			return System.Math.Sqrt(dot(this, this));
		}
	}

	public double sqrMagnitude{
		get{
			return dot(this, this);
		}
	}

	public DVec3 normalized{
		get{
			var l = this.magnitude;
			if (l == 0.0)
				return DVec3.zero;
			return this/l;
		}
	}

	public override string ToString(){
		return string.Format("DVec3({0}, {1}, {2})", x, y, z);
	}

	public static double dot(DVec3 a, DVec3 b){
		return a.x * b.x + a.y * b.y + a.z * b.z;
	}

	public static DVec3 add(DVec3 a, DVec3 b){
		return new DVec3(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static DVec3 subtract(DVec3 a, DVec3 b){
		return new DVec3(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static DVec3 scale(DVec3 arg, double factor){
		return new DVec3(arg.x * factor, arg.y * factor, arg.z*factor);
	}

	public static DVec3 scale(DVec3 a, DVec3 b){
		return new DVec3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	static public DVec3 abs(DVec3 arg){
		return new DVec3(System.Math.Abs(arg.x), System.Math.Abs(arg.y), System.Math.Abs(arg.z));
	}

	static public DVec3 operator- (DVec3 a, DVec3 b){
		return subtract(a, b);
	}

	static public DVec3 operator+ (DVec3 a, DVec3 b){
		return add(a, b);
	}

	public static DVec3 operator/ (DVec3 a, double d){
		return new DVec3(a.x / d, a.y /d, a.z/d);
	}

	public static DVec3 operator* (DVec3 a, double f){
		return new DVec3(a.x*f, a.y*f, a.z*f);
	}

	public static double operator| (DVec3 a, DVec3 b){
		return dot(a, b);
	}

	public static DVec3 operator- (DVec3 arg){
		return new DVec3(-arg.x, -arg.y, -arg.z);
	}

	public void normalize(){
		this = this.normalized;
	}

	public static double distance(DVec3 a, DVec3 b){
		return (b - a).magnitude;
	}

	public double distanceTo(DVec3 arg){
		return distance(this, arg);
	}

	public static bool operator==(DVec3 a, DVec3 b){
		return (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
	}

	public static bool operator!=(DVec3 a, DVec3 b){
		return !(a == b);
	}

	public override int GetHashCode(){
		int result = Utility.beginHash();
		result = Utility.combineHashValues(result, x.GetHashCode());
		result = Utility.combineHashValues(result, y.GetHashCode());
		result = Utility.combineHashValues(result, z.GetHashCode());
		return result;
	}

	public override bool Equals(object other){
		if (! (other is DVec3))
			return false;
		DVec3 v = (DVec3)other;

		return v.x.Equals(x) && v.y.Equals(y) && v.z.Equals(z);
	}

	public Vector3 toVector3(){
		return new Vector3((float)x, (float)y, (float)z);
	}
}

