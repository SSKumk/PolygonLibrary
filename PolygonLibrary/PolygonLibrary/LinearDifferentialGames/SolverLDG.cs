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
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// This class holds information about LDG-problem and can solve it.
  /// </summary>
  public class SolverLDG {

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
    private SortedDictionary<TNum, ConvexPolytop> W;


    /// <summary>
    /// Constructor which creates GameData and init three StableBridges
    /// </summary>
    /// <param name="workDir">The directory where source file and result folder are placed</param>
    /// <param name="fileName">The name of source file</param>
    /// <exception cref="ArgumentException">If there are no path to working directory this exception is thrown</exception>
    public SolverLDG(string workDir, string fileName) {
      origDir = Directory.GetCurrentDirectory();
      try {
        Directory.SetCurrentDirectory(workDir);
      }
      catch {
        throw new ArgumentException("A problem to switch to the folder WorkDir:", workDir);
      }

      this.workDir  = Directory.GetCurrentDirectory();
      this.fileName = fileName;
      gd            = new GameData(this.workDir + "/" + fileName);
      W             = new StableBridge2D(gd.ProblemName, "Br", 0, TubeType.Bridge);
      // gnuplotDat = gd.path + "gnuplot-dat/";
    }

    /// <summary>
    /// Computes LDG 
    /// </summary>
    public void Compute() {
      TNum t = gd.T;
      TNum tPred;

      W[t] = gd.M; // в финальный момент стабильный мост совпадает с целевым множеством

      bool bridgeIsNotDegenerate = true;
      while (Tools.GT(t, gd.t0)) {
        tPred =  t;
        t     -= gd.dt;
        if (bridgeIsNotDegenerate) { // Формула Пшеничного
          ConvexPolytop  Sum   = MinkowskiSum.BySandipDas(W[tPred], gd.Ps[tPred]);
          ConvexPolytop? WNext = MinkowskiDiff.Naive(Sum, gd.Qs[tPred]);
          if (WNext is null) {
            bridgeIsNotDegenerate = false;
            Console.WriteLine($"The bridge become degenerate at t = {t}.");
          } else {
            W[t] = WNext;
          }
        }
      }
    }

  }

}
