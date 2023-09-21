using System.Formats.Asn1;
using System.Numerics;
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
  private static double GenInner(RandomLC rnd) {
    return rnd.NextInt(1, 999) / 1000.0;
  }

  /// <summary>
  /// Generates a full-dimension hypercube in the specified dimension.
  /// </summary>
  /// <param name="cubeDim">The dimension of the hypercube.</param>
  /// <param name="pureCube">The list of cube vertices of given dimension.</param>
  /// <param name="facesDim">The dimensions of the faces of the hypercube to put points on.</param>
  /// <param name="amount">The amount of points to be placed into random set of faces of faceDim dimension.</param>
  /// <param name="seed">The seed to be placed into GRandomLC. If null, the _random be used.</param>
  /// <returns>A list of points representing the hypercube possibly with inner points.</returns>
  private static List<Geometry<TNum, TConv>.Point> Cube<TNum, TConv>(int cubeDim, out List<Geometry<TNum, TConv>.Point> pureCube, uint? seed
                                                                   , IEnumerable<int>? facesDim = null, int amount = 1)
    where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
    IFloatingPoint<TNum>
    where TConv : INumConvertor<TNum> {
    if (cubeDim == 1) {
      List<Geometry<TNum, TConv>.Point> oneDimCube = new List<Geometry<TNum, TConv>.Point>()
        {
          new Geometry<TNum, TConv>.Point(new TNum[] { TNum.Zero })
        , new Geometry<TNum, TConv>.Point(new TNum[] { TNum.One })
        };
      pureCube = oneDimCube;

      return oneDimCube;
    }

    RandomLC random = new RandomLC(seed);
    List<List<TNum>> cube_prev = new List<List<TNum>>();
    List<List<TNum>> cube = new List<List<TNum>>();
    cube_prev.Add(new List<TNum>() { TNum.Zero });
    cube_prev.Add(new List<TNum>() { TNum.One });

    for (int i = 1; i < cubeDim; i++) {
      cube.Clear();

      foreach (List<TNum> coords in cube_prev) {
        cube.Add(new List<TNum>(coords) { TNum.Zero });
        cube.Add(new List<TNum>(coords) { TNum.One });
      }

      cube_prev = new List<List<TNum>>(cube);
    }

    List<Geometry<TNum, TConv>.Point> Cube = new List<Geometry<TNum, TConv>.Point>();

    foreach (List<TNum> v in cube) {
      Cube.Add(new Geometry<TNum, TConv>.Point(v.ToArray()));
    }

    pureCube = new List<Geometry<TNum, TConv>.Point>(Cube);

    if (facesDim is not null) { // накидываем точки на грани нужных размерностей
      List<int> vectorsInds = Enumerable.Range(0, cubeDim).ToList(); // генерируем список [0,1,2, ... , cubeDim - 1]
      foreach (int dim in facesDim) {
        List<List<int>> allPoints = Subsets(vectorsInds, cubeDim - dim); //todo dim = cubeDim отдельно обработать

        if (cubeDim == dim) {
          for (int i = 0; i < amount; i++) {
            TNum[] point = new TNum[cubeDim];
            for (int j = 0; j < cubeDim; j++) {
              point[j] = TConv.FromDouble(GenInner(random));
            }

            Cube.Add(new Geometry<TNum, TConv>.Point(point));
          }

          continue;
        }

        // Если размерность грани, куда нужно поместить точку меньше размерности куба
        foreach (List<int> fixedInd in allPoints) {
          List<G.Point> smallCube = OtherTests.Cube(cubeDim - dim, out List<G.Point> _, 0);
          foreach (G.Point pointCube in smallCube) {
            for (int k = 0; k < amount; k++) {
              double[] point = new double[cubeDim];
              int s = 0;
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

  private List<DG.Point> ToDDPoints(List<G.Point> from) {
    return from.Select(p => {
      ddouble[] pDD = new ddouble[p.Dim];
      for (int i = 0; i < p.Dim; i++) {
        pDD[i] = p[i];
      }

      return new DG.Point(pDD);
    }).ToList();
  }

  [Test]
  public void FindEtalon() {
    List<G.Point> cube4D_double = Cube(4, out List<G.Point> pureCube_double, 0, new[] { 1, 2, 3, 4 }, 1);
    List<DG.Point> cube4D_double_double = ToDDPoints(cube4D_double);

    var P1 = G.GiftWrapping.WrapPolytop(cube4D_double);
    var P2 = DG.GiftWrapping.WrapPolytop(cube4D_double_double);
  }
}