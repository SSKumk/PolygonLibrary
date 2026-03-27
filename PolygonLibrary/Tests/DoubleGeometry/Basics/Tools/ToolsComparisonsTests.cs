using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class ToolsComparisonsTests {

  private double _savedEps;

  [SetUp]
  public void SetUp() {
    _savedEps = Tools.Eps;
  }

  [TearDown]
  public void TearDown() {
    Tools.Eps = _savedEps;
  }

  [Test]
  public void EpsSetter_UpdatesEpsAndDerivedEpsG() {
    Tools.Eps = 1e-6;

    Assert.Multiple(() => {
      Assert.That(Tools.Eps, Is.EqualTo(1e-6));
      Assert.That(Tools.EpsG, Is.EqualTo(1e-4));
    });
  }

  [Test]
  public void EqualityAndInequality_UseStrictBoundaryAtEps() {
    Tools.Eps = 1e-6;

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(0.5e-6), Is.True);
      Assert.That(Tools.EQ(-0.5e-6), Is.True);
      Assert.That(Tools.EQ(1e-6), Is.False);
      Assert.That(Tools.EQ(-1e-6), Is.False);
      Assert.That(Tools.EQ(1.0, 1.0 + 0.5e-6), Is.True);
      Assert.That(Tools.EQ(1.0, 1.0 + 1e-6), Is.False);
      Assert.That(Tools.NE(1.0, 1.0 + 0.5e-6), Is.False);
      Assert.That(Tools.NE(1.0, 1.0 + 1e-6), Is.True);
    });
  }

  [Test]
  public void OrderingPredicates_HandleInteriorAndBoundaryValues() {
    Tools.Eps = 1e-6;

    Assert.Multiple(() => {
      Assert.That(Tools.GT(0.5e-6), Is.False);
      Assert.That(Tools.GT(1.0e-6), Is.False);
      Assert.That(Tools.GT(1.5e-6), Is.True);
      Assert.That(Tools.LT(-0.5e-6), Is.False);
      Assert.That(Tools.LT(-1.0e-6), Is.False);
      Assert.That(Tools.LT(-1.5e-6), Is.True);
      Assert.That(Tools.GE(-1.0e-6), Is.True);
      Assert.That(Tools.GE(-1.5e-6), Is.False);
      Assert.That(Tools.LE(1.0e-6), Is.True);
      Assert.That(Tools.LE(1.5e-6), Is.False);
    });
  }

  [Test]
  public void CmpAndSign_AreConsistentWithTolerance() {
    Tools.Eps = 1e-6;

    Assert.Multiple(() => {
      Assert.That(Tools.CMP(0.5e-6), Is.EqualTo(0));
      Assert.That(Tools.CMP(2.0e-6), Is.EqualTo(+1));
      Assert.That(Tools.CMP(-2.0e-6), Is.EqualTo(-1));
      Assert.That(Tools.Sign(0.5e-6), Is.EqualTo(0));
      Assert.That(Tools.Sign(2.0e-6), Is.EqualTo(+1));
      Assert.That(Tools.Sign(-2.0e-6), Is.EqualTo(-1));
      Assert.That(Tools.CMP(10.0, 10.0 + 0.5e-6), Is.EqualTo(0));
      Assert.That(Tools.CMP(10.0, 10.0 + 2.0e-6), Is.EqualTo(-1));
      Assert.That(Tools.CMP(10.0 + 2.0e-6, 10.0), Is.EqualTo(+1));
    });
  }

  [Test]
  public void TNumComparer_UsesCurrentEpsValue() {
    Tools.Eps = 1e-3;

    Assert.That(Tools.TComp.Compare(1.0, 1.0005), Is.EqualTo(0));

    Tools.Eps = 1e-6;

    Assert.That(Tools.TComp.Compare(1.0, 1.0005), Is.EqualTo(-1));
  }

}
