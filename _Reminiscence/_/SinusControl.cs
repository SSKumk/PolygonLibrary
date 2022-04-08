using System;

namespace Robust
{
	public class SinusControl : ControlBase
	{
		private SinusControlParam param;

		public SinusControl(SinusControlParam param)
		{
			this.param = param;
			calculated = true;
			history = new History();
			history.CreateToken("time");
			history.CreateToken("control");
		}

		public SinusControlParam Param
		{
			get { return param; }
			set
			{
				param = value;
				history.ClearHistory();
			}
		}

		public override double[] GetControl(double[] x, double time)
		{
			double[] u = new double[param.Amplitude.Length];
			for (int i = 0; i < u.Length; i++)
				u[i] = param.Amplitude[i] * Math.Sin(
					time * 2 * Math.PI / param.Length[i]);
			history.AddRecord("time", time);
			history.AddRecord("control", u);
			return u;
		}

		public override int CalculationTime { get { return 0; } }
		public override void Calculate(AsyncCalculation result) {}
		public override void ToCompactForm() {}
	}
}