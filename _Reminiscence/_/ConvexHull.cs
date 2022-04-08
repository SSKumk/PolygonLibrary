using System;
using System.Collections;
//using NUnit.Framework;

namespace Robust
{
	internal class ConvexHull
	{
		private static void QuickHull_AddPoints(Point[] pts, 
			ArrayList previndexer, ArrayList hull, int a, int b)
		{
			int index, max = -1;
			double cross, maxcross = 0;
			int len = (previndexer == null) ? pts.Length : previndexer.Count;
			if (previndexer == null) hull.Add(a);
			ArrayList indexer = new ArrayList(len);
			for (int i = 0; i < len; i++)
			{
				index = (previndexer == null) ? i : (int)previndexer[i];
				cross = (pts[b].y - pts[a].y) * (pts[index].x - pts[a].x) - 
					(pts[b].x - pts[a].x) * (pts[index].y - pts[a].y);
				if (cross > 0) indexer.Add(index);
				if (cross > maxcross) { maxcross = cross; max = index; }
			}
			if (max != -1)
			{
				QuickHull_AddPoints(pts, indexer, hull, a, max);
				hull.Add(max);
				QuickHull_AddPoints(pts, indexer, hull, max, b);
			}
		}

		public static Point[] QuickHull(Point[] pts)
		{
			int xmin = 0, xmax = 0, ymin = 0, ymax = 0;
			for (int i = 1; i < pts.Length; i++)
			{
				if (pts[i].x < pts[xmin].x || pts[i].x == pts[xmin].x 
					&& pts[i].y < pts[xmin].y) xmin = i;
				if (pts[i].x > pts[xmax].x || pts[i].x == pts[xmax].x
					&& pts[i].y > pts[xmax].y) xmax = i;
				if (pts[i].y < pts[ymin].y || pts[i].y == pts[ymin].y
					&& pts[i].x > pts[ymin].x) ymin = i;
				if (pts[i].y > pts[ymax].y || pts[i].y == pts[ymax].y
					&& pts[i].x < pts[ymax].x) ymax = i;
			}
			ArrayList hull = new ArrayList(pts.Length);
			if (ymax != xmax)
				QuickHull_AddPoints(pts, null, hull, xmax, ymax);
			if (xmin != ymax)
				QuickHull_AddPoints(pts, null, hull, ymax, xmin);
			if (ymin != xmin)
				QuickHull_AddPoints(pts, null, hull, xmin, ymin);
			if (xmax != ymin)
				QuickHull_AddPoints(pts, null, hull, ymin, xmax);
			if (hull.Count == 0) hull.Add(xmax);
			Point[] res = new Point[hull.Count];
			for (int i = 0; i < hull.Count; i++)
				res[i] = pts[(int)hull[i]];
			return res;
		}
	}

/*#if DEBUG
	[TestFixture]public class ConvexHullTest
	{
		[Test]public void QuickHullTest0()
		{
			Point[] pts = {new Point(1, 1), new Point(1, 1),
				new Point(1, 1)};
			Point[] hull = ConvexHull.QuickHull(pts);
			Assert.AreEqual(1, hull.Length);
			Assert.AreEqual("{1, 1}", hull[0].ToString());
		}
		[Test]public void QuickHullTest1()
		{
			Point[] pts = {new Point(1, 1), new Point(-1, -1),
				new Point(0, 0)};
			Point[] hull = ConvexHull.QuickHull(pts);
			Assert.AreEqual(2, hull.Length);
			Assert.AreEqual("{1, 1}", hull[0].ToString());
			Assert.AreEqual("{-1, -1}", hull[1].ToString());
		}
		[Test]public void QuickHullTest2()
		{
			Point[] pts = {new Point(1, 0), new Point(-1, 0),
				new Point(0, 1), new Point(0, -1),
				new Point(1, 1), new Point(-1, 1),
				new Point(1, -1), new Point(-1, -1)};
			Point[] hull = ConvexHull.QuickHull(pts);
			Assert.AreEqual(4, hull.Length);
			Assert.AreEqual("{1, 1}", hull[0].ToString());
			Assert.AreEqual("{-1, 1}", hull[1].ToString());
			Assert.AreEqual("{-1, -1}", hull[2].ToString());
			Assert.AreEqual("{1, -1}", hull[3].ToString());
		}
		[Test]public void QuickHullTest3()
		{
			Point[] pts = {new Point(4, 2), new Point(4, -2),
				new Point(-4, 2), new Point(-4, -2),
				new Point(0, 6), new Point(0, -6)};
			Point[] hull = ConvexHull.QuickHull(pts);
			Assert.AreEqual(6, hull.Length);
			Assert.AreEqual("{4, 2}", hull[0].ToString());
			Assert.AreEqual("{0, 6}", hull[1].ToString());
			Assert.AreEqual("{-4, 2}", hull[2].ToString());
			Assert.AreEqual("{-4, -2}", hull[3].ToString());
			Assert.AreEqual("{0, -6}", hull[4].ToString());
			Assert.AreEqual("{4, -2}", hull[5].ToString());
		}
	}
#endif*/
}
