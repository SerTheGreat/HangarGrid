using System;
using System.Collections.Generic;
using UnityEngine;

namespace HangarGrid
{
	/// <summary>
	/// Manages everything related to grid visualization
	/// </summary>
	public class GridManager
	{
		
		private bool gridEnabled = false;
		
		int numberOfLines;
		float step = 1f;
		Material lineMaterial = new Material (Shader.Find("Particles/Additive"));
		Bounds bounds;
		GameObject[] verticalXOYLines;
		GameObject[] horizontalXOYLines;
		GameObject[] verticalYOZLines;
		GameObject[] horizontalYOZLines;
		GameObject[] lateralXOZLines;
		GameObject[] longitudalXOZLines;
		GameObject[] axisLines;
		
		public void showGrid(Bounds bounds) {
			if (gridEnabled) {
				return;
			}
			gridEnabled = true;
			this.bounds = bounds;
			float distance = Mathf.Max(new float[] {bounds.size.x, bounds.size.y, bounds.size.z});
			numberOfLines = 2 * (Mathf.RoundToInt(Math.Abs(distance) / step) + 1);
			verticalXOYLines = new GameObject[numberOfLines];
			horizontalXOYLines = new GameObject[numberOfLines];
			verticalYOZLines = new GameObject[numberOfLines];
			horizontalYOZLines = new GameObject[numberOfLines];
			lateralXOZLines = new GameObject[numberOfLines];
			longitudalXOZLines = new GameObject[numberOfLines];
			axisLines = new GameObject[3];
			
			for (int i = 0; i < numberOfLines; i++) {
				verticalXOYLines[i] = initializeLineRenderer(Color.green, 0.5f);;
				horizontalXOYLines[i] = initializeLineRenderer(Color.green, 0.5f);
				verticalYOZLines[i] = initializeLineRenderer(Color.cyan, 0.5f);
				horizontalYOZLines[i] = initializeLineRenderer(Color.cyan, 0.5f);
				lateralXOZLines[i] = initializeLineRenderer(Color.yellow, 0.5f);
				longitudalXOZLines[i] = initializeLineRenderer(Color.yellow, 0.5f);
			}
			
			for (int i=0; i < axisLines.Length; i++) {
				axisLines[i] = initializeLineRenderer(Color.green, 0.5f);
			}
		}
		
		public void hideGrid() {
			if (!gridEnabled) {
				return;
			}
			gridEnabled = false;
			clearGrid();
		}
		
		private GameObject initializeLineRenderer(Color baseColor, float opacity) {
				GameObject gameObject = new GameObject();
				LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
				lineRenderer.material = lineMaterial;
				lineRenderer.SetVertexCount(2);
				return gameObject;
		}
		
		public void updateGrid(Part part) {
			if (!gridEnabled) {
				return;
			}
			for (int i=0; i<numberOfLines; i++) {
				int positionIndex = i - numberOfLines / 2 + (i + 1 > numberOfLines / 2 ? 1 : 0);
				updateLine(verticalXOYLines[i], Vector3.right * positionIndex * step, Vector3.up, 0.005f, Color.green, 0.5f, part.transform, bounds);
				updateLine(verticalYOZLines[i], Vector3.forward * positionIndex * step, Vector3.up, 0.005f, Color.cyan, 0.5f, part.transform, bounds);
			    updateLine(lateralXOZLines[i], Vector3.forward * positionIndex * step, Vector3.right, 0.005f, Color.yellow, 0.5f, part.transform, bounds);
				updateLine(horizontalXOYLines[i], Vector3.up * positionIndex * step, Vector3.right, 0.005f, Color.green, 0.5f, part.transform, bounds);
				updateLine(horizontalYOZLines[i], Vector3.up * positionIndex * step, Vector3.forward, 0.005f, Color.cyan, 0.5f, part.transform, bounds);
			    updateLine(longitudalXOZLines[i], Vector3.right * positionIndex * step, Vector3.forward, 0.005f, Color.yellow, 0.5f, part.transform, bounds);
			}
			
			updateLine(axisLines[0], Vector3.zero, Vector3.up, 0.03f, Color.green, 0.5f, part.transform, bounds);
			updateLine(axisLines[1], Vector3.zero, Vector3.right, 0.03f, Color.green, 0.5f, part.transform, bounds);
			updateLine(axisLines[2], Vector3.zero, Vector3.forward, 0.03f, Color.green, 0.5f, part.transform, bounds);
		}
		
		private void updateLine(GameObject gameObject, Vector3 localPosition, Vector3 localDirection, float width, Color baseColor, float opacity, Transform transform, Bounds bounds) {
			LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
			Ray rayToStart = new Ray(transform.TransformPoint(localPosition), transform.TransformDirection(-localDirection));
			Ray rayToEnd = new Ray(transform.TransformPoint(localPosition), transform.TransformDirection(localDirection));
			float lengthToEnd;
			float lengthToStart;
			bool startFits = bounds.IntersectRay(rayToStart, out lengthToStart);
			bool endFits = bounds.IntersectRay(rayToEnd, out lengthToEnd);
			//Lengths are returned with inverse sign
			lineRenderer.SetPosition(0, rayToStart.GetPoint(lengthToStart));
			lineRenderer.SetPosition(1, rayToEnd.GetPoint(lengthToEnd));
			lineRenderer.SetWidth(width, width);
			if (!startFits && !endFits && !bounds.Contains(rayToStart.origin)) {
				lineRenderer.SetColors(Color.clear, Color.clear);
				return;
			}
			lineRenderer.SetColors(new Color(baseColor.r, baseColor.g, baseColor.b, opacity), new Color(baseColor.r, baseColor.g, baseColor.b, opacity));
		}
		
		public void clearGrid() {
			for (int i=0; i<numberOfLines; i++) {
				GameObject.Destroy(verticalXOYLines[i]);
				GameObject.Destroy(horizontalXOYLines[i]);
				GameObject.Destroy(verticalYOZLines[i]);
				GameObject.Destroy(horizontalYOZLines[i]);
				GameObject.Destroy(lateralXOZLines[i]);
				GameObject.Destroy(longitudalXOZLines[i]);
			}
			for (int i=0; i < axisLines.Length; i++) {
				GameObject.Destroy(axisLines[i]);
			}
		}
		
	}
}
