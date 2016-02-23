using System;
using UnityEngine;

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
		private ApplicationLauncherButton launcherButton;
		
		GridManager gridManager = new GridManager();
		DirectionGuidesManager guidesManager = new DirectionGuidesManager();
		
		public void Awake() {
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
		
		public void Update() {
			EditorLogic editor = EditorLogic.fetch;
			Bounds bounds = editor.editorBounds;

		    /*if ( Input.GetMouseButtonDown(0)){
				 RaycastHit hit;
    			 Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
    			Transform select = GameObject.FindWithTag("select").transform;
    			if (Physics.Raycast (ray, hit, 100.0)){
    				select.tag = "none";
    				hit.collider.transform.tag = "select";
    			}
    		}				
			}*/
		    
		    guidesManager.updateGuides(EditorLogic.SelectedPart, Color.red, 5f);
			
		    if (EditorLogic.RootPart != null) {
		    	if (modEnabled) {
		    		gridManager.showGrid(bounds);
		    	}
				gridManager.updateGrid(EditorLogic.RootPart);
		    } else {
		    	gridManager.hideGrid();
			}
			
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