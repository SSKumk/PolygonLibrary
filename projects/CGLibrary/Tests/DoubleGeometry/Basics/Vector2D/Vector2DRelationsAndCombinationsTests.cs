using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class Vector2DRelationsAndCombinationsTests {

  [Test]
  public void ParallelDirectionAndOrthogonalityRelations_WorkAsExpected() {
    Vector2D e1 = Vector2D.E1;
    Vector2D same = new Vector2D(2.0, 0.0);
    Vector2D opposite = new Vector2D(-3.0, 0.0);
    Vector2D orthogonal = Vector2D.E2;

    Assert.Multiple(() => {
      Assert.That(Vector2D.AreParallel(e1, same), Is.True);
      Assert.That(Vector2D.AreParallel(e1, opposite), Is.True);
      Assert.That(Vector2D.AreParallel(e1, orthogonal), Is.False);
      Assert.That(Vector2D.AreCodirected(e1, same), Is.True);
      Assert.That(Vector2D.AreCodirected(e1, opposite), Is.False);
      Assert.That(Vector2D.AreCounterdirected(e1, opposite), Is.True);
      Assert.That(Vector2D.AreCounterdirected(e1, same), Is.False);
      Assert.That(Vector2D.AreOrthogonal(e1, orthogonal), Is.True);
      Assert.That(Vector2D.AreOrthogonal(Vector2D.Zero, same), Is.True);
    });
  }

  [Test]
  public void IsBetweenTest() {
    double pi = Tools.PI;
    Vector2D
      v0 = Vector2D.FromPolar(-pi, 1)
    , v1 = Vector2D.FromPolar(-7 * pi / 8, 1)
    , v2 = Vector2D.FromPolar(-3 * pi / 4, 1)
    , v3 = Vector2D.FromPolar(-pi / 2, 1)
    , v4 = Vector2D.FromPolar(-pi / 4, 1)
    , v5 = Vector2D.FromPolar(0, 1)
    , v6 = Vector2D.FromPolar(pi / 4, 1)
    , v7 = Vector2D.FromPolar(pi / 2, 1)
    , v8 = Vector2D.FromPolar(3 * pi / 4, 1)
    , v9 = Vector2D.FromPolar(7 * pi / 8, 1)
      ;

    Assert.Multiple(() => {
                      //===========================================================
                      // TRUE tests

                      //-------------------------------------
                      // Small cones
                      // Group 1: Small cone, the first vector has the polar angle pi, the second has the polar angle less than 0
                      Assert.That(v1.IsBetween(v0, v2), "Group 1: Test #1");
                      Assert.That(v1.IsBetween(v0, v3), "Group 1: Test #2");
                      Assert.That(v1.IsBetween(v0, v4), "Group 1: Test #3");
                      Assert.That(v3.IsBetween(v0, v4), "Group 1: Test #4");

                      // Group 2: Small cone, both vectors have the polar angle less than 0
                      Assert.That(v2.IsBetween(v1, v3), "Group 2: Test #1");
                      Assert.That(v2.IsBetween(v1, v4), "Group 2: Test #2");
                      Assert.That(v2.IsBetween(v1, v5), "Group 2: Test #3");

                      // Group 3: Small cone, the first vector has a negative polar angle, the second has a positive polar angle
                      Assert.That(v4.IsBetween(v3, v6), "Group 3: Test #1");
                      Assert.That(v5.IsBetween(v3, v6), "Group 3: Test #2");

                      // Group 4: Small cone, the first vector has the polar angle 0, the second has a positive polar angle
                      Assert.That(v6.IsBetween(v5, v7), "Group 4: Test #1");
                      Assert.That(v6.IsBetween(v5, v8), "Group 4: Test #2");
                      Assert.That(v7.IsBetween(v5, v8), "Group 4: Test #3");
                      Assert.That(v6.IsBetween(v5, v9), "Group 4: Test #4");
                      Assert.That(v7.IsBetween(v5, v9), "Group 4: Test #5");
                      Assert.That(v8.IsBetween(v5, v9), "Group 4: Test #6");

                      // Group 5: Small cone, both vectors have a positive polar angle
                      Assert.That(v7.IsBetween(v6, v8), "Group 5: Test #1");
                      Assert.That(v7.IsBetween(v6, v0), "Group 5: Test #2");
                      Assert.That(v8.IsBetween(v7, v9), "Group 5: Test #3");
                      Assert.That(v8.IsBetween(v7, v0), "Group 5: Test #4");

                      // Group 6: Small cone, the first vector has a positive polar angle, the second has a negative polar angle
                      Assert.That(v9.IsBetween(v7, v2), "Group 6: Test #1");
                      Assert.That(v0.IsBetween(v7, v2), "Group 6: Test #2");
                      Assert.That(v1.IsBetween(v7, v2), "Group 6: Test #3");
                      Assert.That(v9.IsBetween(v8, v3), "Group 6: Test #4");
                      Assert.That(v0.IsBetween(v8, v3), "Group 6: Test #5");
                      Assert.That(v1.IsBetween(v8, v3), "Group 6: Test #6");

                      //-------------------------------------
                      // Flat cones
                      // Group 7: Flat cone, the first vector has the polar angle pi, the second has the polar angle 0
                      Assert.That(v1.IsBetween(v0, v5), "Group 7: Test #1");
                      Assert.That(v3.IsBetween(v0, v5), "Group 7: Test #2");
                      Assert.That(v4.IsBetween(v0, v5), "Group 7: Test #3");

                      // Group 8: Flat cone, the first vector has negative polar angle
                      Assert.That(v3.IsBetween(v2, v6), "Group 8: Test #1");
                      Assert.That(v4.IsBetween(v2, v6), "Group 8: Test #2");
                      Assert.That(v5.IsBetween(v2, v6), "Group 8: Test #3");
                      Assert.That(v4.IsBetween(v3, v7), "Group 8: Test #4");
                      Assert.That(v5.IsBetween(v3, v7), "Group 8: Test #5");
                      Assert.That(v6.IsBetween(v3, v7), "Group 8: Test #6");

                      // Group 9: Flat cone, the first vector has the polar angle 0, the second has the polar angle pi
                      Assert.That(v6.IsBetween(v5, v0), "Group 9: Test #1");
                      Assert.That(v7.IsBetween(v5, v0), "Group 9: Test #2");
                      Assert.That(v9.IsBetween(v5, v0), "Group 9: Test #3");

                      // Group 10: Flat cone, the first vector has positive polar angle
                      Assert.That(v7.IsBetween(v6, v2), "Group 10: Test #1");
                      Assert.That(v8.IsBetween(v6, v2), "Group 10: Test #2");
                      Assert.That(v9.IsBetween(v6, v2), "Group 10: Test #3");
                      Assert.That(v0.IsBetween(v6, v2), "Group 10: Test #4");
                      Assert.That(v1.IsBetween(v6, v2), "Group 10: Test #5");
                      Assert.That(v8.IsBetween(v7, v3), "Group 10: Test #6");
                      Assert.That(v9.IsBetween(v7, v3), "Group 10: Test #7");
                      Assert.That(v0.IsBetween(v7, v3), "Group 10: Test #8");
                      Assert.That(v1.IsBetween(v7, v3), "Group 10: Test #9");
                      Assert.That(v2.IsBetween(v7, v3), "Group 10: Test #10");

                      //-------------------------------------
                      // Large cones

                      // Group 11: Large cone with angle less than 3*pi/2, the first vector has the polar angle pi
                      Assert.That(v1.IsBetween(v0, v6), "Group 11: Test #1");
                      Assert.That(v2.IsBetween(v0, v6), "Group 11: Test #2");
                      Assert.That(v3.IsBetween(v0, v6), "Group 11: Test #3");
                      Assert.That(v4.IsBetween(v0, v6), "Group 11: Test #4");
                      Assert.That(v5.IsBetween(v0, v6), "Group 11: Test #5");

                      // Group 12: Large cone with angle less than 3*pi/2, the first vector has negative polar angle
                      Assert.That(v3.IsBetween(v2, v7), "Group 12: Test #1");
                      Assert.That(v4.IsBetween(v2, v7), "Group 12: Test #2");
                      Assert.That(v5.IsBetween(v2, v7), "Group 12: Test #3");
                      Assert.That(v6.IsBetween(v2, v7), "Group 12: Test #4");

                      // Group 13: Large cone with angle less than 3*pi/2, the first vector has the polar angle -pi/2
                      Assert.That(v4.IsBetween(v3, v9), "Group 13: Test #1");
                      Assert.That(v5.IsBetween(v3, v9), "Group 13: Test #2");
                      Assert.That(v6.IsBetween(v3, v9), "Group 13: Test #3");
                      Assert.That(v7.IsBetween(v3, v9), "Group 13: Test #4");
                      Assert.That(v8.IsBetween(v3, v9), "Group 13: Test #5");

                      // Group 14: Large cone with angle less than 3*pi/2, the first vector has the polar angle 0
                      Assert.That(v6.IsBetween(v5, v1), "Group 14: Test #1");
                      Assert.That(v7.IsBetween(v5, v1), "Group 14: Test #2");
                      Assert.That(v8.IsBetween(v5, v1), "Group 14: Test #3");
                      Assert.That(v9.IsBetween(v5, v1), "Group 14: Test #4");
                      Assert.That(v0.IsBetween(v5, v1), "Group 14: Test #5");

                      // Group 15: Large cone with angle equal to 3*pi/2, the first vector has the polar angle 0
                      Assert.That(v1.IsBetween(v0, v7), "Group 15: Test #1");
                      Assert.That(v2.IsBetween(v0, v7), "Group 15: Test #2");
                      Assert.That(v3.IsBetween(v0, v7), "Group 15: Test #3");
                      Assert.That(v4.IsBetween(v0, v7), "Group 15: Test #4");
                      Assert.That(v5.IsBetween(v0, v7), "Group 15: Test #5");
                      Assert.That(v6.IsBetween(v0, v7), "Group 15: Test #6");

                      // Group 16: Large cone with angle equal to 3*pi/2, the first vector has negative polar angle
                      Assert.That(v3.IsBetween(v2, v8), "Group 16: Test #1");
                      Assert.That(v4.IsBetween(v2, v8), "Group 16: Test #2");
                      Assert.That(v5.IsBetween(v2, v8), "Group 16: Test #3");
                      Assert.That(v6.IsBetween(v2, v8), "Group 16: Test #4");
                      Assert.That(v7.IsBetween(v2, v8), "Group 16: Test #5");

                      // Group 17: Large cone with angle equal to 3*pi/2, the first vector has the polar angle -pi/2
                      Assert.That(v4.IsBetween(v3, v0), "Group 17: Test #1");
                      Assert.That(v5.IsBetween(v3, v0), "Group 17: Test #2");
                      Assert.That(v6.IsBetween(v3, v0), "Group 17: Test #3");
                      Assert.That(v7.IsBetween(v3, v0), "Group 17: Test #4");
                      Assert.That(v8.IsBetween(v3, v0), "Group 17: Test #5");
                      Assert.That(v9.IsBetween(v3, v0), "Group 17: Test #6");

                      // Group 18: Large cone with angle equal to 3*pi/2, the first vector has the polar angle 0
                      Assert.That(v6.IsBetween(v5, v3), "Group 18: Test #1");
                      Assert.That(v7.IsBetween(v5, v3), "Group 18: Test #2");
                      Assert.That(v8.IsBetween(v5, v3), "Group 18: Test #3");
                      Assert.That(v9.IsBetween(v5, v3), "Group 18: Test #4");
                      Assert.That(v0.IsBetween(v5, v3), "Group 18: Test #5");
                      Assert.That(v1.IsBetween(v5, v3), "Group 18: Test #6");
                      Assert.That(v2.IsBetween(v5, v3), "Group 18: Test #7");

                      // Group 19: Large cone with angle equal to 3*pi/2, the first vector has positive polar angle
                      Assert.That(v7.IsBetween(v6, v4), "Group 19: Test #1");
                      Assert.That(v8.IsBetween(v6, v4), "Group 19: Test #2");
                      Assert.That(v9.IsBetween(v6, v4), "Group 19: Test #3");
                      Assert.That(v0.IsBetween(v6, v4), "Group 19: Test #4");
                      Assert.That(v1.IsBetween(v6, v4), "Group 19: Test #5");
                      Assert.That(v2.IsBetween(v6, v4), "Group 19: Test #6");
                      Assert.That(v3.IsBetween(v6, v4), "Group 19: Test #7");

                      // Group 20: Large cone with angle equal to 3*pi/2, the first vector has the polar angle pi/2
                      Assert.That(v8.IsBetween(v7, v5), "Group 20: Test #1");
                      Assert.That(v9.IsBetween(v7, v5), "Group 20: Test #2");
                      Assert.That(v0.IsBetween(v7, v5), "Group 20: Test #3");
                      Assert.That(v1.IsBetween(v7, v5), "Group 20: Test #4");
                      Assert.That(v2.IsBetween(v7, v5), "Group 20: Test #5");
                      Assert.That(v3.IsBetween(v7, v5), "Group 20: Test #6");
                      Assert.That(v4.IsBetween(v7, v5), "Group 20: Test #7");

                      // Group 21: Large cone with angle greater than 3*pi/2, the first vector has the polar angle 0
                      Assert.That(v1.IsBetween(v0, v8), "Group 21: Test #1");
                      Assert.That(v2.IsBetween(v0, v8), "Group 21: Test #2");
                      Assert.That(v3.IsBetween(v0, v8), "Group 21: Test #3");
                      Assert.That(v4.IsBetween(v0, v8), "Group 21: Test #4");
                      Assert.That(v5.IsBetween(v0, v8), "Group 21: Test #5");
                      Assert.That(v6.IsBetween(v0, v8), "Group 21: Test #6");
                      Assert.That(v7.IsBetween(v0, v8), "Group 21: Test #7");

                      Assert.That(v1.IsBetween(v0, v9), "Group 21: Test #8");
                      Assert.That(v2.IsBetween(v0, v9), "Group 21: Test #9");
                      Assert.That(v3.IsBetween(v0, v9), "Group 21: Test #10");
                      Assert.That(v4.IsBetween(v0, v9), "Group 21: Test #11");
                      Assert.That(v5.IsBetween(v0, v9), "Group 21: Test #12");
                      Assert.That(v6.IsBetween(v0, v9), "Group 21: Test #13");
                      Assert.That(v7.IsBetween(v0, v9), "Group 21: Test #14");
                      Assert.That(v8.IsBetween(v0, v9), "Group 21: Test #15");

                      // Group 22: Large cone with angle greater than 3*pi/2, the first vector has negative polar angle, the second one has positive
                      Assert.That(v2.IsBetween(v1, v9), "Group 22: Test #1");
                      Assert.That(v3.IsBetween(v1, v9), "Group 22: Test #2");
                      Assert.That(v4.IsBetween(v1, v9), "Group 22: Test #3");
                      Assert.That(v5.IsBetween(v1, v9), "Group 22: Test #4");
                      Assert.That(v6.IsBetween(v1, v9), "Group 22: Test #5");
                      Assert.That(v7.IsBetween(v1, v9), "Group 22: Test #6");
                      Assert.That(v8.IsBetween(v1, v9), "Group 22: Test #7");

                      // Group 23: Large cone with angle greater than 3*pi/2, the first vector has negative polar angle, the second one has 0
                      Assert.That(v2.IsBetween(v1, v0), "Group 23: Test #1");
                      Assert.That(v3.IsBetween(v1, v0), "Group 23: Test #2");
                      Assert.That(v4.IsBetween(v1, v0), "Group 23: Test #3");
                      Assert.That(v5.IsBetween(v1, v0), "Group 23: Test #4");
                      Assert.That(v6.IsBetween(v1, v0), "Group 23: Test #5");
                      Assert.That(v7.IsBetween(v1, v0), "Group 23: Test #6");
                      Assert.That(v8.IsBetween(v1, v0), "Group 23: Test #7");
                      Assert.That(v9.IsBetween(v1, v0), "Group 23: Test #8");

                      // Group 24: Large cone with angle greater than 3*pi/2, the first vector has the polar angle -pi/2
                      Assert.That(v4.IsBetween(v3, v2), "Group 24: Test #1");
                      Assert.That(v5.IsBetween(v3, v2), "Group 24: Test #2");
                      Assert.That(v6.IsBetween(v3, v2), "Group 24: Test #3");
                      Assert.That(v7.IsBetween(v3, v2), "Group 24: Test #4");
                      Assert.That(v8.IsBetween(v3, v2), "Group 24: Test #5");
                      Assert.That(v9.IsBetween(v3, v2), "Group 24: Test #6");
                      Assert.That(v0.IsBetween(v3, v2), "Group 24: Test #7");
                      Assert.That(v1.IsBetween(v3, v2), "Group 24: Test #8");

                      // Group 25: Large cone with angle greater than 3*pi/2, the first vector has the polar angle 0
                      Assert.That(v6.IsBetween(v5, v4), "Group 25: Test #1");
                      Assert.That(v7.IsBetween(v5, v4), "Group 25: Test #2");
                      Assert.That(v8.IsBetween(v5, v4), "Group 25: Test #3");
                      Assert.That(v9.IsBetween(v5, v4), "Group 25: Test #4");
                      Assert.That(v0.IsBetween(v5, v4), "Group 25: Test #5");
                      Assert.That(v1.IsBetween(v5, v4), "Group 25: Test #6");
                      Assert.That(v2.IsBetween(v5, v4), "Group 25: Test #7");
                      Assert.That(v3.IsBetween(v5, v4), "Group 25: Test #8");

                      // Group 26: Large cone with angle greater than 3*pi/2, both vectors have positive polar angles
                      Assert.That(v9.IsBetween(v8, v6), "Group 26: Test #1");
                      Assert.That(v0.IsBetween(v8, v6), "Group 26: Test #2");
                      Assert.That(v1.IsBetween(v8, v6), "Group 26: Test #3");
                      Assert.That(v2.IsBetween(v8, v6), "Group 26: Test #4");
                      Assert.That(v3.IsBetween(v8, v6), "Group 26: Test #5");
                      Assert.That(v4.IsBetween(v8, v6), "Group 26: Test #6");
                      Assert.That(v5.IsBetween(v8, v6), "Group 26: Test #7");

                      //===========================================================
                      // FALSE tests
                      // Representative boundary and outside-the-cone cases from the legacy suite
                      Assert.That(v0.IsBetween(v0, v2), Is.False, "Group 101: Test #1");
                      Assert.That(v2.IsBetween(v0, v2), Is.False, "Group 101: Test #2");
                      Assert.That(v1.IsBetween(v1, v3), Is.False, "Group 102: Test #1");
                      Assert.That(v3.IsBetween(v1, v3), Is.False, "Group 102: Test #2");
                      Assert.That(v3.IsBetween(v0, v2), Is.False, "Group 201: Test #1");
                      Assert.That(v4.IsBetween(v0, v2), Is.False, "Group 201: Test #2");
                      Assert.That(v7.IsBetween(v3, v6), Is.False, "Group 203: Test #1");
                      Assert.That(v8.IsBetween(v3, v6), Is.False, "Group 203: Test #2");
                      Assert.That(v1.IsBetween(v5, v0), Is.False, "Group 209: Test #1");
                      Assert.That(v2.IsBetween(v5, v0), Is.False, "Group 209: Test #2");
                      Assert.That(v8.IsBetween(v0, v7), Is.False, "Group 215: Test #1");
                      Assert.That(v9.IsBetween(v0, v7), Is.False, "Group 215: Test #2");
                      Assert.That(v9.IsBetween(v0, v8), Is.False, "Group 221: Test #1");
                      Assert.That(v7.IsBetween(v8, v6), Is.False, "Group 223: Test #1");
                    });
  }

  [Test]
  public void ExplicitCastFromVector_PreservesCoordinates() {
    Vector source = new Vector(new[] { 2.5, -4.0 });
    Vector2D converted = (Vector2D)source;

    Vector2DAssert.AreEqual(converted, new Vector2D(2.5, -4.0));
  }

  [Test]
  public void LinearCombinationOfTwoThreeAndEnumerablePoints_WorksAsImplemented() {
    Vector2D p1 = new Vector2D(1.0, 0.0);
    Vector2D p2 = new Vector2D(0.0, 2.0);
    Vector2D p3 = new Vector2D(-1.0, 1.0);

    Vector2D twoPoint = Vector2D.LinearCombination(p1, 2.0, p2, -0.5);
    Vector2D threePoint = Vector2D.LinearCombination(p1, 2.0, p2, -0.5, p3, 3.0);
    Vector2D enumerable = Vector2D.LinearCombination(new[] { p1, p2, p3 }, new[] { 2.0, -0.5, 3.0 });

    Assert.Multiple(() => {
      Vector2DAssert.AreEqual(twoPoint, new Vector2D(2.0, -1.0));
      Vector2DAssert.AreEqual(threePoint, new Vector2D(-1.0, 2.0));
      Vector2DAssert.AreEqual(enumerable, threePoint);
    });
  }

}
