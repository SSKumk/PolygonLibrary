using System;
//using NUnit.Framework;

namespace Robust
{
	public class Bridge : CalculateBase, ITimePoly
	{
		private BridgeParam param;
		private Poly[] bridge;

		public Bridge(BridgeParam param)
		{
			this.param = param;
		}

		public BridgeParam Param
		{
			get { return param; }
			set
			{
				if (!param.Equals(value))
				{
					param = value;
					calculated = false;
					bridge = null;
				}
			}
		}

		public Poly this[int time]
		{
			get { return Calculated ? bridge[time] : null; }
		}

		public int nTime
		{
			get { return Calculated ? bridge.Length : 0; }
		}

		public override int CalculationTime
		{
			get 
			{ 
				return Calculated ? 0 : param.Time.Count +
					EquivMatrices.GetCalculationTime(param.EquivParam); 
			}
		}

		public override void Calculate(AsyncCalculation result)
		{
			if (Calculated) return;
			EquivMatrices equiv = new EquivMatrices(param.EquivParam);
			equiv.Calculate(result);
			Calculate(result, equiv);
		}

		internal int CalculationTimeWithoutEquiv
		{
			get { return Calculated ? 0 : param.Time.Count; }
		}

		internal void Calculate(AsyncCalculation result, EquivMatrices equiv)
		{
			if (Calculated) return;
			int time = param.Time.Count;
			bridge = new Poly[time];
			bridge[time-1] = new Poly(param.M);
			result.Ready++;
			for (int t = time-2; t >= 0; t--)
			{
				if (!bridge[t+1].isEmpty)
				{
					double delta = param.Time[t+1] - param.Time[t];
					Poly BP = new Poly(equiv.EquivMatrix(param.B, t+1), param.P).Mul(delta);
					Poly CQ = new Poly(equiv.EquivMatrix(param.C, t+1), param.Q).Mul(delta);
					bridge[t] = bridge[t+1].Add(BP).Sub(CQ);
				}
				else
					bridge[t] = bridge[t+1];
				result.Ready++;
			}
			calculated = true;
		}

		public void ToCompactForm()
		{
			if (bridge == null) return;
			foreach (Poly p in bridge) p.ToCompactForm();
		}
	}

/*#if DEBUG
	[TestFixture]public class BridgeTest
	{
		[Test]public void SimpleBridge()
		{
			double[,] A = new double[,] { {0, 0}, {0, 0} };
			double[,] B = new double[,] { {1}, {0} };
			double[,] C = new double[,] { {0}, {1} };
			double[] Mu = new double[] {1};
			double[] Nu = new double[] {1};
			Point[] M = new Point[4];
			M[0] = new Point(10, 10); M[1] = new Point(-10, -10);
			M[2] = new Point(-10, 10); M[3] = new Point(10, -10);
			BridgeParam param = new BridgeParam(A, B, C, 0, 1,
				Mu, Nu, M, 0, 2, .1);
			Bridge bridge = new Bridge(param);
			bridge.Calculate();
			Poly p = bridge[0];
			double maxx = 0, maxy = 0;
			for (int i = 0; i < p.nPoints; i++)
			{
				if (p[i].x > maxx) maxx = p[i].x;
				if (p[i].y > maxy) maxy = p[i].y;
			}
			Assert.AreEqual(12, maxx, 1e-4);
			Assert.AreEqual(8, maxy, 1e-4);
		}

		[Test]public void ComplexBridge()
		{
			double[,] A = new double[,] {{0, 1}, {0, 0}};
			double[,] B = new double[,] {{0}, {1}};
			double[,] C = new double[,] {{1}, {0}};
			int Row1 = 0, Row2 = 1;
			double[] Mu = new double[] {1};
			double[] Nu = new double[] {1};
			Point[] M = new Point[128];
			for (int i = 0; i < M.Length; i++)
				M[i] = new Point(
					2 * Math.Cos(i * 2 * Math.PI / M.Length),
					2 * Math.Sin(i * 2 * Math.PI / M.Length));
			double T0 = 0, T1 = 4, Delta = .05;
			BridgeParam param = new BridgeParam(A, B, C, 
				Row1, Row2, Mu, Nu, M, T0, T1, Delta);
			Bridge b = new Bridge(param);
			b.Calculate();
			Poly p = b[0];
			double maxx = 0, maxy = 0;
			for (int i = 0; i < p.nPoints; i++)
			{
				if (p[i].x > maxx) maxx = p[i].x;
				if (p[i].y > maxy) maxy = p[i].y;
			}
			Assert.AreEqual(5.8882, maxx, 1e-4);
			Assert.AreEqual(3.6424, maxy, 1e-4);
		}

		[Test]public void ButtonBridge()
		{
			double[,] A = new double[,] {{1, 2}, {0, 1}};
			double[,] B = new double[,] {{0}, {1}};
			double[,] C = new double[,] {{1}, {0}};
			int Row1 = 0, Row2 = 1;
			double[] Mu = new double[] {1};
			double[] Nu = new double[] {.9};
			double R = Math.Sqrt(100);
			Point[] M = new Point[128];
			for (int i = 0; i < M.Length; i++)
				M[i] = new Point(
					R * Math.Cos(i * 2 * Math.PI / M.Length),
					R * Math.Sin(i * 2 * Math.PI / M.Length));
			double T0 = 0, T1 = 8, Delta = .05;
			BridgeParam param = new BridgeParam(A, B, C, 
				Row1, Row2, Mu, Nu, M, T0, T1, Delta);
			Bridge b = new Bridge(param);
			b.Calculate();
			Poly p = b[0];
		}
	}
#endif*/
}
