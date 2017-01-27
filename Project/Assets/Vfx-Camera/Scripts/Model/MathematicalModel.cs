using System;
using UnityEngine;
using System.Collections;
using Unity.Vfx.Cameras.Model;

namespace Unity.Vfx.Cameras.Model
{

	/// <summary>
	/// Thin lense assumptions!
	/// </summary>
	public class MathematicalModel
	{
		public virtual void ForceAFOV(float afov, ref PhysicalCameraModel camera)
		{
			camera.m_Lens.m_FocalLength = camera.m_Body.m_SensorWidth/ToRad(afov);
		}

		public virtual void ComputeDependants(ref PhysicalCameraModel camera)
		{
			
		}

		public float ComputerHAFOV(ref PhysicalCameraModel camera )
		{
			return ToDeg(camera.m_Body.m_SensorWidth/ camera.m_Lens.m_FocalLength);
		}

		private float ToRad(float rads)
		{
			return (float)(Math.PI * rads / 180.0);
		}

		private float ToDeg(float degrees)
		{
			return (float)(degrees * 180 / Math.PI);
		}

	}

}
