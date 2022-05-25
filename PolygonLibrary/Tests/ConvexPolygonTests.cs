using System;
using System.Diagnostics;
using System.Collections.Generic;
using PolygonLibrary.Basics;
using PolygonLibrary.Segments;
using PolygonLibrary.Toolkit;
using PolygonLibrary.Polygons;

namespace Tests
{
	[TestClass]
	public class ConvexPolygonTests
	{
		#region Data
		PolygonLibrary.Polygons.ConvexPolygons.GammaPair[] gps = new PolygonLibrary.Polygons.ConvexPolygons.GammaPair[]
		{
			new PolygonLibrary.Polygons.ConvexPolygons.GammaPair (new Vector2D(1,0), 1),
			new PolygonLibrary.Polygons.ConvexPolygons.GammaPair (new Vector2D(0,1), 1),
			new PolygonLibrary.Polygons.ConvexPolygons.GammaPair (new Vector2D(-1,0), 1),
			new PolygonLibrary.Polygons.ConvexPolygons.GammaPair (new Vector2D(0,-1), 1)
		};

		Point2D[] ps1 = new Point2D[]
		{
			new Point2D(1,1),
			new Point2D(-1,1),
			new Point2D(-1,-1),
			new Point2D(1,-1)
		};

		Point2D[] ps2 = new Point2D[]
		{
			new Point2D(-1,-1),
			new Point2D(1,1),
			new Point2D(-1,1),
			new Point2D(1,-1)
		};
		#endregion

		private void PrintCP(PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon cp)
		{
			Debug.IndentSize = 2;

			Debug.WriteLine("Number of contours: " + cp.Contours.Count + ", Contour vertices: ");
			foreach (Point2D p in cp.Contour.Vertices) {
				Debug.WriteLine("  (" + p.x + ";" + p.y + ")");
			}

			Debug.WriteLine("Vertices: ");
			foreach (Point2D p in cp.Vertices) {
				Debug.WriteLine("  (" + p.x + ";" + p.y + ")");
			}

			Debug.WriteLine("Edges: ");
			Debug.Indent();
			foreach (Segment e in cp.Edges) {
				Debug.WriteLine(e);
			}

			Debug.Unindent();

			Debug.WriteLine("Support function: ");
			Debug.Indent();
			foreach (PolygonLibrary.Polygons.ConvexPolygons.GammaPair gp in cp.SF) {
				Debug.WriteLine(gp);
			}

			Debug.Unindent();
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void CreateCPOfPointsTest1()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon cp = new PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon(ps1);
			PrintCP(cp);
			Assert.IsTrue(true);
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void CreateCPOfPointsTest2()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon cp = new PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon(ps2, true);
			PrintCP(cp);
			Assert.IsTrue(true);
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void CreateCPOfCFTest1()
		{
			PolygonLibrary.Polygons.ConvexPolygons.SupportFunction sf = new PolygonLibrary.Polygons.ConvexPolygons.SupportFunction(gps);
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon cp = new PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon(sf);
			PrintCP(cp);
			Assert.IsTrue(true);
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void ContainsTest1()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon cp = PolygonTools.Circle(10, 10, 2, 100);
			Point2D
				inside1 = new Point2D(11.5, 10.1),
				inside2 = new Point2D(10, 10),
				inside3 = new Point2D(8.9, 10),

				boundary1 = new Point2D(cp.Contour[0]),
				boundary2 = new Point2D(cp.Contour[1]),
				boundary3 = new Point2D(cp.Contour[cp.Contour.Count - 1]),
				boundary4 = new Point2D(10, 8),
				boundary5 = new Point2D(8, 10),
				boundary6 = (Point2D)((Vector2D)cp.Contour[0] + (Vector2D)cp.Contour[1]) / 2,
				boundary7 = (Point2D)((Vector2D)cp.Contour[0] + (Vector2D)cp.Contour[cp.Contour.Count - 1]) / 2,
				boundary8 = (Point2D)((Vector2D)cp.Contour[55] + (Vector2D)cp.Contour[56]) / 2,

				outside1 = new Point2D(12.1, 10),
				outside2 = cp.Contour[0] + 1.1 * (cp.Contour[1] - cp.Contour[0]),
				outside3 = cp.Contour[0] + 1.1 * (cp.Contour[cp.Contour.Count - 1] - cp.Contour[0]),
				outside4 = new Point2D(7.9, 10),
				outside5 = new Point2D(6, 11);

			Assert.IsTrue(cp.Contains(inside1), "inside1");
			Assert.IsTrue(cp.Contains(inside2), "inside2");
			Assert.IsTrue(cp.Contains(inside3), "inside3");

			Assert.IsTrue(cp.Contains(boundary1), "boundary1");
			Assert.IsTrue(cp.Contains(boundary2), "boundary2");
			Assert.IsTrue(cp.Contains(boundary3), "boundary3");
			Assert.IsTrue(cp.Contains(boundary4), "boundary4");
			Assert.IsTrue(cp.Contains(boundary5), "boundary5");
			Assert.IsTrue(cp.Contains(boundary6), "boundary6");
			Assert.IsTrue(cp.Contains(boundary7), "boundary7");
			Assert.IsTrue(cp.Contains(boundary8), "boundary8");

			Assert.IsFalse(cp.Contains(outside1), "outside1");
			Assert.IsFalse(cp.Contains(outside2), "outside2");
			Assert.IsFalse(cp.Contains(outside3), "outside3");
			Assert.IsFalse(cp.Contains(outside4), "outside4");
			Assert.IsFalse(cp.Contains(outside5), "outside5");
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void ContainsTest2()
		{
			List<Point2D> vs = new List<Point2D>()
			{
				new Point2D (-2, -1),
				new Point2D (-1, -2),
				new Point2D ( 1, -2),
				new Point2D ( 2, -1),
				new Point2D ( 2,  1),
				new Point2D ( 1,  2),
				new Point2D (-1,  2),
				new Point2D (-2,  1)
			};
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon cp = new PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon(vs);
			List<Point2D> testPoints = new List<Point2D>()
			{
				new Point2D(-2, -1),  // 0th vertex

				new Point2D(-1, -3),  // outside the polygon cone
				new Point2D(-2, -3),  // outside the polygon cone
				new Point2D(-3, -3),  // outside the polygon cone
				new Point2D(-3,  0),  // outside the polygon cone
				new Point2D(-3,  1),  // outside the polygon cone

				new Point2D(-1.5, -1.5),  // right boundary of the polygon cone, boundary of thr polygon
				new Point2D(-1, -2),      // right boundary of the polygon cone, vertex
				new Point2D(0,  -3),      // right boundary of the polygon cone, outside

				new Point2D(-2,  0),  // left boundary of the polygon cone, boundary of thr polygon
				new Point2D(-2,  1),  // left boundary of the polygon cone, vertex
				new Point2D(-2,  3),  // left boundary of the polygon cone, outside

				new Point2D(-1, -1.5),  // ray passing through 2nd edge, inside
				new Point2D( 0, -2),    // ray passing through 2nd edge, boundary
				new Point2D( 2, -3),    // ray passing through 2nd edge, outside

				new Point2D( 0, -0.5),  // ray passing through some middle edge, inside
				new Point2D( 2,  0),    // ray passing through some middle edge, boundary
				new Point2D( 6,  1),    // ray passing through some middle edge, inside

				new Point2D( 0,  1),  // ray passing through some vertex, inside
				new Point2D( 1,  2),  // ray passing through some vertex, boundary - vertex
				new Point2D( 3,  4),  // ray passing through some vertex, outside

				new Point2D(-11.0/6, 0),  // ray passing through the edge before prenultimate, inside
				new Point2D(-1.5, -1.5),  // ray passing through the edge before prenultimate, vertex
				new Point2D(-1, 4)        // ray passing through the edge before prenultimate, outside
			};

			bool[] res = new bool[]
			{
				true,
				false, false, false, false, false,
				true, true, false,
				true, true, false,
				true, true, false,
				true, true, false,
				true, true, false,
				true, true, false
			};

			for (int i = 0; i < testPoints.Count; i++) {
				Assert.AreEqual(cp.Contains(testPoints[i]), res[i],
					"ContainsTest2, test #" + i + " failed, point = " + testPoints[i]);
			}
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void SumTest1()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon
				cp1 = PolygonTools.RectangleTurned(0, -1, 0, 1, Math.PI / 4),
				cp2 = PolygonTools.RectangleParallel(-1, -1, 1, 1),
				cp3 = cp1 + cp2;
			Point2D[] res = new Point2D[]
			{
				new Point2D(2,1),
				new Point2D(1,2),
				new Point2D(-1,2),
				new Point2D(-2,1),
				new Point2D(-2,-1),
				new Point2D(-1,-2),
				new Point2D(1,-2),
				new Point2D(2,-1)
			};

			Assert.IsTrue(cp3.Contour.Count == res.Length, "Sum 1: wrong number of vertices");
			foreach (Point2D p in res) {
				Assert.IsTrue(cp3.Contour.Vertices.Contains(p), "Sum 1: vertex " + p + " is not in the resultant polygon");
			}
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void SumTest2()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon
				cp = PolygonTools.RectangleParallel(-1, -1, 1, 1),
				cp3 = cp + cp;
			Point2D[] res = new Point2D[]
			{
				new Point2D(2,2),
				new Point2D(-2,2),
				new Point2D(-2,-2),
				new Point2D(2,-2)
			};

			Assert.IsTrue(cp3.Contour.Count == res.Length, "Sum 2: wrong number of vertices");
			foreach (Point2D p in res) {
				Assert.IsTrue(cp3.Contour.Vertices.Contains(p), "Sum 2: vertex " + p + " is not in the resultant polygon");
			}
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void DiffTest1()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon
				cp1 = PolygonTools.RectangleTurned(0, -3, 0, 3, Math.PI / 4),
				cp2 = PolygonTools.RectangleParallel(-1, -1, 1, 1),
				cp3 = cp1 - cp2;

			Point2D[] res = new Point2D[]
			{
				new Point2D(1,0),
				new Point2D(0,1),
				new Point2D(-1,0),
				new Point2D(0,-1)
			};

			Assert.IsTrue(cp3.Contour.Count == res.Length, "Diff 1: wrong number of vertices");
			foreach (Point2D p in res) {
				Assert.IsTrue(cp3.Contour.Vertices.Contains(p), "Diff 1: vertex " + p + " is not in the resultant polygon");
			}
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void DiffTest2()
		{
			Point2D[] vs2 = new Point2D[]
			{
				new Point2D(0, 0),
				new Point2D(-1, 0),
				new Point2D(0, -1)
			};

			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon
				cp1 = PolygonTools.RectangleTurned(0, -3, 0, 3, Math.PI / 4),
				cp2 = new PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon(vs2),
				cp3 = cp1 - cp2;

			Point2D[] res = new Point2D[]
			{
				new Point2D(-2,0),
				new Point2D(0,-2),
				new Point2D(2.5,0.5),
				new Point2D(0.5,2.5)
			};

			Assert.IsTrue(cp3.Contour.Count == res.Length, "Diff 2: wrong number of vertices");
			foreach (Point2D p in res) {
				Assert.IsTrue(cp3.Contour.Vertices.Contains(p), "Diff 2: vertex " + p + " is not in the resultant polygon");
			}
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void DiffTest3()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon
				cp1 = PolygonTools.RectangleParallel(-1, -1, 1, 1),
				cp2 = PolygonTools.RectangleParallel(-1, 0, 1, 0),
				cp3 = cp1 - cp2;

			Point2D[] res = new Point2D[]
			{
				new Point2D(0,1),
				new Point2D(0,-1)
			};

			Assert.IsTrue(cp3.Contour.Count == res.Length, "Diff 3: wrong number of vertices");
			foreach (Point2D p in res) {
				Assert.IsTrue(cp3.Contour.Vertices.Contains(p), "Diff 3: vertex " + p + " is not in the resultant polygon");
			}
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void DiffTest4()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon
				cp1 = PolygonTools.RectangleParallel(-1, 0, 1, 0),
				cp2 = PolygonTools.RectangleParallel(-3, 4, -1, 4),
				cp3 = cp1 - cp2;

			Point2D[] res = new Point2D[]
			{
				new Point2D(2,-4)
			};

			Assert.IsTrue(cp3.Contour.Count == res.Length, "Diff 4: wrong number of vertices");
			foreach (Point2D p in res) {
				Assert.IsTrue(cp3.Contour.Vertices.Contains(p), "Diff 4: vertex " + p + " is not in the resultant polygon");
			}
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void DiffTest5()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon
				cp1 = PolygonTools.RectangleTurned(-1, 0, 1, 0, Math.PI / 4),
				cp2 = PolygonTools.RectangleTurned(-3, 4, -1, 4, Math.PI / 4),
				cp3 = cp1 - cp2;

			Point2D[] res = new Point2D[]
			{
				new Point2D(2,-4)
			};

			Assert.IsTrue(cp3.Contour.Count == res.Length, "Diff 5: wrong number of vertices");
			foreach (Point2D p in res) {
				Assert.IsTrue(cp3.Contour.Vertices.Contains(p), "Diff 5: vertex " + p + " is not in the resultant polygon");
			}
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void DiffTest6()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon
				cp1 = PolygonTools.RectangleTurned(-1, 0, 1, 0, Math.PI / 4),
				cp2 = PolygonTools.RectangleTurned(-3.01, 4, -1, 4, Math.PI / 4),
				cp3 = cp1 - cp2;

			Assert.IsTrue(cp3 == null, "Diff 6: the difference is not empty");
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void DiffTest7()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon
				cp1 = PolygonTools.RectangleParallel(-1, 0, 1, 0),
				cp2 = PolygonTools.RectangleParallel(-3.01, 4, -1, 4),
				cp3 = cp1 - cp2;

			Assert.IsTrue(cp3 == null, "Diff 7: the difference is not empty");
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void DiffTest8()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon
				cp1 = PolygonTools.RectangleParallel(-1, 0, 1, 0),
				cp2 = PolygonTools.RectangleParallel(0, -1, 0, 1),
				cp3 = cp1 - cp2;

			Assert.IsTrue(cp3 == null, "Diff 8: the difference is not empty");
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void DiffTest9()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon
				cp1 = PolygonTools.RectangleParallel(-1, -1, 1, 1),
				cp2 = PolygonTools.Circle(0, 0, 1.01, 100),
				cp3 = cp1 - cp2;

			Assert.IsTrue(cp3 == null, "Diff 9: the difference is not empty");
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void WeightTest1()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon cp = new PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon(ps1, true);
			double s = cp.Square;

			Assert.IsTrue(Tools.EQ(s, 4), "WeightTest1: wrong square");
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void WeightTest2()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon cp = new PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon(
				new Point2D[]
				{
					new Point2D(-2, -1),
					new Point2D(-1, -2),
					new Point2D( 1, -2),
					new Point2D( 2, -1),
					new Point2D( 2,  1),
					new Point2D( 1,  2),
					new Point2D(-1,  2),
					new Point2D(-2,  1)
				}, true);
			double s = cp.Square;

			Assert.IsTrue(Tools.EQ(s, 14), "WeightTest1: wrong square");
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void RandomPoint1()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon cp = new PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon(ps1, true);
			int N = 10, i;
			Point2D[] rndPoints = new Point2D[N];
			for (i = 0; i < N; i++) {
				rndPoints[i] = cp.GenerateRandomPoint();
			}

			for (i = 0; i < N; i++) {
				Assert.IsTrue(cp.Contains(rndPoints[i]), "RandomPoint1: a point is obtained that is outside the polygon");
			}
		}

		[TestCategory("ConvexPolygonTests"), TestMethod()]
		public void RandomPoint2()
		{
			PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon cp = new PolygonLibrary.Polygons.ConvexPolygons.ConvexPolygon(
				new Point2D[]
				{
					new Point2D(-2, -1),
					new Point2D(-1, -2),
					new Point2D( 1, -2),
					new Point2D( 2, -1),
					new Point2D( 2,  1),
					new Point2D( 1,  2),
					new Point2D(-1,  2),
					new Point2D(-2,  1)
				}, true);
			int N = 100000, i;
			Point2D[] rndPoints = new Point2D[N];
			for (i = 0; i < N; i++) {
				rndPoints[i] = cp.GenerateRandomPoint();
			}

			Point2D[]
				cornerPoints = Array.FindAll(rndPoints, p => Math.Abs(p.x) > 1 && Math.Abs(p.y) > 1),
				_01Points = Array.FindAll(rndPoints, p => 0 < p.x && p.x < 1 && 0 < p.y && p.y < 1);

			double cornPointsFreq = cornerPoints.Length * 7, _01PointsFreq = _01Points.Length * 14;

			for (i = 0; i < N; i++) {
				Assert.IsTrue(cp.Contains(rndPoints[i]), "RandomPoint2: a point is obtained that is outside the polygon");
			}
		}
	}
}
