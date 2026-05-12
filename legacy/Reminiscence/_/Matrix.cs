using System;
//using NUnit.Framework;

namespace Robust
{
	internal struct Matrix : ICloneable
	{
		private double[,] matrix;

		public Matrix(double[,] matrix) 
		{
			this.matrix = matrix;
		}

		public static implicit operator double[,] (Matrix m)
		{
			return m.matrix;
		}

		public static explicit operator Matrix (double[,] m)
		{
			return new Matrix(m);
		}

		public static Matrix operator * (Matrix a, double x)
		{
			double[,] z = new double[a.Rows, a.Columns];
			for (int i = 0; i < a.Rows; i++)
				for (int j = 0; j < a.Columns; j++)
					z[i, j] = a[i, j] * x;
			return new Matrix(z);
		}

		public static Vector operator * (Matrix a, Vector b)
		{
			if (a.Columns != b.Length)
				throw new Exception("Illegal matrix multiplication");
			double[] z = new double[a.Rows];
			for (int i = 0; i < a.Rows; i++)
				for (int j = 0; j < a.Columns; j++)
					z[i] += a[i, j] * b[j];
			return new Vector(z);
		}

		public static Matrix operator * (Matrix a, Matrix b)
		{
			if (a.Columns != b.Rows)
				throw new Exception("Illegal matrix multiplication");
			double[,] z = new double[a.Rows, b.Columns];
			for (int i = 0; i < a.Rows; i++)
				for (int j = 0; j < b.Columns; j++)
					for (int k = 0; k < a.Columns; k++)
						z[i, j] += a[i, k] * b[k, j];
			return new Matrix(z);
		}

		public Matrix CutRows(int row1, int row2)
		{
			double[,] z = new double[2, Columns];
			for (int i = 0; i < Columns; i++) 
			{
				z[0, i] = matrix[row1, i];
				z[1, i] = matrix[row2, i];
			}
			return new Matrix(z);
		}

		public int Rows
		{
			get { return matrix.GetLength(0); }
		}

		public int Columns
		{
			get { return matrix.GetLength(1); }
		}

		public double this [int row, int col]
		{
			get { return matrix[row, col]; }
			set { matrix[row, col] = value; }
		}

		public object Clone()
		{ 
			return new Matrix((double[,])matrix.Clone());
		}

		public override bool Equals(object obj)
		{
			if (this.GetType() != obj.GetType()) return false;
			Matrix that = (Matrix)obj;
			if (this.Rows != that.Rows || 
				this.Columns != that.Columns) return false;
			for (int i = 0; i < this.Rows; i++)
				for (int j = 0; j < this.Columns; j++)
					if (this[i, j] != that[i, j]) return false;
			return true;
		}

		public override int GetHashCode()
		{
			return matrix.GetHashCode ();
		}

		public static bool operator == (Matrix a, Matrix b)
		{
			return a.Equals(b);
		}

		public static bool operator != (Matrix a, Matrix b)
		{
			return !a.Equals(b);
		}

#if DEBUG
		public override string ToString()
		{
			string str = "{";
			for (int i = 0; i < Rows; i++)
			{
				str += "{";
				for (int j = 0; j < Columns; j++)
				{
					str += matrix[i, j].ToString();
					if (j < Columns - 1) str += ", ";
				}
				str += "}";
				if (i < Rows - 1) str += ", ";
			}
			return str + "}";
		}
#endif
	}

/*#if DEBUG
	[TestFixture]public class MatrixTest
	{
		[Test]public void Clone()
		{
			Matrix m = new Matrix(new double[,] 
				{{1, 2, 3}, {4, 5, 6}});
			Matrix n = (Matrix)m.Clone();
			n[1, 0] = 9;
			Assert.AreEqual("{{1, 2, 3}, {4, 5, 6}}", m.ToString());
			Assert.AreEqual("{{1, 2, 3}, {9, 5, 6}}", n.ToString());
		}
		[Test]public void ScalarMul()
		{
			Matrix m = new Matrix(new double[,]
				{{1, 2}, {3, 4}});
			Matrix z = m * 2;
			Assert.AreEqual("{{2, 4}, {6, 8}}", z.ToString());
		}
		[Test]public void VectorMul()
		{
			Matrix m = new Matrix(new double[,] 
				{{1, 2, 3}, {4, 5, 6}});
			Vector v = new Vector(new double[] {1, 2, 3});
			Vector z = m * v;
			Assert.AreEqual("{14, 32}", z.ToString());
		}
		[Test]public void MatrixMul()
		{
			Matrix m = new Matrix(new double[,]
				{{1, 2, 3}, {4, 5, 6}});
			Matrix n = new Matrix(new double[,]
				{{1, 2}, {1, 2}, {0, 0}});
			Matrix z = m * n;
			Assert.AreEqual("{{3, 6}, {9, 18}}", z.ToString());
		}
		[Test]public void CutRows()
		{
			Matrix m = new Matrix(new double[,]
				{{1}, {2}, {3}, {4}});
			Matrix z = m.CutRows(1, 3);
			Assert.AreEqual("{{2}, {4}}", z.ToString());
		}
		[Test]public void Equals()
		{
			Matrix m = new Matrix(new double[,]
				{{1, 2}, {3, 4}});
			Assert.IsFalse(m.Equals("{{1, 2}, {3, 4}}"));
			Assert.IsFalse(m.Equals(new Matrix(
				new double[,] {{1}, {3}})));
			Assert.IsFalse(m.Equals(new Matrix(
				new double[,] {{1, 7}, {3, 4}})));
			Assert.IsTrue(m.Equals(new Matrix(
				new double[,] {{1, 2}, {3, 4}})));
		}
	}
#endif*/
}