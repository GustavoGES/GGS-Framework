﻿namespace GGS_Framework
{
	[System.Serializable]
	public class ReorderableListExampleStruct
	{
		#region Class Members
		public string displayName;
		public float exampleFloat;
		public bool exampleBool;
		#endregion

		#region Class Accesors
		public string DisplayName
		{
			get { return displayName; }
		}
		#endregion
	}
}