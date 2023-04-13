using System;
using System.Collections;
//using NUnit.Framework;

namespace Robust
{
	internal class SupportPoly
	{
		private const double FloatPrec = Constants.FloatPrec;
		private readonly double[] f;
		private readonly Point[] norm;
		private readonly bool empty;

		public SupportPoly()
		{
			empty = true;
		}

		public SupportPoly(Point[] points)
		{
			PointsToSupport(points, out f, out norm);
		}

		public SupportPoly(double[,] matrix, double[] limits)
		{
			Point[] pts = ConstrainsToPoints(matrix, limits);
			PointsToSupport(pts, out f, out norm);
		}

		public SupportPoly(double[,] matrix, double[][] box)
		{
			Point[] pts = new Point[box.Length];
			for (int i = 0; i < pts.Length; i++)
				pts[i] = new Point(matrix, box[i]);
			PointsToSupport(pts, out f, out norm);
		}

		public SupportPoly(PointPoly poly)
		{
			if (poly.isEmpty)
				empty = true;
			else
			{
				Point[] points = new Point[poly.nPoints];
				for (int i = 0; i < points.Length; i++)
					points[i] = poly[i];
				PointsToSupport(points, out f, out norm);
			}
		}

		private SupportPoly(bool empty, double[] f, Point[] norm)
		{
			this.empty = empty; this.f = f; this.norm = norm;
		}

		private void PointsToSupport(Point[] points, 
			out double[] f, out Point[] norm)
		{
			Point[] pts = ConvexHull.QuickHull(points);
			if (pts.Length == 1)
			{
				norm = new Point[] {
					new Point(1, 0), new Point(0, 1),
					new Point(-1, 0), new Point(0, -1)};
				f = new double[] {
					pts[0].x, pts[0].y, -pts[0].x, -pts[0].y};
			}
			else if (pts.Length == 2)
			{
				Point n0 = CalcNorm(pts[0], pts[1]);
				Point n1 = new Point(-n0.y, n0.x);
				Point n2 = new Point(-n0.x, -n0.y);
				Point n3 = new Point(n0.y, -n0.x);
				double f0 = Math.Max(CalcFunc(pts[0], n0), CalcFunc(pts[1], n0));
				double f1 = Math.Max(CalcFunc(pts[0], n1), CalcFunc(pts[1], n1));
				double f2 = Math.Max(CalcFunc(pts[0], n2), CalcFunc(pts[1], n2));
				double f3 = Math.Max(CalcFunc(pts[0], n3), CalcFunc(pts[1], n3));
				norm = new Point[] {n0, n1, n2, n3};
				f = new double[] {f0, f1, f2, f3};
			}
			else
			{
				int len = pts.Length;
				f = new double[len]; norm = new Point[len];
				for (int i = 0; i < len; i++)
				{
					int j = CInc(i, len);
					norm[j] = CalcNorm(pts[i], pts[j]);
					f[j] = CalcFunc(pts[i], norm[j]);
				}
			}
		}

		private Point[] ConstrainsToPoints(double[,] matrix,
			double[] limits)
		{
			int k = limits.Length, n = (int)Math.Pow(2, k);
			double[] v = new double[k];
			Point[] pts = new Point[n];
			for (int i = 0; i < n; i++)
			{
				for (int j = 0, p = 1; j < k; j++, p *= 2)
					if ((i & p) != 0) v[j] = limits[j];
					else v[j] = -limits[j];
				pts[i] = new Point(matrix, v);
			}
			return pts;
		}

		private int CInc(int x, int limit)
		{
			return x == limit-1 ? 0 : x+1;
		}

		private int CDec(int x, int limit)
		{
			return x == 0 ? limit-1 : x-1;
		}

		private Point RotateVector(Point point, Point axis)
		{
			return new Point(point.x * axis.x + point.y * axis.y,
				point.y * axis.x - point.x * axis.y);
		}

		private Point CalcNorm(Point a, Point b)
		{
			double nx = b.y - a.y, ny = a.x - b.x;
			double n = Math.Sqrt(nx * nx + ny * ny);
			return new Point(nx / n, ny / n);
		}

		private double CalcFunc(Point p, Point norm)
		{
			return p.x * norm.x + p.y * norm.y;
		}

		private Point CalcPoint(double f1, Point norm1,
			double f2, Point norm2)
		{
			double d = norm1.x * norm2.y - norm2.x * norm1.y;
			return new Point(
				(f1 * norm2.y - f2 * norm1.y) / d, 
				(f2 * norm1.x - f1 * norm2.x) / d);
		}

		public bool isEmpty
		{
			get { return empty; }
		}

		public int nPoints
		{
			get { return empty ? 0 : f.Length; }
		}

		public Point this [int i]
		{
			get 
			{
				if (empty) return new Point();
				int j = CDec(i, f.Length);
				return CalcPoint(f[i], norm[i], f[j], norm[j]);
			}
		}

		private void Gather(SupportPoly p, out double[] zf1, out double[] zf2, 
			out Point[] znorm, out bool[] from1, out bool[] from2)
		{
			int n = nPoints, m = p.nPoints;
			ArrayList rf1 = new ArrayList(n + m);
			ArrayList rf2 = new ArrayList(n + m);
			ArrayList rnorm = new ArrayList(n + m);
			ArrayList rfrom1 = new ArrayList(n + m);
			ArrayList rfrom2 = new ArrayList(n + m);
			Point z; int start = 0; 
			double max = double.NegativeInfinity;
			for (int k = 0; k < m; k++)
			{
				z = RotateVector(p.norm[k], norm[0]);
				if (z.y >= 0 && z.x > max) 
				{
					start = k; max = z.x;
				}
			}
			int i = 0, j = start, istep = 0, jstep = 0;
			do
			{
				z = RotateVector(p.norm[j], norm[i]);
				if (z.x >= 1 - FloatPrec) 
				{
					rf1.Add(f[i]); rf2.Add(p.f[j]); rnorm.Add(norm[i]);
					rfrom1.Add(true); rfrom2.Add(true);
					i = CInc(i, n); j = CInc(j, m);
					istep++; jstep++;
				}
				else if (z.y > 0)
				{
					rf1.Add(f[i]); rf2.Add(CalcFunc(p[j], norm[i])); 
					rnorm.Add(norm[i]); rfrom1.Add(true); rfrom2.Add(false);
					i = CInc(i, n); istep++;
				}
				else
				{
					rf1.Add(CalcFunc(this[i], p.norm[j])); rf2.Add(p.f[j]); 
					rnorm.Add(p.norm[j]); rfrom1.Add(false); rfrom2.Add(true);
					j = CInc(j, m); jstep++;
				}
			} 
			while (istep < n || jstep < m);
			int last = rnorm.Count - 1;
			if ((Point)rnorm[0] == (Point)rnorm[last]) 
			{ 
				rf1.RemoveAt(last); rf2.RemoveAt(last); rnorm.RemoveAt(last); 
				rfrom1.RemoveAt(last); rfrom2.RemoveAt(last);
			}
			zf1 = (double[])rf1.ToArray(typeof(double));
			zf2 = (double[])rf2.ToArray(typeof(double));
			znorm = (Point[])rnorm.ToArray(typeof(Point));
			from1 = (bool[])rfrom1.ToArray(typeof(bool));
			from2 = (bool[])rfrom2.ToArray(typeof(bool));
		}

		public SupportPoly Add(SupportPoly p)
		{
			if (p.empty) return this;
			if (empty) return p;
			double[] zf, zf2; Point[] znorm; bool[] from1, from2;
			Gather(p, out zf, out zf2, out znorm, out from1, out from2);
			for (int i = 0; i < zf.Length; i++)
				zf[i] += zf2[i];
			return new SupportPoly(false, zf, znorm);
		}

		public SupportPoly Sub(SupportPoly p)
		{
			if (empty || p.empty) return this;
			Queue susp = new Queue(nPoints + p.nPoints);
			double[] zf, zf2; Point[] znorm; bool[] from1, from2;
			Gather(p, out zf, out zf2, out znorm, out from1, out from2);
			for (int i = 0; i < zf.Length; i++)
			{
				zf[i] -= zf2[i]; 
				if (from2[i]) susp.Enqueue(i);
			}
			int count = zf.Length;
			BitArray drop = new BitArray(count, false);
			bool zempty = false; Point z;
			int cur, l, r, remain = count;
			while (susp.Count != 0 && !zempty)
			{
				cur = (int)susp.Dequeue();
				if (!drop[cur]) 
				{
					l = CDec(cur, count); r = CInc(cur, count);
					while (drop[l]) l = CDec(l, count);
					while (drop[r]) r = CInc(r, count);
					z = CalcPoint(zf[l], znorm[l], 
						zf[cur], znorm[cur]);
					if (CalcFunc(z, znorm[r]) > zf[r])
					{
						susp.Enqueue(l); susp.Enqueue(r); 
						remain--; drop[cur] = true;
						z = RotateVector(znorm[r], znorm[l]);
						if (z.y <= 0) zempty = true;
					}
				}
			}
			if (zempty) 
				return new SupportPoly(true, null, null);
			double[] df = new double[remain];
			Point[] dnorm = new Point[remain];
			for (int i = 0, j = 0; i < remain; i++, j++)
			{
				while (drop[j]) j++;
				df[i] = zf[j]; dnorm[i] = znorm[j];
			}
			return new SupportPoly(false, df, dnorm);
		}

		public SupportPoly Mul(double x)
		{
			return new SupportPoly(false, (Vector)f * x, norm);
		}

		public Point[] TangentPoints(Point dir)
		{
			Point[] tang = new Point[2];
			tang[0] = tang[1] = new Point();
			for (int i = 0; i < nPoints; i++)
			{
				int j = CInc(i, nPoints);
				if (norm[i].x * dir.x + norm[i].y * dir.y > 0 && 
					norm[j].x * dir.x + norm[j].y * dir.y <= 0) 
					tang[0] = this[j];
				if (norm[i].x * dir.x + norm[i].y * dir.y < 0 && 
					norm[j].x * dir.x + norm[j].y * dir.y >= 0) 
					tang[1] = this[j];
			}
			return tang;
		}

		public double ContainsIndex(Point point)
		{
			double dist = double.NegativeInfinity;
			for (int i = 0; i < nPoints; i++)
				dist = Math.Max(dist, CalcFunc(point, norm[i]) / f[i]);
			return dist;
		}

		public Point AimContainsVector(Point point, SupportPoly add, out double index)
		{
			double idx = ContainsIndex(point);
			Point v = Point.Empty;
			if (idx <= 1)
			{
				index = double.NegativeInfinity;
				for (int i = 0; i < nPoints; i++)
				{
					double val = CalcFunc(point, norm[i]) / f[i];
					if (val > index) { index = val; v = norm[i]; }
				}
			}
			else
			{
				double[] zf1, zf2; Point[] znorm; bool[] from1, from2;
				Gather(add, out zf1, out zf2, out znorm, out from1, out from2);
				index = double.NegativeInfinity;
				for (int i = 0; i < znorm.Length; i++)
				{
					double val = (CalcFunc(point, znorm[i]) - zf1[i]) / zf2[i];
					if (val > index) { index = val; v = znorm[i]; }
				}
				index++;
			}
			return v;
		}

		public double KappaAimIndex(Point point, SupportPoly add, double kappa)
		{
			double[] zf1, zf2; Point[] znorm; bool[] from1, from2;
			Gather(add, out zf1, out zf2, out znorm, out from1, out from2);
			double idx = double.NegativeInfinity;
			for (int i = 0; i < znorm.Length; i++)
			{
				double xl = CalcFunc(point, znorm[i]);
				double k = (xl - kappa) / zf1[i];
				if (k > 1) k = (xl - kappa - zf1[i]) / zf2[i] + 1;
				if (k < 0) k = 0;
				if (k > idx) idx = k;
			}
			return idx;
		}

		public Point NearestPoint(Point point)
		{
			double max = double.NegativeInfinity;
			int near = 0;
			for (int i = 0; i < nPoints; i++)
			{
				double d = CalcFunc(point, norm[i]) - f[i];
				if (d > max) { max = d; near = i; }
			}
			return point - norm[near] * max;
			/*double d = double.PositiveInfinity;
			Point nearest = this[0];
			for (int i = 0; i < nPoints; i++)
			{
				Point p = this[i];
				double zd = (p.x-point.x)*(p.x-point.x) + (p.y-point.y)*(p.y-point.y);
				if (zd < d) { d = zd; nearest = p; }
			}
			return nearest;*/
		}

#if DEBUG
		public override string ToString()
		{
			if (empty) return "empty";
			string str = "{";
			for (int i = 0; i < nPoints; i++)
			{
				str += this[i].ToString();
				if (i < nPoints - 1) str += ", ";
			}
			return str + "}";
		}
#endif
	}

/*#if DEBUG
	[TestFixture]public class SupportPolyTest
	{
		[Test]public void Ctors()
		{
			SupportPoly p = new SupportPoly();
			Assert.AreEqual("empty", p.ToString());
			p = new SupportPoly(new Point[] {new Point(1, 1)});
			Assert.AreEqual("{{1, 1}, {1, 1}, {1, 1}, {1, 1}}", p.ToString());
			p = new SupportPoly(new Point[] 
				{new Point(1, 1), new Point(-1, -1)});
			Assert.AreEqual("{{1, 1}, {-1, -1}, {-1, -1}, {1, 1}}", p.ToString());
			p = new SupportPoly(new Point[] 
				{new Point(1, 1), new Point(1, -1), 
				new Point(-1, 1), new Point(-1, -1)});
			Assert.AreEqual("{{1, -1}, {1, 1}, {-1, 1}, {-1, -1}}", p.ToString());
			p = new SupportPoly(new Point[] 
				{new Point(-2, -2), new Point(-2, -1), 
				new Point(2, -1)});
			Assert.AreEqual("{{-2, -2}, {2, -1}, {-2, -1}}", p.ToString());
			double[,] m = {{1, 0}, {0, 1}};
			double[] w = {1, 2};
			p = new SupportPoly(m, w);
			Assert.AreEqual("{{1, -2}, {1, 2}, {-1, 2}, {-1, -2}}", p.ToString());
		}
		[Test]public void Add()
		{
			SupportPoly p = new SupportPoly(new Point[] 
				{new Point(1, 1), new Point(1, -1), 
				new Point(-1, 1),	new Point(-1, -1)});
			SupportPoly q = new SupportPoly(new Point[] 
				{new Point(0, 4), new Point(-4, 0), 
				new Point(4, 0)});
			SupportPoly z = p.Add(q);
			Assert.AreEqual("{{5, -1}, {5, 1}, {1, 5}, {-1, 5}, {-5, 1}, {-5, -1}}", z.ToString());
		}
		[Test]public void Sub()
		{
			SupportPoly p = new SupportPoly(new Point[] 
				{new Point(4, 2), new Point(4, -2), 
				new Point(-4, 2), new Point(-4, -2), 
				new Point(0, 6), new Point(0, -6)});
			SupportPoly q = new SupportPoly(new Point[] 
				{new Point(0, 8), new Point(0, 0)});
			SupportPoly z = p.Sub(q);
			Assert.AreEqual("{{2, -4}, {0, -2}, {-2, -4}, {0, -6}}", z.ToString());
		}
		[Test]public void SubEmpty()
		{
			Point[] points = new Point[128];
			for (int i = 0; i < points.Length; i++)
				points[i] = new Point(
					Math.Cos(i * 2 * Math.PI / points.Length),
					Math.Sin(i * 2 * Math.PI / points.Length));
			SupportPoly p = new SupportPoly(points);
			SupportPoly q = new SupportPoly(new Point[]
				{new Point(0, 2), new Point(0, -2)});
			SupportPoly z = p.Sub(q);
			Assert.AreEqual("empty", z.ToString());
			p = new SupportPoly(new Point[] 
				{new Point(1, 1), new Point(1, -1), 
				new Point(-1, 1), new Point(-1, -1)});
			q = new SupportPoly(new Point[]
				{new Point(2, 0), new Point(-2, 0)});
			z = p.Sub(q);
			Assert.AreEqual("empty", z.ToString());
		}
		[Test]public void TangentPoints()
		{
			Point[] points = new Point[128];
			for (int i = 0; i < points.Length; i++)
				points[i] = new Point(
					Math.Cos(i * 2 * Math.PI / points.Length),
					Math.Sin(i * 2 * Math.PI / points.Length));
			SupportPoly p = new SupportPoly(points);
			Point[] tang = p.TangentPoints(new Point(0, 1));
			Assert.AreEqual("{-1, 0}", tang[0].ToString());
			Assert.AreEqual("{1, 0}", tang[1].ToString());
		}
	}
#endif*/
}
