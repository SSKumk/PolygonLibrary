using System;

namespace Robust
{
	public class RobustAimControl : ControlBase
	{
		private RobustAimControlParam param;
		private Bridge inner;
		private ReachSet outer;
		private EquivMatrices equiv;

		public RobustAimControl(RobustAimControlParam param)
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
			history.CreateToken("aux_vector");
			history.CreateToken("bridge_main");
			history.CreateToken("bridge_aux");
		}

		public RobustAimControlParam Param
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
					history.ClearHistory();
				}
			}
		}

		public override int CalculationTime
		{
			get 
			{ 
				return Calculated ? 0 : equiv.CalculationTime +
					inner.CalculationTimeWithoutEquiv + 
					outer.CalculationTimeWithoutEquiv;
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
			calculated = true;
		}
		
		public override double[] GetControl(double[] x, double time)
		{
			int t = param.Time.Index(time);
			Point y = new Point(equiv.FundMatrix(t), x);
			double k = inner[t].KappaAimIndex(y, outer[t], param.Kappa);
			Poly aim = k <= 1 ? inner[t].Mul(k) : inner[t].Add(outer[t].Mul(k-1));
			Point vaim = aim.NearestPoint(y) - y;
			int first = param.First ? 1 : -1;
			double[,] matrix = param.First ? param.B : param.C;
			double[,] eqmatrix = equiv.EquivMatrix(matrix, t);
			double[][] constr = param.First ? param.P : param.Q;
			double vdif = double.NegativeInfinity;
			int vidx = 0;
			for (int i = 0; i < constr.Length; i++)
			{
				double[] v = ((Matrix)eqmatrix) * ((Vector)constr[i]);
				double dif = v[0] * vaim.x + v[1] * vaim.y;
				if (dif > vdif) { vdif = dif; vidx = i; }
			}
			double[] u = ((Vector)constr[vidx]) * (first * Math.Min(k, 1));
			history.AddRecord("time", time);
			history.AddRecord("trajectory", y);
			history.AddRecord("control", u);
			history.AddRecord("control_vector", new Point(eqmatrix, u));
			history.AddRecord("control_vectogram", new Poly(eqmatrix, constr));
			history.AddRecord("aux_vector", vaim);
			history.AddRecord("bridge_main", inner[t]);
			history.AddRecord("bridge_aux", aim);
			return u;
		}

		public override void ToCompactForm()
		{
			if (inner != null) inner.ToCompactForm();
			if (outer != null) outer.ToCompactForm();
		}
	}
}