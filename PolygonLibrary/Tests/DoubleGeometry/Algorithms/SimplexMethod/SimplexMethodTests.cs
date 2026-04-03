using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.SharedTests.StaticHelpers;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class SimplexMethodTests {

  [Test]
  public void Solve_BoundedProblem_ReturnsOptimalValueSolutionAndActiveInequalities() {
    List<HyperPlane> hps =
      [
        new(V(1, 1), 4),   // x + y <= 4
        new(V(1, 0), 2),   // x <= 2
        new(V(0, 1), 3),   // y <= 3
        new(V(-1, 0), 0),  // x >= 0
        new(V(0, -1), 0)   // y >= 0
      ];

    SimplexMethod.SimplexMethodResult result = SimplexMethod.Solve(hps, i => i == 0 ? 3.0 : 2.0);

    Assert.Multiple(() => {
      Assert.That(result.Status, Is.EqualTo(SimplexMethod.SimplexMethodResultStatus.Ok));
      Assert.That(Tools.EQ(result.Value, 10.0), Is.True);
      Assert.That(result.Solution, Is.Not.Null);
      Assert.That(result.Solution!.Length, Is.EqualTo(2));
      AssertArraysAreEqual(new[] { 2.0, 2.0 }, result.Solution);
      Assert.That(result.ActiveInequalitiesID.ToHashSet().SetEquals([0, 1]), Is.True);
    });
  }

  [Test]
  public void Solve_InfeasibleProblem_ReturnsNoSolution() {
    List<HyperPlane> hps =
      [
        new(V(1), 0),    // x <= 0
        new(V(-1), -1)   // x >= 1
      ];

    SimplexMethod.SimplexMethodResult result = SimplexMethod.Solve(hps, _ => 1.0);

    Assert.Multiple(() => {
      Assert.That(result.Status, Is.EqualTo(SimplexMethod.SimplexMethodResultStatus.NoSolution));
      Assert.That(result.Solution, Is.Null);
      Assert.That(result.ActiveInequalitiesID, Is.Empty);
    });
  }

  [Test]
  public void Solve_UnboundedProblem_ReturnsUnlimited() {
    List<HyperPlane> hps =
      [
        new(V(-1), 0) // x >= 0
      ];

    SimplexMethod.SimplexMethodResult result = SimplexMethod.Solve(hps, _ => 1.0);

    Assert.Multiple(() => {
      Assert.That(result.Status, Is.EqualTo(SimplexMethod.SimplexMethodResultStatus.Unlimited));
      Assert.That(result.Solution, Is.Null);
      Assert.That(result.ActiveInequalitiesID, Is.Empty);
    });
  }

  [Test]
  public void Solve_FreeVariableProblem_CanReturnNegativeOriginalCoordinate() {
    List<HyperPlane> hps =
      [
        new(V(1), 1),   // x <= 1
        new(V(-1), 2)   // x >= -2
      ];

    SimplexMethod.SimplexMethodResult result = SimplexMethod.Solve(hps, _ => -1.0);

    Assert.Multiple(() => {
      Assert.That(result.Status, Is.EqualTo(SimplexMethod.SimplexMethodResultStatus.Ok));
      Assert.That(result.Solution, Is.Not.Null);
      Assert.That(result.Solution!.Length, Is.EqualTo(1));
      AssertArraysAreEqual(new[] { -2.0 }, result.Solution);
      Assert.That(Tools.EQ(result.Value, 2.0), Is.True);
    });
  }

}
