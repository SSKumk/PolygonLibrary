using System;
//using NUnit.Framework;

namespace Robust
{
	public class ReachSet : CalculateBase, ITimePoly
	{
		private ReachSetParam param;
		private Poly[] reachset;

		public ReachSet(ReachSetParam param)
		{
			this.param = param;
		}

		public ReachSetParam Param
		{
			get { return param; }
			set
			{
				if (!param.Equals(value))
				{
					param = value;
					calculated = false;
					reachset = null;
				}
			}
		}

		public Poly this[int time]
		{
			get { return Calculated ? reachset[time] : null; }
		}

		public int nTime
		{
			get { return Calculated ? reachset.Length : 0; }
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

		internal void Calculate(AsyncCalculation result,
			EquivMatrices equiv)
		{
			if (Calculated) return;
			int time = param.Time.Count;
			reachset = new Poly[time];
			reachset[0] = new Poly(param.ZeroSetEquiv);
			result.Ready++;
			for (int t = 1; t < time; t++)
			{
				double delta = param.Time[t] - param.Time[t-1];
				Poly BP = new Poly(equiv.EquivMatrix(param.B, t-1), param.P).Mul(delta);
				reachset[t] = reachset[t-1].Add(BP);
				result.Ready++;
			}
			calculated = true;
		}

		public void ToCompactForm()
		{
			if (reachset == null) return;
			foreach (Poly p in reachset)
				p.ToCompactForm();
		}
	}

/*#if DEBUG
	[TestFixture]public class ReachSetTest 
	{
		[Test]public void SimpleSet()
		{
			double[,] A = new double[,] {{0, 0}, {0, 0}};
			double[,] B = new double[,] {{1, 0}, {0, 1}};
			double[] Mu = new double[] {1, 2};
			Point[] Zero = new Point[] {new Point(0, 0)};
			int Row1 = 0, Row2 = 1;
			double T0 = 0, T1 = 1, Tdelta = .05;
			ReachSetParam param = new ReachSetParam(A, B,
				Row1, Row2, Mu, Zero, T0, T1, Tdelta);
			ReachSet rs = new ReachSet(param);
			rs.Calculate();
			Poly last = rs[rs.nTime-1];
			Assert.AreEqual(4, last.nPoints);
			Assert.AreEqual("{{1, -2}, {1, 2}, {-1, 2}, {-1, -2}}", last.ToString());
		}
	}
#endif*/
}
