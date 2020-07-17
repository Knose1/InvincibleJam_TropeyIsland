using System;

namespace Com.Github.Knose1.Common.Twinning.Curve
{
	public partial class Cube
	{
		public class Sin : Curve
		{
			public Sin()
			{
				minX = 0;
				maxX = (float)Math.PI / 2;

				SetDefaultY();
			}

			protected override float GetCurve(float x)
			{
				return (float)Math.Sin(x + Math.PI / 2);
			}
		}
	}
}