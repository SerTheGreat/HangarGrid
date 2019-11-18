using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HangarGrid
{
	/// <summary>
	/// Manages everything related to direction guides visualization
	/// </summary>
	public class DirectionGuidesManager
	{
		
		private bool guidesEnabled = false;
		Part activePart = null; //The part that is currently showing guides
		private bool symmetryMode = true;
		
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
			clearGuides(activePart);
		}
		
		public void setSymmetryMode(bool symmetryMode) {
			if (symmetryMode != this.symmetryMode) {
				clearGuides(activePart);
				this.symmetryMode = symmetryMode;
				//on next call to updateGuides() the guides will be recreated according to symmetryMode
			}
		}
		
		public void updateGuides(Part subjectPart, Color color, float length) {
			if (!guidesEnabled) {
				return;
			}
			
			if (subjectPart == null) { //To keep showing guides after the part was deselected
				subjectPart = activePart;
			} else if (activePart != subjectPart) {
				clearGuides(activePart);
				activePart = subjectPart;
			}
			
			if (subjectPart != null) {
				foreach (Part part in getPartList(subjectPart)) {
					DirectionGuidesRenderer guidesRenderer = part.gameObject.GetComponent<DirectionGuidesRenderer>();
					if (guidesRenderer == null) {
						guidesRenderer = part.gameObject.AddComponent<DirectionGuidesRenderer>();
						guidesRenderer.setGuideLegth(5);
					}
					guidesRenderer.updateGuides(color);
				}
			}
			
		}

		//Here all the active symmetry parts are sorted by the range to the screenPoint. Then the closest part and the closest guide that falls in the range is returned  
		public void findClosestDirectionOnScreen(Vector3 screenPoint, int range, out Part part, out Vector3 localDirection) {
			part = null;
			localDirection = Vector3.zero;
			if (!guidesEnabled || (activePart == null)) {
				return;
			}
			List<Part> partList = getPartList(activePart);
			partList.Sort((p1, p2) => (int)(Vector3.Distance(Camera.main.ScreenToWorldPoint(screenPoint), p1.transform.position) -
				                                Vector3.Distance(Camera.main.ScreenToWorldPoint(screenPoint), p2.transform.position)));
			foreach (Part p in partList) {
				DirectionGuidesRenderer guidesRenderer = p.gameObject.GetComponent<DirectionGuidesRenderer>(); //if there's no renderer then something is wrong in the code
				if (guidesRenderer.closestGuideLocalProjection(screenPoint, range, out localDirection)) {
					part = p;
					return;					
				}				 
			}
		}
		
		private void clearGuides(Part subjectPart) {
			if (subjectPart == null) {
				return;
			}
			foreach (Part part in getPartList(activePart)) {
				DirectionGuidesRenderer guidesRenderer = part.gameObject.GetComponent<DirectionGuidesRenderer>();
				UnityEngine.Object.Destroy(guidesRenderer);
			}			
		}

		private List<Part> getPartList(Part part) {
			List<Part> partList = new List<Part>();
			partList.Add(part);
			if (symmetryMode) {
				partList.AddRange(part.symmetryCounterparts);
			}
			return partList;
		}
		
	}
}
