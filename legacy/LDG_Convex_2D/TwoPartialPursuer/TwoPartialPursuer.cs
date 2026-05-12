using System.Globalization;
using LDGObjects;
using PolygonLibrary;
using ParamReaderLibrary;
using PolygonLibrary.Polygons;
using PolygonLibrary.Polygons.ConvexPolygons;
using PolygonLibrary.Toolkit;

namespace TwoPartialPursuer;

/// <summary>
/// This class holds information about TwoPartialPursuer-problem and can solve it.
/// </summary>
class TwoPartialPursuer {
  /// <summary>
  /// The main directory before solving TPP-problem 
  /// </summary>
  private readonly string origDir;

  /// <summary>
  /// The directory where source file and result folder are placed
  /// </summary>
  private string workDir;

  /// <summary>
  /// The directory where .dat files will be saved
  /// </summary>
  private string gnuplotDat;

  /// <summary>
  /// The name of source file
  /// </summary>
  private string fileName;

  /// <summary>
  /// Holds internal information about task
  /// </summary>
  private readonly GameData gd;

  /// <summary>
  /// Game-whole bridge
  /// </summary>
  private StableBridge2D Br;

  /// <summary>
  /// One-partial bridge 
  /// </summary>
  private StableBridge2D W1;

  /// <summary>
  /// Second-partial bridge
  /// </summary>
  private StableBridge2D W2;

  /// <summary>
  /// Aux W1Q1 bridge
  /// </summary>
  private StableBridge2D W1Q1;

  /// <summary>
  /// Aux W1Q2 bridge
  /// </summary>
  private StableBridge2D W1Q2;

  /// <summary>
  /// Aux W2Q1 bridge
  /// </summary>
  private StableBridge2D W2Q1;

  /// <summary>
  /// Aux W2Q2 bridge
  /// </summary>
  private StableBridge2D W2Q2;

  /// <summary>
  /// Constructor which creates GameData and init three StableBridges
  /// </summary>
  /// <param name="workDir">The directory where source file and result folder are placed</param>
  /// <param name="fileName">The name of source file</param>
  /// <exception cref="ArgumentException">If there are no path to working directory this exception is thrown</exception>
  public TwoPartialPursuer(string workDir, string fileName) {
    origDir = Directory.GetCurrentDirectory();
    try {
      Directory.SetCurrentDirectory(workDir);
    }
    catch {
      throw new ArgumentException("A problem to switch to the folder WorkDir:", workDir);
    }

    this.workDir = Directory.GetCurrentDirectory();
    this.fileName = fileName;
    gd = new GameData(this.workDir + "/" + fileName);
    Br = new StableBridge2D(gd.ProblemName, "Br", 0, TubeType.Bridge);
    W1 = new StableBridge2D(gd.ProblemName, "W1", 0, TubeType.Bridge);
    W2 = new StableBridge2D(gd.ProblemName, "W2", 0, TubeType.Bridge);
    gnuplotDat = gd.path + "gnuplot-dat/";

    W1Q1 = new StableBridge2D(gd.ProblemName, "W1Q1", 0, TubeType.Bridge);
    W1Q2 = new StableBridge2D(gd.ProblemName, "W1Q2", 0, TubeType.Bridge);
    W2Q1 = new StableBridge2D(gd.ProblemName, "W2Q1", 0, TubeType.Bridge);
    W2Q2 = new StableBridge2D(gd.ProblemName, "W2Q2", 0, TubeType.Bridge);
  }

  /// <summary>
  /// Computes LDG TwoPartialPursuers
  /// </summary>
  public void Compute() {
    double t = gd.T;
    
    Br.Add(new TimeSection2D(t, gd.M)); //
    W1.Add(new TimeSection2D(t, gd.M)); // W1 = W2 = M at time = T
    W2.Add(new TimeSection2D(t, gd.M)); // W1 = W2 = M at time = T

    double tPred = gd.T;
    t -= gd.dt; // next step
    Br.Add(new TimeSection2D(t
    , Br[^1].section + gd.Ps[tPred] - gd.Qs[tPred] ??
      throw new InvalidOperationException("Br_{N-1} is empty!"))); //Br.Last().section == M
    W1.Add(new TimeSection2D(t
    , W1[^1].section + gd.Ps[tPred] - gd.Qs1[tPred] ??
      throw new InvalidOperationException("W1_{N-1} is empty!"))); //W1.Last().section == M
    W2.Add(new TimeSection2D(t
    , W2[^1].section + gd.Ps[tPred] - gd.Qs2[tPred] ??
      throw new InvalidOperationException("W2_{N-1} is empty!"))); //W2.Last().section == M

    // Main computational loop
    bool flag = true;
    bool brNotDegenerate = true;
    while (flag && Tools.GT(t, gd.t0)) {
      tPred = t;
      t -= gd.dt;
      Console.WriteLine($"t = {t}");
      ConvexPolygon? W11 = W1[^1].section + gd.Ps[tPred] - gd.Qs1[tPred];
      ConvexPolygon? W12 = W1[^1].section + gd.Ps[tPred] - gd.Qs2[tPred];
      ConvexPolygon? W21 = W2[^1].section + gd.Ps[tPred] - gd.Qs1[tPred];
      ConvexPolygon? W22 = W2[^1].section + gd.Ps[tPred] - gd.Qs2[tPred];

      if (W11 == null) {
        flag = false;
        Console.WriteLine("    empty interior of W1Q1 at t = " + t.ToString("#0.0000"));
      }
      if (W12 == null) {
        flag = false;
        Console.WriteLine("    empty interior of W1Q2 at t = " + t.ToString("#0.0000"));
      }
      if (W21 == null) {
        flag = false;
        Console.WriteLine("    empty interior of W2Q1 at t = " + t.ToString("#0.0000"));
      }
      if (W22 == null) {
        flag = false;
        Console.WriteLine("    empty interior of W2Q2 at t = " + t.ToString("#0.0000"));
      }

      if (flag) {
        ConvexPolygon? W1next = ConvexPolygon.IntersectionPolygon(W11, W22);
        ConvexPolygon? W2next = ConvexPolygon.IntersectionPolygon(W12, W21);

        if (W1next == null) {
          flag = false;
          Console.WriteLine("    empty interior of W1 at t = " + t.ToString("#0.0000"));
        }
        if (W2next == null) {
          flag = false;
          Console.WriteLine("    empty interior of W2 at t = " + t.ToString("#0.0000"));
        }
        
        if (flag) {
          W1.Add(new TimeSection2D(t, W1next!));
          W2.Add(new TimeSection2D(t, W2next!));

          W1Q1.Add(new TimeSection2D(t, W11!));
          W1Q2.Add(new TimeSection2D(t, W12!));
          W2Q1.Add(new TimeSection2D(t, W21!));
          W2Q2.Add(new TimeSection2D(t, W22!));
        }

        if (brNotDegenerate) {
          ConvexPolygon? R = Br[^1].section + gd.Ps[tPred] - gd.Qs[tPred];
          if (R == null || R.Contour.Count < 3) {
            brNotDegenerate = false;
          } else {
            Br.Add(new TimeSection2D(t, R));
          }
        }
      }
    }
  }

  /// <summary>
  /// Writes the bridge to a file lying on the path: WorkDir/Problem-name
  /// </summary>
  /// <param name="bridge">Bridge to be written</param>
  private void WriteBrByName(StableBridge2D bridge) {
    var sr = new StreamWriter(gd.path + bridge.ShortProblemName + "_" + StableBridge2D.GenerateFileName(0));
    bridge.WriteToFile(sr);
    sr.Close();
  }

  /// <summary>
  /// Writes Br, Q1 and Q2 into files according to the format.
  /// </summary>
  public void WriteBridges() {
    WriteBrByName(Br);
    WriteBrByName(W1);
    WriteBrByName(W2);
  }

  /// <summary>
  /// Clears GNU-plot directory and writes data into .dat files
  /// </summary>
  public void WriteDat() {
    Directory.CreateDirectory(gnuplotDat);
    Directory.Delete(gnuplotDat, true);
    Directory.CreateDirectory(gnuplotDat);

    WriteGnuplotDat(Br);
    WriteGnuplotDat(W1);
    WriteGnuplotDat(W2);

    if (gd.WriteQTubes) {
      WriteGnuplotDat(gd.QTube1);
      WriteGnuplotDat(gd.QTube2);
    }

    WriteGnuplotDat(W1Q1);
    WriteGnuplotDat(W1Q2);
    WriteGnuplotDat(W2Q1);
    WriteGnuplotDat(W2Q2);
  }

  /// <summary>
  /// Writes bridge into .dat file. Format x-column _space_ y-column
  /// </summary>
  /// <param name="bridge">The bridge to be written</param>
  private void WriteGnuplotDat(StableBridge2D bridge) {
    int i = 0;
    foreach (TimeSection2D ts in bridge) {
      // using (var sw = new StreamWriter($"{gnuplotDat}{bridge.ShortProblemName}_{ts.t:F2}.dat")) {
      using var sw = new StreamWriter($"{gnuplotDat}{bridge.ShortProblemName}_{i:000}.dat");
      foreach (var p in ts.section.Vertices) {
        sw.WriteLine($"{p.x:G3} {p.y:G3}");
      }

      sw.WriteLine($"{ts.section.Vertices[0].x:G3} {ts.section.Vertices[0].y:G3}");
      i++;
    }
  }

  public void WritePlt() {
    string pngs = $"{gd.path}/PNGs/";
    Directory.CreateDirectory(pngs);
    Directory.Delete(pngs, true);
    Directory.CreateDirectory(pngs);

    using var sw = new StreamWriter($"{gnuplotDat}doPNGs.plt");
    sw.WriteLine("reset");
    sw.WriteLine("set term pngcairo size 1600,1200");
    sw.WriteLine("set size ratio 1");
    sw.WriteLine("set xrange [-3:+3]");
    sw.WriteLine("set yrange [-3:+3]");

    if (gd.WriteQTubes) {
      sw.WriteLine($"do for [i=0:{gd.QTube.Count - 1}] {{");
      sw.WriteLine("  set output sprintf(\"../PNGs/QTubes_%03d.png\", i)\n  plot \\");
      sw.WriteLine(
        "    sprintf(\"Q1Tube_%03d.dat\", i) with filledcurves fc 'green' fs transparent solid 0.25 title 'Q1', \\");
      sw.WriteLine(
        "    sprintf(\"Q2Tube_%03d.dat\", i) with filledcurves fc 'blue'  fs transparent solid 0.25 lc 'blue' title  'Q2'");
      sw.WriteLine("  unset output");
      sw.WriteLine("}");
    }

    sw.WriteLine($"do for [i=0:{W1.Count - 1}] {{");
    sw.WriteLine("  set output sprintf(\"../PNGs/Br_%03d.png\", i)\n  plot \\");
    sw.WriteLine(
      "    sprintf(\"W1_%03d.dat\", i) with filledcurves fc 'green' fs transparent solid 0.25 title 'W1', \\");
    sw.WriteLine(
      "    sprintf(\"W2_%03d.dat\", i) with filledcurves fc 'blue'  fs transparent solid 0.25 lc 'blue' title  'W2', \\");
    sw.WriteLine(
      "    sprintf(\"Br_%03d.dat\", i) with filledcurves fc 'red'   fs transparent solid 0.25 lc 'red' title   'Main'");
    sw.WriteLine("  unset output");
    sw.WriteLine("}");

    sw.WriteLine("  set output sprintf(\"../PNGs/000.W1W2_000.png\")\n  plot \\");
    sw.WriteLine("    sprintf(\"W1_000.dat\") with filledcurves fc 'green' fs transparent solid 0.25 title 'W1', \\");
    sw.WriteLine("    sprintf(\"W2_000.dat\") with filledcurves fc 'coral' fs transparent solid 0.25 title 'W2'");
    sw.WriteLine("  unset output");
    sw.WriteLine();

    sw.WriteLine();
    sw.WriteLine($"do for [i=0:{W1Q1.Count - 1}] {{");

    sw.WriteLine("  set output sprintf(\"../PNGs/%03d.W1W2_%03d.png\", 3*i+1, i+1)\n  plot \\");
    sw.WriteLine(
      "    sprintf(\"W1_%03d.dat\", i+1) with filledcurves fc 'green' fs transparent solid 0.25 title 'W1', \\");
    sw.WriteLine("    sprintf(\"W2_%03d.dat\", i+1) with filledcurves fc 'coral' fs transparent solid 0.25 title 'W2'");
    sw.WriteLine("  unset output");
    sw.WriteLine();

    sw.WriteLine("  set output sprintf(\"../PNGs/%03d.toW1_%03d.png\", 3*i+2, i+1)\n  plot \\");
    sw.WriteLine(
      "    sprintf(\"W1Q1_%03d.dat\", i) with filledcurves fc 'green' fs transparent solid 0.25 title 'W1Q1', \\");
    sw.WriteLine(
      "    sprintf(\"W2Q2_%03d.dat\", i) with filledcurves fc 'coral' fs transparent solid 0.25 title 'W2Q2'");
    sw.WriteLine("  unset output");

    sw.WriteLine("  set output sprintf(\"../PNGs/%03d.toW2_%03d.png\", 3*i+3, i+1)\n  plot \\");
    sw.WriteLine(
      "    sprintf(\"W1Q2_%03d.dat\", i) with filledcurves fc 'green' fs transparent solid 0.25 title 'W1Q2', \\");
    sw.WriteLine(
      "    sprintf(\"W2Q1_%03d.dat\", i) with filledcurves fc 'coral' fs transparent solid 0.25 title 'W2Q1'");
    sw.WriteLine("  unset output");

    // sw.WriteLine("  set output sprintf(\"../PNGs/%03d.WQs_%03d.png\", 4*i+4, i+1)\n  plot \\");
    // sw.WriteLine("    sprintf(\"W1Q1_%03d.dat\", i) with filledcurves fc 'green' fs transparent solid 0.25 title 'W1Q1', \\");
    // sw.WriteLine("    sprintf(\"W1Q2_%03d.dat\", i) with filledcurves fc 'black' fs transparent solid 0.25 title 'W1Q2', \\");
    // sw.WriteLine("    sprintf(\"W2Q1_%03d.dat\", i) with filledcurves fc 'coral' fs transparent solid 0.25 title 'W2Q1', \\");
    // sw.WriteLine("    sprintf(\"W2Q2_%03d.dat\", i) with filledcurves fc 'gold' fs transparent solid 0.25 title 'W2Q2'");
    // sw.WriteLine("  unset output");

    sw.WriteLine("}");
  }
}


// Writing the bridge
// StreamWriter sr = new StreamWriter(gd.path + StableBridge2D.GenerateFileName(gd.cValues[0]));
// br.WriteToFile(sr);
// sr.Close();
//
// Directory.SetCurrentDirectory(origDir);
// Console.WriteLine("That's all!");