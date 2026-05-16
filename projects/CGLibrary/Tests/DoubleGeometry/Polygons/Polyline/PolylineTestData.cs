using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

internal static class PolylineTestData {

  public static List<Vector2D> CreateUnitSquareVertices() =>
    new() {
      new Vector2D(0.0, 0.0),
      new Vector2D(1.0, 0.0),
      new Vector2D(1.0, 1.0),
      new Vector2D(0.0, 1.0)
    };

  public static List<Vector2D> CreateUnitSquareVerticesClockwise() =>
    new() {
      new Vector2D(0.0, 0.0),
      new Vector2D(0.0, 1.0),
      new Vector2D(1.0, 1.0),
      new Vector2D(1.0, 0.0)
    };

  public static List<Vector2D> CreateIndentedPentagonVertices() =>
    new() {
      new Vector2D(0.0, 0.0),
      new Vector2D(1.0, 0.0),
      new Vector2D(1.0, 1.0),
      new Vector2D(0.0, 1.0),
      new Vector2D(0.1, 0.5)
    };

}
