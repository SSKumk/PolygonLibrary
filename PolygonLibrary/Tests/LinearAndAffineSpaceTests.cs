using NUnit.Framework;
using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

namespace Tests;

[TestFixture]
public class LinearSpaceTests {

  /// <summary>
  /// Simple test for the AddVector method of the LinearBasis class.
  /// </summary>
  [Test]
  public void AddVectorTest() {
    LinearBasis basis = new LinearBasis();
    Assert.That(basis.IsEmpty, Is.True, "Initial basis should be empty.");

    bool added1 = basis.AddVector(new Vector(new double[] { 1, 0 }));

    Assert.That(basis.IsFullDim, Is.False, "The basis with a single vector is not full dimensional, while the space is two dimensional.");

    bool added2 = basis.AddVector(new Vector(new double[] { 0, 1 }));
    bool added3 = basis.AddVector(new Vector(new double[] { 1, 1 }));
    bool added4 = basis.AddVector(new Vector(new double[] { 0, 0 }));

    Assert.That(added1, Is.True, "The addition of the linear independent vector should be successful.");
    Assert.That(added2, Is.True, "The addition of the linear independent vector should be successful.");
    Assert.That(added3, Is.False, "The addition of the linear dependent vector should not be successful.");
    Assert.That(added4, Is.False, "The addition of the zero vector should not be successful.");

    Assert.That(basis.Count, Is.EqualTo(2), "The count of vectors in the basis should be equal to 2.");
    Assert.That(basis.IsEmpty, Is.False, "The basis should not be empty after adding vectors.");

    Assert.That(basis.IsFullDim, Is.True, "The basis should have full dimension after adding two linearly independent vectors.");

    Assert.That(basis.Dim, Is.EqualTo(2), "The dimension of the basis should be equal to 2.");
  }

  /// <summary>
  /// Tests the constructor of the LinearBasis class that takes a collection of vectors as input.
  /// </summary>
  [Test]
  public void ConstructorWithVectorsTest() {
    List<Vector> vectors = new List<Vector>()
      {
        new Vector(new double[] { 2, 0, 0 })
      , new Vector(new double[] { 0, 3, 0 })
      , new Vector(new double[] { 0, 0, 4 })
      , new Vector(new double[] { -1, 1, 5.7412 })
      };

    LinearBasis basis = new LinearBasis(vectors);

    Assert.That
      (
       basis.Count
     , Is.EqualTo(3)
     , "The number of vectors in the basis should be equal to the number of linearly independent input vectors."
      );

    Assert.That(basis.Dim, Is.EqualTo(3), "The dimension of the basis should be equal to the first vector dimension.");

    Assert.That
      (basis.IsFullDim, Is.True, "The basis should have full dimension if subset of the input vectors (d-dim) are forming d-basis.");

    Assert.That(basis.IsEmpty, Is.False, "The basis should not be empty if at least one input vector are provided.");
  }

  /// <summary>
  /// Simple test for the Expansion method of the LinearBasis class.
  /// </summary>
  [Test]
  public void ExpansionTest() {
    List<Vector> vectors = new List<Vector>()
      {
        new Vector(new double[] { 1, 0, 0 })
      , new Vector(new double[] { 0, 1, 0 })
      , new Vector(new double[] { 0, 0, 1 })
      };

    LinearBasis basis = new LinearBasis(vectors);
    Vector      v     = new Vector(new double[] { -2, 3, double.Pi * 2 });

    Vector expansion = basis.Expansion(v);

    Assert.That(v.CompareTo(expansion), Is.EqualTo(0), "Wrong expansion of a vector in the affine basis.");
  }

}

[TestFixture]
public class AffineSpaceTests {

  /// <summary>
  /// Checks that all vectors in the basis have unit length.
  /// </summary>
  /// <param name="basis">Basis to be checked</param>
  private static void CheckVectorsLength(AffineBasis basis) {
    foreach (Vector bvec in basis.Basis) {
      Assert.That(Tools.CMP(bvec.Length, 1), Is.EqualTo(0), "All vectors in the basis must have an unit length.");
    }
  }

  /// <summary>
  /// Simple constructor test
  /// </summary>
  [Test]
  public void ConstructorWithOriginTest() {
    Point origin = new Point(new double[] { 1, 2, 3 });

    AffineBasis basis = new AffineBasis(origin);

    Assert.That(basis.Origin.CompareTo(origin), Is.EqualTo(0), "The origin point of the affine basis should be set correctly.");
    Assert.That(basis, Is.Empty, "The linear basis associated with the affine basis should be initially empty.");
    Assert.That(basis.FullDim, Is.False, "The affine basis should not be full dimension if no vectors are added.");
    Assert.That(basis.IsEmpty, Is.True, "The affine basis should be empty if no vectors are added.");
  }

  /// <summary>
  /// Vector constructor test
  /// </summary>
  [Test]
  public void ConstructorWithOriginAndVectorsTest() {
    Point origin = new Point(new double[] { 1, 2, 3 });

    List<Vector> vectors = new List<Vector>()
      {
        new Vector(new double[] { 2, 0, 0 })
      , new Vector(new double[] { 0, 6, 0 })
      , new Vector(new double[] { 0, 0, 12.4455364363 })
      , new Vector(new double[] { double.Pi, 5, double.E })
      };


    AffineBasis basis = new AffineBasis(origin, vectors);


    Assert.That(basis.Origin.CompareTo(origin), Is.EqualTo(0), "The origin point of the affine basis should be set correctly.");

    Assert.That
      (
       basis
     , Has.Count.EqualTo(3)
     , "The number of vectors in the basis should be equal to the number of linearly independent input vectors."
      );

    Assert.That
      (
       basis.FullDim
     , Is.True
     , "The affine basis should be full dimension if the input vectors (d-dim) are linearly independent and form the dD basis."
      );

    Assert.That(basis.IsEmpty, Is.False, "The affine basis should not be empty if input non zero vectors are provided.");

    AffineBasis.CheckCorrectness(basis);
  }


  /// <summary>
  /// Point constructor test
  /// </summary>
  [Test]
  public void ConstructorWithOriginAndPointsTest() {
    Point origin = new Point(new double[] { -1, double.Pi, -3 });

    List<Point> points = new List<Point>()
      {
        new Point(new double[] { 2, 3, 4 })
      , new Point(new double[] { 3, 4, 5 })
      , new Point(new double[] { 4, 5, 6 })
      , new Point(new double[] { -6, -3, 0 })
      , new Point(new double[] { double.Pi / 2, 5, 5 })
      };

    AffineBasis basis = new AffineBasis(origin, points);

    Assert.That(basis.Origin.CompareTo(origin), Is.EqualTo(0), "The origin point of the affine basis should be set correctly.");

    Assert.That
      (
       basis
     , Has.Count.EqualTo(3)
     , "The number of vectors in the basis should be equal to the number of linearly independent input vectors."
      );

    Assert.That(basis.FullDim, Is.True, "The affine basis should be full dimension if the input points span a linear space.");
    Assert.That(basis.IsEmpty, Is.False, "The affine basis should not be empty if non zero input points are provided.");

    AffineBasis.CheckCorrectness(basis);
  }

  /// <summary>
  /// Simple vector adding test
  /// </summary>
  [Test]
  public void AddVectorToBasisTest() {
    Point origin = new Point(new double[] { 1, 2, 3 });

    List<Vector> vectors = new List<Vector>()
      {
        new Vector(new double[] { 1, 0, 0 })
      , new Vector(new double[] { 0, 1, 0 })
      };

    AffineBasis basis     = new AffineBasis(origin, vectors);
    Vector      newVector = new Vector(new double[] { 1, 1, 1 });

    bool result = basis.AddVectorToBasis(newVector);
    basis.AddVectorToBasis(newVector);

    Vector newVector0 = new Vector(origin);
    bool   result0    = basis.AddVectorToBasis(newVector0);


    Assert.That(result, Is.True, "The vector should be added to the linear basis associated with the affine basis.");
    Assert.That(result0, Is.False, "The zero vector should not be added to the linear basis associated with the affine basis.");
    Assert.That(basis, Has.Count.EqualTo(3), "The linear basis associated with the affine basis should have one more vector.");
    Assert.That(basis.FullDim, Is.True, "The affine basis should be full dimension if the current basis vectors span a linear space.");
    Assert.That(basis.IsEmpty, Is.False, "The affine basis should not be empty if input non zero vectors are provided.");

    AffineBasis.CheckCorrectness(basis);
  }

  /// <summary>
  /// Simple add point test
  /// </summary>
  [Test]
  public void AddPointToBasisTest() {
    Point origin = new Point(new double[] { 1, 2, 3 });

    List<Point> points = new List<Point>()
      {
        new Point(new double[] { 2, 3, 4 })
      , new Point(new double[] { 3, -4, 5 })
      };

    AffineBasis basis    = new AffineBasis(origin, points);
    Point       newPoint = new Point(new double[] { 4, 5, -6 });


    bool result = basis.AddPointToBasis(newPoint);
    basis.AddPointToBasis(newPoint);

    bool result0 = basis.AddVectorToBasis(new Vector(origin));


    Assert.That(result, Is.True, "The point should be added to the linear basis associated with the affine basis.");
    Assert.That(result0, Is.False, "The zero point should not be added to the linear basis associated with the affine basis.");
    Assert.That(basis.Count, Is.EqualTo(3), "The linear basis associated with the affine basis should have one more vector.");
    Assert.That(basis.FullDim, Is.True, "The affine basis should be full dimension if the current basis vectors span a linear space.");
    Assert.That(basis.IsEmpty, Is.False, "The affine basis should not be empty if input points are provided.");

    AffineBasis.CheckCorrectness(basis);
  }

  /// <summary>
  /// Simple expansion test
  /// </summary>
  [Test]
  public void ExpansionWithVectorTest() {
    Point origin = new Point(new double[] { double.Pi, 1, 4 });

    List<Vector> vectors = new List<Vector>()
      {
        new Vector(new double[] { 1, 0, 0 })
      , new Vector(new double[] { 0, 1, 0 })
      , new Vector(new double[] { 0, 0, 1 })
      };

    AffineBasis basis = new AffineBasis(origin, vectors);
    Vector      v     = new Vector(new double[] { double.Pi, double.Pi / 2, double.Pi / 4 });

    Vector expansion = basis.Expansion(v);

    Assert.That(expansion.CompareTo(v), Is.EqualTo(0), "Wrong expansion of a vector in the affine basis.");
    AffineBasis.CheckCorrectness(basis);
  }

  /// <summary>
  /// Simple expansion with point test
  /// </summary>
  [Test]
  public void ExpansionWithPointTest() {
    Point origin = new Point(new double[] { 1, 0, 0 });

    List<Vector> vectors = new List<Vector>()
      {
        new Vector(new double[] { 1, 0, 0 })
      , new Vector(new double[] { 0, 1, 0 })
      , new Vector(new double[] { 0, 0, 1 })
      };

    AffineBasis basis = new AffineBasis(origin, vectors);
    Point       p     = new Point(new double[] { double.Pi, double.Pi / 2, double.Pi / 4 });

    Vector expansion = basis.Expansion(p);


    Assert.That(expansion.CompareTo(p - origin), Is.EqualTo(0), "Wrong expansion of a point in the affine basis.");
  }

  /// <summary>
  /// Simple test
  /// </summary>
  [Test]
  public void ProjectToAffineSpace_1() {
    Point origin = new Point(new double[] { 0, 0 });

    List<Vector> basis = new List<Vector>
      {
        new Vector(new double[] { 1, 0 })
      , new Vector(new double[] { 0, 1 })
      };

    HashSet<Point> points = new HashSet<Point>
      {
        new Point(new double[] { 1, 1 })
      , new Point(new double[] { 2, 3 })
      , new Point(new double[] { -1, 4 })
      };

    HashSet<Point> expected = points;

    IEnumerable<Point> result = Tools.ProjectToAffineSpace(origin, basis, points);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }


  /// <summary>
  /// Reflect simple test
  /// </summary>
  [Test]
  public void ProjectToAffineSpace_2() {
    Point origin = new Point(new double[] { 0, 0 });

    List<Vector> basis = new List<Vector>
      {
        new Vector(new double[] { -1, 0 })
      , new Vector(new double[] { 0, -1 })
      };

    HashSet<Point> points = new HashSet<Point>
      {
        new Point(new double[] { 1, 1 })
      , new Point(new double[] { 2, 3 })
      , new Point(new double[] { -1, 4 })
      };

    HashSet<Point> expected = new HashSet<Point>
      {
        new Point(new double[] { -1, -1 })
      , new Point(new double[] { -2, -3 })
      , new Point(new double[] { 1, -4 })
      };

    IEnumerable<Point> result = Tools.ProjectToAffineSpace(origin, basis, points);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }


  /// <summary>
  /// Reflect and shift simple test 
  /// </summary>
  [Test]
  public void ProjectToAffineSpace_3() {
    Point origin = new Point(new double[] { 2, 2 });

    List<Vector> basis = new List<Vector>
      {
        new Vector(new double[] { -1, 0 })
      , new Vector(new double[] { 0, -1 })
      };

    HashSet<Point> points = new HashSet<Point>
      {
        new Point(new double[] { 1, 1 })
      , new Point(new double[] { 2, 4 })
      , new Point(new double[] { -4, 4 })
      };

    HashSet<Point> expected = new HashSet<Point>
      {
        new Point(new double[] { 1, 1 })
      , new Point(new double[] { 0, -2 })
      , new Point(new double[] { 6, -2 })
      };

    IEnumerable<Point> result = Tools.ProjectToAffineSpace(origin, basis, points);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }

  /// <summary>
  /// 4D-test
  /// </summary>
  [Test]
  public void ProjectToAffineSpace_4() {
    Point origin = new Point(new double[] { 0, 0, 0, 0 });

    List<Vector> basis = new List<Vector>
      {
        new Vector(new double[] { 1, 0, 0, 0 })
      , new Vector(new double[] { 0, 1, 0, 0 })
      };

    HashSet<Point> points = new HashSet<Point>
      {
        new Point(new double[] { 0, 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0, 0 })
      , new Point(new double[] { 0, 1, 0, 0 })
      , new Point(new double[] { 1, 1, 1, 1 })
      };

    HashSet<Point> expected = new HashSet<Point>
      {
        new Point(new double[] { 0, 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0, 0 })
      , new Point(new double[] { 0, 1, 0, 0 })
      , new Point(new double[] { 1, 1, 0, 0 })
      };

    IEnumerable<Point> result = Tools.ProjectToAffineSpace(origin, basis, points);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }

}
