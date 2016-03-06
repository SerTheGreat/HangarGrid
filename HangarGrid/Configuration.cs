using System;
using UnityEngine;

namespace HangarGrid
{
	/// <summary>
	/// Used to load and hold configurable properties
	/// </summary>
	public class Configuration
	{
		
		private KeyCode defaultAlignUpAxis = KeyCode.J;
		private KeyCode defaultAlignForwardAxis = KeyCode.N;
		private KeyCode defaultAlignRightAxis = KeyCode.M;
		private KeyCode defaultAlignToGrid = KeyCode.L;
		private KeyCode defaultToggleSymmetryGuides = KeyCode.K;
		private KeyCode defaultBindGridToPart = KeyCode.G;
		
		public KeyCode alignUpAxis;
		public KeyCode alignForwardAxis;
		public KeyCode alignRightAxis;
		public KeyCode alignToGrid;
		public KeyCode toggleSymmetryGuides; 
		public KeyCode bindGridToPart;
		public int guideSelectionTolerance = 20;
		
		private static Configuration instance = null;
		
		public static Configuration Instance {
			get {
				if (instance == null) {
					instance = new Configuration();
				}
				return instance;
			}
		}
		
		private Configuration()
		{
			loadConfiguration("HangarGrid");
		}
		
		private void loadConfiguration(string root) {
			ConfigNode[] nodes = GameDatabase.Instance.GetConfigNodes(root);
			if ((nodes == null) || (nodes.Length == 0)) {
        	    return;
        	}
			int.TryParse(nodes[0].GetValue("guideSelectionTolerance"), out guideSelectionTolerance);
			foreach(ConfigNode node in nodes[0].nodes) {
				if (node.name == "HotKeys") {
					tryParseKeyCode(node.GetValue("alignUpAxis"), defaultAlignUpAxis, out alignUpAxis);
					tryParseKeyCode(node.GetValue("alignForwardAxis"), defaultAlignForwardAxis, out alignForwardAxis);
					tryParseKeyCode(node.GetValue("alignRightAxis"), defaultAlignRightAxis, out alignRightAxis);
					tryParseKeyCode(node.GetValue("alignToGrid"), defaultAlignToGrid, out alignToGrid);
					tryParseKeyCode(node.GetValue("toggleSymmetryGuides") , defaultToggleSymmetryGuides, out toggleSymmetryGuides);
					tryParseKeyCode(node.GetValue("bindGridToPart"), defaultBindGridToPart, out bindGridToPart);
				}
			}
		}
		
		private bool tryParseKeyCode(string value, KeyCode defaultValue, out KeyCode result) {
			try {
				result = (KeyCode)System.Enum.Parse(typeof(KeyCode), value, true);
				return true;
			} catch {
				result = defaultValue;
				return false;
			}
		}
				
	}
}
