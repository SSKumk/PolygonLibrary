using System;

namespace Robust
{
	public class ExtremalAimControl : ControlBase
	{
		private ExtremalAimControlParam param;
		private Bridge[] bridges;
		private EquivMatrices equiv;

		public ExtremalAimControl(ExtremalAimControlParam param)
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
			history.CreateToken("aux_vector");
			history.CreateToken("bridges_aux");
		}

		public ExtremalAimControlParam Param
		{
			get { return param; }
			set
			{
				if (param.Equals(value)) return;
				bridges = ReallocateBridges(value.BridgeParams,	param.BridgeParams, bridges);
				equiv.Param = value.EquivParam;
				param = value;
				calculated = false;
				history.ClearHistory();
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
				int time = equiv.CalculationTime;
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
			calculated = true;
		}
		
		public override double[] GetControl(double[] x, double time)
		{
			int t = param.Time.Index(time);
			Point y = new Point(equiv.FundMatrix(t), x);
			int first = param.First ? 1 : -1;
			double[,] matrix = param.First ? param.B : param.C;
			double[,] eqmatrix = equiv.EquivMatrix(matrix, t);
			double[][] constr = param.First ? param.P : param.Q;
			int index = 0;
			while (index < param.CCount && bridges[index][t].ContainsIndex(y) >= 1) index++;
			double[] u; Point vaim = Point.Empty;
			if (index > 0)
			{
				index--;
				vaim = bridges[index][t].NearestPoint(y) - y;
				double vdif = double.NegativeInfinity;
				int vidx = 0;
				for (int i = 0; i < constr.Length; i++)
				{
					double[] v = ((Matrix)eqmatrix) * ((Vector)constr[i]);
					double dif = v[0] * vaim.x + v[1] * vaim.y;
					if (dif > vdif) { vdif = dif; vidx = i; }
				}
				u = ((Vector)constr[vidx]) * first;
			}
			else if (index == 0 && param.First) u = new double[param.P[0].Length];
			else 
			{
				vaim = Point.Empty - y;
				double vdif = double.NegativeInfinity;
				int vidx = 0;
				for (int i = 0; i < constr.Length; i++)
				{
					double[] v = ((Matrix)eqmatrix) * ((Vector)constr[i]);
					double dif = v[0] * vaim.x + v[1] * vaim.y;
					if (dif > vdif) { vdif = dif; vidx = i; }
				}
				u = ((Vector)constr[vidx]) * first;
			}
			Poly[] auxpoly = new Poly[bridges.Length];
			for (int i = 0; i < auxpoly.Length; i++) auxpoly[i] = bridges[i][t];
			history.AddRecord("time", time);
			history.AddRecord("trajectory", y);
			history.AddRecord("control", u);
			history.AddRecord("control_vector", new Point(eqmatrix, u));
			history.AddRecord("control_vectogram", new Poly(eqmatrix, constr));
			history.AddRecord("aux_vector", vaim);
			history.AddRecord("bridges_aux", auxpoly);
			return u;
		}

		public override void ToCompactForm()
		{
			foreach (Bridge bridge in bridges)
				bridge.ToCompactForm();
		}
	}
}
