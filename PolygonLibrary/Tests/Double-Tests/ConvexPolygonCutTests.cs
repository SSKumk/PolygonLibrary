using NUnit.Framework;

namespace DoubleTests;
using static CGLibrary.Geometry<double, Convertors.DConvertor>;

public partial class ConvexPolygonTests {

  private readonly ConvexPolygon circle = PolygonTools.Circle(0, 0, 2, 4);

  //todo Придумать как тестировать
  [Category("ConvexPolygonTests"), Test]
  public void DoCutTest() {
    for (int i = 0; i < circle.Vertices.Count; i++) {
      for (int j = 0; j < circle.Vertices.Count; j++) {
        try {
          (ConvexPolygon cp1,ConvexPolygon cp2) = circle.CutConvexPolygon(i ,j);
        }
        catch (ArgumentException) {
        }
      }
    }
  }
}
