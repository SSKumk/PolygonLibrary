using System;

namespace Robust
{
	public class Simulator : SimulatorBase
	{
		private SimulatorParam param;

		public Simulator(SimulatorParam param)
		{
			this.param = param;
			history = new History();
			history.CreateToken("time");
			history.CreateToken("trajectory");
			history.CreateToken("firstplayer");
			history.CreateToken("secondplayer");
		}

		public SimulatorParam Param
		{
			get { return param; }
			set
			{
				param = value;
				history.ClearHistory();
				calculated = false;
			}
		}

		public override int CalculationTime
		{
			get 
			{ 
				return Calculated ? 0 : param.Time.Count +
					param.FirstPlayer.CalculationTime +
					param.SecondPlayer.CalculationTime; 
			}
		}

		public override void Calculate(AsyncCalculation result)
		{
			if (Calculated) return;
			param.FirstPlayer.Calculate(result);
			param.SecondPlayer.Calculate(result);
			param.FirstPlayer.StartHistory();
			param.SecondPlayer.StartHistory();
			StartHistory();
			DifSystem ds = param.DifSystem;
			int time = param.Time.Count;
			double[] x = null, u = null, v = null;
			for (int t = 0; t < time; t++)
			{
				if (t > 0)
					x = ds.Solve(x, u, v, param.Time[t-1], param.Time[t]);
				else
					x = param.InitialPoint;
				u = param.FirstPlayer.GetControl(x, param.Time[t]);
				v = param.SecondPlayer.GetControl(x, param.Time[t]);
				history.AddRecord("time", param.Time[t]);
				history.AddRecord("trajectory", x);
				history.AddRecord("firstplayer", u);
				history.AddRecord("secondplayer", v);
				result.Ready++;
			}
			param.FirstPlayer.StopHistory();
			param.SecondPlayer.StopHistory();
			StopHistory();
			calculated = true;
		}

		public double[][] GetTrajectory()
		{
			return GetVectorRecord("trajectory");
		}

		public double[][] GetFirstPlayerRecord()
		{
			return GetVectorRecord("firstplayer");
		}

		public double[][] GetSecondPlayerRecord()
		{
			return GetVectorRecord("secondplayer");
		}

		public double[] GetTimeAxis()
		{
			return (double[])history.GetRecords("time").ToArray(typeof(double));
		}

		public Point[] GetOriginalTrajectory(int row1, int row2)
		{
			double[][] traj = GetTrajectory();
			Point[] p = new Point[traj[0].Length];
			for (int i = 0; i < p.Length; i++)
				p[i] = new Point(traj[row1][i], traj[row2][i]);
			return p;
		}

		private double[][] GetVectorRecord(string token)
		{
			double[][] rec = (double[][])history.GetRecords(token).ToArray(typeof(double[]));
			double[][] v = new double[rec[0].Length][];
			for (int i = 0; i < v.Length; i++)
			{
				v[i] = new double[rec.Length];
				for (int j = 0; j < v[i].Length; j++) v[i][j] = rec[j][i];
			}
			return v;
		}
	}
}