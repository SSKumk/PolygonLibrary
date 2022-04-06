using System;
using System.Collections;

namespace Robust
{
	public class ParamBase
	{
		public ArrayList CheckABC(double[,] A, double[,] B, double[,] C)
		{
			ArrayList errors = new ArrayList();
			if (A == null) errors.Add(Constants.AIsNull);
			if (B == null) errors.Add(Constants.BIsNull);
			if (C == null) errors.Add(Constants.CIsNull);
			if (errors.Count == 0)
			{
				int n = A.GetLength(0);
				if (n != A.GetLength(1)) errors.Add(Constants.ASizes);
				if (n != B.GetLength(0)) errors.Add(Constants.ABSizes);
				if (n != C.GetLength(0)) errors.Add(Constants.ACSizes);
			}
			return errors;
		}

		public ArrayList CheckAB(double[,] A, double[,] B)
		{
			ArrayList errors = new ArrayList();
			if (A == null) errors.Add(Constants.AIsNull);
			if (B == null) errors.Add(Constants.BIsNull);
			if (errors.Count == 0)
			{
				int n = A.GetLength(0);
				if (n != A.GetLength(1)) errors.Add(Constants.ASizes);
				if (n != B.GetLength(0)) errors.Add(Constants.ABSizes);
			}
			return errors;
		}

		public ArrayList CheckMuNu(double[] Mu, double[] Nu, 
			double[,] B, double[,] C)
		{
			ArrayList errors = new ArrayList();
			if (Mu == null) errors.Add(Constants.MuIsNull);
			if (Nu == null) errors.Add(Constants.NuIsNull);
			if (errors.Count == 0 && B != null && C != null)
			{
				if (B.GetLength(1) != Mu.Length) errors.Add(Constants.MuBSizes);
				if (C.GetLength(1) != Nu.Length) errors.Add(Constants.NuCSizes);
			}
			return errors;
		}

		public ArrayList CheckPQ(double[][] P, double[][] Q, double[,] B, double[,] C)
		{
			ArrayList errors = new ArrayList();
			if (P == null) errors.Add(Constants.PIsNull);
			if (Q == null) errors.Add(Constants.QIsNull);
			if (errors.Count == 0)
			{
				if (P.Length < 2) errors.Add(Constants.PSmall);
				if (Q.Length < 2) errors.Add(Constants.QSmall);
			}
			if (errors.Count == 0)
			{
				int dp = P[0].Length, dq = Q[0].Length;
				for (int i = 1; i < P.Length; i++)
					if (P[i].Length != dp) { errors.Add(Constants.PSizes); break; }
				for (int i = 1; i < Q.Length; i++)
					if (Q[i].Length != dq) { errors.Add(Constants.QSizes); break; }
				if (B != null && C != null)
				{
					if (B.GetLength(1) != dp) errors.Add(Constants.PBSizes);
					if (C.GetLength(1) != dq) errors.Add(Constants.QCSizes);
				}
			}
			return errors;
		}

		public ArrayList CheckMu(double[] Mu, double[,] B)
		{
			ArrayList errors = new ArrayList();
			if (Mu == null) errors.Add(Constants.MuIsNull);
			if (errors.Count == 0 && B != null)
				if (B.GetLength(1) != Mu.Length) errors.Add(Constants.MuBSizes);
			return errors;
		}

		public ArrayList CheckP(double[][] P, double[,] B)
		{
			ArrayList errors = new ArrayList();
			if (P == null) errors.Add(Constants.PIsNull);
			if (errors.Count == 0)
				if (P.Length < 2) errors.Add(Constants.PSmall);
			if (errors.Count == 0)
			{
				int dp = P[0].Length;
				for (int i = 1; i < P.Length; i++)
					if (P[i].Length != dp) { errors.Add(Constants.PSizes); break; }
				if (B != null)
					if (B.GetLength(1) != dp) errors.Add(Constants.PBSizes);
			}
			return errors;
		}

		public ArrayList CheckTerminalSet(Point[] M)
		{
			ArrayList errors = new ArrayList();
			if (M == null) errors.Add(Constants.MIsNull);
			if (M != null && M.Length == 0) errors.Add(Constants.MIsNull);
			return errors;
		}

		public ArrayList CheckRows(int Row1, int Row2, 
			double[,] A)
		{
			ArrayList errors = new ArrayList();
			if (A != null && A.GetLength(0) == A.GetLength(1))
			{
				int n = A.GetLength(0);
				if (Row1 == Row2) errors.Add(Constants.RowsEqual);
				if (Row1 < 0 || Row1 >= n) errors.Add(Constants.Row1Wrong);
				if (Row2 < 0 || Row2 >= n) errors.Add(Constants.Row2Wrong);
			}
			return errors;
		}

		public ArrayList CheckTimeDelta(double T0, double T1,
			double Delta)
		{
			ArrayList errors = new ArrayList();
			if (T0 >= T1) errors.Add(Constants.TimeWrong);
			if (Delta <= 0) errors.Add(Constants.TimeDeltaWrong);
			return errors;
		}

		public ArrayList CheckPoint(double[] point, double[,] A)
		{
			ArrayList errors = new ArrayList();
			if (A.GetLength(0) != point.Length) errors.Add(Constants.PointSize);
			return errors;
		}

		public bool EqualSets(Point[] a, Point[] b)
		{
			if (a.Length != b.Length) return false;
			for (int i = 0; i < a.Length; i++)
				if (a[i] != b[i]) return false;
			return true;
		}

		public bool EqualBoxes(double[][] a, double[][] b)
		{
			if (a.Length != b.Length) return false;
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i].Length != b[i].Length) return false;
				for (int j = 0; j < a[i].Length; j++)
					if (a[i][j] != b[i][j]) return false;
			}
			return true;
		}

		protected double[][] ConstrainsToBoxes(double[] lim)
		{
			int k = lim.Length, n = (int)Math.Pow(2, k);
			double[][] box = new double[n][];
			for (int i = 0; i < n; i++)
			{
				box[i] = new double[k];
				for (int j = 0, pow = 1; j < k; j++, pow *= 2)
					if ((i & pow) != 0) box[i][j] = lim[j];
					else box[i][j] = -lim[j];
			}
			return box;
		}
	}
}
