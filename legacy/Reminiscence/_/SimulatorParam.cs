using System;
using System.Collections;

namespace Robust
{
	public class SimulatorParam : ParamBase
	{
		private readonly DifSystem ds;
		private readonly double[] point;
		private readonly Time time;
		private readonly IControl first, second;

		public SimulatorParam(double[,] A, double[,] B, 
			double[,] C, double[] InitialPoint, 
			double T0, double T1, double Delta, 
			IControl FirstPlayer, IControl SecondPlayer)
		{
			ArrayList errors = new ArrayList();
			errors.AddRange(CheckABC(A, B, C));
			errors.AddRange(CheckPoint(InitialPoint, A));
			errors.AddRange(CheckTimeDelta(T0, T1, Delta));
			if (errors.Count != 0)
				throw new ErrorListException(
					(string[])errors.ToArray(typeof(string)));
			this.ds = new DifSystem(A, B, C);
			this.point = InitialPoint;
			this.time = new Time(T0, T1, Delta);
			this.first = FirstPlayer; 
			this.second = SecondPlayer; 
		}

		public SimulatorParam(IDifSystemFunc func, double[] 
			InitialPoint, double T0, double T1, double Delta, 
			IControl FirstPlayer, IControl SecondPlayer)
		{
			ArrayList errors = new ArrayList();
			if (InitialPoint.Length != func.Dim)
				errors.Add(Constants.PointSize);
			errors.AddRange(CheckTimeDelta(T0, T1, Delta));
			if (errors.Count != 0)
				throw new ErrorListException(
					(string[])errors.ToArray(typeof(string)));
			this.ds = new DifSystem(func);
			this.point = InitialPoint;
			this.time = new Time(T0, T1, Delta);
			this.first = FirstPlayer; 
			this.second = SecondPlayer; 
		}

		public double[] InitialPoint { get { return point; } }
		public Time Time { get { return time; } }
		public IControl FirstPlayer { get { return first; } }
		public IControl SecondPlayer { get { return second; } }
		internal DifSystem DifSystem { get { return ds; } }
	}
}
