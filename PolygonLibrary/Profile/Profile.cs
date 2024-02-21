using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using DoubleDouble;
using Tests.ToolsTests;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsBase<double, Tests.DConvertor>;

namespace Profile;

// Для проверки H-избыточности
// todo 1) Взять симплекс метод из Гуэрцитрона, запустить в отдельном проекте
// todo 2) Сравнить по скорости и качеству с alglib.


class Program {

  public static double[] MakeOneVector(int dim) {
    double[] v = new double[dim];
    for (int i = 0; i < dim; i++) {
      v[i] = 1;
    }

    return v;
  }

  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    var HSphere = Sphere(4, 10, 20, 1).HRepresentation;

    StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/H-Sphere-4-10-20.txt");
    System.Console.WriteLine(Directory.GetCurrentDirectory());

    writer.WriteLine(HSphere.Count);
    writer.WriteLine(4);
    foreach (HyperPlane hp in HSphere) {
      writer.Write($"{hp.ConstantTerm}");
      for (int i = 0; i < hp.Normal.Dim; i++) {
        writer.Write($" {hp.Normal[i]}");
      }
      writer.WriteLine();
    }
    writer.Close();
    System.Console.WriteLine("Yes!");
  }


  private static void ProfileMinkSum() {
    Stopwatch timer = new Stopwatch();
    timer.Restart();

    // var P = GiftWrapping.WrapFaceLattice(Sphere_list(3, 4, 100, 1).Select(s => s.ExpandTo(4)));
    // var P = GiftWrapping.WrapFaceLattice(Sphere_list(3, 4, 100, 1));
    // var Q = GiftWrapping.WrapFaceLattice(Sphere_list(3, 100, 4, 1));
    // var Q = Simplex4D_FL;
    // var P = GiftWrapping.WrapFaceLattice(Sphere_list(3, 10, 10, 1));
    // var Q = GiftWrapping.WrapFaceLattice(Sphere_list(3, 20, 4, 1));
    var P = GiftWrapping.WrapFaceLattice(Cube5D_list);
    var Q = GiftWrapping.WrapFaceLattice(SimplexRND5D_list);
    Console.WriteLine($"GW = {timer.ElapsedMilliseconds}");

    const int N = 100;

    FaceLattice? x = null;
    timer.Restart();
    for (int i = 0; i < N; i++) {
      x = MinkSumSDas(P, Q);
    }
    Console.WriteLine($"MinkSumSDas = {timer.ElapsedMilliseconds}");

    FaceLattice? y = null;
    timer.Restart();
    for (int i = 0; i < N; i++) {
      y = MinkSumCH(P, Q);
    }
    Console.WriteLine($"MinkSumCH = {timer.ElapsedMilliseconds}");

    if (!x!.Equals(y)) { throw new ArgumentException("AAAAAA"); }
  }

}
