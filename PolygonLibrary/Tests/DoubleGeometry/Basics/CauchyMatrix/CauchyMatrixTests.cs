using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class CauchyMatrixTests {

  [Test]
  public void Indexer_AtReferenceInstant_ReturnsIdentityMatrix() {
    CauchyMatrix cauchy = new(Matrix.Eye(2), 3.0, 0.1);

    CauchyMatrixAssert.AreEqual(cauchy[3.0], Matrix.Eye(2), 1e-12);
  }

  [Test]
  public void ZeroMatrix_ProducesIdentityForAnyInstant() {
    CauchyMatrix cauchy = new(new Matrix(new double[,] { { 0, 0 }, { 0, 0 } }), 0.0, 0.25);

    CauchyMatrixAssert.AreEqual(cauchy[1.75], Matrix.Eye(2), 1e-12);
    CauchyMatrixAssert.AreEqual(cauchy[-2.5], Matrix.Eye(2), 1e-12);
  }

  [Test]
  public void DiagonalMatrix_MatchesExactExponentialForPositiveAndNegativeInstants() {
    Matrix a = new(new double[,] { { 1, 0 }, { 0, 2 } });
    CauchyMatrix cauchy = new(a, 0.0, 0.01);

    Matrix expectedAtPositive = new(new double[,] {
      { Math.Exp(-1.0), 0 },
      { 0, Math.Exp(-2.0) }
    });
    Matrix expectedAtNegative = new(new double[,] {
      { Math.Exp(1.0), 0 },
      { 0, Math.Exp(2.0) }
    });

    CauchyMatrixAssert.AreEqual(cauchy[1.0], expectedAtPositive, 1e-8);
    CauchyMatrixAssert.AreEqual(cauchy[-1.0], expectedAtNegative, 1e-8);
  }

  [Test]
  public void NonMultipleInstant_UsesPartialRungeKuttaStepAndMatchesClosedFormForNilpotentMatrix() {
    Matrix a = new(new double[,] { { 0, 1 }, { 0, 0 } });
    CauchyMatrix cauchy = new(a, 0.0, 0.1);

    Matrix expected = new(new double[,] {
      { 1, -0.35 },
      { 0, 1 }
    });

    CauchyMatrixAssert.AreEqual(cauchy[0.35], expected, 1e-12);
  }

}
