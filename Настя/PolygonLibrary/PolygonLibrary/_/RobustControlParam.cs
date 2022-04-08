using System;
using System.Collections;

namespace Robust
{
	public class RobustControlParam : ParamBase
	{
		private readonly double[,] a, b, c;
		private readonly double[] mu, nu;
		private readonly Point[] m;
		private readonly int row1, row2;
		private readonly Time time;
		private readonly double outercoef;
		private readonly bool first;
		private readonly BridgeParam inner;
		private readonly ReachSetParam outer;
		private readonly EquivMatricesParam equiv;

		public RobustControlParam(double[,] A, double[,] B, double[,] C, int Row1, int Row2, 
			double[] Mu, double[] Nu, Point[] M, double T0, double T1, double Delta, 
			double OuterCoef, bool First)
		{
			ArrayList errors = new ArrayList();
			errors.AddRange(CheckABC(A, B, C));
			errors.AddRange(CheckMuNu(Mu, Nu, B, C));
			errors.AddRange(CheckRows(Row1, Row2, A));
			errors.AddRange(CheckTerminalSet(M));
			errors.AddRange(CheckTimeDelta(T0, T1, Delta));
			if (errors.Count != 0)
				throw new ErrorListException((string[])errors.ToArray(typeof(string)));
			this.a = A; this.b = B; this.c = C; this.mu = Mu; this.nu = Nu; this.m = M;
			this.row1 = Row1; this.row2 = Row2; this.time = new Time(T0, T1, Delta);
			this.outercoef = OuterCoef; this.first = First;
			inner = new BridgeParam(A, B, C, Row1, Row2, Mu, Nu, M, T0, T1, Delta);
			outer = new ReachSetParam(A, C, Row1, Row2, Nu,
				new Point[] { new Point(0, 0) }, T0, T1, Delta);
			equiv = new EquivMatricesParam(A, Row1, Row2, T0, T1, Delta);
		}

		public double[,] A { get { return a; } }
		public double[,] B { get { return b; } }
		public double[,] C { get { return c; } }
		public double[] Mu { get { return mu; } }
		public double[] Nu { get { return nu; } }
		public Point[] M { get { return m; } }
		public int Row1 { get { return row1; } }
		public int Row2 { get { return row2; } }
		public Time Time { get { return time; } }
		public double OuterCoef { get { return outercoef; } }
		public bool First { get { return first; } }
		public BridgeParam InnerParam { get { return inner; } }
		public ReachSetParam OuterParam { get { return outer; } }
		internal EquivMatricesParam EquivParam { get { return equiv; } }

		public override bool Equals(object obj)
		{
			if (this.GetType() != obj.GetType()) return false;
			RobustControlParam that = (RobustControlParam)obj;
			if (!this.inner.Equals(that.inner) || this.outercoef != that.outercoef ||
				this.first != that.first) return false;
			return true;
		}

		public override int GetHashCode()
		{
			return inner.GetHashCode();
		}
	}
}