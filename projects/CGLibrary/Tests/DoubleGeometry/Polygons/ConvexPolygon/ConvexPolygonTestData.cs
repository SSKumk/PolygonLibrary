using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

internal static class ConvexPolygonTestData {

  public static readonly GammaPair[] AxisAlignedSquareGammaPairs =
    new[] {
      new GammaPair(new Vector2D(1, 0), 1),
      new GammaPair(new Vector2D(0, 1), 1),
      new GammaPair(new Vector2D(-1, 0), 1),
      new GammaPair(new Vector2D(0, -1), 1)
    };

  public static readonly Vector2D[] SquareVerticesClockwiseStartingTopRight =
    new[] {
      new Vector2D(1, 1),
      new Vector2D(-1, 1),
      new Vector2D(-1, -1),
      new Vector2D(1, -1)
    };

  public static readonly Vector2D[] CrossedSquarePoints =
    new[] {
      new Vector2D(-1, -1),
      new Vector2D(1, 1),
      new Vector2D(-1, 1),
      new Vector2D(1, -1)
    };

  public static List<Vector2D> CreateOctagonVertices() =>
    new() {
      new Vector2D(-2, -1),
      new Vector2D(-1, -2),
      new Vector2D(1, -2),
      new Vector2D(2, -1),
      new Vector2D(2, 1),
      new Vector2D(1, 2),
      new Vector2D(-1, 2),
      new Vector2D(-2, 1)
    };

  public static List<Vector> CreateSquareVectors() =>
    SquareVerticesClockwiseStartingTopRight.Select(v => new Vector(new[] { v.x, v.y })).ToList();

}
