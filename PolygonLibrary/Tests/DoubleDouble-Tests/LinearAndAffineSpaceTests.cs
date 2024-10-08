using DoubleDouble;
using NUnit.Framework;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;

namespace Tests.DoubleDouble_Tests;

[TestFixture]
public class LinearSpaceTests
{

  /// <summary>
  /// Simple test for the AddVector method of the LinearBasis class.
  /// </summary>
  [Test]
  public void AddVectorTest()
  {
    LinearBasis basis = new LinearBasis(2, 0);
    Assert.That(basis.Empty, Is.True, "Initial basis should be empty.");

    bool added1 = basis.AddVector(new Vector(new ddouble[] { 1, 0 }));

    Assert.That
      (basis.IsFullDim, Is.False, "The basis with a single vector is not full dimensional, while the space is two dimensional.");

    bool added2 = basis.AddVector(new Vector(new ddouble[] { 0, 1 }));
    bool added3 = basis.AddVector(new Vector(new ddouble[] { 1, 1 }));
    bool added4 = basis.AddVector(new Vector(new ddouble[] { 0, 0 }));

    Assert.That(added1, Is.True, "The addition of the linear independent vector should be successful.");
    Assert.That(added2, Is.True, "The addition of the linear independent vector should be successful.");
    Assert.That(added3, Is.False, "The addition of the linear dependent vector should not be successful.");
    Assert.That(added4, Is.False, "The addition of the zero vector should not be successful.");

    Assert.That(basis.SubSpaceDim, Is.EqualTo(2), "The count of vectors in the basis should be equal to 2.");
    Assert.That(basis.Empty, Is.False, "The basis should not be empty after adding vectors.");

    Assert.That(basis.IsFullDim, Is.True, "The basis should have full dimension after adding two linearly independent vectors.");

    Assert.That(basis.SpaceDim, Is.EqualTo(2), "The dimension of the basis should be equal to 2.");
  }

  /// <summary>
  /// Tests the constructor of the LinearBasis class that takes a collection of vectors as input.
  /// </summary>
  [Test]
  public void ConstructorWithVectorsTest()
  {
    List<Vector> vectors = new List<Vector>()
      {
        new Vector(new ddouble[] { 2, 0, 0 })
      , new Vector(new ddouble[] { 0, 3, 0 })
      , new Vector(new ddouble[] { 0, 0, 4 })
      , new Vector(new ddouble[] { -1, 1, 5.7412 })
      };

    LinearBasis basis = new LinearBasis(vectors);

    Assert.That
      (
       basis.SubSpaceDim
     , Is.EqualTo(3)
     , "The number of vectors in the basis should be equal to the number of linearly independent input vectors."
      );

    Assert.That(basis.SpaceDim, Is.EqualTo(3), "The dimension of the basis should be equal to the first vector dimension.");

    Assert.That
      (
       basis.IsFullDim
     , Is.True
     , "The basis should have full dimension if subset of the input vectors (d-dim) are forming d-basis."
      );

    Assert.That(basis.Empty, Is.False, "The basis should not be empty if at least one input vector are provided.");
  }

}

[TestFixture]
public class AffineSpaceTests
{

  /// <summary>
  /// Checks that all vectors in the basis have unit length.
  /// </summary>
  /// <param name="basis">Basis to be checked</param>
  private static void CheckVectorsLength(AffineBasis basis)
  {
    foreach (Vector bvec in basis.LinBasis)
    {
      Assert.That(Tools.CMP(bvec.Length, 1), Is.EqualTo(0), "All vectors in the basis must have an unit length.");
    }
  }

  /// <summary>
  /// Simple constructor test
  /// </summary>
  [Test]
  public void ConstructorWithOriginTest()
  {
    Vector origin = new Vector(new ddouble[] { 1, 2, 3 });

    AffineBasis basis = new AffineBasis(origin);

    Assert.That(basis.Origin.CompareTo(origin), Is.EqualTo(0), "The origin point of the affine basis should be set correctly.");
    Assert.That(basis, Is.Empty, "The linear basis associated with the affine basis should be initially empty.");
    Assert.That(basis.IsFullDim, Is.False, "The affine basis should not be full dimension if no vectors are added.");
    Assert.That(basis.IsEmpty, Is.True, "The affine basis should be empty if no vectors are added.");
  }

  /// <summary>
  /// Vector constructor test
  /// </summary>
  [Test]
  public void ConstructorWithOriginAndVectorsTest()
  {
    Vector origin = new Vector(new ddouble[] { 1, 2, 3 });

    List<Vector> vectors = new List<Vector>()
      {
        new Vector(new ddouble[] { 2, 0, 0 })
      , new Vector(new ddouble[] { 0, 6, 0 })
      , new Vector(new ddouble[] { 0, 0, 12.4455364363 })
      , new Vector(new ddouble[] { ddouble.PI, 5, ddouble.E })
      };


    AffineBasis basis = AffineBasis.FromVectors(origin, vectors);


    Assert.That(basis.Origin.CompareTo(origin), Is.EqualTo(0), "The origin point of the affine basis should be set correctly.");

    Assert.That
      (
       basis.SubSpaceDim
     , Is.EqualTo(3)
     , "The number of vectors in the basis should be equal to the number of linearly independent input vectors."
      );

    Assert.That
      (
       basis.IsFullDim
     , Is.True
     , "The affine basis should be full dimension if the input vectors (d-dim) are linearly independent and form the dD basis."
      );

    Assert.That(basis.IsEmpty, Is.False, "The affine basis should not be empty if input non zero vectors are provided.");

    AffineBasis.CheckCorrectness(basis);
  }


  [Test]
  public void ConstructorWithOriginAndPointsTest()
  {
    Vector origin = new Vector(new ddouble[] { -1, ddouble.PI, -3 });

    List<Vector> points = new List<Vector>()
      {
        new Vector(new ddouble[] { 2, 3, 4 })
      , new Vector(new ddouble[] { 3, 4, 5 })
      , new Vector(new ddouble[] { 4, 5, 6 })
      , new Vector(new ddouble[] { -6, -3, 0 })
      , new Vector(new ddouble[] { ddouble.PI / 2, 5, 5 })
      };

    AffineBasis basis = AffineBasis.FromPoints(origin, points);

    Assert.That(basis.Origin.CompareTo(origin), Is.EqualTo(0), "The origin point of the affine basis should be set correctly.");

    Assert.That
      (
       basis.SubSpaceDim
     , Is.EqualTo(3)
     , "The number of vectors in the basis should be equal to the number of linearly independent input vectors."
      );

    Assert.That(basis.IsFullDim, Is.True, "The affine basis should be full dimension if the input points span a linear space.");
    Assert.That(basis.IsEmpty, Is.False, "The affine basis should not be empty if non zero input points are provided.");

    AffineBasis.CheckCorrectness(basis);
  }

  /// <summary>
  /// Simple vector adding test
  /// </summary>
  [Test]
  public void AddVectorTest()
  {
    Vector origin = new Vector(new ddouble[] { 1, 2, 3 });

    List<Vector> vectors = new List<Vector>() { new Vector(new ddouble[] { 1, 0, 0 }), new Vector(new ddouble[] { 0, 1, 0 }) };

    AffineBasis basis = AffineBasis.FromVectors(origin, vectors, false);
    Vector newVector = new Vector(new ddouble[] { 1, 1, 1 });

    bool result = basis.AddVector(newVector);
    basis.AddVector(newVector);

    Vector newVector0 = new Vector(origin);
    bool result0 = basis.AddVector(newVector0);


    Assert.That(result, Is.True, "The vector should be added to the linear basis associated with the affine basis.");
    Assert.That(result0, Is.False, "The zero vector should not be added to the linear basis associated with the affine basis.");
    Assert.That(basis.SubSpaceDim, Is.EqualTo(3), "The linear basis associated with the affine basis should have one more vector.");
    Assert.That
      (basis.IsFullDim, Is.True, "The affine basis should be full dimension if the current basis vectors span a linear space.");
    Assert.That(basis.IsEmpty, Is.False, "The affine basis should not be empty if input non zero vectors are provided.");

    AffineBasis.CheckCorrectness(basis);
  }


  /// <summary>
  /// Simple test
  /// </summary>
  [Test]
  public void ProjectToAffineSpace_1()
  {
    Vector origin = new Vector(new ddouble[] { 0, 0 });

    List<Vector> basis = new List<Vector> { new Vector(new ddouble[] { 1, 0 }), new Vector(new ddouble[] { 0, 1 }) };

    SortedSet<Vector> swarm = new SortedSet<Vector>
      {
        new Vector(new ddouble[] { 1, 1 }), new Vector(new ddouble[] { 2, 3 }), new Vector(new ddouble[] { -1, 4 })
      };

    SortedSet<Vector> expected = swarm;

    AffineBasis aBasis = AffineBasis.FromVectors(origin, basis);
    IEnumerable<Vector> result = aBasis.ProjectPoints(swarm);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }


  /// <summary>
  /// Reflect simple test
  /// </summary>
  [Test]
  public void ProjectToAffineSpace_2()
  {
    Vector origin = new Vector(new ddouble[] { 0, 0 });

    List<Vector> basis = new List<Vector> { new Vector(new ddouble[] { -1, 0 }), new Vector(new ddouble[] { 0, -1 }) };

    SortedSet<Vector> swarm = new SortedSet<Vector>
      {
        new Vector(new ddouble[] { 1, 1 }), new Vector(new ddouble[] { 2, 3 }), new Vector(new ddouble[] { -1, 4 })
      };

    SortedSet<Vector> expected = new SortedSet<Vector>
      {
        new Vector(new ddouble[] { -1, -1 }), new Vector(new ddouble[] { -2, -3 }), new Vector(new ddouble[] { 1, -4 })
      };

    AffineBasis aBasis = AffineBasis.FromVectors(origin, basis);
    IEnumerable<Vector> result = aBasis.ProjectPoints(swarm);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }


  /// <summary>
  /// Reflect and shift simple test
  /// </summary>
  [Test]
  public void ProjectToAffineSpace_3()
  {
    Vector origin = new Vector(new ddouble[] { 2, 2 });

    List<Vector> basis = new List<Vector> { new Vector(new ddouble[] { -1, 0 }), new Vector(new ddouble[] { 0, -1 }) };

    SortedSet<Vector> swarm = new SortedSet<Vector>
      {
        new Vector(new ddouble[] { 1, 1 }), new Vector(new ddouble[] { 2, 4 }), new Vector(new ddouble[] { -4, 4 })
      };

    SortedSet<Vector> expected = new SortedSet<Vector>
      {
        new Vector(new ddouble[] { 1, 1 }), new Vector(new ddouble[] { 0, -2 }), new Vector(new ddouble[] { 6, -2 })
      };

    AffineBasis aBasis = AffineBasis.FromVectors(origin, basis);
    IEnumerable<Vector> result = aBasis.ProjectPoints(swarm);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }

  /// <summary>
  /// 4D-test
  /// </summary>
  [Test]
  public void ProjectToAffineSpace_4()
  {
    Vector origin = new Vector(new ddouble[] { 0, 0, 0, 0 });

    List<Vector> basis = new List<Vector> { new Vector(new ddouble[] { 1, 0, 0, 0 }), new Vector(new ddouble[] { 0, 1, 0, 0 }) };

    SortedSet<Vector> swarm = new SortedSet<Vector>
      {
        new Vector(new ddouble[] { 0, 0, 0, 0 })
      , new Vector(new ddouble[] { 1, 0, 0, 0 })
      , new Vector(new ddouble[] { 0, 1, 0, 0 })
      , new Vector(new ddouble[] { 1, 1, 1, 1 })
      };

    SortedSet<Vector> expected = new SortedSet<Vector>
      {
        new Vector(new ddouble[] { 0, 0 })
      , new Vector(new ddouble[] { 1, 0 })
      , new Vector(new ddouble[] { 0, 1 })
      , new Vector(new ddouble[] { 1, 1 })
      };

    AffineBasis aBasis = AffineBasis.FromVectors(origin, basis);
    IEnumerable<Vector> result = aBasis.ProjectPoints(swarm);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }

}
