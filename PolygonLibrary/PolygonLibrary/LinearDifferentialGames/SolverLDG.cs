using System.Globalization;
using System.IO;
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
    private readonly StableBridge W;

    /// <summary>
    /// Gets the section of the bridge at given time.
    /// </summary>
    /// <param name="t">The time instant.</param>
    /// <returns>The section of the bridge.</returns>
    public ConvexPolytop GetSection(TNum t) => W[t];


    /// <summary>
    /// Constructor which creates GameData and init three StableBridges
    /// </summary>
    /// <param name="workDir">The directory where source file and result folder are placed</param>
    /// <param name="fileName">The name of source file without extension. It is '.c'.</param>
    /// <exception cref="ArgumentException">If there is no path to working directory, this exception is thrown</exception>
    public SolverLDG(string workDir, string fileName) {
      CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

      this.workDir = workDir;
      if (workDir[^1] == '/') {
        StringBuilder sb = new StringBuilder(workDir);
        sb[^1]  = '/';
        workDir = sb.ToString();
      }
      else if (workDir[^1] != '/') {
        workDir += '/';
      }

      this.fileName = fileName;
      gd            = new GameData(this.workDir + fileName + ".c");
      Directory.CreateDirectory(workDir + gd.ProblemName);

      W             = new StableBridge(new CauchyMatrix.TimeComparer());
      // foreach (KeyValuePair<TNum, ConvexPolytop> P in gd.Ps) {
      //   using ParamWriter pr = new ParamWriter($"{workDir + gd.ProblemName}/{TConv.ToDouble(P.Key):F2}) P {fileName}.tsection");
      //   pr.WriteNumber("t", TConv.ToDouble(P.Key), "F3");
      //   P.Value.WriteIn(pr, ConvexPolytop.Rep.FLrep);
      // }
      // foreach (KeyValuePair<TNum, ConvexPolytop> Q in gd.Qs) {
      //   using ParamWriter pr = new ParamWriter($"{workDir + gd.ProblemName}/{TConv.ToDouble(Q.Key):F2}) Q {fileName}.tsection");
      //   pr.WriteNumber("t", TConv.ToDouble(Q.Key), "F3");
      //   Q.Value.WriteIn(pr, ConvexPolytop.Rep.FLrep);
      // }
    }

    public ConvexPolytop? DoNextSection(ConvexPolytop predSec, ConvexPolytop predP, ConvexPolytop predQ) {
      ConvexPolytop  sum  = MinkowskiSum.BySandipDas(predSec, predP);
      ConvexPolytop? next = MinkowskiDiff.Geometric(sum, predQ);

      return next;
    }

    // public void WriteTSection(TNum t, ParamWriter pr) {
    //   pr.WriteNumber("t", TConv.ToDouble(t), "F3");
    //   W[t].WriteIn(pr, ConvexPolytop.Rep.FLrep);
    // }

    /// <summary>
    /// Computes LDG 
    /// </summary>
    public void Solve(bool isNaive, bool isNeedWrite, bool isDouble) {
      Stopwatch timer    = new Stopwatch();
      string    filesDir = workDir + gd.ProblemName;
      double    eps      = TConv.ToDouble(Tools.Eps);

      string valType = isDouble ? "double" : "ddouble";
      string algType = isNaive ? "Naive" : "Geometric";

      if (isNeedWrite) {
        Directory.CreateDirectory($"{filesDir}/{valType}/{algType}/{eps:e0}/");
      }

      TNum t = gd.T;
      TNum tPred;

      W[t] = gd.M; // в финальный момент стабильный мост совпадает с целевым множеством

      bool bridgeIsNotDegenerate = true;
      while (Tools.GT(t, gd.t0)) {
        if (isNeedWrite) {
          using ParamWriter pr = new ParamWriter
            ($"{filesDir}/{valType}/{algType}/{eps:e0}/{TConv.ToDouble(t):F2}){fileName}.cpolytop");
          W[t].WriteIn(pr, ConvexPolytop.Rep.FLrep);


          // using ParamWriter pr = new ParamWriter($"{filesDir}/{TConv.ToDouble(t):F2}){fileName}.tsection");
          // pr.WriteNumber("t", TConv.ToDouble(t), "F3");
          // W[t].WriteIn(pr, ConvexPolytop.Rep.FLrep);
          // W[t].WriteTXT_3D($"{filesDir}/{TConv.ToDouble(t):F2}){fileName}");
        }

        tPred =  t;
        t     -= gd.dt;
        if (bridgeIsNotDegenerate) { // Формула Пшеничного
          timer.Restart();
          ConvexPolytop Sum = MinkowskiSum.BySandipDas(W[tPred], gd.Ps[tPred], true);

          ConvexPolytop? WNext;
          if (isNaive) {
            WNext = MinkowskiDiff.Naive(Sum, gd.Qs[tPred]);
          }
          else {
            WNext = MinkowskiDiff.Geometric(Sum, gd.Qs[tPred]);
          }

          timer.Stop();

          if (WNext is null) {
            bridgeIsNotDegenerate = false;
            Console.WriteLine($"The bridge become degenerate at t = {t}.");
          }
          else {
            Console.WriteLine($"{TConv.ToDouble(t):F2}) = {timer.Elapsed.TotalMilliseconds}. Vrep.Count = {WNext.Vrep.Count}");
            W[t] = WNext;
          }
        }
      }
    }

    public static void WriteSimplestTask_TerminalSet_GameItself(int dim, string folderPath) {
      Vector vP = Vector.Ones(dim);
      Vector vQ = TConv.FromDouble(0.5) * Vector.Ones(dim);
      Vector vM = Tools.Two * Vector.Ones(dim);
      // Matrix
      using (StreamWriter writer = new StreamWriter(folderPath + "simplestGame_" + dim + "D.c")) {
        writer.WriteLine("// Name of the problem");
        writer.WriteLine($"ProblemName = \"Cubes{dim}D\";");
        writer.WriteLine();
        writer.WriteLine();
        writer.WriteLine("// ==================================================");

        WriteSimplestDynamics(dim, writer);

        WriteConstraintBlock(writer, "P", vP, vP);
        WriteConstraintBlock(writer, "Q", vQ, vQ);

        writer.WriteLine("// The goal type of the game");
        writer.WriteLine("// \"Itself\" - the game itself");
        writer.WriteLine("// \"Epigraph\" - the game with epigraphic of the payoff function");
        writer.WriteLine("GoalType = \"Itself\";");
        writer.WriteLine();

        writer.WriteLine("// The type of the M");
        writer.WriteLine
          (
           "// \"TerminalSet\" - the explicit terminal set assigment. In Rd if goal type is \"Itself\", in R{d+1} if goal type is \"Epigraph\""
          );
        writer.WriteLine("// \"DistToOrigin\" - the game with epigraph of the payoff function as distance to the origin.");
        writer.WriteLine
          ("// \"DistToPolytop\" - the game with epigraph of the payoff function as distance to the given polytop.");
        writer.WriteLine("MType = \"TerminalSet\";");
        writer.WriteLine();

        WriteConstraintBlock(writer, "M", vM, vM);
      }
    }

    public static void WriteSimplestTask_Payoff_Supergraphic_2D(string folderPath) {
      Vector vP = Vector.Ones(2);
      Vector vQ = TConv.FromDouble(0.5) * Vector.Ones(2);
      using (StreamWriter writer = new StreamWriter(folderPath + "simplestSupergraphic.c")) {
        writer.WriteLine("// Name of the problem");
        writer.WriteLine($"ProblemName = \"Cubes2D\";");
        writer.WriteLine();
        writer.WriteLine();
        writer.WriteLine("// ==================================================");

        WriteSimplestDynamics(2, writer);

        WriteConstraintBlock(writer, "P", vP, vP);
        WriteConstraintBlock(writer, "Q", vQ, vQ);

        writer.WriteLine("// The goal type of the game");
        writer.WriteLine("// \"Itself\" - the game itself");
        writer.WriteLine("// \"Epigraph\" - the game with epigraphic of the payoff function");
        writer.WriteLine("GoalType = \"Epigraph\";");
        writer.WriteLine();

        writer.WriteLine("// The type of the M");
        writer.WriteLine
          (
           "// \"TerminalSet\" - the explicit terminal set assigment. In Rd if goal type is \"Itself\", in R{d+1} if goal type is \"Epigraph\""
          );
        writer.WriteLine("// \"DistToOrigin\" - the game with epigraph of the payoff function as distance to the origin.");
        writer.WriteLine
          ("// \"DistToPolytop\" - the game with epigraph of the payoff function as distance to the given polytop.");
        writer.WriteLine("MType = \"DistToOrigin\";");
        writer.WriteLine();

        writer.WriteLine("MBallType = \"Ball_1\";");
        writer.WriteLine("MCMax = 5;");
        writer.WriteLine();
        writer.WriteLine("// ==================================================");
      }
    }

    private static void WriteConstraintBlock(TextWriter writer, string setType, Vector left, Vector right) {
      writer.WriteLine($"{setType}SetType = \"RectParallel\";");
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
      writer.WriteLine($"pDim = {dim};");
      writer.WriteLine();
      writer.WriteLine("// The useful control matrix");
      writer.WriteLine($"B = {Matrix.Eye(dim)};");
      writer.WriteLine();
      writer.WriteLine("// Dimension of the disturbance");
      writer.WriteLine($"qDim = {dim};");
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
      writer.WriteLine($"d = {dim};");
      writer.WriteLine();
      writer.WriteLine("// The indices to project onto");
      writer.Write("projJ = {");
      for (int i = 0; i < dim - 1; i++) {
        writer.Write($"{i}, ");
      }
      writer.Write($"{dim - 1}}};\n");
      writer.WriteLine();

      writer.WriteLine("// ==================================================");
    }

  }

}
