using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

public static class LinearBasisAssert {

  public static void IsOrthonormal(LinearBasis basis) {
    if (basis.Empty) {
      return;
    }

    for (int i = 0; i < basis.SubSpaceDim; i++) {
      Assert.That(basis[i].Length, Is.EqualTo(1.0).Within(Tools.Eps), $"Basis vector {i} is not normalized.");
      for (int j = i + 1; j < basis.SubSpaceDim; j++) {
        Assert.That(basis[i] * basis[j], Is.EqualTo(0.0).Within(Tools.Eps), $"Basis vectors {i} and {j} are not orthogonal.");
      }
    }

#if DEBUG
    Assert.DoesNotThrow(basis.CheckCorrectness, "CheckCorrectness failed.");
#endif
  }

  public static void AreEqual(LinearBasis actual, LinearBasis expected, string message = "") {
    Assert.That(actual.SpaceDim, Is.EqualTo(expected.SpaceDim), $"Space dimensions differ. {message}");
    Assert.That(actual.SubSpaceDim, Is.EqualTo(expected.SubSpaceDim), $"Subspace dimensions differ. {message}");
    for (int i = 0; i < actual.SubSpaceDim; i++) {
      VectorAssert.AreEqual(actual[i], expected[i], message);
    }
  }

}
