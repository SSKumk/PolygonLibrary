using System;
using System.Collections;

namespace Robust
{
	public class SinusControlParam : ParamBase
	{
		private readonly double[] amp, len;

		public SinusControlParam(double[] Amplitude, double[] Length)
		{
			ArrayList errors = new ArrayList();
			if (Amplitude.Length != Length.Length) errors.Add(Constants.SinusWrong);
			if (errors.Count != 0)
				throw new ErrorListException((string[])errors.ToArray(typeof(string)));
			this.amp = Amplitude; 
			this.len = Length;
		}

		public double[] Amplitude { get { return amp; } }
		public double[] Length { get { return len; } }
	}
}
