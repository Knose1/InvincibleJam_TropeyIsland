///-----------------------------------------------------------------
/// Author : #DEVELOPER_NAME#
/// Date : #DATE#
///-----------------------------------------------------------------



namespace Com.Github.Knose1.Common.Twinning.Curve
{
	//delegate float CurveDelegate(float lerpX);

	public abstract class Curve
	{
		public float minX = 0;
		public float maxX = 1;
		public float minY = float.NaN;
		public float maxY = float.NaN;

		protected Curve()
		{
			SetDefaultY();
		}

		protected void SetDefaultYNaNCheck()
		{
			if (float.IsNaN(minY)) minY = GetCurve(minX);
			if (float.IsNaN(maxY)) maxY = GetCurve(maxX);
		}

		public void SetDefaultY()
		{
			minY = GetCurve(minX);
			maxY = GetCurve(maxX);
		}

		abstract protected float GetCurve(float x);

		public float In(float lerpX)
		{
			if (lerpX < 0) lerpX = 0;
			else if (lerpX > 1) lerpX = 1;

			float curveY = GetCurve((maxX - minX) * lerpX + minX);

			return (curveY - minY) / (maxY - minY);
		}

		public float Out(float lerpX)
		{
			if (lerpX < 0) lerpX = 0;
			else if (lerpX > 1) lerpX = 1;

			float curveY = GetCurve((minX - maxX) * lerpX + maxX);

			return (curveY - maxY) / (minY - maxY);
		}

		public float InOut(float lerpX)
		{
			if (lerpX < 0) lerpX = 0;
			else if (lerpX > 1) lerpX = 1;

			if (lerpX < 0.5) return In(lerpX * 2) / 2;
			else return Out(lerpX * 2 - 1) / 2 + 0.5f;
		}


		public float OutIn(float lerpX)
		{
			if (lerpX < 0) lerpX = 0;
			else if (lerpX > 1) lerpX = 1;

			if (lerpX < 0.5) return Out(lerpX * 2) / 2;
			else return In(lerpX * 2 - 1) / 2 + 0.5f;
		}
	}
}