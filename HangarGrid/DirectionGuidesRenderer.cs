using System;
using UnityEngine;
using System.Collections.Generic;

namespace HangarGrid
{
	/// <summary>
	/// A special renderer that wraps 3 LineRenderers each for its own direction guide 
	/// </summary>
	public class DirectionGuidesRenderer : MonoBehaviour
	{
		
		float guideLength = 5;
		Material lineMat = new Material (Shader.Find("Legacy Shaders/Particles/Additive"));
		Vector3[] guideLocalDirections = new Vector3[] {Vector3.up, Vector3.forward, Vector3.right};
		Color[] guideColors = new Color[] {new Color(1f, 0f, 0.5f, 1f), new Color(0.3f, 0f, 1f, 1f), Color.red};
		GameObject[] guides;
		
		Dictionary<Vector3, Color> debugPoints = new Dictionary<Vector3, Color>();
		
		public DirectionGuidesRenderer()
		{
			guides = new GameObject[guideLocalDirections.Length];
			for (int i=0; i<guides.Length; i++) {
				guides[i]=initializeLineRenderer();
			}
		}
		
		public void setGuideLegth(float guideLength) {
			this.guideLength = guideLength;
		}
		
		public void drawDebugPoints() {
			foreach (Vector3 p in debugPoints.Keys) {
				Color color;
				debugPoints.TryGetValue(p, out color);
				GUI.color = color;
				GUI.Button(new Rect(p.x-10, Screen.height-p.y-10, 20, 20), "0", HighLogic.Skin.button);
			}
		}
		
		//Returns true if found guide is within range on the screen
		public bool closestGuideLocalProjection(Vector3 screenPoint, int range, out Vector3 localDirection) {
			Vector3 originScreenProjection = Camera.main.WorldToScreenPoint(transform.position);
			originScreenProjection.z = 0; //After projection z contains distance to the screen
			Vector3 hypo = screenPoint - originScreenProjection;
			int foundIndex = -1;
			float minDistance = Screen.width + Screen.height; //just a number that is greater than any distance on the screen
			for (int i=0; i < guides.Length; i++) {
				Vector3 guideScreenProjection = Camera.main.WorldToScreenPoint(transform.TransformPoint(guideLocalDirections[i]));
				guideScreenProjection.z = 0;
				float a = Vector3.Angle(hypo, guideScreenProjection - originScreenProjection);
				float distance = Mathf.Abs( hypo.magnitude * Mathf.Sin(a * Mathf.Deg2Rad));
				if (distance < minDistance) {
					foundIndex = i;
					minDistance = distance;
				}
			}
			localDirection = guideLocalDirections[foundIndex];
			
			Vector3 startProjection = Camera.main.WorldToScreenPoint(transform.TransformPoint(guideLocalDirections[foundIndex] * guideLength));
			startProjection.z = 0;
			Vector3 endProjection = Camera.main.WorldToScreenPoint(transform.TransformPoint(-guideLocalDirections[foundIndex] * guideLength));
			endProjection.z = 0;
			float nearestEndProjectionLength = Vector3.Distance(screenPoint, startProjection) < Vector3.Distance(screenPoint, endProjection) ?
				Vector3.Distance(startProjection, originScreenProjection) :
				Vector3.Distance(endProjection, originScreenProjection);
			float distanceToOriginProjection = Vector3.Distance(screenPoint, originScreenProjection);
			return (minDistance <= range) && (nearestEndProjectionLength + range > distanceToOriginProjection);
		}
		
		protected void OnDestroy() {
			if (guides != null) {
				foreach (GameObject guide in guides) {
					Destroy(guide);
				}
			}
		}
		
		public void updateGuides(Color color) {
			for (int i = 0; i < guideLocalDirections.Length; i++) {
				updateLine(guides[i], guideLocalDirections[i], guideColors[i]);
			}
		}
		
		private GameObject initializeLineRenderer() {
			GameObject gameObject = new GameObject();
			LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
			lineRenderer.material = lineMat;
			lineRenderer.startWidth = 0.04f;
			lineRenderer.endWidth = 0.004f;
			lineRenderer.positionCount = 2;
			return gameObject;
		}
		
		private void updateLine(GameObject gameObject, Vector3 direction, Color color) {
			LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
			lineRenderer.startColor = lineRenderer.endColor = color;
			lineRenderer.SetPosition(0, transform.TransformPoint(-direction * guideLength));
			lineRenderer.SetPosition(1, transform.TransformPoint(direction * guideLength));		
		}
		
	}
}
