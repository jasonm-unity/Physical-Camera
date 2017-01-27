using System;
using System.ComponentModel.Design;
using Unity.Vfx.Cameras.Model;
using UnityEngine;
using UnityEditor;

namespace Unity.Vfx.Cameras.Editor
{
	[ExecuteInEditMode]
	[CustomEditor(typeof(PhysicalCamera))]
	public class PhysicalCameraEditor : UnityEditor.Editor
	{
		public bool m_ShowCamera = true;
		public bool m_showLens = true;
		public bool m_showBody = true;

		public override void OnInspectorGUI()
		{
			var physCamera = this.serializedObject;
			EditorGUI.BeginChangeCheck();
			physCamera.Update();

			// Start --------------------------------------------------------

			var property = this.serializedObject.FindProperty("m_Mode");
			AddEnumPopup(property, "Mode", typeof(PhysicalCameraMode));


			property = this.serializedObject.FindProperty("m_AssociatedCameraObj");
			EditorGUILayout.PropertyField(property);

			// models
			var camModel = this.serializedObject.FindProperty("m_cameraModel");
			var lensModel = camModel.FindPropertyRelative("m_Lens");
			var bodyModel = camModel.FindPropertyRelative("m_Body");

			m_ShowCamera = EditorGUILayout.Foldout(m_ShowCamera, "Camera");
			if (m_ShowCamera)
				OnInspectorGuiCamera(camModel, lensModel);

			m_showLens = EditorGUILayout.Foldout(m_showLens, "Camera Lens");
			if (m_showLens)
				OnInspectorGuiLens(lensModel);

			m_showBody = EditorGUILayout.Foldout(m_showBody, "Camera body");
			if (m_showBody)
				OnInspectorGuiBody(bodyModel);

			// Done -------------------------------------------------------------
			physCamera.ApplyModifiedProperties();
			EditorGUI.EndChangeCheck();
		}

		private void OnInspectorGuiCamera(SerializedProperty camModel, SerializedProperty lensModel)
		{
			var camObj = serializedObject.targetObject as PhysicalCamera;

			SerializedProperty property;
			property = camModel.FindPropertyRelative("m_ProjectionMode");
			AddEnumPopup(property, "Projection", typeof(EProjectionMode));

			property = camModel.FindPropertyRelative("m_NearClippingPlane");
			AddFloatProperty(property, "Near clipping plane", (oldv, newv) =>
			{
				if (newv < 0f) return 0f;
				else return newv;
			});

			property = camModel.FindPropertyRelative("m_FarClippingPlane");
			AddFloatProperty(property, "Far clipping plane", (oldv, newv) => {
				if (newv < 0f) return 0f;
				else return newv;
			});

			property = camModel.FindPropertyRelative("m_AutoFocus");
			AddBoolProperty(property, "Autofocus");

			property = camModel.FindPropertyRelative("m_Exposure");
			AddFloatProperty(property, "Exposure(?)");

			// Fake property
			if(camObj.m_cameraModel.m_ProjectionMode < EProjectionMode.Ortographic )
			{
				EditorGUILayout.BeginHorizontal();

				var orgValue = camObj.m_cameraModel.HorzAFOV;
				var newValue = EditorGUILayout.FloatField("Horizontal AFOV (deg)", orgValue);
				if (orgValue != newValue)
				{
					if (newValue < 0.00001f) newValue = 0.00001f;
					if (newValue > 180f) newValue = 180f;

					camObj.m_cameraModel.HorzAFOV = newValue;
				}

				EditorGUILayout.EndHorizontal();
			}

		}

		private void OnInspectorGuiBody(SerializedProperty bodyModel)
		{
			var camObj = serializedObject.targetObject as PhysicalCamera;

			var property = bodyModel.FindPropertyRelative("m_SensorWidth");
			AddFloatSlider(property, "Sensor width (mm)", null, 1000f, 0.001f, 0.1f);

			property = bodyModel.FindPropertyRelative("m_SensorHeight");
			AddFloatSlider(property, "Sensor height (mm)", null, 1000f, 0.001f, 0.1f);

			// Fake property
			{
				EditorGUILayout.BeginHorizontal();
				var orgValue = camObj.m_cameraModel.m_Body.AspectRatio;
				var newValue = EditorGUILayout.Slider("Aspect ratio (w/h)", orgValue, 0.1f, 20f);
				if (orgValue != newValue)
					camObj.m_cameraModel.m_Body.AspectRatio = newValue;
				EditorGUILayout.EndHorizontal();
			}

			property = bodyModel.FindPropertyRelative("m_ShutterSpeed");
			AddFloatSlider(property, "Shutter speed (sec.)", null, 1f, 1/100000f, 5*60f);

			property = bodyModel.FindPropertyRelative("m_ISO");
			AddIntSlider(property, "ISO", null, 25, 32000);

			property = bodyModel.FindPropertyRelative("m_HDR");
			AddBoolProperty(property, "HDR");

			property = bodyModel.FindPropertyRelative("m_LensShiftX");
			AddFloatSlider(property, "Lens Shift X", null, 1f, -1f, 1f);

			property = bodyModel.FindPropertyRelative("m_LensShiftY");
			AddFloatSlider(property, "Lens Shift Y", null, 1f, -1f, 1f);

			if (camObj.m_cameraModel.m_ProjectionMode < EProjectionMode.Ortographic)
			{
				property = bodyModel.FindPropertyRelative("m_PerspectiveCorrection");
				AddFloatSlider(property, "Perspective Correction", null, 1f, -1f, 1f);
			}
		}

		private void OnInspectorGuiLens(SerializedProperty lensModel)
		{
			var property = lensModel.FindPropertyRelative("m_FocalLength");
			AddFloatSlider(property, "Focal length", null, 1000f, 1/1000f, 2f);

			property = lensModel.FindPropertyRelative("m_FocalDepth");
			AddFloatSlider(property, "Focal depth(?)", null, 1, 0f, float.MaxValue);

			property = lensModel.FindPropertyRelative("m_FStop");
			AddFloatSlider(property, "F-Stop", null, 1f, 0.25f, 128f);

			property = lensModel.FindPropertyRelative("m_Aperture");
			AddFloatSlider(property, "Aperture(?)", null, 1f, -1f, 1f);

			property = lensModel.FindPropertyRelative("m_ApertureAspectRatio");
			AddFloatSlider(property, "Aperture aspect ratio(?)", null, 1f, -1f, 1f);

			property = lensModel.FindPropertyRelative("m_ApertureEdge");
			AddFloatSlider(property, "Aperture edge(?)", null, 1f, -1f, 1f);

			property = lensModel.FindPropertyRelative("m_Distortion");
			AddFloatSlider(property, "Distortion (?)", null, 1f, -1f, 1f);

		}

		private delegate T OnValueChangedDelegate<T>(T oldValue, T newValue);

		void AddEnumPopup(SerializedProperty porperty, string text, Type typeOfEnum, OnValueChangedDelegate<int> onChange  =null)
		{
			Rect ourRect = EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginProperty(ourRect, GUIContent.none, porperty);

			int selectionFromInspector = porperty.intValue;
			string[] enumNamesList = System.Enum.GetNames(typeOfEnum);
			var actualSelected = EditorGUILayout.Popup(text, selectionFromInspector, enumNamesList);
			if (onChange != null && actualSelected != porperty.intValue)
				actualSelected = onChange(porperty.intValue, actualSelected);
			porperty.intValue = actualSelected;
			EditorGUI.EndProperty();
			EditorGUILayout.EndHorizontal();
		}

		void AddIntProperty(SerializedProperty porperty, string text, OnValueChangedDelegate<int> onChange = null)
		{
			Rect ourRect = EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginProperty(ourRect, GUIContent.none, porperty);

			var orgValue = porperty.intValue;
			var newValue = EditorGUILayout.IntField(text, orgValue);
			if (onChange != null && orgValue != newValue)
				newValue = onChange(orgValue, newValue);
			porperty.intValue = newValue;

			EditorGUI.EndProperty();
			EditorGUILayout.EndHorizontal();
		}

		void AddFloatProperty(SerializedProperty porperty, string text, OnValueChangedDelegate<float> onChange = null, float factor = 1f)
		{
			Rect ourRect = EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginProperty(ourRect, GUIContent.none, porperty);

			var orgValue = porperty.floatValue * factor;
			var newValue = EditorGUILayout.FloatField(text, orgValue) / factor;
			if (onChange != null && orgValue != newValue)
				newValue = onChange(orgValue, newValue);
			porperty.floatValue = newValue;

			EditorGUI.EndProperty();
			EditorGUILayout.EndHorizontal();
		}

		void AddFloatSlider(SerializedProperty porperty, string text, OnValueChangedDelegate<float> onChange = null, float factor = 1f, float min = 0, float max = float.MaxValue)
		{
			Rect ourRect = EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginProperty(ourRect, GUIContent.none, porperty);

			var orgValue = porperty.floatValue * factor;
			var newValue = EditorGUILayout.Slider(text, orgValue, min * factor, max * factor) / factor;
			if (onChange != null && orgValue != newValue)
				newValue = onChange(orgValue, newValue);
			porperty.floatValue = newValue;

			EditorGUI.EndProperty();
			EditorGUILayout.EndHorizontal();
		}

		void AddIntSlider(SerializedProperty porperty, string text, OnValueChangedDelegate<int> onChange = null, int min = 0, int max = int.MaxValue)
		{
			Rect ourRect = EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginProperty(ourRect, GUIContent.none, porperty);

			var orgValue = porperty.intValue;
			var newValue = EditorGUILayout.IntSlider(text, orgValue, min, max);
			if (onChange != null && orgValue != newValue)
				newValue = onChange(orgValue, newValue);
			porperty.intValue = newValue;

			EditorGUI.EndProperty();
			EditorGUILayout.EndHorizontal();
		}


		void AddBoolProperty(SerializedProperty porperty, string text, OnValueChangedDelegate<bool> onChange = null)
		{
			Rect ourRect = EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginProperty(ourRect, GUIContent.none, porperty);

			var orgValue = porperty.boolValue;
			var newValue = EditorGUILayout.Toggle(text, orgValue);
			if (onChange != null && orgValue != newValue)
				newValue = onChange(orgValue, newValue);
			porperty.boolValue = newValue;

			EditorGUI.EndProperty();
			EditorGUILayout.EndHorizontal();
		}

	}
}
