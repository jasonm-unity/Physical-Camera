using UnityEngine;
using System.Collections;
using Unity.Vfx.Cameras.Model;

namespace Unity.Vfx.Cameras
{

	public enum PhysicalCameraMode
	{
		Active,
		Passive
	}

	[ExecuteInEditMode]
	public class PhysicalCamera : MonoBehaviour
	{
		public PhysicalCameraMode m_Mode;
		public PhysicalCameraModel m_cameraModel;

		public Camera m_AssociatedCameraObj;

		public PhysicalCamera()
		{
			m_cameraModel.SetupDefaultValues();
		}
	}
}
