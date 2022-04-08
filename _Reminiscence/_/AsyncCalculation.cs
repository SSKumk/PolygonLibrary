using System;

namespace Robust
{
	public delegate void CalcProgressDelegate(int ready);
	public delegate void CalcCompletedDelegate();
	public delegate void CalcFailedDelegate(Exception ex);
	
	public class AsyncCalculation
	{
		private delegate void AsyncCalcDelegate();
		private ICalculate calc;
		private int ready;
		
		public AsyncCalculation(ICalculate calc)
		{
			this.calc = calc;
		}

		public void StartCalculations()
		{
			AsyncCalcDelegate calc = new AsyncCalcDelegate(StartAsyncCalc);
			calc.BeginInvoke(null, null);
		}

		private void StartAsyncCalc()
		{
			try
			{
				calc.Calculate(this);
				if (CalculationsCompleted != null) CalculationsCompleted();
			}
			catch (Exception ex)
			{
				if (CalculationsFailed != null) CalculationsFailed(ex);
			}
		}

		public int Ready
		{
			get { return ready; }
			set 
			{ 
				ready = value; 
				if (CalculationsProgress != null) CalculationsProgress(ready); 
			}
		}
		
		public int Total
		{
			get { return calc.CalculationTime; }
		}

		public event CalcProgressDelegate CalculationsProgress;
		public event CalcCompletedDelegate CalculationsCompleted;
		public event CalcFailedDelegate CalculationsFailed;
	}
}
