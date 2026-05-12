using System;
//using NUnit.Framework;

namespace Robust
{
	public class SwitchControl : ControlBase
	{
		private SwitchControlParam param;
		private Bridge[] bridges;
		private SwitchLine[] surf;
		private EquivMatrices equiv;

		public SwitchControl(SwitchControlParam param)
		{
			this.param = param;
			bridges = new Bridge[param.CCount];
			for (int i = 0; i < bridges.Length; i++)
				bridges[i] = new Bridge(param.BridgeParams[i]);
			equiv = new EquivMatrices(param.EquivParam);
			history = new History();
			history.CreateToken("time");
			history.CreateToken("trajectory");
			history.CreateToken("control");
			history.CreateToken("control_vector");
			history.CreateToken("control_vectogram");
			history.CreateToken("bridges_aux");
			history.CreateToken("switchingsurface");
		}

		public SwitchControlParam Param
		{
			get { return param; }
			set
			{
				if (!param.Equals(value))
				{
					bridges = ReallocateBridges(value.BridgeParams,	
						param.BridgeParams, bridges);
					param = value;
					equiv.Param = param.EquivParam;
					calculated = false;
					surf = null; 
					history.ClearHistory();
				}
			}
		}

		private Bridge[] ReallocateBridges(BridgeParam[] newparam, BridgeParam[] oldparam, 
			Bridge[] oldbridge)
		{
			Bridge[] newbridge = new Bridge[newparam.Length];
			for (int i = 0; i < newparam.Length; i++)
				for (int j = 0; j < oldparam.Length; j++)
					if (newparam[i].Equals(oldparam[j]))
						newbridge[i] = oldbridge[j];
			for (int i = 0; i < newbridge.Length; i++)
				if (newbridge[i] == null)
					newbridge[i] = new Bridge(newparam[i]);
			return newbridge;
		}

		public override int CalculationTime
		{
			get 
			{ 
				if (Calculated) return 0;
				int time = param.Time.Count + equiv.CalculationTime;
				for (int i = 0; i < bridges.Length; i++)
					time += bridges[i].CalculationTimeWithoutEquiv;
				return time;
			}
		}

		public override void Calculate(AsyncCalculation result)
		{
			if (Calculated) return;
			equiv.Calculate(result);
			for (int i = 0; i < bridges.Length; i++)
				bridges[i].Calculate(result, equiv);
			if (bridges[bridges.Length-1][0].isEmpty)
				throw new ErrorListException(new string[] { Constants.SwitchBridgeBreak });
			int time = param.Time.Count;
			int npoly = bridges.Length;
			surf = new SwitchLine[time];
			double[][,] matrix = equiv.EquivMatrix(param.First ? param.B : param.C);
			Poly[] polies = new Poly[npoly];
			for (int t = 0; t < time; t++)
			{
				for (int i = 0; i < npoly; i++)
					polies[i] = bridges[i][t];
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
			for (int i = 0; i < limits.Length; i++)
				u[i] = limits[i] * sign[i] * first;
			double[,] matrix = param.First ? param.B : param.C;
			double[,] eqmatrix = equiv.EquivMatrix(matrix, t);
			Poly[] auxpoly = new Poly[bridges.Length];
			for (int i = 0; i < auxpoly.Length; i++) auxpoly[i] = bridges[i][t];
			history.AddRecord("time", time);
			history.AddRecord("trajectory", y);
			history.AddRecord("control", u);
			history.AddRecord("control_vector", new Point(eqmatrix, u));
			history.AddRecord("control_vectogram", new Poly(eqmatrix, limits));
			history.AddRecord("bridges_aux", auxpoly);
			history.AddRecord("switchingsurface", surf[t]);
			return u;
		}

		public override void ToCompactForm()
		{
			foreach (Bridge bridge in bridges)
				bridge.ToCompactForm();
		}
	}

/*#if DEBUG
	[TestFixture]public class SwitchControlTest
	{
		[Test]public void CalculationTime()
		{
			double[,] A = new double[,] {{0, 1}, {0, 0}};
			double[,] B = new double[,] {{0}, {1}};
			double[,] C = new double[,] {{1}, {0}};
			double[] Mu = new double[] {1};
			double[] Nu = new double[] {1};
			Point[] M = new Point[128];
			for (int i = 0; i < M.Length; i++)
				M[i] = new Point(
					2 * Math.Cos(i * 2 * Math.PI / M.Length),
					2 * Math.Sin(i * 2 * Math.PI / M.Length));
			SwitchControlParam p1 = new SwitchControlParam(
				A, B, C, 0, 1, Mu, Nu, M, 0, 4, .05, .1, 1, 5, true);
			SwitchControl rc = new SwitchControl(p1);
			int time = p1.Time.Count;
			Assert.AreEqual(time*(5+2), rc.CalculationTime);
			rc.Calculate();
			SwitchControlParam p2 = new SwitchControlParam(
				A, B, C, 0, 1, Mu, Nu, M, 0, 4, .05, .1, 1, 9, true);
			rc.Param = p2;
			Assert.AreEqual(time*(4+1), rc.CalculationTime);
		}
	}
#endif*/
}