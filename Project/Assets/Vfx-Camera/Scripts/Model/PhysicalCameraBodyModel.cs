using UnityEngine;

namespace Unity.Vfx.Cameras.Model
{

	[System.Serializable]
	public struct PhysicalCameraBodyModel
	{
		public float m_SensorWidth;
		public float m_SensorHeight;
		public float m_LensShiftX;
		public float m_LensShiftY;
		public float m_PerspectiveCorrection;

		public float m_ShutterSpeed;
		public int m_ISO;
		public bool m_HDR;

		public void SetupDefaultValues()
		{
			m_SensorWidth = 35/1000f;
			m_SensorHeight = 26/1000f;
			m_LensShiftX = 0f;
			m_LensShiftY = 0f;
			m_PerspectiveCorrection = 0;

			m_ShutterSpeed = 250;
			m_ISO = 200;
			m_HDR = false;
		}

		public bool IsValid()
		{
			return m_SensorWidth > 0 && m_SensorHeight > 0 && m_ShutterSpeed > 0 && m_ISO > 0;
		}

		public float AspectRatio
		{
			get { return m_SensorWidth/m_SensorHeight; }
			set
			{
				if( value != 0f)
					m_SensorHeight = m_SensorWidth/value; 
				
			}
		}
	}
}
