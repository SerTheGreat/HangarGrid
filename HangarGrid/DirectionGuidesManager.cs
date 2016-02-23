using System;
using System.Collections.Generic;
using UnityEngine;

namespace HangarGrid
{
	/// <summary>
	/// Manages everything related to direction guides visualization
	/// </summary>
	public class DirectionGuidesManager
	{
		
		private bool guidesEnabled = false;
		Part prevSelectedPart = null;
		
		public void showGuides() {
			if (guidesEnabled) {
				return;
			}
			guidesEnabled = true;
		}
		
		public void hideGuides() {
			if (!guidesEnabled) {
				return;
			}
			guidesEnabled = false;
			clearGuides(prevSelectedPart);
		}
		
		public void updateGuides(Part subjectPart, Color color, float length) {
			if (!guidesEnabled) {
				return;
			}
			
			if (subjectPart == null) { //To keep showing guides after the part was deselected
				subjectPart = prevSelectedPart;
			} else if (prevSelectedPart != subjectPart) {
				clearGuides(prevSelectedPart);
				prevSelectedPart = subjectPart;
			}
			
			if (subjectPart != null) {
				foreach (Part part in getPartList(subjectPart)) {
					DirectionGuidesRenderer guidesRenderer = part.gameObject.GetComponent<DirectionGuidesRenderer>();
					if (guidesRenderer == null) {
						guidesRenderer = part.gameObject.AddComponent<DirectionGuidesRenderer>();
					}
					guidesRenderer.updateGuides(part.transform, color, length);
				}
			}
			
		}
		
		private void clearGuides(Part subjectPart) {
			if (subjectPart == null) {
				return;
			}
			foreach (Part part in getPartList(prevSelectedPart)) {
				DirectionGuidesRenderer guidesRenderer = part.gameObject.GetComponent<DirectionGuidesRenderer>();
				GameObject.DestroyObject(guidesRenderer);
			}			
		}

		private List<Part> getPartList(Part part) {
			List<Part> partList = new List<Part>();
			partList.AddRange(part.symmetryCounterparts);
			partList.Add(part);
			return partList;
		}
		
	}
}
