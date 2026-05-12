using System;
using System.Collections;

namespace Robust
{
	public class BridgeParam : ParamBase
	{
		private readonly double[,] a, b, c;
		//private readonly double[] mu, nu;
		private readonly double[][] p, q;
		private readonly Point[] m;
		private readonly int row1, row2;
		private readonly Time time;
		private readonly EquivMatricesParam equiv;

		public BridgeParam(double[,] A, double[,] B, double[,] C,
			int Row1, int Row2, double[] Mu, double[] Nu, 
			Point[] M, double T0, double T1, double Delta)
		{
			ArrayList errors = new ArrayList();
			errors.AddRange(CheckABC(A, B, C));
			errors.AddRange(CheckMuNu(Mu, Nu, B, C));
			errors.AddRange(CheckRows(Row1, Row2, A));
			errors.AddRange(CheckTerminalSet(M));
			errors.AddRange(CheckTimeDelta(T0, T1, Delta));
			if (errors.Count != 0)
				throw new ErrorListException((string[])errors.ToArray(typeof(string)));
			this.a = A; this.b = B; this.c = C; this.m = M;
			this.p = ConstrainsToBoxes(Mu);
			this.q = ConstrainsToBoxes(Nu);
			this.row1 = Row1; this.row2 = Row2;
			this.time = new Time(T0, T1, Delta);
			equiv = new EquivMatricesParam(A, Row1, Row2, T0, T1, Delta);
		}

		public BridgeParam(double[,] A, double[,] B, double[,] C, int Row1, int Row2, 
			double[][] P, double[][] Q, Point[] M, double T0, double T1, double Delta)
		{
			ArrayList errors = new ArrayList();
			errors.AddRange(CheckABC(A, B, C));
			errors.AddRange(CheckPQ(P, Q, B, C));
			errors.AddRange(CheckRows(Row1, Row2, A));
			errors.AddRange(CheckTerminalSet(M));
			errors.AddRange(CheckTimeDelta(T0, T1, Delta));
			if (errors.Count != 0)
				throw new ErrorListException((string[])errors.ToArray(typeof(string)));
			this.a = A; this.b = B; this.c = C; this.m = M;
			this.p = P; this.q = Q; this.row1 = Row1; this.row2 = Row2;
			this.time = new Time(T0, T1, Delta);
			equiv = new EquivMatricesParam(A, Row1, Row2, T0, T1, Delta);
		}

		public double[,] A { get { return a; } }
		public double[,] B { get { return b; } }
		public double[,] C { get { return c; } }
		public double[][] P { get { return p; } }
		public double[][] Q { get { return q; } }
		public Point[] M { get { return m; } }
		public int Row1 { get { return row1; } }
		public int Row2 { get { return row2; } }
		public Time Time { get { return time; } }
		internal EquivMatricesParam EquivParam 
		{ get { return equiv; } }

		public override bool Equals(object obj)
		{
			if (this.GetType() != obj.GetType()) return false;
			BridgeParam that = (BridgeParam)obj;
			if ((Matrix)this.a != (Matrix)that.a || (Matrix)this.b != (Matrix)that.b ||
				(Matrix)this.c != (Matrix)that.c || !EqualBoxes(this.p, that.p) ||
				!EqualBoxes(this.q, that.q) || !EqualSets(this.m, that.m) ||
				this.row1 != that.row1 || this.row2 != that.row2 || this.time != that.time) 
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			return a.GetHashCode() + b.GetHashCode() + c.GetHashCode() + p.GetHashCode() +
				q.GetHashCode() + m.GetHashCode() + row1.GetHashCode() + row2.GetHashCode() +
				time.GetHashCode();
		}
	}
}
