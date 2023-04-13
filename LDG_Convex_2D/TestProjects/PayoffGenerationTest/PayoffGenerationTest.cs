using NUnit.Framework;

using LDGObjects;
using PolygonLibrary.Polygons.ConvexPolygons;

namespace Tests
{
  [TestFixtureAttribute]
  public class PayoffGenerationTest
  {
    [Test]
    public void TestDistanceToSet1()
    {
      GameData gd = new GameData("../../../Tests/dist1.c", ComputationType.StableBridge);

      ConvexPolygon
        cp1 = gd.PayoffLevelSet(0),
        cp2 = gd.PayoffLevelSet(1),
        cp3 = gd.PayoffLevelSet(1, 5);

      StreamWriter sr;

      sr = new StreamWriter("../../../Tests/dist11.res");
      for (int i = 0; i < cp1.Contour.Count; i++)
        sr.WriteLine(cp1.Contour[i].x + " " + cp1.Contour[i].y);
      sr.Close();

      sr = new StreamWriter("../../../Tests/dist12.res");
      for (int i = 0; i < cp2.Contour.Count; i++)
        sr.WriteLine(cp2.Contour[i].x + " " + cp2.Contour[i].y);
      sr.Close();

      sr = new StreamWriter("../../../Tests/dist13.res");
      for (int i = 0; i < cp3.Contour.Count; i++)
        sr.WriteLine(cp3.Contour[i].x + " " + cp3.Contour[i].y);
      sr.Close();
    }

    [Test]
    public void TestDistanceToSet2()
    {
      GameData gd = new GameData("../../../Tests/dist2.c", ComputationType.StableBridge);

      ConvexPolygon
        cp1 = gd.PayoffLevelSet(0),
        cp2 = gd.PayoffLevelSet(1),
        cp3 = gd.PayoffLevelSet(1, 5);

      StreamWriter sr;

      sr = new StreamWriter("../../../Tests/dist21.res");
      for (int i = 0; i < cp1.Contour.Count; i++)
        sr.WriteLine(cp1.Contour[i].x + " " + cp1.Contour[i].y);
      sr.Close();

      sr = new StreamWriter("../../../Tests/dist22.res");
      for (int i = 0; i < cp2.Contour.Count; i++)
        sr.WriteLine(cp2.Contour[i].x + " " + cp2.Contour[i].y);
      sr.Close();

      sr = new StreamWriter("../../../Tests/dist23.res");
      for (int i = 0; i < cp3.Contour.Count; i++)
        sr.WriteLine(cp3.Contour[i].x + " " + cp3.Contour[i].y);
      sr.Close();
    }


    [Test]
    public void TestMink1()
    {
      GameData gd = new GameData("../../../Tests/mink1.c", ComputationType.StableBridge);

      ConvexPolygon
        cp1 = gd.PayoffLevelSet(0.5),
        cp2 = gd.PayoffLevelSet(1),
        cp3 = gd.PayoffLevelSet(1.5);

      StreamWriter sr;

      sr = new StreamWriter("../../../Tests/mink11.res");
      for (int i = 0; i < cp1.Contour.Count; i++)
        sr.WriteLine(cp1.Contour[i].x + " " + cp1.Contour[i].y);
      sr.Close();

      sr = new StreamWriter("../../../Tests/mink12.res");
      for (int i = 0; i < cp2.Contour.Count; i++)
        sr.WriteLine(cp2.Contour[i].x + " " + cp2.Contour[i].y);
      sr.Close();

      sr = new StreamWriter("../../../Tests/mink13.res");
      for (int i = 0; i < cp3.Contour.Count; i++)
        sr.WriteLine(cp3.Contour[i].x + " " + cp3.Contour[i].y);
      sr.Close();
    }
    
    [Test]
    public void TestMink2()
    {
      GameData gd = new GameData("../../../Tests/mink2.c", ComputationType.StableBridge);

      ConvexPolygon
        cp1 = gd.PayoffLevelSet(0.5),
        cp2 = gd.PayoffLevelSet(1),
        cp3 = gd.PayoffLevelSet(1.5);

      StreamWriter sr;

      sr = new StreamWriter("../../../Tests/mink21.res");
      for (int i = 0; i < cp1.Contour.Count; i++)
        sr.WriteLine(cp1.Contour[i].x + " " + cp1.Contour[i].y);
      sr.Close();

      sr = new StreamWriter("../../../Tests/mink22.res");
      for (int i = 0; i < cp2.Contour.Count; i++)
        sr.WriteLine(cp2.Contour[i].x + " " + cp2.Contour[i].y);
      sr.Close();

      sr = new StreamWriter("../../../Tests/mink23.res");
      for (int i = 0; i < cp3.Contour.Count; i++)
        sr.WriteLine(cp3.Contour[i].x + " " + cp3.Contour[i].y);
      sr.Close();
    }
  }
}