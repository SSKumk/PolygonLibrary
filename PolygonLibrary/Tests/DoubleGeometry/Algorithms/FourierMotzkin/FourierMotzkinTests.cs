using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.SharedTests.StaticHelpers;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class FourierMotzkinTests {

  [Test]
  public void EliminateVariableNaive_BuildsCombinedInequalityAndPreservesNeutralConstraints() {
    HyperPlane upper = new(V(1, 1), 3);   // x + y <= 3
    HyperPlane lower = new(V(-1, 1), 1);  // -x + y <= 1
    HyperPlane neutral = new(V(0, 1), 2); // y <= 2

    FourierMotzkin fm = new([upper, lower, neutral]);
    FourierMotzkin eliminated = fm.EliminateVariableNaive(1);

    Assert.That(eliminated.HPs, Has.Count.EqualTo(2));
    Assert.That(eliminated.HPs.Any(hp => hp == neutral), Is.True, "Neutral inequality should be preserved as-is.");

    HyperPlane combined = eliminated.HPs.Single(hp => hp != neutral);
    Assert.Multiple(() => {
      Assert.That(Tools.EQ(combined.Normal[0]), Is.True);
      Assert.That(Tools.EQ(combined.Normal[1], 1.0), Is.True);
      Assert.That(Tools.EQ(combined.ConstantTerm, 2.0), Is.True);
    });
  }

  [Test]
  public void EliminateVariableNaive_UsesOneBasedVariableIndex() {
    HyperPlane upper = new(V(1, 1), 3);  // x + y <= 3
    HyperPlane lower = new(V(1, -1), 1); // x - y <= 1

    FourierMotzkin fm = new([upper, lower]);
    FourierMotzkin eliminated = fm.EliminateVariableNaive(2);

    Assert.That(eliminated.HPs, Has.Count.EqualTo(1));
    HyperPlane combined = eliminated.HPs[0];
    Assert.Multiple(() => {
      Assert.That(Tools.EQ(combined.Normal[0], 1.0), Is.True);
      Assert.That(Tools.EQ(combined.Normal[1]), Is.True);
      Assert.That(Tools.EQ(combined.ConstantTerm, 2.0), Is.True);
    });
  }

  [Test]
  public void EliminateVariableNaive_WithoutLowerOrUpperBounds_LeavesOnlyNeutralConstraints() {
    HyperPlane neutral1 = new(V(0, 1), 2);
    HyperPlane neutral2 = new(V(0, -1), 0);
    HyperPlane upper = new(V(1, 1), 3);

    FourierMotzkin fm = new([upper, neutral1, neutral2]);
    FourierMotzkin eliminated = fm.EliminateVariableNaive(1);

    Assert.Multiple(() => {
      Assert.That(eliminated.HPs, Has.Count.EqualTo(2));
      Assert.That(eliminated.HPs.Any(hp => hp == neutral1), Is.True);
      Assert.That(eliminated.HPs.Any(hp => hp == neutral2), Is.True);
    });
  }

  [Test]
  public void EliminateVariableNaive_SkipsZeroCombinedInequality() {
    HyperPlane upper = new(V(1), 1);   // x <= 1
    HyperPlane lower = new(V(-1), -1); // x >= 1

    FourierMotzkin fm = new([upper, lower]);
    FourierMotzkin eliminated = fm.EliminateVariableNaive(1);

    Assert.That(eliminated.HPs, Is.Empty);
  }

}
