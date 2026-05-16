using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

internal static class SupportFunctionTestData {

  public static readonly double DiagonalCoord = double.Sqrt(2.0) / 2.0;

  public static GammaPair[] CreateGps1() =>
    new[] {
      new GammaPair(new Vector2D(-1, 1), 1),
      new GammaPair(new Vector2D(1, -1), 1),
      new GammaPair(new Vector2D(-1, 1), 2),
      new GammaPair(new Vector2D(-1, -1), 1),
      new GammaPair(new Vector2D(1, 1), 1)
    };

  public static GammaPair[] CreateGps1True() {
    double v = DiagonalCoord;
    return
      new[] {
        new GammaPair(new Vector2D(v, v), v),
        new GammaPair(new Vector2D(-v, v), v),
        new GammaPair(new Vector2D(-v, -v), v),
        new GammaPair(new Vector2D(v, -v), v)
      };
  }

  public static GammaPair[] CreateGps2() =>
    new[] {
      new GammaPair(new Vector2D(0, 1), 1),
      new GammaPair(new Vector2D(0, -1), 1),
      new GammaPair(new Vector2D(-1, 0), 1),
      new GammaPair(new Vector2D(1, 0), 1)
    };

  public static GammaPair[] CreateGps2True() =>
    new[] {
      new GammaPair(new Vector2D(1, 0), 1),
      new GammaPair(new Vector2D(0, 1), 1),
      new GammaPair(new Vector2D(-1, 0), 1),
      new GammaPair(new Vector2D(0, -1), 1)
    };

  public static List<Vector2D> CreateFindConeOctagon() =>
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

  public static List<Vector2D> CreateFindConeHexagon() {
    List<double> vertexAngles = new() { 0.0, 60.0, 120.0, 180.0, 240.0, 300.0 };
    return vertexAngles.Select(
      angle => {
        double radians = angle * Tools.PI / 180.0;
        return new Vector2D(double.Cos(radians), double.Sin(radians));
      }
    ).ToList();
  }

}
