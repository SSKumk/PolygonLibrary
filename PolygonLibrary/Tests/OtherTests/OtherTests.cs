using System.Numerics;
using CGLibrary;
using Convertors;
using DoubleDouble;
using NUnit.Framework;
using ddG = Tests.OtherTests.AuxFunc<DoubleDouble.ddouble, Convertors.DDConvertor>;
using dG = Tests.OtherTests.AuxFunc<double, Convertors.DConvertor>;


namespace Tests.OtherTests;

public class AuxFunc<TNum, TConv> : ToolsTests.ToolsTests<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>
  where TConv : INumConvertor<TNum> { }
// /// <summary>
// /// Rotates the given swarm of points in the space by given unitary matrix.
// /// </summary>
// /// <param name="S">The swarm of points to rotate.</param>
// /// <param name="rotation">Matrix to rotate a swarm.</param>
// /// <returns>The rotated swarm of points.</returns>
// private static List<Geometry<TNum, TConv>.Point>
//   Rotate<TNum, TConv>(IEnumerable<Geometry<TNum, TConv>.Point> S, Geometry<TNum, TConv>.Matrix rotation)
//   where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
//   IFloatingPoint<TNum>
//   where TConv : INumConvertor<TNum> {
//   IEnumerable<Geometry<TNum, TConv>.Vector> rotated = S.Select(s => new Geometry<TNum, TConv>.Vector(s) * rotation);
//
//   return rotated.Select(v => new Geometry<TNum, TConv>.Point(v)).ToList();
// }

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
  /// Generates a random TNum value in (0,1): a value between 0 and 1, excluding the values 0 and 1.
  /// </summary>
  /// <returns>The generated random TNum value.</returns>
  private static TNum GenInner<TNum, TConv>(RandomLC rnd)
    where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
    IFloatingPoint<TNum>
    where TConv : INumConvertor<TNum> {
    return TConv.FromInt(rnd.NextInt(1, 999)) / TConv.FromDouble(1000.0);
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
  private static List<Geometry<TNum, TConv>.Point> Cube<TNum, TConv>(int                                   cubeDim
                                                                   , out List<Geometry<TNum, TConv>.Point> pureCube
                                                                   , uint?                                 seed
                                                                   , IEnumerable<int>?                     facesDim = null
                                                                   , int                                   amount   = 1)
    where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
    IFloatingPoint<TNum>
    where TConv : INumConvertor<TNum> {
    if (cubeDim == 1) {
      List<Geometry<TNum, TConv>.Point> oneDimCube = new List<Geometry<TNum, TConv>.Point>()
        {
          new Geometry<TNum, TConv>.Point(new TNum[] { TNum.Zero }), new Geometry<TNum, TConv>.Point(new TNum[] { TNum.One })
        };
      pureCube = oneDimCube;

      return oneDimCube;
    }

    RandomLC         random    = new RandomLC(seed);
    List<List<TNum>> cube_prev = new List<List<TNum>>();
    List<List<TNum>> cube      = new List<List<TNum>>();
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

    if (facesDim is not null) {                                      // накидываем точки на грани нужных размерностей
      List<int> vectorsInds = Enumerable.Range(0, cubeDim).ToList(); // генерируем список [0,1,2, ... , cubeDim - 1]
      foreach (int dim in facesDim) {
        List<List<int>> allPoints = Subsets(vectorsInds, cubeDim - dim); //todo dim = cubeDim отдельно обработать

        if (cubeDim == dim) {
          for (int i = 0; i < amount; i++) {
            TNum[] point = new TNum[cubeDim];
            for (int j = 0; j < cubeDim; j++) {
              point[j] = GenInner<TNum, TConv>(random);
            }

            Cube.Add(new Geometry<TNum, TConv>.Point(point));
          }

          continue;
        }

        // Если размерность грани, куда нужно поместить точку меньше размерности куба
        foreach (List<int> fixedInd in allPoints) {
          List<Geometry<TNum, TConv>.Point> smallCube = OtherTests.Cube
            (cubeDim - dim, out List<Geometry<TNum, TConv>.Point> _, 0);
          foreach (Geometry<TNum, TConv>.Point pointCube in smallCube) {
            for (int k = 0; k < amount; k++) {
              TNum[] point = new TNum[cubeDim];
              int    s     = 0;
              for (int j = 0; j < cubeDim; j++) {
                point[j] = -TNum.One;
              }

              foreach (int ind in fixedInd) { // на выделенных местах размещаем 1-ки и 0-ки
                point[ind] = pointCube[s];
                s++;
              }

              for (int j = 0; j < cubeDim; j++) {
                if (Geometry<TNum, TConv>.Tools.EQ(point[j], -TNum.One)) {
                  point[j] = GenInner<TNum, TConv>(random);
                }
              }

              Cube.Add(new Geometry<TNum, TConv>.Point(point));
            }
          }
        }
      }
    }

    return Cube;
  }

  // [Test]
  // public void Atan2Test() {
  //   var x = ddouble.Atan2(-1e-15, -1);
  //   var y = double.Atan2(-1e-15, -1);
  //
  //   Console.WriteLine(x);
  //   Console.WriteLine(y);
  // }
  //
  // [Test]
  // public void AcosTest() {
  //   var x = ddouble.Acos(1);
  //   var y = double.Acos(1);
  //   Console.WriteLine(x);
  //   Console.WriteLine(y);
  // }

  private List<dG.Point> ToDPoints(List<ddG.Point> from) {
    return from.Select
                (
                 p => {
                   double[] pDD = new double[p.Dim];
                   for (int i = 0; i < p.Dim; i++) {
                     pDD[i] = (double)p[i];
                   }

                   return new dG.Point(pDD);
                 }
                )
               .ToList();
  }


  [Test]
  public void FindEtalon_3D() {
    int        cubeDim = 3;
    ddG.Matrix x1x2    = ddG.GenRotationMatrix(cubeDim, 1, 2, ddG.Tools.PI / 4);
    ddG.Matrix x1x3    = ddG.GenRotationMatrix(cubeDim, 1, 3, ddG.Tools.PI / 4);

    ddG.Matrix x1x2x1x3 = x1x2 * x1x3;

    List<ddG.Point> cube_DD             = Cube(cubeDim, out List<ddG.Point> _, 0, new[] { 1, 2, 3, 4 }, 5);
    var             cube_DD_rotated     = ddG.Rotate(cube_DD, x1x2x1x3);
    List<dG.Point>  cube_double_rotated = ToDPoints(cube_DD_rotated);


    var P1 = dG.GiftWrapping.WrapPolytop(cube_double_rotated);
    var P2 = ddG.GiftWrapping.WrapPolytop(cube_DD_rotated);

    Console.WriteLine($"{P1.Faces.First().Normal}");
    Console.WriteLine(P2.Faces.First().Normal);
  }

  [Test]
  public void FindEtalon_4D() {
    int        cubeDim = 4;
    ddG.Matrix x1x2    = ddG.GenRotationMatrix(cubeDim, 1, 2, ddG.Tools.PI / 4);
    ddG.Matrix x1x3    = ddG.GenRotationMatrix(cubeDim, 1, 3, ddG.Tools.PI / 4);
    ddG.Matrix x1x4    = ddG.GenRotationMatrix(cubeDim, 1, 4, ddG.Tools.PI / 4);

    ddG.Matrix x1x2x1x3x1x4 = x1x2 * x1x3 * x1x4;

    List<ddG.Point> cube_DD             = Cube(cubeDim, out List<ddG.Point> _, 0, new[] { 1, 2, 3, 4 }, 5);
    var             cube_DD_rotated     = ddG.Rotate(cube_DD, x1x2x1x3x1x4);
    List<dG.Point>  cube_double_rotated = ToDPoints(cube_DD_rotated);


    var P1 = dG.GiftWrapping.WrapPolytop(cube_double_rotated);
    var P2 = ddG.GiftWrapping.WrapPolytop(cube_DD_rotated);

    Console.WriteLine($"{P1.Faces.First().Normal}");
    Console.WriteLine(P2.Faces.First().Normal);
  }

  [Test]
  public void FindEtalon_8D() {
    int        cubeDim = 8;
    ddG.Matrix x1x2    = ddG.GenRotationMatrix(cubeDim, 1, 2, ddG.Tools.PI / 4);
    ddG.Matrix x1x3    = ddG.GenRotationMatrix(cubeDim, 1, 3, ddG.Tools.PI / 4);
    ddG.Matrix x1x4    = ddG.GenRotationMatrix(cubeDim, 1, 4, ddG.Tools.PI / 4);
    ddG.Matrix x1x5    = ddG.GenRotationMatrix(cubeDim, 1, 5, ddG.Tools.PI / 4);
    ddG.Matrix x1x6    = ddG.GenRotationMatrix(cubeDim, 1, 6, ddG.Tools.PI / 4);
    ddG.Matrix x1x7    = ddG.GenRotationMatrix(cubeDim, 1, 7, ddG.Tools.PI / 4);
    ddG.Matrix x1x8    = ddG.GenRotationMatrix(cubeDim, 1, 8, ddG.Tools.PI / 4);

    ddG.Matrix x1_x8 = x1x2 * x1x3 * x1x4 * x1x5 * x1x6 * x1x7 * x1x8;

    List<ddG.Point> cube_DD             = Cube(cubeDim, out List<ddG.Point> _, 0, new[] { 1, 2, 3, 4 }, 0);
    var             cube_DD_rotated     = ddG.Rotate(cube_DD, x1_x8);
    List<dG.Point>  cube_double_rotated = ToDPoints(cube_DD_rotated);


    var P1 = dG.GiftWrapping.WrapPolytop(cube_double_rotated);
    var P2 = ddG.GiftWrapping.WrapPolytop(cube_DD_rotated);

    Console.WriteLine($"{P1.Faces.First().Normal}");
    Console.WriteLine(P2.Faces.First().Normal);
  }

}
