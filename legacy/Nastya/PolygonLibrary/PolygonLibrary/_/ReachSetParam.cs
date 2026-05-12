using System;
using System.Collections;

namespace Robust
{
	public class ReachSetParam : ParamBase
	{
		private readonly double[,] a, b;
		private readonly double[][] p;
		private readonly Point[] zero;
		private readonly int row1, row2;
		private readonly Time time;
		private readonly EquivMatricesParam equiv;

		public ReachSetParam(double[,] A, double[,] B, int Row1, int Row2, double[] Mu, 
			Point[] ZeroSetEquiv, double T0, double T1, double Delta)
		{
			ArrayList errors = new ArrayList();
			errors.AddRange(CheckAB(A, B));
			errors.AddRange(CheckMu(Mu, B));
			errors.AddRange(CheckRows(Row1, Row2, A));
			errors.AddRange(CheckTerminalSet(ZeroSetEquiv));
			errors.AddRange(CheckTimeDelta(T0, T1, Delta));
			if (errors.Count != 0)
				throw new ErrorListException((string[])errors.ToArray(typeof(string)));
			this.a = A; this.b = B; this.zero = ZeroSetEquiv;
			this.p = ConstrainsToBoxes(Mu);
			this.row1 = Row1; this.row2 = Row2; this.time = new Time(T0, T1, Delta);
			equiv = new EquivMatricesParam(A, Row1, Row2, T0, T1, Delta);
		}

		public ReachSetParam(double[,] A, double[,] B, int Row1, int Row2, double[][] P, 
			Point[] ZeroSetEquiv, double T0, double T1, double Delta)
		{
			ArrayList errors = new ArrayList();
			errors.AddRange(CheckAB(A, B));
			errors.AddRange(CheckP(P, B));
			errors.AddRange(CheckRows(Row1, Row2, A));
			errors.AddRange(CheckTerminalSet(ZeroSetEquiv));
			errors.AddRange(CheckTimeDelta(T0, T1, Delta));
			if (errors.Count != 0)
				throw new ErrorListException((string[])errors.ToArray(typeof(string)));
			this.a = A; this.b = B; this.p = P; this.zero = ZeroSetEquiv;
			this.row1 = Row1; this.row2 = Row2; this.time = new Time(T0, T1, Delta);
			equiv = new EquivMatricesParam(A, Row1, Row2, T0, T1, Delta);
		}

		public double[,] A { get { return a; } }
		public double[,] B { get { return b; } }
		public double[][] P { get { return p; } }
		public Point[] ZeroSetEquiv { get { return zero; } }
		public int Row1 { get { return row1; } }
		public int Row2 { get { return row2; } }
		public Time Time { get { return time; } }
		internal EquivMatricesParam EquivParam 
		{ get { return equiv; } }
	
		public override bool Equals(object obj)
		{
			if (this.GetType() != obj.GetType()) return false;
			ReachSetParam that = (ReachSetParam)obj;
			if ((Matrix)this.a != (Matrix)that.a || (Matrix)this.b != (Matrix)that.b ||
				!EqualBoxes(this.p, that.p) || !EqualSets(this.zero, that.zero) ||
				this.row1 != that.row1 || this.row2 != that.row2 || this.time != that.time) 
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			return a.GetHashCode() + b.GetHashCode() + p.GetHashCode() + zero.GetHashCode() +
				row1.GetHashCode() + row2.GetHashCode() + time.GetHashCode();
		}
	}
}
