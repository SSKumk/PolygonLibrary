using System;

namespace Robust
{
	public abstract class CalculateBase : ICalculate
	{
		protected bool calculated = false;

		public bool Calculated
		{
			get { return calculated; }
		}

		public abstract int CalculationTime { get; }

		public virtual void Calculate()
		{
			if (Calculated) return;
			Calculate(new AsyncCalculation(this));
		}

		public abstract void Calculate(AsyncCalculation result);
	}

	public abstract class ControlBase : CalculateBase, IControl
	{
		protected History history;

		public History History
		{
			get { return history; }
		}

		public void StartHistory()
		{
			history.StartHistory();
		}

		public void StopHistory()
		{
			history.StopHistory();
		}

		public abstract double[] GetControl(double[] x, double time);
		public abstract void ToCompactForm();
	}

	public abstract class SimulatorBase : CalculateBase, ISimulator
	{
		protected History history;

		public History History
		{
			get { return history; }
		}

		public void StartHistory()
		{
			history.StartHistory();
		}

		public void StopHistory()
		{
			history.StopHistory();
		}
	}
}