using System;
//using NUnit.Framework;

namespace Robust
{
	public class RobustControl : ControlBase
	{
		private RobustControlParam param;
		private Bridge inner;
		private ReachSet outer;
		private SwitchLine[] surf;
		private TimePoly add;
		private EquivMatrices equiv;

		public RobustControl(RobustControlParam param)
		{
			this.param = param;
			inner = new Bridge(param.InnerParam);
			outer = new ReachSet(param.OuterParam);
			equiv = new EquivMatrices(param.EquivParam);
			history = new History();
			history.CreateToken("time");
			history.CreateToken("trajectory");
			history.CreateToken("control");
			history.CreateToken("control_vector");
			history.CreateToken("control_vectogram");
			history.CreateToken("bridge_main");
			history.CreateToken("bridge_aux");
			history.CreateToken("switchingsurface");
		}

		public RobustControlParam Param
		{
			get { return param; }
			set
			{
				if (!param.Equals(value))
				{
					param = value;
					inner.Param = param.InnerParam;
					outer.Param = param.OuterParam;
					equiv.Param = param.EquivParam;
					calculated = false;
					surf = null; add = null;
					history.ClearHistory();
				}
			}
		}

		public override int CalculationTime
		{
			get 
			{ 
				return Calculated ? 0 : param.Time.Count +
					inner.CalculationTimeWithoutEquiv + 
					outer.CalculationTimeWithoutEquiv +
					equiv.CalculationTime;
			}
		}

		public override void Calculate(AsyncCalculation result)
		{
			if (Calculated) return;
			equiv.Calculate(result);
			inner.Calculate(result, equiv);
			if (inner[0].isEmpty)
				throw new ErrorListException(new string[] { Constants.RobustBridgeBreak });
			outer.Calculate(result, equiv);
			int time = param.Time.Count;
			add = new TimePoly(time);
			surf = new SwitchLine[time];
			double[][,] matrix = equiv.EquivMatrix(param.First ? param.B : param.C);
			Poly[] polies = new Poly[2];
			for (int t = 0; t < time; t++)
			{
				polies[0] = inner[t];
				polies[1] = add[t] = inner[t].Add(outer[t].Mul(param.OuterCoef));
				surf[t] = new SwitchLine(polies, matrix[t]);
				result.Ready++;
			}
			calculated = true;
		}
		
		public override double[] GetControl(double[] x, double time)
		{
			int t = param.Time.Index(time);
			Point y = new Point(equiv.FundMatrix(t), x);
			int first = param.First ? 1 : -1;
			double[] limits = param.First ? param.Mu : param.Nu;
			double[] u = new double[limits.Length];
			int[] sign = surf[t].PointSign(y);
			double idx = inner[t].ContainsIndex(y);
			if (idx > 1) idx = 1;
			for (int i = 0; i < u.Length; i++)
			{
				u[i] = limits[i] * idx * sign[i] * first;
				if (u[i] > limits[i]) u[i] = limits[i];
				if (u[i] < -limits[i]) u[i] = -limits[i];
			}
			double[,] matrix = param.First ? param.B : param.C;
			double[,] eqmatrix = equiv.EquivMatrix(matrix, t);
			history.AddRecord("time", time);
			history.AddRecord("trajectory", y);
			history.AddRecord("control", u);
			history.AddRecord("control_vector", new Point(eqmatrix, u));
			history.AddRecord("control_vectogram", new Poly(eqmatrix, limits));
			history.AddRecord("bridge_main", inner[t]);
			history.AddRecord("bridge_aux", add[t]);
			history.AddRecord("switchingsurface", surf[t]);
			return u;
		}

		public override void ToCompactForm()
		{
			if (inner != null) inner.ToCompactForm();
			if (outer != null) outer.ToCompactForm();
			if (add != null) add.ToCompactForm();
		}
	}

/*#if DEBUG
	[TestFixture]public class RobustControlTest
	{
		[Test]public void CalculationTime()
		{
			double[,] A = new double[,] {{0, 1}, {0, 0}};
			double[,] B = new double[,] {{0}, {1}};
			double[,] C = new double[,] {{1}, {0}};
			double[] Mu = new double[] {1};
			double[] Nu = new double[] {1};
			double[] Mu1 = new double[] {.5};
			Point[] M = new Point[128];
			for (int i = 0; i < M.Length; i++)
				M[i] = new Point(
					2 * Math.Cos(i * 2 * Math.PI / M.Length),
					2 * Math.Sin(i * 2 * Math.PI / M.Length));
			RobustControlParam p1 = new RobustControlParam(
				A, B, C, 0, 1, Mu, Nu, M, 0, 4, .05, 1, true);
			RobustControl rc = new RobustControl(p1);
			int time = p1.Time.Count;
			Assert.AreEqual(time*4, rc.CalculationTime);
			rc.Calculate();
			RobustControlParam p2 = new RobustControlParam(
				A, B, C, 0, 1, Mu1, Nu, M, 0, 4, .05, 1, true);
			rc.Param = p2;
			Assert.AreEqual(time*2, rc.CalculationTime);
			rc.Calculate();
			RobustControlParam p3 = new RobustControlParam(
				A, B, C, 0, 1, Mu1, Nu, M, 0, 4, .05, 1, false);
			rc.Param = p3;
			Assert.AreEqual(time, rc.CalculationTime);
		}
	}
#endif*/
}