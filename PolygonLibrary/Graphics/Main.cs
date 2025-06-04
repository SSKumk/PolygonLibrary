using CGLibrary;
using DoubleDouble;
using Graphics.Draw;
using LDG;
using Rationals;

namespace Graphics;

public class Program {

  public static void Main() {
    // const string pathTemp = "F:/Temp/";

    // const string pathLdg = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    string pathLdg = "E:\\Work\\LDG\\";

    // string visConfig = "Oscillator-hexagon.Blender";
    // string visConfig = "Oscillator-pyramid.Blender";
    // string visConfig = "Oscillator.Blender";
    string visConfig = "Oscillator-mass.Blender";
    // string visConfig = "MassDot-mass.Blender";

    double eps  = double.Parse("1e-8");
    // ddouble eps = ddouble.Parse("1e-15");
    // Rational eps = Rational.Parse("1/10000000000000000");

    Visualization<double, DConvertor>   visD  = new Visualization<double, DConvertor>(pathLdg, visConfig, eps);
    // Visualization<ddouble, DDConvertor> visDD = new Visualization<ddouble, DDConvertor>(pathLdg, visConfig, eps);
    // Visualization<Rational, RConvertor> visR08 =
      // new Visualization<Rational, RConvertor>(pathLdg, visConfig, Rational.Parse("1/100000000"));
    // Visualization<Rational, RConvertor> visR16 =
      // new Visualization<Rational, RConvertor>(pathLdg, visConfig, Rational.Parse("1/10000000000000000"));


    visD.ForBlender();
    // visDD.ForBlender();
    // visR08.ForBlender();
    // visR16.ForBlender();
  }

}
