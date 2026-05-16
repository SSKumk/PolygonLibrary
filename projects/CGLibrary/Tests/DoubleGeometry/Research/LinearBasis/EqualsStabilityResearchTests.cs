using System.Diagnostics;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using LinearBasisType = CGLibrary.Geometry<double, Tests.DConvertor>.LinearBasis;
using AffineBasisType = CGLibrary.Geometry<double, Tests.DConvertor>.AffineBasis;
using VectorType = CGLibrary.Geometry<double, Tests.DConvertor>.Vector;

namespace Tests.DoubleGeometry.Research.LinearBasisResearch;

[TestFixture]
public class EqualsStabilityResearchTests {

  [Test]
  public void LinearBasis_WellConditionedEqualCase_BothEqualitiesAgree() {
    LinearBasisType baseline = BuildPlaneBasis(Matrix.Eye(3));
    LinearBasisType candidate = BuildEquivalentPlaneBasis(Matrix.Eye(3), scale: 1.0, delta: 1e-6);

    Assert.Multiple(() => {
      Assert.That(CurrentLinearEquals(baseline, candidate), Is.True);
      Assert.That(GeometricLinearEquals(baseline, candidate), Is.True);
    });
  }

  [Test]
  public void LinearBasis_RepresentativeBorderlineEqualCase_GeometricEqualityCanSucceedWhereCurrentFails() {
    Matrix rotation = MakeRotation3D(0, 2, Math.PI / 3.0);
    LinearBasisType baseline = BuildPlaneBasis(rotation);
    LinearBasisType candidate = BuildEquivalentPlaneBasis(rotation, scale: 1e4, delta: 1e-8);

    Assert.Multiple(() => {
      Assert.That(CurrentLinearEquals(baseline, candidate), Is.False);
      Assert.That(GeometricLinearEquals(baseline, candidate), Is.True);
    });
  }

  [Test]
  public void LinearBasis_RepresentativeNearEpsDifferentCase_BothEqualitiesCanCollapseDifferentPlanes() {
    Matrix rotation = MakeRotation3D(0, 2, Math.PI / 3.0);
    LinearBasisType baseline = BuildPlaneBasis(rotation);
    LinearBasisType candidate = BuildPerturbedPlaneBasis(rotation, eta: 1e-8);

    Assert.Multiple(() => {
      Assert.That(CurrentLinearEquals(baseline, candidate), Is.True);
      Assert.That(GeometricLinearEquals(baseline, candidate), Is.True);
    });
  }

  [Test]
  public void AffineBasis_RepresentativeBorderlineEqualCase_GeometricEqualityCanSucceedWhereCurrentFails() {
    Matrix rotation = MakeRotation3D(1, 2, Math.PI / 6.0) * MakeRotation3D(0, 1, Math.PI / 4.0);
    AffineBasisType baseline = BuildAffinePlane(rotation, V(2.0, -1.0, 3.0));
    AffineBasisType candidate = BuildEquivalentAffinePlane(rotation, scale: 1e8, delta: 1e-8, V(2.0, -1.0, 3.0));

    Assert.Multiple(() => {
      Assert.That(CurrentAffineEquals(baseline, candidate), Is.False);
      Assert.That(GeometricAffineEquals(baseline, candidate), Is.True);
    });
  }

  [Test]
  public void AffineBasis_RepresentativeNearEpsDifferentCase_BothEqualitiesCanCollapseParallelPlanes() {
    Matrix rotation = MakeRotation3D(0, 2, Math.PI / 3.0);
    AffineBasisType baseline = BuildAffinePlane(rotation, V(2.0, -1.0, 3.0));
    AffineBasisType candidate = BuildParallelShiftedAffinePlane(rotation, eta: 1e-8, V(2.0, -1.0, 3.0));

    Assert.Multiple(() => {
      Assert.That(CurrentAffineEquals(baseline, candidate), Is.True);
      Assert.That(GeometricAffineEquals(baseline, candidate), Is.True);
    });
  }

  [Test]
  [Explicit("Research timing only. Run manually when comparing current and geometric equality costs.")]
  public void EqualityTiming_RepresentativeCases_CurrentEqualityIsUsuallyCheaperOnEqualCases() {
    Matrix rotation = MakeRotation3D(0, 2, Math.PI / 3.0);

    LinearBasisType linearEqualStableLeft = BuildPlaneBasis(Matrix.Eye(3));
    LinearBasisType linearEqualStableRight = BuildEquivalentPlaneBasis(Matrix.Eye(3), 1.0, 1e-6);

    LinearBasisType linearEqualBorderlineLeft = BuildPlaneBasis(rotation);
    LinearBasisType linearEqualBorderlineRight = BuildEquivalentPlaneBasis(rotation, 1e4, 1e-8);

    AffineBasisType affineEqualStableLeft = BuildAffinePlane(Matrix.Eye(3), V(2.0, -1.0, 3.0));
    AffineBasisType affineEqualStableRight = BuildEquivalentAffinePlane(Matrix.Eye(3), 1.0, 1e-6, V(2.0, -1.0, 3.0));

    AffineBasisType affineEqualBorderlineLeft = BuildAffinePlane(rotation, V(2.0, -1.0, 3.0));
    AffineBasisType affineEqualBorderlineRight = BuildEquivalentAffinePlane(rotation, 1e4, 1e-8, V(2.0, -1.0, 3.0));

    double linearStableCurrent = Benchmark(() => CurrentLinearEquals(linearEqualStableLeft, linearEqualStableRight));
    double linearStableGeom = Benchmark(() => GeometricLinearEquals(linearEqualStableLeft, linearEqualStableRight));
    double linearBorderCurrent = Benchmark(() => CurrentLinearEquals(linearEqualBorderlineLeft, linearEqualBorderlineRight));
    double linearBorderGeom = Benchmark(() => GeometricLinearEquals(linearEqualBorderlineLeft, linearEqualBorderlineRight));

    double affineStableCurrent = Benchmark(() => CurrentAffineEquals(affineEqualStableLeft, affineEqualStableRight));
    double affineStableGeom = Benchmark(() => GeometricAffineEquals(affineEqualStableLeft, affineEqualStableRight));
    double affineBorderCurrent = Benchmark(() => CurrentAffineEquals(affineEqualBorderlineLeft, affineEqualBorderlineRight));
    double affineBorderGeom = Benchmark(() => GeometricAffineEquals(affineEqualBorderlineLeft, affineEqualBorderlineRight));

    TestContext.Progress.WriteLine($"Linear stable: current={linearStableCurrent:F2}ms, geom={linearStableGeom:F2}ms");
    TestContext.Progress.WriteLine($"Linear borderline: current={linearBorderCurrent:F2}ms, geom={linearBorderGeom:F2}ms");
    TestContext.Progress.WriteLine($"Affine stable: current={affineStableCurrent:F2}ms, geom={affineStableGeom:F2}ms");
    TestContext.Progress.WriteLine($"Affine borderline: current={affineBorderCurrent:F2}ms, geom={affineBorderGeom:F2}ms");
  }

  private static bool CurrentLinearEquals(LinearBasisType a, LinearBasisType b) => a.Equals(b);

  private static bool GeometricLinearEquals(LinearBasisType a, LinearBasisType b) {
    if (a.SpaceDim != b.SpaceDim || a.SubSpaceDim != b.SubSpaceDim) {
      return false;
    }

    return a.SpanSameSpace(b) && b.SpanSameSpace(a);
  }

  private static bool CurrentAffineEquals(AffineBasisType a, AffineBasisType b) => a.Equals(b);

  private static bool GeometricAffineEquals(AffineBasisType a, AffineBasisType b) {
    if (a.SpaceDim != b.SpaceDim || a.SubSpaceDim != b.SubSpaceDim) {
      return false;
    }

    if (!GeometricLinearEquals(a.LinBasis, b.LinBasis)) {
      return false;
    }

    return a.Contains(b.Origin) && b.Contains(a.Origin);
  }

  private static LinearBasisType BuildPlaneBasis(Matrix rotation) {
    return new LinearBasisType(3, [
      rotation.TakeRowVector(0),
      rotation.TakeRowVector(1),
    ]);
  }

  private static LinearBasisType BuildEquivalentPlaneBasis(Matrix rotation, double scale, double delta) {
    Matrix leftTransform = new(new double[,] {
      { scale, scale },
      { scale, scale * (1.0 + delta) },
    });
    Matrix transformedRows = leftTransform * BuildRowsMatrix(rotation.TakeRowVector(0), rotation.TakeRowVector(1));

    return new LinearBasisType(3, [
      transformedRows.TakeRowVector(0),
      transformedRows.TakeRowVector(1),
    ]);
  }

  private static LinearBasisType BuildPerturbedPlaneBasis(Matrix rotation, double eta) {
    VectorType row1 = rotation.TakeRowVector(0);
    VectorType row2 = rotation.TakeRowVector(1);
    VectorType normal = rotation.TakeRowVector(2);

    return new LinearBasisType(3, [
      row1,
      row2 + eta * normal,
    ]);
  }

  private static AffineBasisType BuildAffinePlane(Matrix rotation, VectorType baseOrigin) {
    return new AffineBasisType(baseOrigin, BuildPlaneBasis(rotation), needCopy: false);
  }

  private static AffineBasisType BuildEquivalentAffinePlane(Matrix rotation, double scale, double delta, VectorType baseOrigin) {
    VectorType alongPlaneShift = 0.7 * rotation.TakeRowVector(0) - 0.4 * rotation.TakeRowVector(1);

    return new AffineBasisType(baseOrigin + alongPlaneShift, BuildEquivalentPlaneBasis(rotation, scale, delta), needCopy: false);
  }

  private static AffineBasisType BuildParallelShiftedAffinePlane(Matrix rotation, double eta, VectorType baseOrigin) {
    VectorType normal = rotation.TakeRowVector(2);

    return new AffineBasisType(baseOrigin + eta * normal, BuildPlaneBasis(rotation), needCopy: false);
  }

  private static Matrix MakeRotation3D(int axis1, int axis2, double angle) {
    double c = Math.Cos(angle);
    double s = Math.Sin(angle);
    double[,] data = {
      { 1.0, 0.0, 0.0 },
      { 0.0, 1.0, 0.0 },
      { 0.0, 0.0, 1.0 },
    };

    data[axis1, axis1] = c;
    data[axis1, axis2] = -s;
    data[axis2, axis1] = s;
    data[axis2, axis2] = c;

    return new Matrix(data);
  }

  private static Matrix BuildRowsMatrix(VectorType row1, VectorType row2) {
    return new Matrix(new double[,] {
      { row1[0], row1[1], row1[2] },
      { row2[0], row2[1], row2[2] },
    });
  }

  private static VectorType V(double x, double y, double z) => new(new[] { x, y, z });

  private static double Benchmark(Func<bool> func) {
    const int iterations = 200_000;
    bool sink = false;

    for (int i = 0; i < 5_000; i++) {
      sink ^= func();
    }

    long start = Stopwatch.GetTimestamp();
    for (int i = 0; i < iterations; i++) {
      sink ^= func();
    }
    long end = Stopwatch.GetTimestamp();

    GC.KeepAlive(sink);

    return (end - start) * 1000.0 / Stopwatch.Frequency;
  }

}
