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

    string visConfig = "Oscillator-pyramid.Blender";
    // string visConfig = "Oscillator.Blender";
    // string visConfig = "Oscillator-mass.Blender";
    // string visConfig = "MassDot-mass.Blender";
    
    // Visualization<double, DConvertor>   visD  = new Visualization<double, DConvertor>(pathLdg, visConfig, 1e-08);
    // Visualization<ddouble, DDConvertor> visDD = new Visualization<ddouble, DDConvertor>(pathLdg, visConfig, ddouble.Parse("1e-08"));
    // Visualization<Rational, RConvertor> visR08 =
      // new Visualization<Rational, RConvertor>(pathLdg, visConfig, Rational.Parse("1/100000000"));
    // Visualization<Rational, RConvertor> visR16 =
      // new Visualization<Rational, RConvertor>(pathLdg, visConfig, Rational.Parse("1/10000000000000000"));


    // visD.ForBlender();
    // visDD.ForBlender();
    // visR08.ForBlender();
    // visR16.ForBlender();



    string pathForTests = "F:\\Works\\IMM\\Аспирантура\\LDG\\Visualization\\Temp\\";
    Geometry<double,DConvertor>.ConvexPolytop sumCH = Geometry<double, DConvertor>.ConvexPolytop.CreateFromReader(new Geometry<double, DConvertor>.ParamReader($"{pathForTests}pyramid-CH.cpolytope"));
    Geometry<double,DConvertor>.ConvexPolytop sum = Geometry<double, DConvertor>.ConvexPolytop.CreateFromReader(new Geometry<double, DConvertor>.ParamReader($"{pathForTests}pyramid.cpolytope"));
    VisTools.DrawPolytopePLY(sumCH, $"{pathForTests}pyramid-CH");
    VisTools.DrawPolytopePLY(sum, $"{pathForTests}pyramid");
  }

}
