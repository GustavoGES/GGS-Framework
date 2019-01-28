﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EasingCurvesWindow : EditorWindow {

	#region Class members
	private GUISkin darkSkin;
	private GUISkin lightSkin;
	private GUISkin skin;

	private bool useScrollBar;
	private float scrollPosition;

	private EasingCurves.List previewEase;
	private float currentPreviewTime;

	private Rect windowRect;
	#endregion

	#region Class accesors
	public RectOffset WindowOffset { get { return new RectOffset (-5, (useScrollBar) ? -19 : -5, -5, -5); } }

	private float Resolution { get { return 40; } }
	private float PreviewDelay { get { return 0.05f; } }

	private Vector2 Size { get { return new Vector2 (175, 175); } }
	private Vector2 Spacing { get { return new Vector2 (30, 10); } }
	#endregion

	#region Class overrides
	private void OnEnable () {
		darkSkin = EditorGUIUtility.Load ("EasingCurves/EasingCurves_DarkSkin.guiskin") as GUISkin;
		lightSkin = EditorGUIUtility.Load ("EasingCurves/EasingCurves_LightSkin.guiskin") as GUISkin;
		skin = (EditorGUIUtility.isProSkin) ? darkSkin : lightSkin;

		EditorApplication.update += delegate { Repaint (); };
	}

	private void OnGUI () {
		windowRect = WindowOffset.Add (new Rect (Vector2.zero, position.size));

		Dictionary<string, Rect> rects = ExtendedRect.VerticalRects (windowRect,
			new RectLayoutElement ("Header", 24),
			new RectLayoutElement (5),
			new RectLayoutElement ("Easings"));

		DrawHeader (rects["Header"]);
		DrawEasings (rects["Easings"]);
	}
	#endregion

	#region Class implementation
	[MenuItem ("Tools/Tween/Easing Curves")]
	static void Init () {
		EasingCurvesWindow window = (EasingCurvesWindow) GetWindow (typeof (EasingCurvesWindow));
		window.Show ();
	}

	private void DrawHeader (Rect rect) {
		GUI.Label (rect, "Easing Curves", skin.GetStyle ("Header"));
	}

	private void DrawEasings (Rect rect) {
		skin.GetStyle ("Background").Draw (rect);
		rect = new RectOffset (0, 0, -4, -4).Add (rect);

		int easeCount = System.Enum.GetValues (typeof (EasingCurves.List)).Length;

		Vector2 tableSize = Vector2.zero;
		List<Rect> tableRects = GetTableRects (easeCount, rect.width, Size, Spacing, out tableSize);
		Vector2 tableOffset = new Vector2 ((rect.width - tableSize.x) / 2f, 0);

		useScrollBar = tableSize.y > rect.height;
		if (useScrollBar) {
			Rect scrollBarRect = new Rect (position.width - 14, rect.yMin, 14, rect.height);
			scrollPosition = GUI.VerticalScrollbar (scrollBarRect, scrollPosition, scrollBarRect.height, 0, tableSize.y);
		}

		GUI.BeginGroup (rect);
		for (int i = 0; i < tableRects.Count; i++) {
			Rect easeRect = new Rect (tableRects[i].position + tableOffset, tableRects[i].size);
			easeRect.y -= scrollPosition;
			DrawEase (easeRect, (EasingCurves.List) System.Enum.GetValues (typeof (EasingCurves.List)).GetValue (i));
		}
		GUI.EndGroup ();
	}

	private void DrawEase (Rect rect, EasingCurves.List ease) {
		Dictionary<string, Rect> rects = ExtendedRect.VerticalRects (rect,
		new RectLayoutElement ("Header", 20),
		new RectLayoutElement (1),
		new RectLayoutElement ("Ease"));

		Rect originalEaseRect = rects["Ease"];
		Rect easeRect = originalEaseRect;

		GUI.Label (rects["Header"], ease.ToString ().ToTitleCase (), skin.GetStyle ("Header"));
		skin.GetStyle ("Ease").Draw (easeRect);

		GUI.BeginGroup (easeRect);
		easeRect.position = Vector2.zero;
		easeRect = new RectOffset (-2, -2, 0, 0).Add (easeRect);

		float verticalOffset = easeRect.height / 4f;
		Vector2 offset = new Vector2 (easeRect.xMin, easeRect.height - verticalOffset);

		Vector2 bottomLeft = offset;
		Vector2 bottomRight = offset + Vector2.right * easeRect.width;

		Vector2 topLeft = new Vector2 (easeRect.xMin, verticalOffset);
		Vector2 topRight = new Vector2 (easeRect.xMax, verticalOffset);

		Handles.color = Color.red;
		Handles.DrawLine (bottomLeft, bottomRight);
		Handles.DrawLine (topLeft, topRight);
		Handles.color = Color.white;

		for (int i = 0; i <= Resolution; i++) {
			float previousPercent = (i > 0 ? i - 1 : i) / Resolution;
			float percent = i / Resolution;

			Vector2 prevPosition = new Vector2 (offset.x + previousPercent * easeRect.width, offset.y + -EasingCurves.GetEaseValue (ease, previousPercent) * (easeRect.height - verticalOffset * 2));
			Vector2 position = new Vector2 (offset.x + percent * easeRect.width, offset.y + -EasingCurves.GetEaseValue (ease, percent) * (easeRect.height - verticalOffset * 2));
			Handles.DrawLine (prevPosition, position);
		}

		GUI.EndGroup ();

		if (originalEaseRect.Contains (Event.current.mousePosition) && Event.current.type == EventType.MouseDown) {
			previewEase = ease;
			currentPreviewTime = Time.realtimeSinceStartup;
		}

		if (ease == previewEase) {
			float previewTime = Mathf.Clamp (Time.realtimeSinceStartup - currentPreviewTime, 0, 1 + PreviewDelay);

			if (previewTime <= PreviewDelay)
				previewTime = 0;
			else
				previewTime -= PreviewDelay;

			Vector2 position = new Vector2 (offset.x + previewTime * easeRect.width, offset.y + -EasingCurves.GetEaseValue (ease, previewTime) * (easeRect.height - verticalOffset * 2));

			Rect circleRect = new Rect (originalEaseRect.position + position - Vector2.one * 4f, Vector2.one * 8);
			skin.GetStyle ("Circle").Draw (circleRect);

			Rect arrowRect = new Rect (originalEaseRect.xMax + 5, originalEaseRect.y + position.y - 6, 20, 12);
			skin.GetStyle ("Arrow").Draw (arrowRect);
		}
	}

	private List<Rect> GetTableRects (int cellCount, float tableWidth, Vector2 cellSize, Vector2 cellSpacing, out Vector2 tableSize) {
		int columCount = 0;
		float currentWidth = 0;
		while (true) {
			currentWidth += cellSize.x + ((columCount == 0) ? 0 : cellSpacing.x);

			if (currentWidth < tableWidth)
				columCount++;
			else
				break;
		}

		Vector2Int tableCellCount = new Vector2Int (columCount, Mathf.CeilToInt ((float) cellCount / (float) columCount));
		List<Rect> rects = new List<Rect> ();

		for (int y = 0; y < tableCellCount.y; y++) {
			for (int x = 0; x < tableCellCount.x; x++) {
				if (rects.Count < cellCount) {
					Vector2 position = new Vector2 (cellSize.x * x + cellSpacing.x * x, cellSize.y * y + cellSpacing.y * y);
					rects.Add (new Rect (position, cellSize));
				}
			}
		}

		tableSize = new Vector2 (tableCellCount.x * cellSize.x + (tableCellCount.x - 1) * cellSpacing.x, tableCellCount.y * cellSize.y + (tableCellCount.y - 1) * cellSpacing.y);
		return rects;
	}
	#endregion

	#region Interface implementation
	#endregion
}