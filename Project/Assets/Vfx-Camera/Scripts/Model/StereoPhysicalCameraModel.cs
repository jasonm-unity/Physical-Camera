using UnityEngine;

namespace Unity.Vfx.Cameras.Model
{

	[System.Serializable]
	public struct StereoPhysicalCameraModel
	{
		public int m_StereoMode;
		public float m_EyeDistance;
		public bool m_SwapEyes;
		public Color m_LeftFilter;
		public Color m_RightFilter;
	}
}

