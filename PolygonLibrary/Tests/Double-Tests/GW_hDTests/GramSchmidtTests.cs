using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.Double_Tests.GW_hDTests; 

[TestFixture]
public class GramSchmidtTests {

  /// <summary>
  /// Simplest test
  /// </summary>
  [Test]
  public void TestGramSchmidt_1() {
    Vector[] input    = { new Vector(new double[] { 1, 0 }), new Vector(new double[] { 0, 1 }) };
    Vector[] expected = { new Vector(new double[] { 1, 0 }), new Vector(new double[] { 0, 1 }) };

    List<Vector> output = Vector.GramSchmidt(input);

    // Assert
    Assert.That(output.Count, Is.EqualTo(expected.Length), $"Lengths don't match {output.Count} and {expected.Length}");
    for (int i = 0; i < expected.Length; i++) {
      Assert.That(expected[i].CompareTo(output[i]) == 0, Is.True, $"Don't match: {expected[i]} and {output[i]}!");
    }
  }

  /// <summary>
  /// 3D Int test
  /// </summary>
  [Test]
  public void TestGramSchmidt_2() {
    Vector[] input =
      {
        new Vector(new double[] { 1, 0, 0 })
      , new Vector(new double[] { 1, 1, 0 })
      , new Vector(new double[] { 1, 1, 1 })
      };

    Vector[] expected =
      {
        new Vector(new double[] { 1, 0, 0 })
      , new Vector(new double[] { 0, 1, 0 })
      , new Vector(new double[] { 0, 0, 1 })
      };

    List<Vector> output = Vector.GramSchmidt(input);

    Assert.That(output.Count, Is.EqualTo(expected.Length), $"Lengths don't match {output.Count} and {expected.Length}");
    for (int i = 0; i < expected.Length; i++) {
      Assert.That(expected[i].CompareTo(output[i]) == 0, Is.True, $"Don't match: {expected[i]} and {output[i]}!");
    }
  }

  /// <summary>
  /// 3x2 Double test
  /// </summary>
  [Test]
  public void TestGramSchmidt_3() {
    Vector[] input = { new Vector(new double[] { 1.5, -2.3, 0.7 }), new Vector(new double[] { 0.8, 1.2, -0.5 }) };

    Vector[] expected =
      {
        new Vector(new double[] { 0.529338504930439, -0.811652374226673, 0.247024635634205 })
      , new Vector(new double[] { 0.844642153633652, 0.476742255517412, -0.243508632514901 })
      };

    List<Vector> output = Vector.GramSchmidt(input);

    Assert.That(output.Count, Is.EqualTo(expected.Length), $"Lengths don't match {output.Count} and {expected.Length}");
    for (int i = 0; i < expected.Length; i++) {
      Assert.That(expected[i].CompareTo(output[i]) == 0, Is.True, $"Don't match: {expected[i]} and {output[i]}!");
    }
  }

  /// <summary>
  /// Linear dependent test
  /// </summary>
  [Test]
  public void TestGramSchmidt_4() {
    Vector[] input =
      {
        new Vector(new double[] { 1, 0, 0 })
      , new Vector(new double[] { 0, 1, 0 })
      , new Vector(new double[] { 0, 0, 1 })
      , new Vector(new double[] { 2, 3, -5 })
      , new Vector(new double[] { 7, 1, 5 })
      };

    Vector[] expected =
      {
        new Vector(new double[] { 1, 0, 0 })
      , new Vector(new double[] { 0, 1, 0 })
      , new Vector(new double[] { 0, 0, 1 })
      };

    List<Vector> output = Vector.GramSchmidt(input);

    Assert.That(output.Count, Is.EqualTo(expected.Length), $"Lengths don't match {output.Count} and {expected.Length}");
    for (int i = 0; i < expected.Length; i++) {
      Assert.That(expected[i].CompareTo(output[i]) == 0, Is.True, $"Don't match: {expected[i]} and {output[i]}!");
    }
  }
  
  
  /// <summary>
  /// Simple test
  /// </summary>
  [Test]
  public void TestGramSchmidt_TwoArg_1() {
    Vector[] orthonormal =
      {
        new Vector(new double[] { 1, 0, 0, 0 })
      , new Vector(new double[] { 0, 1, 0, 0 })
      };

    Vector[] Basis =
      {
        new Vector(new double[] { 1, 0, 0, 0 })
      , new Vector(new double[] { 1, 1, 0, 0 })
      , new Vector(new double[] { 1, 1, 1, 0 })
      , new Vector(new double[] { 1, 1, 1, 1 })
      };

    Vector[] expected =
      {
        new Vector(new double[] { 1, 0, 0, 0 })
      , new Vector(new double[] { 0, 1, 0, 0 })
      , new Vector(new double[] { 0, 0, 1, 0 })
      , new Vector(new double[] { 0, 0, 0, 1 })
      };

    List<Vector> output = Vector.GramSchmidt(orthonormal, Basis);

    Assert.That(output.Count, Is.EqualTo(expected.Length), $"Lengths don't match {output.Count} and {expected.Length}");
    for (int i = 0; i < expected.Length; i++) {
      Assert.That(expected[i].CompareTo(output[i]) == 0, Is.True, $"Don't match: {expected[i]} and {output[i]}!");
    }
  }

}