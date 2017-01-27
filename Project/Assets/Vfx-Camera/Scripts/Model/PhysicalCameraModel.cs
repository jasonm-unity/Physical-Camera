using UnityEngine;

namespace Unity.Vfx.Cameras.Model
{

	public enum EProjectionMode
	{
		Perspective,
		Stereoscopic,
		Ortographic,
		Orthographic_Stereo
	}

	[System.Serializable]
	public struct PhysicalCameraModel
	{
		public MathematicalModel m_MathModel;

		public EProjectionMode m_ProjectionMode;

		public float m_NearClippingPlane;
		public float m_FarClippingPlane;
		public bool m_AutoFocus;
		public float m_Exposure; // tone mapping / hdr

		public PhysicalCameraBodyModel m_Body;
		public PhysicalCameraLensModel m_Lens;
		public StereoPhysicalCameraModel m_Stereo;

		public float HorzAFOV
		{
			get { return m_MathModel.ComputerHAFOV( ref this ); }
			set
			{
				m_MathModel.ForceAFOV(value, ref this);
			}
		}

		public void SetupDefaultValues()
		{
			m_MathModel = new MathematicalModel();
			m_ProjectionMode = EProjectionMode.Perspective;
			m_NearClippingPlane = 0.0f;
			m_FarClippingPlane = float.MaxValue;

			m_AutoFocus = false;
			m_Exposure = 1.0f;

			m_Body.SetupDefaultValues();
			m_Lens.SetupDefaultValues();

			m_MathModel.ComputeDependants(ref this);
		}

		public bool IsValid()
		{
			return m_Body.IsValid() && m_Lens.IsValid();
		}

	}
}
