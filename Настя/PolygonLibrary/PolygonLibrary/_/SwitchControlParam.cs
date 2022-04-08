using System;
using System.Collections;

namespace Robust
{
	public class SwitchControlParam : ParamBase
	{
		private readonly double[,] a, b, c;
		private readonly double[] mu, nu;
		private readonly Point[] m;
		private readonly int row1, row2;
		private readonly Time time;
		private readonly double cmin, cmax;
		private readonly int ccount;
		private readonly bool first;
		private readonly BridgeParam[] bridges;
		private readonly EquivMatricesParam equiv;

		public SwitchControlParam(double[,] A, double[,] B, double[,] C, int Row1, int Row2, 
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
			this.a = A; this.b = B; this.c = C;
			this.mu = Mu; this.nu = Nu; this.m = M;
			this.row1 = Row1; this.row2 = Row2;
			this.time = new Time(T0, T1, Delta);
			this.cmin = CMin; this.cmax = CMax;
			this.ccount = CCount; 
			this.first = First;
			this.bridges = new BridgeParam[ccount];
			for (int i = 0; i < ccount; i++)
			{
				double c = (cmax - cmin) * i / (ccount - 1) + cmin;
				Point[] cm = new Point[m.Length];
				for (int j = 0; j < cm.Length; j++) cm[j] = m[j] * c;
				bridges[i] = new BridgeParam(A, B, C, Row1, Row2, Mu, Nu, cm, T0, T1, Delta);
			}
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
		public double CMin { get { return cmin; } }
		public double CMax { get { return cmax; } }
		public int CCount { get { return ccount; } }
		public bool First { get { return first; } }
		internal BridgeParam[] BridgeParams { get { return bridges; } }
		internal EquivMatricesParam EquivParam { get { return equiv; } }

		public override bool Equals(object obj)
		{
			if (this.GetType() != obj.GetType()) return false;
			SwitchControlParam that = (SwitchControlParam)obj;
			if (this.cmin != that.cmin) return false;
			if (this.cmax != that.cmax) return false;
			if (this.ccount != that.ccount) return false;
			if (!this.bridges[0].Equals(that.bridges[0]))
				return false;
			if (this.first != that.first) return false;
			return true;
		}

		public override int GetHashCode()
		{
			return bridges[0].GetHashCode();
		}
	}
}