using System;
//using NUnit.Framework;

namespace Robust
{
	public class SwitchLine
	{
		private Point[][] lines;
		private Point[] direction;
		
		public SwitchLine(Poly[] polies, double[,] EquivMatrix)
		{
			int nlines = EquivMatrix.GetLength(1);
			int zero = 0;
			while (zero < polies.Length && polies[zero].isEmpty)
				zero++;
			int npoints = polies.Length - zero;
			lines = new Point[nlines][];
			direction = new Point[nlines];
			for (int i = 0; i < nlines; i++)
			{
				lines[i] = new Point[npoints * 2];
				direction[i] = new Point(EquivMatrix[0, i], EquivMatrix[1, i]);
				for (int j = 0; j < npoints; j++)
				{
					Point[] tang = polies[j + zero].TangentPoints(direction[i]);
					lines[i][npoints - j - 1] = tang[0];
					lines[i][npoints + j] = tang[1];
				}
			}
		}

		public int nLines 
		{
			get { return lines.Length; }
		}

		public int nPoints(int line)
		{
			return lines[line].Length;
		}

		public Point this[int line, int point]
		{
			get { return lines[line][point]; }
		}

		private void CatchPoint(Point p, Point a, Point b, 
			Point dir, bool extend, ref int sign, ref int catched)
		{
			Point ba = b - a, pa = p - a;
			double d = ba.x * dir.y - ba.y * dir.x;
			double dt = pa.x * dir.y - pa.y * dir.x;
			double dd = ba.x * pa.y - ba.y * pa.x;
			if ((dt >= 0 && dt < d) || extend)
			{
				catched++;
				sign = -Math.Sign(dd) * Math.Sign(d);
			}
		}

		private int PointSign(Point point, Point[] line, 
			Point dir)
		{
			int sign = 0, catched = 0;
			for (int i = 0; i < line.Length - 1; i++)
				CatchPoint(point, line[i], line[i+1], dir, false,
					ref sign, ref catched);
			if (catched >= 2) return 0;
			if (catched == 0) 
				CatchPoint(point, line[0], line[line.Length-1], 
					dir, true, ref sign, ref catched);
			return sign;
		}
		
		public int[] PointSign(Point point)
		{
			int[] sign = new int[nLines];
			for (int line = 0; line < nLines; line++)
				sign[line] = PointSign(point, lines[line],
					direction[line]);
			return sign;
		}
	}

/*#if DEBUG
	[TestFixture]public class SwitchLineTest
	{
		[Test]public void EmptyPolies()
		{
            Poly[] polies = new Poly[2];
			polies[0] = polies[1] = new Poly();
			double[,] matrix = new double[,] {{0}, {1}};
			SwitchLine sl = new SwitchLine(polies, matrix);
			Assert.AreEqual(1, sl.nLines);
			Assert.AreEqual(0, sl.nPoints(0));
		}
		[Test]public void SwitchLineCalc()
		{
			Poly[] polies = new Poly[3];
			polies[0] = new Poly();
			Point[] points = new Point[128];
			for (int i = 0; i < points.Length; i++)
				points[i] = new Point(
					Math.Cos(i * 2 * Math.PI / points.Length),
					Math.Sin(i * 2 * Math.PI / points.Length));
			polies[1] = new Poly(points);
			for (int i = 0; i < points.Length; i++)
				points[i] = new Point(
					2 * Math.Cos(i * 2 * Math.PI / points.Length),
					2 * Math.Sin(i * 2 * Math.PI / points.Length));
			polies[2] = new Poly(points);
			double[,] matrix = new double[,] {{0}, {1}};
			SwitchLine sl = new SwitchLine(polies, matrix);
			Assert.AreEqual(1, sl.nLines);
			Assert.AreEqual(4, sl.nPoints(0));
			Assert.AreEqual("{-2, 0}", sl[0, 0].ToString());
			Assert.AreEqual("{-1, 0}", sl[0, 1].ToString());
			Assert.AreEqual("{1, 0}", sl[0, 2].ToString());
			Assert.AreEqual("{2, 0}", sl[0, 3].ToString());
		}
		[Test]public void PointSign()
		{
			Poly[] polies = new Poly[1];
			Point[] points = new Point[128];
			for (int i = 0; i < points.Length; i++)
				points[i] = new Point(
					Math.Cos(i * 2 * Math.PI / points.Length),
					Math.Sin(i * 2 * Math.PI / points.Length));
			polies[0] = new Poly(points);
			double[,] matrix = new double[,] {{0}, {1}};
			SwitchLine sl = new SwitchLine(polies, matrix);
			Assert.AreEqual(-1, sl.PointSign(new Point(0, 1))[0]);
			Assert.AreEqual(1, sl.PointSign(new Point(0, -1))[0]);
			Assert.AreEqual(0, sl.PointSign(new Point(0, 0))[0]);
			Assert.AreEqual(-1, sl.PointSign(new Point(8, 1))[0]);
		}
	}
#endif*/
}
