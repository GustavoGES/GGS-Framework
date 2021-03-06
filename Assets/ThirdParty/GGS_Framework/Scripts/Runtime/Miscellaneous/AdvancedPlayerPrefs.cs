﻿using UnityEngine;

namespace GGS_Framework
{
	public class AdvancedPlayerPrefs
	{
		#region Class Implementation
		public static void SetBool (string key, bool value)
		{
			PlayerPrefs.SetInt (key, value ? 1 : 0);
		}

		public static bool GetBool (string key, bool defaultValue)
		{
			return PlayerPrefs.GetInt (key, defaultValue ? 1 : 0) == 1 ? true : false;
		}

		public static bool GetBool (string key)
		{
			return PlayerPrefs.GetInt (key) == 1 ? true : false;
		}
		#endregion
	}
}