using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

internal static class FaceLatticeTestData {

  public static (FLNode v0, FLNode v1, FLNode v2, FLNode e01, FLNode e12, FLNode e20, FLNode face) CreateTriangleHierarchy() {
    FLNode v0 = new(FaceLatticeAssert.V(0, 0));
    FLNode v1 = new(FaceLatticeAssert.V(2, 0));
    FLNode v2 = new(FaceLatticeAssert.V(1, 1));

    FLNode e01 = new([v0, v1]);
    FLNode e12 = new([v1, v2]);
    FLNode e20 = new([v0, v2]);
    FLNode face = new([e01, e12, e20]);

    return (v0, v1, v2, e01, e12, e20, face);
  }

  public static FaceLattice CreateTriangleLattice() {
    var (v0, v1, v2, e01, e12, e20, face) = CreateTriangleHierarchy();

    return new FaceLattice([
      new SortedSet<FLNode> { v0, v1, v2 },
      new SortedSet<FLNode> { e01, e12, e20 },
      new SortedSet<FLNode> { face }
    ]);
  }

  public static FaceLattice CreateTriangleLatticeReordered() {
    FLNode v2 = new(FaceLatticeAssert.V(1, 1));
    FLNode v1 = new(FaceLatticeAssert.V(2, 0));
    FLNode v0 = new(FaceLatticeAssert.V(0, 0));

    FLNode e20 = new([v2, v0]);
    FLNode e01 = new([v1, v0]);
    FLNode e12 = new([v2, v1]);
    FLNode face = new([e20, e01, e12]);

    return new FaceLattice([
      new SortedSet<FLNode> { v2, v0, v1 },
      new SortedSet<FLNode> { e20, e01, e12 },
      new SortedSet<FLNode> { face }
    ]);
  }

}
