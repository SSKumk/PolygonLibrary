using System;
using System.Linq;
using System.Collections.Generic;
using static System.Math;
using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;
using PolygonLibrary.Polygons;

namespace Tests
{
	[TestClass]
	public class SupportFunctionTests
	{
		#region Data
		GammaPair[] gps1 = new GammaPair[]
		{
			new GammaPair (new Vector2D(-1,1), 1),
			new GammaPair (new Vector2D(1,-1), 1),
			new GammaPair (new Vector2D(-1,1), 2),
			new GammaPair (new Vector2D(-1,-1), 1),
			new GammaPair (new Vector2D(1,1), 1),
		};

		static double v = Math.Sqrt(2) / 2;

		GammaPair[] gps1_true = new GammaPair[]
		{
			new GammaPair (new Vector2D(v,v), v),
			new GammaPair (new Vector2D(-v,v), v),
			new GammaPair (new Vector2D(-v,-v), v),
			new GammaPair (new Vector2D(v,-v), v)
		};

		GammaPair[] gps2 = new GammaPair[]
		{
			new GammaPair (new Vector2D(0,1), 1),
			new GammaPair (new Vector2D(0,-1), 1),
			new GammaPair (new Vector2D(-1,0), 1),
			new GammaPair (new Vector2D(1,0), 1)
		};

		GammaPair[] gps2_true = new GammaPair[]
		{
			new GammaPair (new Vector2D(1,0), 1),
			new GammaPair (new Vector2D(0,1), 1),
			new GammaPair (new Vector2D(-1,0), 1),
			new GammaPair (new Vector2D(0,-1), 1)
		};
		#endregion

		[TestCategory("SuppFuncTests"), TestMethod()]
		public void CreateSFTest1()
		{
			SupportFunction sf = new SupportFunction(gps1);

			Assert.AreEqual(sf.Count, 4, "Wrong number of resultant vectors");

			for (int i = 0; i < 4; i++) {
				Assert.IsTrue(sf[i].Equals(gps1_true[i]), "i = " + i);
			}
		}

		[TestCategory("SuppFuncTests"), TestMethod()]
		public void CreateSFTest2()
		{
			SupportFunction sf = new SupportFunction(gps2);

			Assert.AreEqual(sf.Count, 4, "Wrong number of resultant vectors");

			for (int i = 0; i < 4; i++) {
				Assert.IsTrue(sf[i].Equals(gps2_true[i]), "i = " + i);
			}
		}

		[TestCategory("SuppFuncTests"), TestMethod()]
		public void ComputeSFTest1()
		{
			SupportFunction sf = new SupportFunction(gps1);

			Vector2D[] vs = new Vector2D[]
			{
				new Vector2D(1, 0),
				new Vector2D(1, 0.5),
				new Vector2D(v, v),
				new Vector2D(1, 1),
				new Vector2D(0, 1),
				new Vector2D(v, -v),
				new Vector2D(-1, -1),
				new Vector2D(1, -0.5)
			};
			double[] gs = new double[] { 1, 1, v, 1, 1, v, 1, 1 };

			for (int i = 0; i < vs.Length; i++) {
				Assert.IsTrue(Tools.EQ(sf.FuncVal(vs[i]), gs[i]), "i = " + i);
			}
		}

		[TestCategory("SuppFuncTests"), TestMethod()]
		public void CrossPairsTest()
		{
			GammaPair
				g1 = new GammaPair(new Vector2D(1, 0), 1),
				g2 = new GammaPair(new Vector2D(1, 0), 2),
				g3 = new GammaPair(new Vector2D(0, 1), 1),
				g4 = new GammaPair(new Vector2D(1, 1), 3);
			Point2D
				p13 = new Point2D(1, 1),
				p14 = new Point2D(1, 2),
				p23 = new Point2D(2, 1),
				p24 = new Point2D(2, 1),
				p34 = new Point2D(2, 1),
				p;

			bool hasException = false;
			try
			{
				GammaPair.CrossPairs(g1, g2);
			}
			catch
			{
				hasException = true;
			}
			Assert.IsTrue(hasException, "No exception during crossing parallel lines");

			p = GammaPair.CrossPairs(g1, g3);
			Assert.IsTrue(p.Equals(p13), "Bad crossing g1 and g3");

			p = GammaPair.CrossPairs(g3, g1);
			Assert.IsTrue(p.Equals(p13), "Bad crossing g3 and g1");

			p = GammaPair.CrossPairs(g1, g4);
			Assert.IsTrue(p.Equals(p14), "Bad crossing g1 and g4");

			p = GammaPair.CrossPairs(g2, g3);
			Assert.IsTrue(p.Equals(p23), "Bad crossing g2 and g3");

			p = GammaPair.CrossPairs(g2, g3);
			Assert.IsTrue(p.Equals(p24), "Bad crossing g21 and g3");

			p = GammaPair.CrossPairs(g3, g4);
			Assert.IsTrue(p.Equals(p34), "Bad crossing g3 and g4");
		}

		[TestCategory("SuppFuncTests"), TestMethod()]
		public void CombineSFTest()
		{
			SupportFunction
				sf1 = new SupportFunction(gps1_true, false),
				sf2 = new SupportFunction(gps2_true, false),

				sf11 = SupportFunction.CombineFunctions(sf1, sf1, 1, 1),
				sf22 = SupportFunction.CombineFunctions(sf2, sf2, 1, 1),
				sf12 = SupportFunction.CombineFunctions(sf1, sf2, 1, 1);

			int i;

			for (i = 0; i < sf11.Count; i++)
			{
				Assert.IsTrue(sf11[i].Normal.Equals(sf1[i].Normal), "sf11: " + i + "th normal is wrong");
				Assert.IsTrue(Tools.EQ(sf11[i].Value, 2 * sf1[i].Value), "sf11: " + i + "th value is wrong");
			}

			for (i = 0; i < sf22.Count; i++)
			{
				Assert.IsTrue(sf22[i].Normal.Equals(sf2[i].Normal), "sf22: " + i + "th normal is wrong");
				Assert.IsTrue(Tools.EQ(sf22[i].Value, 2 * sf2[i].Value), "sf22: " + i + "th value is wrong");
			}

			for (i = 0; i < sf12.Count; i++)
			{
				if (i % 2 != 0)
				{
					Assert.IsTrue(sf12[i].Normal.Equals(sf1[i / 2].Normal), "sf12: " + i + "th normal is wrong");
					Assert.IsTrue(Tools.EQ(sf12[i].Value, 3 * sf1[i / 2].Value), "sf12: " + i + "th value is wrong");
				}
				else
				{
					Assert.IsTrue(sf12[i].Normal.Equals(sf2[i / 2].Normal), "sf12: " + i + "th normal is wrong");
					Assert.IsTrue(Tools.EQ(sf12[i].Value, 2 * sf2[i / 2].Value), "sf12: " + i + "th value is wrong");
				}
			}
		}

		[TestCategory("SuppFuncTests"), TestMethod()]
		public void FindTest1()
		{
			List<Point2D> ps = new List<Point2D>()
			{
					new Point2D(-2, -1),
					new Point2D(-1, -2),
					new Point2D( 1, -2),
					new Point2D( 2, -1),
					new Point2D( 2,  1),
					new Point2D( 1,  2),
					new Point2D(-1,  2),
					new Point2D(-2,  1)
			};
			SupportFunction sf = new SupportFunction(ps);
			List<double> testAngles = new List<double>()
				{ 0.0, 30.0, 45.0, 60.0, 90.0, 120.0, 135.0, 270.0, 300.0, 315.0, 330.0 };
			double[,] res = new double[,]
				{
					{7, 0}, {0, 1}, {0, 1}, {1, 2}, {1, 2}, {2, 3}, {2, 3}, {5, 6}, {6, 7}, {6, 7}, {7, 0}
				};
			List<Vector2D> testVecs = testAngles.Select(
				a => { double a1 = a * PI / 180; return new Vector2D(Cos(a1), Sin(a1)); }
				).ToList();

			int i, j, k;
			for (k = 0; k < testVecs.Count; k++)
			{
				sf.FindCone(testVecs[k], out i, out j);
				Assert.IsTrue(i == res[k, 0] && j == res[k, 1],
					"FindCone1: test #" + k + " failed, angle = " + testAngles[k]);
			}
		}

		[TestCategory("SuppFuncTests"), TestMethod()]
		public void FindTest2()
		{
			List<double> vertAngles = new List<double>()
				{ 0.0, 60.0, 120.0, 180.0, 240.0, 300.0 };
			List<Point2D> ps = vertAngles.Select(
				a => { double a1 = a * PI / 180; return new Point2D(Cos(a1), Sin(a1)); }
				).ToList();
			SupportFunction sf = new SupportFunction(ps);
			List<double> testAngles = new List<double>()
				{ -30.0, -15.0, 0.0, 15.0, 30.0, 60.0, 90.0, 180.0, 210.0, 270.0, 300.0};
			double[,] res = new double[,]
				{
					{4, 5},  {5, 0}, {5, 0}, {5, 0}, {5, 0}, {0, 1}, {0, 1}, {2, 3}, {2, 3}, {3, 4}, {4, 5}
				};
			List<Vector2D> testVecs = testAngles.Select(
				a => { double a1 = a * PI / 180; return new Vector2D(Cos(a1), Sin(a1)); }
				).ToList();

			int i, j, k;
			for (k = 0; k < testVecs.Count; k++)
			{
				sf.FindCone(testVecs[k], out i, out j);
				Assert.IsTrue(i == res[k, 0] && j == res[k, 1],
					"FindCone2: test #" + k + " failed, angle = " + testAngles[k]);
			}
		}
	}
}
