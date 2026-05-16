using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

[TestFixture]
public class ConvexPolytopSimplexVertexRecoveryTests {

  private static List<Vector> FindMaximizingVertices(ConvexPolytop polytope, Vector objective) {
    double maxValue = polytope.Vrep.Max(v => v * objective);

    return polytope.Vrep.Where(v => Tools.EQ(v * objective, maxValue)).ToList();
  }

  [Test]
  public void FindInitialVertex_Simplex_ForShiftedTriangleFaceOptimum_ReturnsMaximizingVertex() {
    ConvexPolytop triangle = ConvexPolytopTestData.CreateShiftedStandardSimplex(2);
    Vector objective = Vector.Ones(2);

    SimplexMethod.SimplexMethodResult simplex = SimplexMethod.Solve(triangle.Hrep, _ => 1.0);
    Assert.That(simplex.Solution, Is.Not.Null);

    Vector simplexPoint = new(simplex.Solution!, false);
    Vector? vertex = ConvexPolytop.FindInitialVertex_Simplex(triangle.Hrep, out List<HyperPlane>? activeHPs);
    List<Vector> maximizingVertices = FindMaximizingVertices(triangle, objective);
    List<HyperPlane> active = activeHPs!;

    Assert.Multiple(() => {
      Assert.That(triangle.Vrep.Contains(simplexPoint), Is.False);
      Assert.That(vertex, Is.Not.Null);
      Assert.That(triangle.Vrep.Contains(vertex!), Is.True);
      Assert.That(maximizingVertices.Contains(vertex!), Is.True);
      Assert.That(activeHPs, Is.Not.Null);
      Assert.That(active, Has.Count.GreaterThanOrEqualTo(2));
      Assert.That(active.All(hp => hp.Contains(vertex!)), Is.True);
    });
  }

  [Test]
  public void FindInitialVertex_Simplex_ForShiftedTetrahedronFaceOptimum_ReturnsMaximizingVertex() {
    ConvexPolytop tetrahedron = ConvexPolytopTestData.CreateShiftedStandardSimplex(3);
    Vector objective = Vector.Ones(3);

    SimplexMethod.SimplexMethodResult simplex = SimplexMethod.Solve(tetrahedron.Hrep, _ => 1.0);
    Assert.That(simplex.Solution, Is.Not.Null);

    Vector simplexPoint = new(simplex.Solution!, false);
    Vector? vertex = ConvexPolytop.FindInitialVertex_Simplex(tetrahedron.Hrep, out List<HyperPlane>? activeHPs);
    List<Vector> maximizingVertices = FindMaximizingVertices(tetrahedron, objective);
    List<HyperPlane> active = activeHPs!;

    Assert.Multiple(() => {
      Assert.That(tetrahedron.Vrep.Contains(simplexPoint), Is.False);
      Assert.That(simplex.BasisInequalitiesID.Count(), Is.EqualTo(1));
      Assert.That(vertex, Is.Not.Null);
      Assert.That(tetrahedron.Vrep.Contains(vertex!), Is.True);
      Assert.That(maximizingVertices.Contains(vertex!), Is.True);
      Assert.That(activeHPs, Is.Not.Null);
      Assert.That(active, Has.Count.GreaterThanOrEqualTo(3));
      Assert.That(active.All(hp => hp.Contains(vertex!)), Is.True);
    });
  }

  [TestCase(4)]
  [TestCase(5)]
  public void FindInitialVertex_Simplex_ForHigherDimensionalShiftedSimplex_RefinesNonVertexSimplexOptimumToVertex(int dim) {
    ConvexPolytop simplexPolytope = ConvexPolytopTestData.CreateShiftedStandardSimplex(dim);
    Vector objective = Vector.Ones(dim);

    SimplexMethod.SimplexMethodResult simplex = SimplexMethod.Solve(simplexPolytope.Hrep, _ => 1.0);
    Assert.That(simplex.Solution, Is.Not.Null);

    Vector simplexPoint = new(simplex.Solution!, false);
    Vector? vertex = ConvexPolytop.FindInitialVertex_Simplex(simplexPolytope.Hrep, out List<HyperPlane>? activeHPs);
    List<Vector> maximizingVertices = FindMaximizingVertices(simplexPolytope, objective);
    List<HyperPlane> active = activeHPs!;

    Assert.Multiple(() => {
      Assert.That(simplexPolytope.Vrep.Contains(simplexPoint), Is.False);
      Assert.That(simplex.BasisInequalitiesID.Count(), Is.LessThan(dim));
      Assert.That(vertex, Is.Not.Null);
      Assert.That(simplexPolytope.Vrep.Contains(vertex!), Is.True);
      Assert.That(maximizingVertices.Contains(vertex!), Is.True);
      Assert.That(activeHPs, Is.Not.Null);
      Assert.That(active, Has.Count.GreaterThanOrEqualTo(dim));
      Assert.That(active.All(hp => hp.Contains(vertex!)), Is.True);
    });
  }

  [TestCase(2)]
  [TestCase(3)]
  [TestCase(4)]
  [TestCase(5)]
  public void FindInitialVertex_Simplex_ForDegenerateMaximizingVertex_ReturnsVertexAndFullActiveSet(int dim) {
    List<HyperPlane> hps = ConvexPolytopTestData.CreateDegenerateVertexHrep(dim);
    ConvexPolytop polytope = ConvexPolytopTestData.CreateDegenerateVertexPolytope(dim);

    SimplexMethod.SimplexMethodResult simplex = SimplexMethod.Solve(hps, _ => 1.0);
    Assert.That(simplex.Solution, Is.Not.Null);

    Vector simplexPoint = new(simplex.Solution!, false);
    Vector? vertex = ConvexPolytop.FindInitialVertex_Simplex(hps, out List<HyperPlane>? activeHPs);
    List<HyperPlane> active = activeHPs!;

    Assert.Multiple(() => {
      Assert.That(vertex, Is.Not.Null);
      ConvexPolytopAssert.AssertVectorsAreEqual(simplexPoint, Vector.Zero(dim));
      ConvexPolytopAssert.AssertVectorsAreEqual(vertex!, Vector.Zero(dim));
      Assert.That(polytope.Vrep.Contains(simplexPoint), Is.True);
      Assert.That(polytope.Vrep.Contains(vertex!), Is.True);
      Assert.That(activeHPs, Is.Not.Null);
      Assert.That(active, Has.Count.EqualTo(dim + 1));
      Assert.That(active.All(hp => hp.Contains(vertex!)), Is.True);
      Assert.That(simplex.BasisInequalitiesID.Count(), Is.EqualTo(dim));
    });
  }

}
