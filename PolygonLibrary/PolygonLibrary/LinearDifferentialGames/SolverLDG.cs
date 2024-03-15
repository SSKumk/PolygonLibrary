using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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

    // /// <summary>
    // /// The main directory before solving the problem
    // /// </summary>
    // private readonly string origDir;

    /// <summary>
    /// The directory where source file and result folder are placed
    /// </summary>
    private readonly string workDir;

    // /// <summary>
    // /// The directory where .dat files will be saved
    // /// </summary>
    // private string gnuplotDat;

    /// <summary>
    /// The name of source file
    /// </summary>
    private readonly string fileName;

    /// <summary>
    /// Holds internal information about task
    /// </summary>
    private readonly GameData gd;

    /// <summary>
    /// Game-whole bridge
    /// </summary>
    private readonly SortedDictionary<TNum, ConvexPolytop> W;


    /// <summary>
    /// Constructor which creates GameData and init three StableBridges
    /// </summary>
    /// <param name="workDir">The directory where source file and result folder are placed</param>
    /// <param name="fileName">The name of source file without extension. It is '.c'.</param>
    /// <exception cref="ArgumentException">If there are no path to working directory this exception is thrown</exception>
    public SolverLDG(string workDir, string fileName) {
      CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

      this.workDir = workDir;
      if (workDir[^1] == '/') {
        StringBuilder sb = new StringBuilder(workDir);
        sb[^1]  = '/';
        workDir = sb.ToString();
      } else if (workDir[^1] != '/') {
        workDir += '/';
      }

      if (!Directory.Exists(workDir)) { //Cur dir must be in work directory
        Directory.CreateDirectory(workDir);
      }
      this.fileName = fileName;
      gd            = new GameData(this.workDir + fileName + ".c");
      W             = new SortedDictionary<TNum, ConvexPolytop>();
    }

    /// <summary>
    /// Computes LDG 
    /// </summary>
    public void Solve(bool isNeedWrite) {
      string filesDir = workDir + gd.ProblemName;
      if (isNeedWrite) {
        if (!Directory.Exists(filesDir)) { //Cur dir must be in work directory
          Directory.CreateDirectory(filesDir);
        }
      }

      TNum t = gd.T;
      TNum tPred;

      W[t] = gd.M; // в финальный момент стабильный мост совпадает с целевым множеством

      bool bridgeIsNotDegenerate = true;
      while (Tools.GT(t, gd.t0)) {
        if (isNeedWrite) {
          W[t].WriteTXT_3D($"{filesDir}/{TConv.ToDouble(t):F2}){fileName}");
        }

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

    public static void WriteSimplestFile(int dim, string folderPath) {
      Vector vP = Vector.Ones(dim);
      Vector vQ = TConv.FromDouble(0.5) * Vector.Ones(dim);
      Vector vM = Tools.Two * Vector.Ones(dim);
      // Matrix
      using (StreamWriter writer = new StreamWriter(folderPath + "simplest.c")) {
        writer.WriteLine("// Name of the problem");
        writer.WriteLine
          ($"ProblemName = \"Cubes3D(P#{vP.ToStringDouble()})(Q#{vQ.ToStringDouble()})(M#{vM.ToStringDouble()})\";");
        writer.WriteLine();
        writer.WriteLine();
        writer.WriteLine("// The goal type of the game");
        writer.WriteLine("// 0 - the game itself");
        writer.WriteLine("// 1 - the game with super-graphic of payoff");
        writer.WriteLine("GoalType = 0;");
        writer.WriteLine();

        writer.WriteLine("// ==================================================");

        WriteSimplestDynamics(dim, writer);

        WriteConstraintBlock(writer, "P", vP, vP);
        WriteConstraintBlock(writer, "Q", vQ, vQ);
        WriteConstraintBlock(writer, "M", vM, vM);
      }
    }

    private static void WriteConstraintBlock(TextWriter writer, string setType, Vector left, Vector right) {
      writer.WriteLine($"{setType}Type = \"RectParallel\";");
      writer.WriteLine($"{setType}RectPLeft = {(-left).ToStringDouble('{', '}')};");
      writer.WriteLine($"{setType}RectPRight = {right.ToStringDouble('{', '}')};");
      writer.WriteLine();
      writer.WriteLine("// ==================================================");
    }

    private static void WriteSimplestDynamics(int dim, TextWriter writer) {
      writer.WriteLine();
      writer.WriteLine("// Block of data defining the dynamics of the game");
      writer.WriteLine("// Dimension of the phase vector");
      writer.WriteLine($"n = {dim};");
      writer.WriteLine();
      writer.WriteLine("// The main matrix");
      writer.WriteLine($"A = {Matrix.Zero(dim)};");
      writer.WriteLine();
      writer.WriteLine("// Dimension of the useful control");
      writer.WriteLine($"p = {dim};");
      writer.WriteLine();
      writer.WriteLine("// The useful control matrix");
      writer.WriteLine($"B = {Matrix.Eye(dim)};");
      writer.WriteLine();
      writer.WriteLine("// Dimension of the disturbance");
      writer.WriteLine($"q = {dim};");
      writer.WriteLine();
      writer.WriteLine("// The disturbance matrix");
      writer.WriteLine($"C = {Matrix.Eye(dim)};");
      writer.WriteLine();
      writer.WriteLine("// The initial instant");
      writer.WriteLine("t0 = 0;");
      writer.WriteLine();
      writer.WriteLine("// The final instant");
      writer.WriteLine("T = 1;");
      writer.WriteLine();
      writer.WriteLine("// The time step");
      writer.WriteLine("dt = 0.2;");
      writer.WriteLine();
      writer.WriteLine("// The dimension of projected space");
      writer.WriteLine("d = 3;");
      writer.WriteLine();
      writer.WriteLine("// The indices to project onto");
      writer.WriteLine($"projJ = {Enumerable.Range(0, dim)};");
      writer.WriteLine();

      writer.WriteLine("// ==================================================");
    }

  }

}
