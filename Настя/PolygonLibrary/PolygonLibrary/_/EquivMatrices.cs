using System;
//using NUnit.Framework;

namespace Robust
{
	public class EquivMatricesParam : ParamBase
	{
		private readonly double[,] a;
		private readonly Time time;
		private readonly int row1, row2;

		public EquivMatricesParam(double[,] A, int Row1, int Row2, 
			double T0, double T1, double Delta)
		{
			this.a = A; this.time = new Time(T0, T1, Delta);
			this.row1 = Row1; this.row2 = Row2;
		}
		
		public double[,] A { get { return a; } }
		public int Row1 { get { return row1; } }
		public int Row2 { get { return row2; } }
		public Time Time { get { return time; } }

		public override bool Equals(object obj)
		{
			if (this.GetType() != obj.GetType()) return false;
			EquivMatricesParam that = (EquivMatricesParam)obj;
			if ((Matrix)this.a != (Matrix)that.a || this.time != that.time ||
				this.row1 != that.row1 ||  this.row2 != that.row2) 
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			return a.GetHashCode() + time.GetHashCode() + row1.GetHashCode() + row2.GetHashCode();
		}
	}
	
	public class EquivMatrices : CalculateBase
	{
		private EquivMatricesParam param;
		private double[][,] fundmatrix;

		public EquivMatrices(EquivMatricesParam param)
		{
			this.param = param;
		}

		public EquivMatricesParam Param
		{
			get { return param; }
			set 
			{
				if (!param.Equals(value))
				{
					param = value; 
					calculated = false;
					fundmatrix = null;
				}
			}
		}

		public override int CalculationTime
		{
			get { return Calculated ? 0 : param.Time.Count; }
		}

		public static int GetCalculationTime(EquivMatricesParam param)
		{
			return param.Time.Count;
		}

		public override void Calculate(AsyncCalculation result)
		{
			if (Calculated) return;
			DifSystem ds = new DifSystem(param.A);
			int ntime = param.Time.Count;
			fundmatrix = new double[ntime][,];
			for (int t = 0; t < ntime; t++)
			{
				fundmatrix[t] = 
					((Matrix)ds.FundMatrix(param.Time[ntime-1], param.Time[t])).
						CutRows(param.Row1, param.Row2);
				result.Ready++;
			}
			/*int ntime = param.Time.Count;
			fundmatrix = new double[ntime][,];
			System.IO.StreamReader read = new System.IO.StreamReader("fund.txt");
			for (int t = 0; t < ntime; t++)
			{
				string s = read.ReadLine();
				fundmatrix[200-t] = new double[2, 8];
				for (int i = 0; i < 8; i++)
					fundmatrix[200-t][0, i] = fundmatrix[200-t][1, i] =
						double.Parse(s.Substring((i+1) * 10, 7));
			}
			read.Close();*/
			calculated = true;
		}

		public double[][,] FundMatrix()
		{
			return fundmatrix;
		}

		public double[,] FundMatrix(int time)
		{
			return fundmatrix[time];
		}

		public double[][,] EquivMatrix(double[,] multmatrix)
		{
			int ntime = fundmatrix.Length;
			double[][,] equiv = new double[ntime][,];
			for (int t = 0; t < ntime; t++)
				equiv[t] = EquivMatrix(multmatrix, t);
			return equiv;
		}

		public double[,] EquivMatrix(double[,] multmatrix, int time)
		{
			return (Matrix)fundmatrix[time] * (Matrix)multmatrix;
		}
	}

/*#if DEBUG
	[TestFixture]public class EquivMatricesTest
	{
		[Test]public void EquivMatrix()
		{
			double[,] A = {{0, 1}, {0, 0}};
			EquivMatricesParam equivparam = 
				new EquivMatricesParam(A, 0, 1, -2, 0, 1);
			EquivMatrices equiv = new EquivMatrices(equivparam);
			equiv.Calculate();
			double[][,] X = equiv.EquivMatrix(new double[,] {{1},{1}});
			Assert.AreEqual("{{3}, {1}}", ((Matrix)X[0]).ToString());
			Assert.AreEqual("{{2}, {1}}", ((Matrix)X[1]).ToString());
			Assert.AreEqual("{{1}, {1}}", ((Matrix)X[2]).ToString());
		}
	}
#endif*/
}