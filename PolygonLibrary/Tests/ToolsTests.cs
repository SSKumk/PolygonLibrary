using NUnit.Framework;

using PolygonLibrary.Toolkit;

namespace Tests
{
    /// <summary>
    ///This is a test class for ToolsTests and is intended
    ///to contain all ToolsTests Unit Tests
    ///</summary>
  [TestFixture]
  public class ToolsTests
  {
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
    private bool[,] res = new bool [,] {
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

    private bool CallComp (double v1, double v2, int func) =>
      func switch {
        0 => Tools.EQ(v1, v2)
        , 1 => Tools.NE(v1, v2)
        , 2 => Tools.GT(v1, v2)
        , 3 => Tools.GE(v1, v2)
        , 4 => Tools.LT(v1, v2)
        , 5 => Tools.LE(v1, v2)
        , _ => throw new ArgumentException()
      };

    /// <summary>
    /// Test for comparison
    ///</summary>
    [Test]
    public void CompTest ()
    {
      bool actual;
      int i, ind;

      for (ind = 0; ind < 6; ind++)
      {
        for (i = 0; i < 13; i++)
        {
          actual = CallComp (a[i, 0], a[i, 1], ind);
          Assert.That(actual, Is.EqualTo(res[i, ind]), "Test #" + i + " isn't passed for " + names[ind]);
        }
      }
    } 

    ///<summary>
    ///A test for Eps
    ///</summary>
    [Test]
    public void EpsTest ()
    {
      double e = 1e-7;
      Tools.Eps = e;
      Assert.That(Tools.Eps, Is.EqualTo(e), "Cannot set comparison precision");
    }
  }
}
