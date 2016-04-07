using System;
using System.Collections.Generic;
using UnityEngine;
using KSP.UI.Screens;

namespace HangarGrid
{
	/// <summary>
	/// A grid plugin for Kerbal Space Program's SPH and VAB 
	/// 
	/// Copyright (C) 2016 Ser
	///This program is free software; you can redistribute it and/or
	///modify it under the terms of the GNU General Public License
	///as published by the Free Software Foundation; either version 2
	///of the License, or (at your option) any later version.
	///
	///This program is distributed in the hope that it will be useful,
	///but WITHOUT ANY WARRANTY; without even the implied warranty of
	///MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	///GNU General Public License for more details.
	/// 
	///You should have received a copy of the GNU General Public License
    ///along with this program.  If not, see <http://www.gnu.org/licenses/>.
	/// </summary>
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class HangarGrid : MonoBehaviour
	{
		
		private bool modEnabled = false;
		private bool symmetryMode = true;
		private Part gridOriginPart = null;
		private ApplicationLauncherButton launcherButton;
		private Configuration conf;
		
		private Part prevSelectedPart = null;
		//private Vector3 prevPosition = Vector3.zero;
		
		GridManager gridManager = new GridManager();
		DirectionGuidesManager guidesManager = new DirectionGuidesManager();
		
		public void Awake() {
			conf = Configuration.Instance;
			//GameEvents.onGUIApplicationLauncherReady.Add(addMenuButton);
			addMenuButton();
		}
		
		private void addMenuButton() {
			if (launcherButton != null) {
				return;
			}
			launcherButton = ApplicationLauncher.Instance.AddModApplication(
			onButtonTrue,
			onButtonFalse,
			null, null, null, null,
			ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.VAB,
			(Texture)GameDatabase.Instance.GetTexture("HangarGrid/textures/toolbarbutton", false));
		}
		
		private void onButtonTrue() {
			gridManager.showGrid(EditorLogic.fetch.editorBounds);
			guidesManager.showGuides();
			modEnabled = true;
		}
		
		private void onButtonFalse() {
			gridManager.hideGrid();
			guidesManager.hideGuides();
			modEnabled = false;
		}
		
		public void OnDestroy() {
			if (launcherButton != null) {
				ApplicationLauncher.Instance.RemoveModApplication(launcherButton);
			}
		}
		
		/*public void FixedUpdate() {
			if ((prevSelectedPart != null) && (prevSelectedPart == EditorLogic.SelectedPart) && Input.GetKey(KeyCode.P)) {
				Vector3 translation = Vector3.Project(EditorLogic.SelectedPart.transform.position - prevPosition, gridOriginPart.transform.up);
				EditorLogic.SelectedPart.transform.Translate(translation, Space.World);
			}
			prevSelectedPart = EditorLogic.SelectedPart;
			if (prevSelectedPart != null) {
				prevPosition = prevSelectedPart.transform.position;
			}
		}*/
		
		public void Update() {
			EditorLogic editor = EditorLogic.fetch;
			Bounds bounds = editor.editorBounds;
			if (EditorLogic.SelectedPart != null) {
				prevSelectedPart = EditorLogic.SelectedPart;
			}
			
			if (Input.GetKeyDown(conf.masterToggle)) {
				if (!modEnabled) {
					//onButtonTrue();
					launcherButton.SetTrue(true);
				} else {
					//onButtonFalse();
					launcherButton.SetFalse(true);
				}
			}
			
			if (Input.GetKeyDown(KeyCode.K)) {
				symmetryMode = !symmetryMode;
				guidesManager.setSymmetryMode(symmetryMode);
			}
			
			if (Input.GetKeyDown(conf.alignUpAxis)) {
				alignSelectedPartToGrid(Vector3.up);
			}
			
			if (Input.GetKeyDown(conf.alignForwardAxis)) {
				alignSelectedPartToGrid(Vector3.forward);
			}
			
			if (Input.GetKeyDown(conf.alignRightAxis)) {
				alignSelectedPartToGrid(Vector3.right);
			}
			
			if (Input.GetKeyDown(conf.alignToGrid)) {
				Part part;
				Vector3 localDirection;
				guidesManager.findClosestDirectionOnScreen(Input.mousePosition, conf.guideSelectionTolerance, out part, out localDirection);
				if (part != null) {
					alignPartToGrid(part, localDirection);
				}
			}
			
			if (Input.GetKeyDown(conf.bindGridToPart)) {
				gridOriginPart = GetPartUnderCursor();
			}
			
		    if (gridOriginPart == null) {
				gridOriginPart = EditorLogic.RootPart; 
			}
		    if (gridOriginPart != null) {
		    	if (modEnabled) {
		    		gridManager.showGrid(bounds);
		    	}
				gridManager.updateGrid(gridOriginPart);
		    } else {
		    	gridManager.hideGrid();
			}
			
			guidesManager.updateGuides(prevSelectedPart, Color.red, 5f);
			
		}
		
		private void alignSelectedPartToGrid(Vector3 localDirection) {
			Part part = EditorLogic.SelectedPart;
			if (part == null) {
				part = GetPartUnderCursor();
			}
			if (part == null) {
				return;
			}
			alignPartToGrid(part, localDirection);
		}
		
		private void alignPartToGrid(Part part, Vector3 localDirection) {
			if (gridOriginPart == null) {
				return;
			}
			SymmetryMethod symMethod = part.symMethod;
			Vector3 originalDirection = part.transform.TransformDirection(localDirection);
			Vector3 targetAxis = Utils.closestAxis(originalDirection, gridOriginPart.transform);
			Vector3 rotationAxis = Vector3.Cross(targetAxis, originalDirection);
			float angle = Utils.directionIndependentAngle(Utils.SignedAngleBetween(originalDirection, targetAxis, rotationAxis));
			part.transform.Rotate(rotationAxis, angle, Space.World);
			foreach (Part symPart in part.symmetryCounterparts) {
			    Vector3 symmetryRotationAxis;
			    if (symMethod == SymmetryMethod.Mirror) {
			    	//In the Mirror symmetry mode the rotation axis and angle should be mirrored in relation the the plane that is orthogonal to the line between parts
			    	symmetryRotationAxis = Vector3.ProjectOnPlane(rotationAxis, symPart.transform.position - part.transform.position) * 2 - rotationAxis;
			    } else {
			    	//In the Radial symmetry mode all the rotation axis' should be the same in the local space of every part
			    	Vector3 rotationAxisLocal = part.transform.InverseTransformDirection(rotationAxis);
			    	symmetryRotationAxis = symPart.transform.TransformDirection(rotationAxisLocal);
			    }
				symPart.transform.Rotate(symmetryRotationAxis, (symMethod == SymmetryMethod.Radial ? 1 : -1) * angle, Space.World);
			}
		}
		
		//Thanks MachXXV
		public static Part GetPartUnderCursor () {
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit)) {
				return new List<Part>(EditorLogic.FindObjectsOfType<Part>()).Find (p => p.gameObject == hit.transform.gameObject);
			}
			return null;
		}
		
		/*void OnPostRender() {
			if (part != null) {
				lineMat = new Material (Shader.Find("Particles/Additive"));
				lineMat.SetColor("color", Color.green);
				GL.Begin(GL.LINES);
				for (int i=-10; i<10; i++) {
					lineMat.SetPass(0);
					Vector3 lineStart = part.transform.TransformPoint(part.transform.right * i) - part.transform.up * 10;
					Vector3 lineEnd = part.transform.TransformPoint(part.transform.right * i) + part.transform.up * 10;
					GL.Color(Color.green);
					GL.Vertex3(lineStart.x, lineStart.y, lineStart.z);
					GL.Vertex3(lineEnd.x, lineEnd.y, lineEnd.z);
					}
				GL.End();
			}
		}*/
		
	}
}