using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using PolygonLibrary;
using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

namespace ToolsTests
{
  /// <summary>
  ///This is a test class for ToolsTests and is intended
  ///to contain all ToolsTests Unit Tests
  ///</summary>
  [TestClass()]
  public class ToolsTests
  {
    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion

    private double[,] a = new double[,] { 
      {12, 12},
      {12, 12.0000000001},
      {12, 12.0001}, 
      {12, 13}, 
      {12, 11.9999999999}, 
      {12, 11.9999},
      {12, 11},
      {12.0000000001, 12},
      {12.0001, 12}, 
      {13, 12}, 
      {11.9999999999, 12}, 
      {11.9999, 12},
      {11, 12}
    };
    private bool[,] res = new bool[,] {
      {true, false, false, true, false, true},
      {true, false, false, true, false, true},
      {false, true, false, false, true, true},
      {false, true, false, false, true, true},
      {true, false, false, true, false, true},
      {false, true, true, true, false, false},
      {false, true, true, true, false, false},
      {true, false, false, true, false, true},
      {false, true, true, true, false, false},
      {false, true, true, true, false, false},
      {true, false, false, true, false, true},
      {false, true, false, false, true, true},
      {false, true, false, false, true, true}
    };
    private string[] names = new string[] { "EQ", "NE", "GT", "GE", "LT", "LE" };

    private bool CallComp(double v1, double v2, int func)
    {
      switch (func)
      {
        case 0:
          return Tools.EQ(v1, v2);
        case 1:
          return Tools.NE(v1, v2);
        case 2:
          return Tools.GT(v1, v2);
        case 3:
          return Tools.GE(v1, v2);
        case 4:
          return Tools.LT(v1, v2);
        case 5:
          return Tools.LE(v1, v2);
        default:
          throw new ArgumentException();
      }
    }

    /// <summary>
    /// Test for comparison
    ///</summary>
    [TestMethod()]
    public void CompTest()
    {
      bool actual;
      int i, ind;

      for (ind = 0; ind < 6; ind++)
      {
        for (i = 0; i < 13; i++)
        {
          actual = CallComp(a[i, 0], a[i, 1], ind);
          Assert.AreEqual(res[i, ind], actual, "Test #" + i + " isn't passed for " + names[ind]);
        }
      }
    }

    ///<summary>
    ///A test for Eps
    ///</summary>
    [TestMethod()]
    public void EpsTest()
    {
      double e = 1e-7;
      Tools.Eps = e;
      Assert.AreEqual(e, Tools.Eps, "Cannot set comparison precision");
    }

    [TestMethod()]
    public void CauchyTest()
    {
      double[,]
        d_ans0 = new double[2, 2] { { 1, 0 }, 
                                    { 0, 1 } },
        d_ans_m1 = new double[2, 2] { { 1, 1 }, 
                                      { 0, 1 } },
        d_ans_p3 = new double[2, 2] { { 1, -3 }, 
                                      { 0, 1 } };

      Matrix d_A = new Matrix(new double[2, 2] { { 0, 1 }, 
                                                 { 0, 0 } });
      CauchyMatrix cm = new CauchyMatrix(d_A, 0, 0.025);
      Matrix res;

      res = cm[0];
      for (int i = 0; i < res.Rows; i++)
        for (int j = 0; j < res.Cols; j++)
          Assert.IsTrue(Tools.EQ(res[i, j], d_ans0[i, j]),
            "Inertial point: bad assertion for t = 0; i = " + i + ", j = " + j);

      res = cm[-1];
      for (int i = 0; i < res.Rows; i++)
        for (int j = 0; j < res.Cols; j++)
          Assert.IsTrue(Tools.EQ(res[i, j], d_ans_m1[i, j]),
            "Inertial point: bad assertion for t = -1; i = " + i + ", j = " + j);

      res = cm[3];
      for (int i = 0; i < res.Rows; i++)
        for (int j = 0; j < res.Cols; j++)
          Assert.IsTrue(Tools.EQ(res[i, j], d_ans_p3[i, j]),
            "Inertial point: bad assertion for t = +3; i = " + i + ", j = " + j);

      double[,]
        o_ans0 = new double[2, 2] { { 1, 0 }, 
                                    { 0, 1 } },
        o_ans_m1 = new double[2, 2] { {  Math.Cos(1), Math.Sin(1) }, 
                                      { -Math.Sin(1), Math.Cos(1) } },
        o_ans_p3 = new double[2, 2] { {  Math.Cos(3), -Math.Sin(3) }, 
                                      {  Math.Sin(3),  Math.Cos(3) } };

      Matrix o_A = new Matrix(new double[2, 2] { { 0, 1 }, 
                                                 { -1, 0 } });
      cm = new CauchyMatrix(o_A, 0, 0.025);

      res = cm[0];
      for (int i = 0; i < res.Rows; i++)
        for (int j = 0; j < res.Cols; j++)
          Assert.IsTrue(Tools.EQ(res[i, j], o_ans0[i, j]),
            "Oscillator: bad assertion for t = 0; i = " + i + ", j = " + j);

      res = cm[-1];
      for (int i = 0; i < res.Rows; i++)
        for (int j = 0; j < res.Cols; j++)
          Assert.IsTrue(Tools.EQ(res[i, j], o_ans_m1[i, j]),
            "Oscillator: bad assertion for t = -1; i = " + i + ", j = " + j);

      res = cm[3];
      for (int i = 0; i < res.Rows; i++)
        for (int j = 0; j < res.Cols; j++)
          Assert.IsTrue(Tools.EQ(res[i, j], o_ans_p3[i, j]),
            "Oscillator: bad assertion for t = +3; i = " + i + ", j = " + j);
    }
  }
}
