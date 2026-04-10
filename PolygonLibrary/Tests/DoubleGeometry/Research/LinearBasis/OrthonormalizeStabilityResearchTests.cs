using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using LinearBasisType = CGLibrary.Geometry<double, Tests.DConvertor>.LinearBasis;
using VectorType = CGLibrary.Geometry<double, Tests.DConvertor>.Vector;

namespace Tests.DoubleGeometry.Research.LinearBasisResearch;

[TestFixture]
public class OrthonormalizeStabilityResearchTests {

  [Test]
  public void Orthonormalize_UnitScaleCodimensionOneCase_BothVariantsRemainAccurate() {
    ResearchCase c = CreateCase(spaceDim: 10, subSpaceDim: 9, spanScale: 1.0, eps: 1e-6, seed: 910u);

    double projectorError = ComputeDirectionError(c.Basis.Orthonormalize(c.Input), c.ExpectedDirection);
    double lqError        = ComputeDirectionError(OrthonormalizeViaLQ(c.Basis, c.Input), c.ExpectedDirection);

    Assert.Multiple(() => {
      Assert.That(projectorError, Is.LessThan(1e-6));
      Assert.That(lqError, Is.LessThan(1e-12));
    });
  }

  [Test]
  public void Orthonormalize_LargeSpanCodimensionOneCase_LQStaysStableWhileProjectorLosesDirection() {
    // Репрезентативный плохо обусловленный случай:
    // большой компонент в span и маленькая ортогональная добавка.
    ResearchCase c = CreateCase(spaceDim: 10, subSpaceDim: 9, spanScale: 1e8, eps: 1e-6, seed: 911u);

    double projectorError = ComputeDirectionError(c.Basis.Orthonormalize(c.Input), c.ExpectedDirection);
    double lqError        = ComputeDirectionError(OrthonormalizeViaLQ(c.Basis, c.Input), c.ExpectedDirection);

    Assert.Multiple(() => {
      Assert.That(projectorError, Is.GreaterThan(1e-2));
      Assert.That(lqError, Is.LessThan(1e-12));
    });
  }

  [Test]
  public void Orthonormalize_HighDimensionalLargeSpanCase_LQKeepsDirectionInCodimensionOne() {
    // Усиленный сценарий той же проблемы в более высокой размерности.
    ResearchCase c = CreateCase(spaceDim: 30, subSpaceDim: 29, spanScale: 1e8, eps: 1e-7, seed: 912u);

    double projectorError = ComputeDirectionError(c.Basis.Orthonormalize(c.Input), c.ExpectedDirection);
    double lqError        = ComputeDirectionError(OrthonormalizeViaLQ(c.Basis, c.Input), c.ExpectedDirection);

    Assert.Multiple(() => {
      Assert.That(projectorError, Is.GreaterThan(1e-1));
      Assert.That(lqError, Is.LessThan(1e-12));
    });
  }

  private static ResearchCase CreateCase(int spaceDim, int subSpaceDim, double spanScale, double eps, uint seed) {
    GRandomLC  random   = new GRandomLC(seed);
    LinearBasisType basis   = LinearBasisType.GenLinearBasis(spaceDim, subSpaceDim, random);
    VectorType      expected = basis.OrthogonalComplement()[0];
    VectorType      span     = BuildSpanVector(basis, random, spanScale);
    VectorType      input    = span + eps * expected;

    return new ResearchCase(basis, input, expected);
  }

  private static VectorType BuildSpanVector(LinearBasisType basis, GRandomLC random, double spanScale) {
    VectorType sum = Vector.Zero(basis.SpaceDim);
    for (int i = 0; i < basis.SubSpaceDim; i++) {
      sum += random.NextPrecise(-1.0, 1.0) * basis[i];
    }

    if (sum.IsZero) {
      sum = basis[0];
    }

    return spanScale * sum.Normalize();
  }

  private static VectorType OrthonormalizeViaLQ(LinearBasisType basis, VectorType input) {
    if (input.IsZero || basis.FullDim) {
      return Vector.Zero(basis.SpaceDim);
    }

    if (basis.Empty) {
      return input.Normalize();
    }

    MatrixMutable orthogonalOperator = MatrixMutable.Eye(basis.SpaceDim);
    orthogonalOperator.SetSubMatrix(0, 0, basis.Basis);
    orthogonalOperator.SetSubMatrix(basis.SubSpaceDim, 0, basis.OrthogonalComplement().Basis);

    int oldDim = basis.SubSpaceDim;
    int newDim = Decomposition.LQ_IncrementalUpdate(ref orthogonalOperator, oldDim, input);
    if (newDim == oldDim) {
      return Vector.Zero(basis.SpaceDim);
    }

    return orthogonalOperator.TakeRowVector(oldDim);
  }

  private static double ComputeDirectionError(VectorType actual, VectorType expected) {
    if (actual.IsZero) {
      return 1.0;
    }

    return Math.Min((actual - expected).Length, (actual + expected).Length);
  }

  private readonly record struct ResearchCase(LinearBasisType Basis, VectorType Input, VectorType ExpectedDirection);
}
