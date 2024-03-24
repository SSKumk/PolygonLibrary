using System.Globalization;
using PolygonLibrary.Polygons.ConvexPolygons;
using PolygonLibrary.Toolkit;
using LDGObjects;
using PolygonLibrary.Basics;

namespace Bridges;

class Bridges {

  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    if (args.Length < 2) {
      Console.WriteLine("The program needs two command line arguments:");
      Console.WriteLine("  - the folder, where the computation data are located");
      Console.WriteLine("	 - the name of the file with input data");

      return;
    }

    Tools.Eps = 1e-10;
    string origDir = Directory.GetCurrentDirectory();
    try {
      Directory.SetCurrentDirectory(args[0]);
    }
    catch {
      Console.WriteLine("A problem to switch to the folder '" + args[0] + "'");

      return;
    }
    GameData gd = new GameData(args[1], ComputationType.StableBridge);

    // Подготовили место для записи
    string dataMain = args[0] + "/" + gd.ShortProblemName + "/";
    Directory.CreateDirectory(dataMain);
    Directory.Delete(dataMain, true);
    Directory.CreateDirectory(dataMain);

    // Сделали батник, который будет запускать построение всех картинок
    using (StreamWriter writer = new StreamWriter(dataMain + "doAllPng.bat")) {
      writer.WriteLine("@echo off");
      writer.WriteLine("for /r ./data/ %%G in (*.plt) do (start \"\" \"%%G\")");
    }

    string dataPath = dataMain + "data/";
    Directory.CreateDirectory(dataPath);

    string pngsPath = dataMain + "pngs/";
    Directory.CreateDirectory(pngsPath);


    StableBridge2D br;

    // Loop on C values
    for (int ic = 0; ic < gd.cValues.Count; ic++) {
      Console.WriteLine("c = " + gd.cValues[ic]);

      double t = gd.T;
      br = new StableBridge2D(gd.ProblemName, gd.ShortProblemName, gd.cValues[ic], TubeType.Bridge);

      ConvexPolygon curSec = gd.PayoffLevelSet(gd.cValues[ic]);

      // If the initial section has no interior, pass to the next one
      if (curSec == null || curSec.Contour.Count < 3) {
        Console.WriteLine("    empty interior of the initial section");

        continue;
      }

      br.Add(new TimeSection2D(t, curSec));

      // Main computational loop
      bool flag = true;
      while (flag && Tools.GT(t, gd.t0)) {
        curSec =  (curSec + gd.Ps[t]) - gd.Qs[t];
        t      -= gd.dt;

        if (curSec == null || curSec.Contour.Count < 3) {
          flag = false;
          Console.WriteLine("    empty interior of the section at t = " + t.ToString("#0.0000"));
        } else
          br.Add(new TimeSection2D(t, curSec));
      }

      // Writing the bridge
      WriteBridge(dataPath, br);

      // Записываю .plt файлы
      WritePlt(dataPath, pngsPath, br);

      // StreamWriter sr = new StreamWriter(gd.path + StableBridge2D.GenerateFileName(gd.cValues[ic]));
      // br.WriteToFile(sr);
      // sr.Close();
    }

    Directory.SetCurrentDirectory(origDir);
    Console.WriteLine("That's all!");
    //Console.ReadKey();
  }


  public static void WriteBridge(string dataPath, StableBridge2D br) {
    string brPath = $"{dataPath}{br.c:F2} = c/";
    Directory.CreateDirectory(brPath);
    int i = 0;
    foreach (TimeSection2D ts in br) {
      // using (StreamWriter writer = new StreamWriter($"{brPath}{br.c:F2}) t = {ts.t:F2}.dat")) {
      using (StreamWriter writer = new StreamWriter($"{brPath}Br_{i:000}.dat")) {
        foreach (Point2D p in ts.section.Vertices) {
          writer.WriteLine($"{p.x:G3} {p.y:G3}");
        }
        writer.WriteLine($"{ts.section.Vertices[0].x:G3} {ts.section.Vertices[0].y:G3}");
      }
      i++;
    }
  }

  public static void WritePlt(string dataPath, string pngsPath, StableBridge2D br) {
    string brName = $"{br.c:F2} = c";
    string brPath = $"{dataPath}{brName}/";
    Directory.CreateDirectory($"{pngsPath}{brName}/");

    using var sw = new StreamWriter($"{brPath}doPNGs.plt");
    sw.WriteLine("reset");
    sw.WriteLine("set term pngcairo size 1600,1200");
    sw.WriteLine("set size ratio 1");
    sw.WriteLine("set xrange [-5:+5]");
    sw.WriteLine("set yrange [-5:+5]");

    sw.WriteLine($"do for [i=0:{br.Count - 1}] {{");
    sw.WriteLine($"  set output sprintf(\"{pngsPath.Replace('\\','/')}{brName}/Br_%03d.png\", i)\n  plot \\");
    sw.WriteLine
      ($"    sprintf(\"{dataPath.Replace('\\', '/')}{brName}/Br_%03d.dat\", i) with filledcurves fc 'red' fs transparent solid 0.25 lc 'red' title 'C = {br.c:F2}'");
    sw.WriteLine("  unset output");
    sw.WriteLine("}");
  }

  // /// <summary>
  // /// Writes bridge into .dat file. Format x-column _space_ y-column
  // /// </summary>
  // /// <param name="bridge">The bridge to be written</param>
  // private void WriteGnuplotDat(StableBridge2D bridge) {
  //   int i = 0;
  //   foreach (TimeSection2D ts in bridge) {
  //     // using (var sw = new StreamWriter($"{gnuplotDat}{bridge.ShortProblemName}_{ts.t:F2}.dat")) {
  //     using var sw = new StreamWriter($"{gnuplotDat}{bridge.ShortProblemName}_{i:000}.dat");
  //     foreach (var p in ts.section.Vertices) {
  //       sw.WriteLine($"{p.x:G3} {p.y:G3}");
  //     }
  //
  //     sw.WriteLine($"{ts.section.Vertices[0].x:G3} {ts.section.Vertices[0].y:G3}");
  //     i++;
  //   }
  // }

}
