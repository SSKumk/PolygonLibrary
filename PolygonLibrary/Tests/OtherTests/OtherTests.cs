using CGLibrary;
using NUnit.Framework;
using DoubleDouble;
using DG = CGLibrary.Geometry<DoubleDouble.ddouble, Convertors.DDConvertor>;
using G = CGLibrary.Geometry<double, Convertors.DConvertor>;

namespace OtherTests;

[TestFixture]
public class OtherTests {

  /// <summary>
  /// Finds all subsets of a given length for an array.
  /// </summary>
  /// <typeparam name="T">The type of the array elements.</typeparam>
  /// <param name="arr">The input array.</param>
  /// <param name="subsetLength">The length of the subsets.</param>
  /// <returns>A list of subsets of the specified length.</returns>
  private static List<List<T>> Subsets<T>(IReadOnlyList<T> arr, int subsetLength) {
    List<List<T>> subsets = new List<List<T>>();

    FindSubset(new List<T>(), 0);

    return subsets;

    void FindSubset(List<T> currentSubset, int currentIndex) {
      if (currentSubset.Count == subsetLength) {
        subsets.Add(new List<T>(currentSubset));

        return;
      }

      if (currentIndex == arr.Count) {
        return;
      }

      FindSubset(new List<T>(currentSubset) { arr[currentIndex] }, currentIndex + 1);
      FindSubset(new List<T>(currentSubset), currentIndex + 1);
    }
  }

  /// <summary>
  /// Generates a random ddouble value in (0,1): a value between 0 and 1, excluding the values 0 and 1.
  /// </summary>
  /// <returns>The generated random ddouble value.</returns>
  private static double GenInner(RandomLC rnd) { return rnd.NextInt(1, 999) / 1000.0; }

  /// <summary>
  /// Generates a full-dimension hypercube in the specified dimension.
  /// </summary>
  /// <param name="cubeDim">The dimension of the hypercube.</param>
  /// <param name="pureCube">The list of cube vertices of given dimension.</param>
  /// <param name="facesDim">The dimensions of the faces of the hypercube to put points on.</param>
  /// <param name="amount">The amount of points to be placed into random set of faces of faceDim dimension.</param>
  /// <param name="seed">The seed to be placed into GRandomLC. If null, the _random be used.</param>
  /// <returns>A list of points representing the hypercube possibly with inner points.</returns>
  private static List<G.Point> Cube(int               cubeDim
                                  , out List<G.Point> pureCube
                                  , uint?             seed
                                  , IEnumerable<int>? facesDim = null
                                  , int               amount   = 1) {

    if (cubeDim == 1) {
      List<G.Point> oneDimCube = new List<G.Point>(){new G.Point(new double[]{0}), new G.Point(new double[]{1})};
      pureCube = oneDimCube;

      return oneDimCube;
    }

    RandomLC           random    = new RandomLC(seed);
    List<List<double>> cube_prev = new List<List<double>>();
    List<List<double>> cube      = new List<List<double>>();
    cube_prev.Add(new List<double>() { 0 });
    cube_prev.Add(new List<double>() { 1 });

    for (int i = 1; i < cubeDim; i++) {
      cube.Clear();

      foreach (List<double> coords in cube_prev) {
        cube.Add(new List<double>(coords) { 0 });
        cube.Add(new List<double>(coords) { 1 });
      }
      cube_prev = new List<List<double>>(cube);
    }

    List<G.Point> Cube = new List<G.Point>();

    foreach (List<double> v in cube) {
      Cube.Add(new G.Point(v.ToArray()));
    }
    pureCube = new List<G.Point>(Cube);

    if (facesDim is not null) {                                      // накидываем точки на грани нужных размерностей
      List<int> vectorsInds = Enumerable.Range(0, cubeDim).ToList(); // генерируем список [0,1,2, ... , cubeDim - 1]
      foreach (int dim in facesDim) {
        List<List<int>> allPoints = Subsets(vectorsInds, cubeDim - dim); //todo dim = cubeDim отдельно обработать

        foreach (List<int> fixedInd in allPoints) {
          List<G.Point> smallCube = OtherTests.Cube(cubeDim - dim, out List<G.Point> _, 0);
          foreach (G.Point pointCube in smallCube) {
            for (int k = 0; k < amount; k++) {
              double[] point = new double[cubeDim];
              int      s     = 0;
              for (int j = 0; j < cubeDim; j++) {
                point[j] = -1;
              }
              foreach (int ind in fixedInd) { // на выделенных местах размещаем 1-ки и 0-ки
                point[ind] = pointCube[s];
                s++;
              }
              for (int j = 0; j < cubeDim; j++) {
                if (G.Tools.EQ(point[j], -1)) {
                  point[j] = GenInner(random);
                }
              }
              Cube.Add(new G.Point(point));
            }
          }
        }
      }
    }

    return Cube;
  }

  [Test]
  public void Atan2Test() {
    var x = ddouble.Atan2(-1e-15, -1);
    var y = double.Atan2(-1e-15, -1);

    Console.WriteLine(x);
    Console.WriteLine(y);
  }

  [Test]
  public void AcosTest() {
    var x = ddouble.Acos(1);
    var y = double.Acos(1);
    Console.WriteLine(x);
    Console.WriteLine(y);
  }


  [Test]
  public void FindEtalon() {
    var x = Cube(4, out List<G.Point> pureCube, 0, new[] { 1 }, 1);
    Console.WriteLine();
  }

}
