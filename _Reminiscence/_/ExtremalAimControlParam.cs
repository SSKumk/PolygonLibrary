using System;
using System.Collections;

namespace Robust
{
	public class ExtremalAimControlParam : ParamBase
	{
		private readonly double[,] a, b, c;
		private readonly double[][] p, q;
		private readonly Point[] m;
		private readonly int row1, row2;
		private readonly Time time;
		private readonly double cmin, cmax;
		private readonly int ccount;
		private readonly bool first;
		private readonly BridgeParam[] bridges;
		private readonly EquivMatricesParam equiv;

		public ExtremalAimControlParam(double[,] A, double[,] B, double[,] C, int Row1, int Row2, 
			double[] Mu, double[] Nu, Point[] M, double T0, double T1, double Delta, 
			double CMin, double CMax, int CCount, bool First)
		{
			ArrayList errors = new ArrayList();
			errors.AddRange(CheckABC(A, B, C));
			errors.AddRange(CheckMuNu(Mu, Nu, B, C));
			errors.AddRange(CheckRows(Row1, Row2, A));
			errors.AddRange(CheckTerminalSet(M));
			errors.AddRange(CheckTimeDelta(T0, T1, Delta));
			if (errors.Count != 0)
				throw new ErrorListException(
					(string[])errors.ToArray(typeof(string)));
			this.a = A; this.b = B; this.c = C;	this.m = M;
			this.p = ConstrainsToBoxes(Mu); this.q = ConstrainsToBoxes(Nu);
			this.row1 = Row1; this.row2 = Row2;
			this.time = new Time(T0, T1, Delta);
			this.cmin = CMin; this.cmax = CMax;
			this.ccount = CCount; this.first = First;
			bridges = new BridgeParam[ccount];
			for (int i = 0; i < ccount; i++)
				bridges[i] = CBridgeParams(i);
			equiv = new EquivMatricesParam(A, Row1, Row2, T0, T1, Delta);
		}

		public ExtremalAimControlParam(double[,] A, double[,] B, double[,] C, int Row1, int Row2, 
			double[][] P, double[][] Q, Point[] M, double T0, double T1, double Delta, 
			double CMin, double CMax, int CCount, bool First)
		{
			ArrayList errors = new ArrayList();
			errors.AddRange(CheckABC(A, B, C));
			errors.AddRange(CheckPQ(P, Q, B, C));
			errors.AddRange(CheckRows(Row1, Row2, A));
			errors.AddRange(CheckTerminalSet(M));
			errors.AddRange(CheckTimeDelta(T0, T1, Delta));
			if (errors.Count != 0)
				throw new ErrorListException(
					(string[])errors.ToArray(typeof(string)));
			this.a = A; this.b = B; this.c = C;	this.m = M;
			this.p = P; this.q = Q; this.row1 = Row1; this.row2 = Row2;
			this.time = new Time(T0, T1, Delta);
			this.cmin = CMin; this.cmax = CMax;
			this.ccount = CCount; this.first = First;
			bridges = new BridgeParam[ccount];
			for (int i = 0; i < ccount; i++)
				bridges[i] = CBridgeParams(i);
			equiv = new EquivMatricesParam(A, Row1, Row2, T0, T1, Delta);
		}

		private BridgeParam CBridgeParams(int idx)
		{
			double c = (cmax - cmin) * idx / (ccount - 1) + cmin;
			Point[] mc = new Point[m.Length];
			for (int j = 0; j < mc.Length; j++) mc[j] = m[j] * c;
			return new BridgeParam(A, B, C, Row1, Row2, P, Q, mc, Time.T0, Time.T1, Time.Delta);
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
		public double CMin { get { return cmin; } }
		public double CMax { get { return cmax; } }
		public int CCount { get { return ccount; } }
		public bool First { get { return first; } }
		internal BridgeParam[] BridgeParams { get { return bridges; } }
		internal EquivMatricesParam EquivParam { get { return equiv; } }

		public override bool Equals(object obj)
		{
			if (this.GetType() != obj.GetType()) return false;
			ExtremalAimControlParam that = (ExtremalAimControlParam)obj;
			if (this.cmin != that.cmin || this.cmax != that.cmax ||
				this.ccount != that.ccount || this.first != that.first) return false;
			if (!this.bridges[0].Equals(that.bridges[0])) return false;
			return true;
		}

		public override int GetHashCode()
		{
			return bridges[0].GetHashCode();
		}
	}
}
