using NUnit.Framework;
using System.Diagnostics;

using CGLibrary;
using G = CGLibrary.Geometry<double, DConvertor>;
namespace Tests
{
	[TestFixture]
	public partial class ConvexPolygonTests
	{
		#region Data
		readonly G.GammaPair[] gps = new G.GammaPair[]
		{
			new G.GammaPair (new G.Vector2D(1,0), 1),
			new G.GammaPair (new G.Vector2D(0,1), 1),
			new G.GammaPair (new G.Vector2D(-1,0), 1),
			new G.GammaPair (new G.Vector2D(0,-1), 1)
		};

		readonly G.Point2D[] ps1 = new G.Point2D[]
		{
			new G.Point2D(1,1),
			new G.Point2D(-1,1),
			new G.Point2D(-1,-1),
			new G.Point2D(1,-1)
		};

		readonly G.Point2D[] ps2 = new G.Point2D[]
		{
			new G.Point2D(-1,-1),
			new G.Point2D(1,1),
			new G.Point2D(-1,1),
			new G.Point2D(1,-1)
		};
		#endregion


		private void PrintCP(G.ConvexPolygon cp)
		{
			Debug.IndentSize = 2;

			Debug.WriteLine("Number of contours: " + cp.Contours.Count + ", Contour vertices: ");
			foreach (G.Point2D p in cp.Contour.Vertices) {
				Debug.WriteLine("  (" + p.x + ";" + p.y + ")");
			}

			Debug.WriteLine("Vertices: ");
			foreach (G.Point2D p in cp.Vertices) {
				Debug.WriteLine("  (" + p.x + ";" + p.y + ")");
			}

			Debug.WriteLine("Edges: ");
			Debug.Indent();
			foreach (G.Segment e in cp.Edges) {
				Debug.WriteLine(e);
			}

			Debug.Unindent();

			Debug.WriteLine("Support function: ");
			Debug.Indent();
			Debug.Assert(cp.SF != null, "cp.SF != null");
			foreach (G.GammaPair gp in cp.SF) {
				Debug.WriteLine(gp);
			}

			Debug.Unindent();
		}

		[Category("ConvexPolygonTests"), Test]
		public void CreateCPOfPointsTest1()
		{
			G.ConvexPolygon cp = new G.ConvexPolygon(ps1, false);
			PrintCP(cp);
			Assert.That(true);
		}

		[Category("ConvexPolygonTests"), Test]
		public void CreateCPOfPointsTest2()
		{
			G.ConvexPolygon cp = new G.ConvexPolygon(ps2, true);
			PrintCP(cp);
			Assert.That(true);
		}

		[Category("ConvexPolygonTests"), Test]
		public void CreateCPOfCFTest1()
		{
			G.SupportFunction sf = new G.SupportFunction(gps);
			G.ConvexPolygon cp = new G.ConvexPolygon(sf);
			PrintCP(cp);
			Assert.That(true);
		}

		[Category("ConvexPolygonTests"), Test]
		public void ContainsTest1()
		{
			G.ConvexPolygon cp = G.PolygonTools.Circle(10, 10, 2, 100);
			G.Point2D
				inside1 = new G.Point2D(11.5, 10.1),
				inside2 = new G.Point2D(10, 10),
				inside3 = new G.Point2D(8.9, 10),

				boundary1 = new G.Point2D(cp.Contour[0]),
				boundary2 = new G.Point2D(cp.Contour[1]),
				boundary3 = new G.Point2D(cp.Contour[^1]),
				boundary4 = new G.Point2D(10, 8),
				boundary5 = new G.Point2D(8, 10),
				boundary6 = (G.Point2D)((G.Vector2D)cp.Contour[0] + (G.Vector2D)cp.Contour[1]) / 2,
				boundary7 = (G.Point2D)((G.Vector2D)cp.Contour[0] + (G.Vector2D)cp.Contour[^1]) / 2,
				boundary8 = (G.Point2D)((G.Vector2D)cp.Contour[55] + (G.Vector2D)cp.Contour[56]) / 2,

				outside1 = new G.Point2D(12.1, 10),
				outside2 = cp.Contour[0] + 1.1 * (cp.Contour[1] - cp.Contour[0]),
				outside3 = cp.Contour[0] + 1.1 * (cp.Contour[^1] - cp.Contour[0]),
				outside4 = new G.Point2D(7.9, 10),
				outside5 = new G.Point2D(6, 11);

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
			List<G.Point2D> vs = new List<G.Point2D>()
			{
				new G.Point2D (-2, -1),
				new G.Point2D (-1, -2),
				new G.Point2D ( 1, -2),
				new G.Point2D ( 2, -1),
				new G.Point2D ( 2,  1),
				new G.Point2D ( 1,  2),
				new G.Point2D (-1,  2),
				new G.Point2D (-2,  1)
			};
			G.ConvexPolygon cp = new G.ConvexPolygon(vs, false);
			List<G.Point2D> testPoints = new List<G.Point2D>()
			{
				new G.Point2D(-2, -1),  // 0th vertex

				new G.Point2D(-1, -3),  // outside the polygon cone
				new G.Point2D(-2, -3),  // outside the polygon cone
				new G.Point2D(-3, -3),  // outside the polygon cone
				new G.Point2D(-3,  0),  // outside the polygon cone
				new G.Point2D(-3,  1),  // outside the polygon cone

				new G.Point2D(-1.5, -1.5),  // right boundary of the polygon cone, boundary of thr polygon
				new G.Point2D(-1, -2),      // right boundary of the polygon cone, vertex
				new G.Point2D(0,  -3),      // right boundary of the polygon cone, outside

				new G.Point2D(-2,  0),  // left boundary of the polygon cone, boundary of thr polygon
				new G.Point2D(-2,  1),  // left boundary of the polygon cone, vertex
				new G.Point2D(-2,  3),  // left boundary of the polygon cone, outside

				new G.Point2D(-1, -1.5),  // ray passing through 2nd edge, inside
				new G.Point2D( 0, -2),    // ray passing through 2nd edge, boundary
				new G.Point2D( 2, -3),    // ray passing through 2nd edge, outside

				new G.Point2D( 0, -0.5),  // ray passing through some middle edge, inside
				new G.Point2D( 2,  0),    // ray passing through some middle edge, boundary
				new G.Point2D( 6,  1),    // ray passing through some middle edge, inside

				new G.Point2D( 0,  1),  // ray passing through some vertex, inside
				new G.Point2D( 1,  2),  // ray passing through some vertex, boundary - vertex
				new G.Point2D( 3,  4),  // ray passing through some vertex, outside

				new G.Point2D(-11.0/6, 0),  // ray passing through the edge before prenultimate, inside
				new G.Point2D(-1.5, -1.5),  // ray passing through the edge before prenultimate, vertex
				new G.Point2D(-1, 4)        // ray passing through the edge before prenultimate, outside
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
			G.ConvexPolygon cp = G.PolygonTools.Circle(10, 10, 2, 100);
			G.Point2D
				inside1 = new G.Point2D(11.5, 10.1),
				inside2 = new G.Point2D(10, 10),
				inside3 = new G.Point2D(8.9, 10),

				boundary1 = new G.Point2D(cp.Contour[0]),
				boundary2 = new G.Point2D(cp.Contour[1]),
				boundary3 = new G.Point2D(cp.Contour[^1]),
				boundary4 = new G.Point2D(10, 8),
				boundary5 = new G.Point2D(8, 10),
				boundary6 = (G.Point2D)((G.Vector2D)cp.Contour[0] + (G.Vector2D)cp.Contour[1]) / 2,
				boundary7 = (G.Point2D)((G.Vector2D)cp.Contour[0] + (G.Vector2D)cp.Contour[^1]) / 2,
				boundary8 = (G.Point2D)((G.Vector2D)cp.Contour[55] + (G.Vector2D)cp.Contour[56]) / 2,

				outside1 = new G.Point2D(12.1, 10),
				outside2 = cp.Contour[0] + 1.1 * (cp.Contour[1] - cp.Contour[0]),
				outside3 = cp.Contour[0] + 1.1 * (cp.Contour[^1] - cp.Contour[0]),
				outside4 = new G.Point2D(7.9, 10),
				outside5 = new G.Point2D(6, 11);

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
			List<G.Point2D> vs = new List<G.Point2D>()
			{
				new G.Point2D (-2, -1),
				new G.Point2D (-1, -2),
				new G.Point2D ( 1, -2),
				new G.Point2D ( 2, -1),
				new G.Point2D ( 2,  1),
				new G.Point2D ( 1,  2),
				new G.Point2D (-1,  2),
				new G.Point2D (-2,  1)
			};
			G.ConvexPolygon cp = new G.ConvexPolygon(vs);
			List<G.Point2D> testPoints = new List<G.Point2D>()
			{
				new G.Point2D(-2, -1),  // 0th vertex

				new G.Point2D(-1, -3),  // outside the polygon cone
				new G.Point2D(-2, -3),  // outside the polygon cone
				new G.Point2D(-3, -3),  // outside the polygon cone
				new G.Point2D(-3,  0),  // outside the polygon cone
				new G.Point2D(-3,  1),  // outside the polygon cone

				new G.Point2D(-1.5, -1.5),  // right boundary of the polygon cone, boundary of thr polygon
				new G.Point2D(-1, -2),      // right boundary of the polygon cone, vertex
				new G.Point2D(0,  -3),      // right boundary of the polygon cone, outside

				new G.Point2D(-2,  0),  // left boundary of the polygon cone, boundary of thr polygon
				new G.Point2D(-2,  1),  // left boundary of the polygon cone, vertex
				new G.Point2D(-2,  3),  // left boundary of the polygon cone, outside

				new G.Point2D(-1, -1.5),  // ray passing through 2nd edge, inside
				new G.Point2D( 0, -2),    // ray passing through 2nd edge, boundary
				new G.Point2D( 2, -3),    // ray passing through 2nd edge, outside

				new G.Point2D( 0, -0.5),  // ray passing through some middle edge, inside
				new G.Point2D( 2,  0),    // ray passing through some middle edge, boundary
				new G.Point2D( 6,  1),    // ray passing through some middle edge, inside

				new G.Point2D( 0,  1),  // ray passing through some vertex, inside
				new G.Point2D( 1,  2),  // ray passing through some vertex, boundary - vertex
				new G.Point2D( 3,  4),  // ray passing through some vertex, outside

				new G.Point2D(-11.0/6, 0),  // ray passing through the edge before penultimate, inside
				new G.Point2D(-1.5, -1.5),  // ray passing through the edge before penultimate, vertex
				new G.Point2D(-1, 4)        // ray passing through the edge before penultimate, outside
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
			G.ConvexPolygon
				cp1 = G.PolygonTools.RectangleTurned(0, -1, 0, 1, Math.PI / 4),
				cp2 = G.PolygonTools.RectangleParallel(-1, -1, 1, 1),
				cp3 = cp1 + cp2;
			G.Point2D[] res = new G.Point2D[]
			{
				new G.Point2D(2,1),
				new G.Point2D(1,2),
				new G.Point2D(-1,2),
				new G.Point2D(-2,1),
				new G.Point2D(-2,-1),
				new G.Point2D(-1,-2),
				new G.Point2D(1,-2),
				new G.Point2D(2,-1)
			};

			Assert.IsTrue(cp3.Contour.Count == res.Length, "Sum 1: wrong number of vertices");
			foreach (G.Point2D p in res) {
				Assert.IsTrue(cp3.Contour.Vertices.Contains(p), "Sum 1: vertex " + p + " is not in the resultant polygon");
			}
		}

		[Category("ConvexPolygonTests"), Test]
		public void SumTest2()
		{
			G.ConvexPolygon
				cp = G.PolygonTools.RectangleParallel(-1, -1, 1, 1),
				cp3 = cp + cp;
			G.Point2D[] res = new G.Point2D[]
			{
				new G.Point2D(2,2),
				new G.Point2D(-2,2),
				new G.Point2D(-2,-2),
				new G.Point2D(2,-2)
			};

			Assert.IsTrue(cp3.Contour.Count == res.Length, "Sum 2: wrong number of vertices");
			foreach (G.Point2D p in res) {
				Assert.IsTrue(cp3.Contour.Vertices.Contains(p), "Sum 2: vertex " + p + " is not in the resultant polygon");
			}
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest1()
		{
			G.ConvexPolygon?
				cp1 = G.PolygonTools.RectangleTurned(0, -3, 0, 3, Math.PI / 4),
				cp2 = G.PolygonTools.RectangleParallel(-1, -1, 1, 1),
				cp3 = cp1 - cp2;

			List<G.Point2D> res = new List<G.Point2D> {
				new G.Point2D(1,0),
				new G.Point2D(0,1),
				new G.Point2D(-1,0),
				new G.Point2D(0,-1)
			};

			Debug.Assert(cp3 != null, nameof(cp3) + " != null");
			CyclicListComparison(cp3.Contour.Vertices, res, "Diff 1");
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest2()
		{
			G.Point2D[] vs2 = new G.Point2D[]
			{
				new G.Point2D(0, 0),
				new G.Point2D(-1, 0),
				new G.Point2D(0, -1)
			};

			G.ConvexPolygon?
				cp1 = G.PolygonTools.RectangleTurned(0, -3, 0, 3, Math.PI / 4),
				cp2 = new G.ConvexPolygon(vs2, false),
				cp3 = cp1 - cp2;

			List<G.Point2D> res = new List<G.Point2D> {
				new G.Point2D(-2,0),
				new G.Point2D(0,-2),
				new G.Point2D(2.5,0.5),
				new G.Point2D(0.5,2.5)
			};

			Debug.Assert(cp3 != null, nameof(cp3) + " != null");
			CyclicListComparison(cp3.Contour.Vertices, res, "Diff 2");
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest3()
		{
			G.ConvexPolygon?
				cp1 = G.PolygonTools.RectangleParallel(-1, -1, 1, 1),
				cp2 = G.PolygonTools.RectangleParallel(-1, 0, 1, 0),
				cp3 = cp1 - cp2;

			List<G.Point2D> res = new List<G.Point2D> {
				new G.Point2D(0,1),
				new G.Point2D(0,-1),
			};

			Debug.Assert(cp3 != null, nameof(cp3) + " != null");
			CyclicListComparison(cp3.Contour.Vertices, res, "Diff 3");
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest4()
		{
			G.ConvexPolygon?
				cp1 = G.PolygonTools.RectangleParallel(-1, 0, 1, 0),
				cp2 = G.PolygonTools.RectangleParallel(-3, 4, -1, 4),
				cp3 = cp1 - cp2;

			List<G.Point2D> res = new List<G.Point2D> {
				new G.Point2D(2,-4)
			};

			Debug.Assert(cp3 != null, nameof(cp3) + " != null");
			CyclicListComparison(cp3.Contour.Vertices, res, "Diff 4");
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest5()
		{
			G.ConvexPolygon?
				cp1 = G.PolygonTools.RectangleTurned(-1, 0, 1, 0, Math.PI / 4),
				cp2 = G.PolygonTools.RectangleTurned(-3, 4, -1, 4, Math.PI / 4),
				cp3 = cp1 - cp2;

			List<G.Point2D> res = new List<G.Point2D> {
				new G.Point2D(2,-4)
			};

			Debug.Assert(cp3 != null, nameof(cp3) + " != null");
			CyclicListComparison(cp3.Contour.Vertices, res, "Diff 5");
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest6()
		{
			G.ConvexPolygon?
				cp1 = G.PolygonTools.RectangleTurned(-1, 0, 1, 0, Math.PI / 4),
				cp2 = G.PolygonTools.RectangleTurned(-3.01, 4, -1, 4, Math.PI / 4),
				cp3 = cp1 - cp2;

			Assert.IsTrue(cp3 == null, "Diff 6: the difference is not empty");
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest7()
		{
			G.ConvexPolygon?
				cp1 = G.PolygonTools.RectangleParallel(-1, 0, 1, 0),
				cp2 = G.PolygonTools.RectangleParallel(-3.01, 4, -1, 4),
				cp3 = cp1 - cp2;

			Assert.IsTrue(cp3 == null, "Diff 7: the difference is not empty");
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest8()
		{
			G.ConvexPolygon?
				cp1 = G.PolygonTools.RectangleParallel(-1, 0, 1, 0),
				cp2 = G.PolygonTools.RectangleParallel(0, -1, 0, 1),
				cp3 = cp1 - cp2;

			Assert.IsTrue(cp3 == null, "Diff 8: the difference is not empty");
		}

		[Category("ConvexPolygonTests"), Test]
		public void DiffTest9()
		{
			G.ConvexPolygon?
				cp1 = G.PolygonTools.RectangleParallel(-1, -1, 1, 1),
				cp2 = G.PolygonTools.Circle(0, 0, 1.01, 100),
				cp3 = cp1 - cp2;

			Assert.IsTrue(cp3 == null, "Diff 9: the difference is not empty");
		}

		[Category("ConvexPolygonTests"), Test]
		public void WeightTest1()
		{
			G.ConvexPolygon cp = new G.ConvexPolygon(ps1, true);
			double s = cp.Square;

			Assert.IsTrue(G.Tools.EQ(s, 4), "WeightTest1: wrong square");
		}

		[Category("ConvexPolygonTests"), Test]
		public void WeightTest2()
		{
			G.ConvexPolygon cp = new G.ConvexPolygon(
				new G.Point2D[]
				{
					new G.Point2D(-2, -1),
					new G.Point2D(-1, -2),
					new G.Point2D( 1, -2),
					new G.Point2D( 2, -1),
					new G.Point2D( 2,  1),
					new G.Point2D( 1,  2),
					new G.Point2D(-1,  2),
					new G.Point2D(-2,  1)
				}, true);
			double s = cp.Square;

			Assert.IsTrue(G.Tools.EQ(s, 14), "WeightTest1: wrong square");
		}

		[Category("ConvexPolygonTests"), Test]
		public void RandomPoint1()
		{
			G.ConvexPolygon cp = new G.ConvexPolygon(ps1, true);
			int N = 10, i;
			G.Point2D[] rndPoints = new G.Point2D[N];
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
			G.ConvexPolygon cp = new G.ConvexPolygon(
				new G.Point2D[]
				{
					new G.Point2D(-2, -1),
					new G.Point2D(-1, -2),
					new G.Point2D( 1, -2),
					new G.Point2D( 2, -1),
					new G.Point2D( 2,  1),
					new G.Point2D( 1,  2),
					new G.Point2D(-1,  2),
					new G.Point2D(-2,  1)
				}, true);
			int N = 100000, i;
			G.Point2D[] rndPoints = new G.Point2D[N];
			for (i = 0; i < N; i++) {
				rndPoints[i] = cp.GenerateRandomPoint();
			}

			G.Point2D[]
				cornerPoints = Array.FindAll(rndPoints, p => Math.Abs(p.x) > 1 && Math.Abs(p.y) > 1),
				_01Points = Array.FindAll(rndPoints, p => 0 < p.x && p.x < 1 && 0 < p.y && p.y < 1);

			double cornPointsFreq = cornerPoints.Length * 7, _01PointsFreq = _01Points.Length * 14;

			for (i = 0; i < N; i++) {
				Assert.IsTrue(cp.Contains(rndPoints[i]), "RandomPoint2: a point is obtained that is outside the polygon");
			}
		}

	}
}
