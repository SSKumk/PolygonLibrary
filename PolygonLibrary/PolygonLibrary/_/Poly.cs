using System;

namespace Robust
{
	public class Poly
	{
		private SupportPoly sp;
		private PointPoly pp;
		
		public Poly()
		{
			sp = new SupportPoly();
		}

		public Poly(Point[] points)
		{
			sp = new SupportPoly(points);
		}

		public Poly(double[,] matrix, double[] limits)
		{
			sp = new SupportPoly(matrix, limits);
		}

		public Poly(double[,] matrix, double[][] box)
		{
			sp = new SupportPoly(matrix, box);
		}

		private Poly(SupportPoly poly)
		{
			sp = poly;
		}

		public static Poly Ball(double radius, int points)
		{
			Point[] pts = new Point[points];
			for (int i = 0; i < points; i++)
				pts[i] = new Point(
					radius * Math.Cos(i * 2*Math.PI/points),
					radius * Math.Sin(i * 2*Math.PI/points));
			return new Poly(pts);
		}

		public bool isEmpty
		{
			get { return sp != null ? sp.isEmpty : pp.isEmpty; }
		}

		public int nPoints
		{
			get { return sp != null ? sp.nPoints : pp.nPoints; }
		}

		public Point this [int i]
		{
			get { return sp != null ? sp[i] : pp[i]; }
		}

		private SupportPoly ToSupport
		{
			get { return sp != null ? sp : new SupportPoly(pp); }
		}
		
		public Poly Add(Poly poly)
		{
			return new Poly(ToSupport.Add(poly.ToSupport));
		}

		public Poly Sub(Poly poly)
		{
			return new Poly(ToSupport.Sub(poly.ToSupport));
		}

		public Poly Mul(double x)
		{
			return new Poly(ToSupport.Mul(x));
		}

		public Point[] TangentPoints(Point dir)
		{
			return ToSupport.TangentPoints(dir);
		}

		public double ContainsIndex(Point point)
		{
			return ToSupport.ContainsIndex(point);
		}

		public Point AimContainsVector(Point point, Poly add, out double index)
		{
			return ToSupport.AimContainsVector(point, add.ToSupport, out index);
		}

		public double KappaAimIndex(Point point, Poly add, double kappa)
		{
			return ToSupport.KappaAimIndex(point, add.ToSupport, kappa);
		}

		public Point NearestPoint(Point point)
		{
			return ToSupport.NearestPoint(point);
		}

		public void ToCompactForm()
		{
			if (sp == null) return;
			pp = new PointPoly(sp);
			sp = null;
		}

#if DEBUG
		public override string ToString()
		{
			return sp != null ? sp.ToString() : pp.ToString();
		}
#endif
	}
}
