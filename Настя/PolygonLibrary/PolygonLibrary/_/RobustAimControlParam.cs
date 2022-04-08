using System;
using System.Collections;

namespace Robust
{
	public class RobustAimControlParam : ParamBase
	{
		private readonly double[,] a, b, c;
		private readonly double[][] p, q;
		private readonly Point[] m;
		private readonly int row1, row2;
		private readonly Time time;
		private readonly double kappa;
		private readonly bool first;
		private readonly BridgeParam inner;
		private readonly ReachSetParam outer;
		private readonly EquivMatricesParam equiv;
		private const double epsilon = 0.1;

		public RobustAimControlParam(double[,] A, double[,] B, double[,] C, int Row1, int Row2, 
			double[] Mu, double[] Nu, Point[] M, double T0, double T1, double Delta, 
			double Kappa, bool First)
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
			this.p = ConstrainsToBoxes(Mu);	this.q = ConstrainsToBoxes(Nu);
			this.row1 = Row1; this.row2 = Row2; this.time = new Time(T0, T1, Delta);
			this.kappa = Kappa; this.first = First;
			inner = new BridgeParam(A, B, C, Row1, Row2, P, Q, M, T0, T1, Delta);
			Point[] ball = new Point[32];
			for (int i = 0; i < ball.Length; i++)
				ball[i] = new Point(epsilon*Math.Cos(i*2*Math.PI/ball.Length),
					epsilon*Math.Sin(i*2*Math.PI/ball.Length));
			outer = new ReachSetParam(A, C, Row1, Row2, Nu, ball, T0, T1, Delta);
			equiv = new EquivMatricesParam(A, Row1, Row2, T0, T1, Delta);
		}

		public RobustAimControlParam(double[,] A, double[,] B, double[,] C, int Row1, int Row2, 
			double[][] P, double[][] Q, Point[] M, double T0, double T1, double Delta, 
			double Kappa, bool First)
		{
			ArrayList errors = new ArrayList();
			errors.AddRange(CheckABC(A, B, C));
			errors.AddRange(CheckPQ(P, Q, B, C));
			errors.AddRange(CheckRows(Row1, Row2, A));
			errors.AddRange(CheckTerminalSet(M));
			errors.AddRange(CheckTimeDelta(T0, T1, Delta));
			if (errors.Count != 0)
				throw new ErrorListException((string[])errors.ToArray(typeof(string)));
			this.a = A; this.b = B; this.c = C; this.p = P; this.q = Q; this.m = M;
			this.row1 = Row1; this.row2 = Row2; this.time = new Time(T0, T1, Delta);
			this.kappa = Kappa; this.first = First;
			inner = new BridgeParam(A, B, C, Row1, Row2, P, Q, M, T0, T1, Delta);
			Point[] ball = new Point[32];
			for (int i = 0; i < ball.Length; i++)
				ball[i] = new Point(epsilon*Math.Cos(i*2*Math.PI/ball.Length),
					epsilon*Math.Sin(i*2*Math.PI/ball.Length));
			outer = new ReachSetParam(A, C, Row1, Row2, Q, ball, T0, T1, Delta);
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
		public double Kappa { get { return kappa; } }
		public bool First { get { return first; } }
		public BridgeParam InnerParam { get { return inner; } }
		public ReachSetParam OuterParam { get { return outer; } }
		internal EquivMatricesParam EquivParam { get { return equiv; } }

		public override bool Equals(object obj)
		{
			if (this.GetType() != obj.GetType()) return false;
			RobustAimControlParam that = (RobustAimControlParam)obj;
			if (!this.inner.Equals(that.inner) || this.kappa != that.kappa ||
				this.first != that.first) return false;
			return true;
		}

		public override int GetHashCode()
		{
			return inner.GetHashCode();
		}
	}
}