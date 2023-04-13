using NUnit.Framework;
using System.Diagnostics;

using PolygonLibrary.Basics;
using PolygonLibrary.Segments;
using PolygonLibrary.Toolkit;
using PolygonLibrary.Polygons;
using PolygonLibrary.Polygons.ConvexPolygons;

namespace Tests
{
	[TestFixture]
	public partial class ConvexPolygonTests
	{
		#region Data
		GammaPair[] gps = new GammaPair[]
		{
			new GammaPair (new Vector2D(1,0), 1),
			new GammaPair (new Vector2D(0,1), 1),
			new GammaPair (new Vector2D(-1,0), 1),
			new GammaPair (new Vector2D(0,-1), 1)
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


		private void PrintCP(ConvexPolygon cp)
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
			foreach (GammaPair gp in cp.SF) {
				Debug.WriteLine(gp);
			}

			Debug.Unindent();
		}

		[Category("ConvexPolygonTests"), Test]
		public void CreateCPOfPointsTest1()
		{
			ConvexPolygon cp = new ConvexPolygon(ps1, false);
			PrintCP(cp);
			Assert.That(true);
		}

		[Category("ConvexPolygonTests"), Test]
		public void CreateCPOfPointsTest2()
		{
			ConvexPolygon cp = new ConvexPolygon(ps2, true);
			PrintCP(cp);
			Assert.That(true);
		}

		[Category("ConvexPolygonTests"), Test]
		public void CreateCPOfCFTest1()
		{
			SupportFunction sf = new SupportFunction(gps);
			ConvexPolygon cp = new ConvexPolygon(sf);
			PrintCP(cp);
			Assert.That(true);
		}

		[Category("ConvexPolygonTests"), Test]
		public void ContainsTest1()
		{
			ConvexPolygon cp = PolygonTools.Circle(10, 10, 2, 100);
			Point2D
				inside1 = new Point2D(11.5, 10.1),
				inside2 = new Point2D(10, 10),
				inside3 = new Point2D(8.9, 10),

				boundary1 = new Point2D(cp.Contour[0]),
				boundary2 = new Point2D(cp.Contour[1]),
				boundary3 = new Point2D(cp.Contour[^1]),
				boundary4 = new Point2D(10, 8),
				boundary5 = new Point2D(8, 10),
				boundary6 = (Point2D)((Vector2D)cp.Contour[0] + (Vector2D)cp.Contour[1]) / 2,
				boundary7 = (Point2D)((Vector2D)cp.Contour[0] + (Vector2D)cp.Contour[^1]) / 2,
				boundary8 = (Point2D)((Vector2D)cp.Contour[55] + (Vector2D)cp.Contour[56]) / 2,

				outside1 = new Point2D(12.1, 10),
				outside2 = cp.Contour[0] + 1.1 * (cp.Contour[1] - cp.Contour[0]),
				outside3 = cp.Contour[0] + 1.1 * (cp.Contour[^1] - cp.Contour[0]),
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

		[Category("ConvexPolygonTests"), Test]
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
			ConvexPolygon cp = new ConvexPolygon(vs, false);
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
				Assert.That(res[i], Is.EqualTo(cp.Contains(testPoints[i])),
					"ContainsTest2, test #" + i + " failed, point = " + testPoints[i]);
			}
		}

		[Category("ConvexPolygonTests"), Test]
		public void ContainsInsideTest1()
		{
			ConvexPolygon cp = PolygonTools.Circle(10, 10, 2, 100);
			Point2D
				inside1 = new Point2D(11.5, 10.1),
				inside2 = new Point2D(10, 10),
				inside3 = new Point2D(8.9, 10),

				boundary1 = new Point2D(cp.Contour[0]),
				boundary2 = new Point2D(cp.Contour[1]),
				boundary3 = new Point2D(cp.Contour[^1]),
				boundary4 = new Point2D(10, 8),
				boundary5 = new Point2D(8, 10),
				boundary6 = (Point2D)((Vector2D)cp.Contour[0] + (Vector2D)cp.Contour[1]) / 2,
				boundary7 = (Point2D)((Vector2D)cp.Contour[0] + (Vector2D)cp.Contour[^1]) / 2,
				boundary8 = (Point2D)((Vector2D)cp.Contour[55] + (Vector2D)cp.Contour[56]) / 2,

				outside1 = new Point2D(12.1, 10),
				outside2 = cp.Contour[0] + 1.1 * (cp.Contour[1] - cp.Contour[0]),
				outside3 = cp.Contour[0] + 1.1 * (cp.Contour[^1] - cp.Contour[0]),
				outside4 = new Point2D(7.9, 10),
				outside5 = new Point2D(6, 11);

			Assert.IsTrue(cp.ContainsInside(inside1), "inside1");
			Assert.IsTrue(cp.ContainsInside(inside2), "inside2");
			Assert.IsTrue(cp.ContainsInside(inside3), "inside3");

			Assert.IsFalse(cp.ContainsInside(boundary1), "boundary1");
			Assert.IsFalse(cp.ContainsInside(boundary2), "boundary2");
			Assert.IsFalse(cp.ContainsInside(boundary3), "boundary3");
			Assert.IsFalse(cp.ContainsInside(boundary4), "boundary4");
			Assert.IsFalse(cp.ContainsInside(boundary5), "boundary5");
			Assert.IsFalse(cp.ContainsInside(boundary6), "boundary6");
			Assert.IsFalse(cp.ContainsInside(boundary7), "boundary7");
			Assert.IsFalse(cp.ContainsInside(boundary8), "boundary8");

			Assert.IsFalse(cp.ContainsInside(outside1), "outside1");
			Assert.IsFalse(cp.ContainsInside(outside2), "outside2");
			Assert.IsFalse(cp.ContainsInside(outside3), "outside3");
			Assert.IsFalse(cp.ContainsInside(outside4), "outside4");
			Assert.IsFalse(cp.ContainsInside(outside5), "outside5");
		}

		[Category("ConvexPolygonTests"), Test]
		public void ContainsInsideTest2()
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
			ConvexPolygon cp = new ConvexPolygon(vs);
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

				new Point2D(-11.0/6, 0),  // ray passing through the edge before penultimate, inside
				new Point2D(-1.5, -1.5),  // ray passing through the edge before penultimate, vertex
				new Point2D(-1, 4)        // ray passing through the edge before penultimate, outside
			};

			bool[] res = new bool[]
			{
				false,
				false, false, false, false, false,
				false, false, false,
				false, false, false,
				true, false, false,
				true, false, false,
				true, false, false,
				true, false, false
			};

			for (int i = 0; i < testPoints.Count; i++) {
				Assert.That(res[i], Is.EqualTo(cp.ContainsInside(testPoints[i])),
					"ContainsTest2, test #" + i + " failed, point = " + testPoints[i]);
			}
		}

		
		
		[Category("ConvexPolygonTests"), Test]
		public void SumTest1()
		{
			ConvexPolygon
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

		[Category("ConvexPolygonTests"), Test]
		public void SumTest2()
		{
			ConvexPolygon
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

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest1()
		{
			ConvexPolygon
				cp1 = PolygonTools.RectangleTurned(0, -3, 0, 3, Math.PI / 4),
				cp2 = PolygonTools.RectangleParallel(-1, -1, 1, 1),
				cp3 = cp1 - cp2;

			List<Point2D> res = new List<Point2D> {
				new Point2D(1,0),
				new Point2D(0,1),
				new Point2D(-1,0),
				new Point2D(0,-1)
			};

			CyclicListComparison(cp3.Contour.Vertices, res, "Diff 1");
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest2()
		{
			Point2D[] vs2 = new Point2D[]
			{
				new Point2D(0, 0),
				new Point2D(-1, 0),
				new Point2D(0, -1)
			};

			ConvexPolygon
				cp1 = PolygonTools.RectangleTurned(0, -3, 0, 3, Math.PI / 4),
				cp2 = new ConvexPolygon(vs2, false),
				cp3 = cp1 - cp2;

			List<Point2D> res = new List<Point2D> {
				new Point2D(-2,0),
				new Point2D(0,-2),
				new Point2D(2.5,0.5),
				new Point2D(0.5,2.5)
			};

			CyclicListComparison(cp3.Contour.Vertices, res, "Diff 2");
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest3()
		{
			ConvexPolygon
				cp1 = PolygonTools.RectangleParallel(-1, -1, 1, 1),
				cp2 = PolygonTools.RectangleParallel(-1, 0, 1, 0),
				cp3 = cp1 - cp2;

			List<Point2D> res = new List<Point2D> {
				new Point2D(0,1),
				new Point2D(0,-1),
			};

			CyclicListComparison(cp3.Contour.Vertices, res, "Diff 3");			
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest4()
		{
			ConvexPolygon
				cp1 = PolygonTools.RectangleParallel(-1, 0, 1, 0),
				cp2 = PolygonTools.RectangleParallel(-3, 4, -1, 4),
				cp3 = cp1 - cp2;

			List<Point2D> res = new List<Point2D> {
				new Point2D(2,-4)
			};

			CyclicListComparison(cp3.Contour.Vertices, res, "Diff 4");
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest5()
		{
			ConvexPolygon
				cp1 = PolygonTools.RectangleTurned(-1, 0, 1, 0, Math.PI / 4),
				cp2 = PolygonTools.RectangleTurned(-3, 4, -1, 4, Math.PI / 4),
				cp3 = cp1 - cp2;

			List<Point2D> res = new List<Point2D> {
				new Point2D(2,-4)
			};

			CyclicListComparison(cp3.Contour.Vertices, res, "Diff 5");
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest6()
		{
			ConvexPolygon
				cp1 = PolygonTools.RectangleTurned(-1, 0, 1, 0, Math.PI / 4),
				cp2 = PolygonTools.RectangleTurned(-3.01, 4, -1, 4, Math.PI / 4),
				cp3 = cp1 - cp2;

			Assert.IsTrue(cp3 == null, "Diff 6: the difference is not empty");
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest7()
		{
			ConvexPolygon
				cp1 = PolygonTools.RectangleParallel(-1, 0, 1, 0),
				cp2 = PolygonTools.RectangleParallel(-3.01, 4, -1, 4),
				cp3 = cp1 - cp2;

			Assert.IsTrue(cp3 == null, "Diff 7: the difference is not empty");
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest8()
		{
			ConvexPolygon
				cp1 = PolygonTools.RectangleParallel(-1, 0, 1, 0),
				cp2 = PolygonTools.RectangleParallel(0, -1, 0, 1),
				cp3 = cp1 - cp2;

			Assert.IsTrue(cp3 == null, "Diff 8: the difference is not empty");
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest9()
		{
			ConvexPolygon
				cp1 = PolygonTools.RectangleParallel(-1, -1, 1, 1),
				cp2 = PolygonTools.Circle(0, 0, 1.01, 100),
				cp3 = cp1 - cp2;

			Assert.IsTrue(cp3 == null, "Diff 9: the difference is not empty");
		}

		[Category("ConvexPolygonTests"), Test]
		public void WeightTest1()
		{
			ConvexPolygon cp = new ConvexPolygon(ps1, true);
			double s = cp.Square;

			Assert.IsTrue(Tools.EQ(s, 4), "WeightTest1: wrong square");
		}

		[Category("ConvexPolygonTests"), Test]
		public void WeightTest2()
		{
			ConvexPolygon cp = new ConvexPolygon(
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

		[Category("ConvexPolygonTests"), Test]
		public void RandomPoint1()
		{
			ConvexPolygon cp = new ConvexPolygon(ps1, true);
			int N = 10, i;
			Point2D[] rndPoints = new Point2D[N];
			for (i = 0; i < N; i++) {
				rndPoints[i] = cp.GenerateRandomPoint();
			}

			for (i = 0; i < N; i++) {
				Assert.IsTrue(cp.Contains(rndPoints[i]), "RandomPoint1: a point is obtained that is outside the polygon");
			}
		}

		[Category("ConvexPolygonTests"), Test]
		public void RandomPoint2()
		{
			ConvexPolygon cp = new ConvexPolygon(
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
