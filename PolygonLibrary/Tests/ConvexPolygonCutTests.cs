using NUnit.Framework;

namespace Tests;
using G = CGLibrary.Geometry<double, DConvertor>;

public partial class ConvexPolygonTests {

  private readonly G.ConvexPolygon circle = G.PolygonTools.Circle(0, 0, 2, 4);

  //todo Придумать как тестировать
  [Category("ConvexPolygonTests"), Test]
  public void DoCutTest() {
    for (int i = 0; i < circle.Vertices.Count; i++) {
      for (int j = 0; j < circle.Vertices.Count; j++) {
        try {
          (G.ConvexPolygon cp1,G.ConvexPolygon cp2) = circle.CutConvexPolygon(i ,j);
        }
        catch (ArgumentException) {
        }
      }
    }
  }
}
