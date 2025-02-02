using Graphics.Draw;
using LDG;
using static CGLibrary.Geometry<double, Graphics.DConvertor>;

namespace Graphics;

public class Program {

  public static void Main() {
    string pathLdg = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    // string pathLdg = "E:\\Work\\LDG\\";

    Visualization vis = new Visualization(pathLdg, "Oscillator3D.Blender-FirstOptimal", "DoubleDouble.ddouble", 1e-015);
    Visualization vis1 = new Visualization(pathLdg, "Oscillator3D.Blender-SecondOptimal", "DoubleDouble.ddouble", 1e-015);


    // Visualization vis = new Visualization(pathLdg, "Oscillator.Blender", "DoubleDouble.ddouble", 1e-015);
    // Visualization vis = new Visualization(pathLdg, "MassDot.Blender", "DoubleDouble.ddouble", 1e-015);
    vis.ForBlender();
    vis1.ForBlender();


    // string pathForTests = "F:\\Works\\IMM\\Проекты\\Визуализация для LDG\\Файлы многогранников\\";

  }

}
