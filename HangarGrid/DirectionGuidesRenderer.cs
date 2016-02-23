using System;
using UnityEngine;

namespace HangarGrid
{
	/// <summary>
	/// A special renderer that wraps 3 LineRenderers each for its own direction guide 
	/// </summary>
	public class DirectionGuidesRenderer : MonoBehaviour
	{
		
		Material lineMat = new Material (Shader.Find("Particles/Additive"));
		GameObject[] guides;
		
		public DirectionGuidesRenderer()
		{
			guides = new GameObject[3];
			for (int i=0; i<guides.Length; i++) {
				guides[i]=initializeLineRenderer();
			}
		}
		
		protected void OnDestroy() {
			if (guides != null) {
				foreach (GameObject guide in guides) {
					Destroy(guide);
				}
			}
		}
		
		public void updateGuides(Transform transform, Color color, float length) {
			updateLine(guides[0], transform, Vector3.up, color, length);
			updateLine(guides[1], transform, Vector3.right, color, length);
			updateLine(guides[2], transform, Vector3.forward, color, length);
		}
		
		private GameObject initializeLineRenderer() {
			GameObject gameObject = new GameObject();
			LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
			lineRenderer.material = lineMat;
			lineRenderer.SetWidth(0.03f,0.01f);
			lineRenderer.SetVertexCount(2);
			return gameObject;
		}
		
		private void updateLine(GameObject gameObject, Transform transform, Vector3 direction, Color color, float length) {
			LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
			lineRenderer.SetColors(color, color);
			lineRenderer.SetPosition(0, transform.TransformPoint(-direction * length));
			lineRenderer.SetPosition(1, transform.TransformPoint(direction * length));		
		}
		
	}
}
