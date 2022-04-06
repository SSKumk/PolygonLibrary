using System;

namespace Robust
{
	internal class PointPoly
	{
		private readonly float[] x, y;
		private readonly bool empty;

		public PointPoly(SupportPoly poly)
		{
			if (poly.isEmpty)
				empty = true;
			else
			{
				x = new float[poly.nPoints];
				y = new float[poly.nPoints];
				for (int i = 0; i < poly.nPoints; i++)
				{
					Point p = poly[i];
					x[i] = (float)p.x; 
					y[i] = (float)p.y;
				}
			}
		}

		public bool isEmpty
		{
			get { return empty; }
		}

		public int nPoints
		{
			get { return empty ? 0 : x.Length; }
		}

		public Point this [int i]
		{
			get { return new Point(x[i], y[i]); }
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
}
