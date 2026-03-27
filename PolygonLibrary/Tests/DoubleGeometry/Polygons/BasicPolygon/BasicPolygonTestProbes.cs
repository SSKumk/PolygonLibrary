using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

internal sealed class BasicPolygonFromContoursProbe : BasicPolygon {

  public BasicPolygonFromContoursProbe(List<Polyline> contours) {
    Contours = contours;
  }

  public override bool Contains(Vector2D p) => false;

  public override bool ContainsInside(Vector2D p) => false;

}

internal sealed class BasicPolygonFromVerticesProbe : BasicPolygon {

  public BasicPolygonFromVerticesProbe(List<Vector2D> vertices) {
    Vertices = new List<Vector2D>(vertices);
  }

  public override bool Contains(Vector2D p) => false;

  public override bool ContainsInside(Vector2D p) => false;

}

internal sealed class BasicPolygonCtorProbe : BasicPolygon {

  public BasicPolygonCtorProbe(List<Vector2D> vertices, bool checkOrient = true, bool checkCross = true) : base(vertices, checkOrient, checkCross) { }

  public BasicPolygonCtorProbe(Vector2D[] vertices, bool checkOrient = true, bool checkCross = true) : base(vertices, checkOrient, checkCross) { }

  public override bool Contains(Vector2D p) => false;

  public override bool ContainsInside(Vector2D p) => false;

}
