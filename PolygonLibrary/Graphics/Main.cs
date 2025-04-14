using CGLibrary;
using DoubleDouble;
using Graphics.Draw;
using LDG;
using Rationals;

namespace Graphics;

public class Program {

  public static void Main() {
    const string pathTemp = "F:/Temp/";

    const string pathLdg = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    // string pathLdg = "E:\\Work\\LDG\\";

    var polytope =
      Geometry<double, DConvertor>.ConvexPolytop.CreateFromReader
        (new Geometry<double, DConvertor>.ParamReader($"{pathTemp}polytope.polytope"));
    var polytopeflrep =
      Geometry<double, DConvertor>.ConvexPolytop.CreateFromReader
        (new Geometry<double, DConvertor>.ParamReader($"{pathTemp}polytopeflrep.polytope"));

    VisTools.DrawPolytopePLY(polytope, $"{pathTemp}polytope");
    VisTools.DrawPolytopePLY(polytopeflrep, $"{pathTemp}polytopeflrep");


    // Visualization<double, DConvertor>   visD  = new Visualization<double, DConvertor>(pathLdg, "Oscillator3D.Blender", 1e-08);
    // Visualization<ddouble, DDConvertor> visDD = new Visualization<ddouble, DDConvertor>(pathLdg, "Oscillator3D.Blender", ddouble.Parse("1e-08"));
    // Visualization<Rational, RConvertor> visR08 =
    //   new Visualization<Rational, RConvertor>(pathLdg, "Oscillator3D.Blender", Rational.Parse("1/100000000"));
    // Visualization<Rational, RConvertor> visR16 =
    //   new Visualization<Rational, RConvertor>(pathLdg, "Oscillator3D.Blender", Rational.Parse("1/10000000000000000"));
    //
    //
    // visD.ForBlender();
    // visDD.ForBlender();
    // visR08.ForBlender();
    // visR16.ForBlender();


    // string pathForTests = "F:\\Works\\IMM\\Проекты\\Визуализация для LDG\\Файлы многогранников\\";
  }

}
