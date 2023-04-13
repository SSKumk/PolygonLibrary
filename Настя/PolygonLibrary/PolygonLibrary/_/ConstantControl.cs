using System;

namespace Robust
{
	public class ConstantControl : ControlBase
	{
		private double[] control;

		public ConstantControl(double[] control)
		{
			this.control = control;
			calculated = true;
			history = new History();
			history.CreateToken("time");
			history.CreateToken("control");
		}

		public override double[] GetControl(double[] x, double time)
		{
			history.AddRecord("time", time);
			history.AddRecord("control", control);
			return control;
		}

		public override int CalculationTime { get { return 0; } }
		public override void Calculate(AsyncCalculation result) {}
		public override void ToCompactForm() {}
	}
}
