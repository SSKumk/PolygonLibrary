using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum>
{
  /// <summary>
  /// This class holds information about LDG-problem and can solve it.
  /// </summary>
  public class SolverLDG
  {
    /// <summary>
    /// The main directory before solving the problem 
    /// </summary>
    private readonly string origDir;

    /// <summary>
    /// The directory where source file and result folder are placed
    /// </summary>
    private string workDir;

    // /// <summary>
    // /// The directory where .dat files will be saved
    // /// </summary>
    // private string gnuplotDat;

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
    private StableBridge2D W;

    /// <summary>
    /// One-partial bridge 
    /// </summary>
    private StableBridge2D W1;

    /// <summary>
    /// Second-partial bridge
    /// </summary>
    private StableBridge2D W2;


    /// <summary>
    /// Constructor which creates GameData and init three StableBridges
    /// </summary>
    /// <param name="workDir">The directory where source file and result folder are placed</param>
    /// <param name="fileName">The name of source file</param>
    /// <exception cref="ArgumentException">If there are no path to working directory this exception is thrown</exception>
    public SolverLDG(string workDir, string fileName)
    {
      origDir = Directory.GetCurrentDirectory();
      try
      {
        Directory.SetCurrentDirectory(workDir);
      }
      catch
      {
        throw new ArgumentException("A problem to switch to the folder WorkDir:", workDir);
      }

      this.workDir = Directory.GetCurrentDirectory();
      this.fileName = fileName;
      gd = new GameData(this.workDir + "/" + fileName);
      W = new StableBridge2D(gd.ProblemName, "Br", 0, TubeType.Bridge);
      // gnuplotDat = gd.path + "gnuplot-dat/";
    }

    /// <summary>
    /// Computes LDG 
    /// </summary>
    public void Compute()
    {
      TNum t = gd.T;

      W.Add(new TimeSection2D(t, gd.M));

      TNum tPred = gd.T;
      t -= gd.dt; // next step
      W.Add(new TimeSection2D(t
      , W[^1].section + gd.Ps[tPred] - gd.Qs[tPred] ?? throw new InvalidOperationException("Br_{N-1} is empty!"))); //Br.Last().section == M

      // Main computational loop
      bool flag = true;
      bool bridgeIsNotDegenerate = true;
      while (flag && Tools.GT(t, gd.t0))
      {
        tPred = t;
        t -= gd.dt;
        if (bridgeIsNotDegenerate)
        {
          ConvexPolytop? WNext = W[^1].section + gd.Ps[tPred] - gd.Qs[tPred]; // Формула Пшеничного
          if (WNext == null)
          {
            bridgeIsNotDegenerate = false;
          }
          else
          {
            W.Add(new TimeSection2D(t, WNext));
          }
        }
      }
    }
  }

  /// <summary>
  /// Writes the bridge to a file lying on the path: WorkDir/Problem-name
  /// </summary>
  /// <param name="bridge">Bridge to be written</param>
  private void WriteBrByName(StableBridge2D bridge)
  {
    var sr = new StreamWriter(gd.path + bridge.ShortProblemName + "_" + StableBridge2D.GenerateFileName(0));
    bridge.WriteToFile(sr);
    sr.Close();
  }

  /// <summary>
  /// Writes Br, Q1 and Q2 into files according to the format.
  /// </summary>
  public void WriteBridges()
  {
    WriteBrByName(W);
    WriteBrByName(W1);
    WriteBrByName(W2);
  }

  /// <summary>
  /// Clears GNU-plot directory and writes data into .dat files
  /// </summary>
  public void WriteDat()
  {
    Directory.CreateDirectory(gnuplotDat);
    Directory.Delete(gnuplotDat, true);
    Directory.CreateDirectory(gnuplotDat);

    WriteGnuplotDat(W);
    WriteGnuplotDat(W1);
    WriteGnuplotDat(W2);

    if (gd.WriteQTubes)
    {
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
  private void WriteGnuplotDat(StableBridge2D bridge)
  {
    int i = 0;
    foreach (TimeSection2D ts in bridge)
    {
      // using (var sw = new StreamWriter($"{gnuplotDat}{bridge.ShortProblemName}_{ts.t:F2}.dat")) {
      using var sw = new StreamWriter($"{gnuplotDat}{bridge.ShortProblemName}_{i:000}.dat");
      foreach (var p in ts.section.Vertices)
      {
        sw.WriteLine($"{p.x:G3} {p.y:G3}");
      }

      sw.WriteLine($"{ts.section.Vertices[0].x:G3} {ts.section.Vertices[0].y:G3}");
      i++;
    }
  }

  public void WritePlt()
  {
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

    if (gd.WriteQTubes)
    {
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

}