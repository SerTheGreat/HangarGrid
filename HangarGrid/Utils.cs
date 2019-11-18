using System;
using UnityEngine;

namespace HangarGrid
{
	/// <summary>
	/// Contains different static utility methods.
	/// </summary>
	public class Utils
	{
		
		public static Vector3 closestAxis(Vector3 vector, Transform transform) {
			Vector3[] axis = new Vector3[] {transform.up, transform.forward, transform.right};
			float minAngle = 360;
			Vector3 foundAxis = Vector3.zero;
			foreach (Vector3 ax in axis) {
				float ang = Math.Abs(directionIndependentAngle(Vector3.Angle(vector, ax)));
				if ((foundAxis.IsZero()) || (ang < minAngle)) {
					foundAxis = ax;
					minAngle = ang;
				}
			}
			return foundAxis;
		}
		
		public static float directionIndependentAngle(float angle) {
			if (Math.Abs(angle) < 90) {
				return angle;
			} else {
				return angle - 180;
			}
		}
		
		public static float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n){
			// angle in [0,180]
			float angle = Vector3.Angle(a,b);
			float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));
			
			// angle in [-179,180]
			float signed_angle = angle * sign;
    		return signed_angle;
		}
		
		public void drawDebugLine(Part part, Vector3 end) {
			GameObject gameObject = part.gameObject;
			Vector3 start = part.transform.position;
			LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
			if (lineRenderer == null) {
				lineRenderer = gameObject.AddComponent<LineRenderer>();
				lineRenderer.material = new Material (Shader.Find("Particles/Additive"));
				lineRenderer.positionCount = 2;
			}
			lineRenderer.startWidth = 0.05f;
			lineRenderer.endWidth = 0.01f;
			lineRenderer.startColor = lineRenderer.endColor = Color.white;
			lineRenderer.SetPosition(0, start);
			lineRenderer.SetPosition(1, end);
		}
		
	}
}
