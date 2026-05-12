using System;

namespace Robust
{
	public interface ICalculate
	{
		bool Calculated { get; }
		int CalculationTime { get; }
		void Calculate();
		void Calculate(AsyncCalculation result);
	}

	public interface IControl : ICalculate
	{
		double[] GetControl(double[] x, double time);
		History History { get; }
		void StartHistory();
		void StopHistory();
		void ToCompactForm();
	}

	public interface ISimulator : ICalculate
	{
		History History { get; }
		void StartHistory();
		void StopHistory();
	}

	public interface ITimePoly
	{
		Poly this[int time] { get; }
		int nTime { get; }
		void ToCompactForm();
	}

	public interface IDifSystemFunc
	{
		int Dim { get; }
		double[] DifFunc(double[] x, double[] u, double[] v);
	}
}