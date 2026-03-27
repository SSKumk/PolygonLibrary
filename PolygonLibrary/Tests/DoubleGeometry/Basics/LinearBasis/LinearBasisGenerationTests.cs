using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.LinearBasisAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class LinearBasisGenerationTests {

  [Test]
  public void GenLinearBasis_FullDim() {
    LinearBasis basis = LinearBasis.GenLinearBasis(4);
    Assert.That(basis.SpaceDim, Is.EqualTo(4));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(4));
    Assert.That(basis.FullDim, Is.True);
    IsOrthonormal(basis);
  }

  [Test]
  public void GenLinearBasis_PartialDim() {
    LinearBasis basis = LinearBasis.GenLinearBasis(5, 2);
    Assert.That(basis.SpaceDim, Is.EqualTo(5));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    Assert.That(basis.FullDim, Is.False);
    IsOrthonormal(basis);
  }

}
