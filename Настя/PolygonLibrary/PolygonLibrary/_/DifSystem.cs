using System;
//using NUnit.Framework;

namespace Robust
{
	public class DifSystem
	{
		private const double DifStep = Constants.DifStep;
		private IDifSystemFunc func;

		private class MatrixFunc : IDifSystemFunc
		{
			private double[,] A, B, C;

			public MatrixFunc(double[,] A, double[,] B, double[,] C)
			{
				this.A = A; this.B = B; this.C = C;
			}

			public int Dim 
			{ 
				get { return A.GetLength(0); }
			}
			
			public double[] DifFunc(double[] x, double[] u, double[] v)
			{ 
				Vector z = (Matrix)A * (Vector)x;
				if (B != null && u != null) z += (Matrix)B * (Vector)u;
				if (C != null && v != null) z += (Matrix)C * (Vector)v;
				return z;
			}
		}

		public DifSystem(double[,] A) 
		{
			this.func = new MatrixFunc(A, null, null);
		}

		public DifSystem(double[,] A, double[,] B, double[,] C)
		{
			this.func = new MatrixFunc(A, B, C);
		}

		public DifSystem(IDifSystemFunc func)
		{
			this.func = func;
		}
		
		public double[] Solve(double[] x, double t0, double t1)
		{
			return Solve(x, null, null, t0, t1);
		}

		public double[] Solve(double[] x, double[] u, double[] v,
			double t0, double t1)
		{
			int count = (int)Math.Ceiling(Math.Abs(t1 - t0) / DifStep);
			int dsign = t1 > t0 ? 1 : -1;
			double t, tt, step, delta;
			step = delta = DifStep * dsign;
			Vector z = (Vector)(double[])x.Clone();
			Vector n0, n1, n2, n3;
			for (int i = 0; i < count; i++)
			{
				t = t0 + i * step; tt = t0 + (i + 1) * step;
				if (dsign == 1 && tt > t1) { tt = t1; delta = tt - t; }
				if (dsign == -1 && tt < t1) { tt = t1; delta = tt - t; }
				n0 = (Vector)func.DifFunc(z, u, v);
				n1 = (Vector)func.DifFunc(z + n0 * (delta/2), u, v);
				n2 = (Vector)func.DifFunc(z + n1 * (delta/2), u, v);
				n3 = (Vector)func.DifFunc(z + n2 * delta, u, v);
				z += (n0 + n1 * 2 + n2 * 2 + n3) * (delta/6);
			}
			return z;
		}

		public double[,] FundMatrix(double t1, double t0)
		{
			int dim = func.Dim;
			double[,] z = new double[dim, dim];
			double[] y, x = new double[dim];
			for (int i = 0; i < dim; i++) 
			{
				x[i] = 1; y = Solve(x, t0, t1);
				for (int j = 0; j < dim; j++) z[j, i] = y[j];
				x[i] = 0;
			}
			return z;
		}
	}

/*#if DEBUG
	[TestFixture]public class DifSystemTest
	{
		private DifSystem ds, cds;
		[SetUp]public void SetUp()
		{
			double[,] A = {{0, 1}, {0, 0}};
			double[,] B = {{0}, {1}};
			double[,] C = {{1}, {0}};
			ds = new DifSystem(A);
			cds = new DifSystem(A, B, C);
		}
		[Test]public void SolveForward()
		{
			Vector z = (Vector)ds.Solve(new double[] {1, 2}, 0, 2);
			Assert.AreEqual("{5, 2}", z.ToString());
		}
		[Test]public void SolveBackward()
		{
			Vector z = (Vector)ds.Solve(new double[] {5, 2}, 2, 0);
			Assert.AreEqual("{1, 2}", z.ToString());
		}
		[Test]public void SolveWithControlForward()
		{
			Vector z = (Vector)cds.Solve(new double[] {1, 2}, 
				new double[] {1}, new double[] {-2}, 0, 2);
			Assert.AreEqual("{3, 4}", z.ToString());
		}
		[Test]public void SolveWithControlBackward()
		{
			Vector z = (Vector)cds.Solve(new double[] {3, 4}, 
				new double[] {1}, new double[] {-2}, 2, 0);
			Assert.AreEqual("{1, 2}", z.ToString());
		}
		[Test]public void FundMatrix()
		{
			Matrix X = (Matrix)ds.FundMatrix(2, 0);
			Assert.AreEqual("{{1, 2}, {0, 1}}", X.ToString());
		}
	}
#endif*/
}
