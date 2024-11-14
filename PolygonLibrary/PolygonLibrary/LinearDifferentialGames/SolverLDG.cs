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

    /// <summary>
    /// The directory where source file and result folder are placed
    /// </summary>
    public readonly string WorkDir;

    /// <summary>
    /// The name of source file
    /// </summary>
    public readonly string FileName;

    public readonly string ProblemPath;

    public readonly string NumericalType;

    public readonly string Eps;

    public readonly string BridgePath;

    public readonly string PsPath;

    public readonly string QsPath;

    /// <summary>
    /// Holds internal information about the task
    /// </summary>
    public readonly GameData gd;

    /// <summary>
    /// Game-whole bridge
    /// </summary>
    public readonly StableBridge W;

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
    public SolverLDG(string workDir, string fileName, bool clean = false) {
      CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

      NumericalType = typeof(TNum).ToString();
      Eps           = $"{TConv.ToDouble(Tools.Eps):e0}";
      WorkDir       = workDir;
      FileName      = fileName;

      gd = new GameData($"{Path.Combine(WorkDir, FileName)}.c");

      ProblemPath = Path.Combine(WorkDir, gd.ProblemName);

      string filesPath = Path.Combine(ProblemPath, NumericalType, Eps);
      if (clean && Directory.Exists(filesPath)) {
        Directory.Delete(filesPath, true);
      }

      BridgePath = Path.Combine(filesPath, "Bridge");
      PsPath     = Path.Combine(filesPath, "Ps");
      QsPath     = Path.Combine(filesPath, "Qs");


      W = new StableBridge(new CauchyMatrix.TimeComparer());
    }

    // Метод для получения пути к секции (Bridge, Ps, Qs)
    private string GetSectionPath(string sectionPrefix, string basePath, TNum t) {
      return Path.Combine(basePath, $"{TConv.ToDouble(t):F2}){sectionPrefix}_{FileName}.cpolytop");
    }

    private void ReadSection(SortedDictionary<TNum, ConvexPolytop> sectionDict, string sectionPrefix, string basePath, TNum t) {
      string filePath = GetSectionPath(sectionPrefix, basePath, t);
      Debug.Assert
        (File.Exists(filePath), $"SolverLDG.ReadSection: There is no {sectionPrefix} section at time {t}. File: {filePath}");

      ParamReader prR = new ParamReader(filePath);
      sectionDict.Add(t, ConvexPolytop.CreateFromReader(prR));
    }

    private void WriteSection(
        SortedDictionary<TNum, ConvexPolytop> sectionDict
      , string                                sectionPrefix
      , string                                basePath
      , TNum                                  t
      , ConvexPolytop.Rep                     repType
      ) {
      ParamWriter prW = new ParamWriter(GetSectionPath(sectionPrefix, basePath, t));
      sectionDict[t].WriteIn(prW, repType);
    }

// Методы для работы с Bridge, Ps, Qs секциями
    public void ReadBridgeSection(TNum  t) => ReadSection(W, "W", BridgePath, t);
    public void WriteBridgeSection(TNum t) => WriteSection(W, "W", BridgePath, t, ConvexPolytop.Rep.FLrep);

    public void ReadPsSection(TNum  t) => ReadSection(gd.Ps, "P", PsPath, t);
    public void WritePsSection(TNum t) => WriteSection(gd.Ps, "P", PsPath, t, ConvexPolytop.Rep.FLrep);

    public void ReadQsSection(TNum  t) => ReadSection(gd.Qs, "Q", QsPath, t);
    public void WriteQsSection(TNum t) => WriteSection(gd.Qs, "Q", QsPath, t, ConvexPolytop.Rep.Vrep);


    public bool BridgeSectionFileExist(TNum t) => File.Exists(GetSectionPath("W", BridgePath, t));
    public bool PsSectionFileExist(TNum     t) => File.Exists(GetSectionPath("P", PsPath, t));
    public bool QsSectionFileExist(TNum     t) => File.Exists(GetSectionPath("Q", QsPath, t));


    public void CleanAll() {
      Console.WriteLine($"Warning! This function will erase all files and folders at the path: {ProblemPath}/");
      Console.WriteLine($"Do you want to continue? [y]es");
      if (Console.ReadKey().KeyChar == 'y') {
        Directory.Delete($"{ProblemPath}", true);
      }
    }


    /// <summary>
    /// Computes the next section of a convex polytope by the second Pontryagin's method.
    /// </summary>
    /// <param name="predSec">The previous section of the bridge.</param>
    /// <param name="predP">The first convex polytope (P) used in the Minkowski sum.</param>
    /// <param name="predQ">The second convex polytope (Q) used in the Minkowski difference.</param>
    /// <returns>The next section of the stable bridge, or null if the operation results in an invalid polytop.</returns>
    public ConvexPolytop? DoNextSection(ConvexPolytop predSec, ConvexPolytop predP, ConvexPolytop predQ) {
      ConvexPolytop  sum  = MinkowskiSum.BySandipDas(predSec, predP, true);
      ConvexPolytop? next = MinkowskiDiff.Geometric(sum, predQ);

      return next;
    }

    /// <summary>
    /// It computes the time sections of the maximal stable bridge of  the game.
    /// </summary>
    public void Solve(bool needWrite) {
      Stopwatch timer = new Stopwatch();

      if (needWrite) {
        Directory.CreateDirectory(BridgePath);
        Directory.CreateDirectory(PsPath);
        Directory.CreateDirectory(QsPath);
      }

      TNum t = gd.T;
      TNum tPred;
      // Обрабатываем терминальный момент времени
      if (BridgeSectionFileExist(t)) {
        if (!BridgeSectionFileExist(t - gd.dt)) {
          ReadBridgeSection(t);
        }
      }
      else {
        W[t] = gd.M.GetInFLrep();
        if (needWrite) {
          WriteBridgeSection(t);
        }
      }


      bool bridgeIsNotDegenerate = true;
      while (Tools.GT(t, gd.t0) && bridgeIsNotDegenerate) {
        if (PsSectionFileExist(t)) {
          if (!PsSectionFileExist(t - gd.dt)) {
            ReadPsSection(t);
          }
        }
        else {
          Matrix Xstar = gd.ProjMatr * gd.cauchyMatrix[t];
          gd.D[t] = Xstar * gd.B;
          TNum t1 = t;
          // Для борьбы с "Captured variable is modified in the outer scope" (Code Inspection: Access to modified captured variable)
          gd.Ps[t] = ConvexPolytop.CreateFromPoints(gd.P.Vrep.Select(pPoint => -gd.dt * gd.D[t1] * pPoint), true);
          if (needWrite) {
            WritePsSection(t);
          }
        }
        // -dt * X(T, t_i) * B * P и dt * X(T, t_i) * C * Q
        if (QsSectionFileExist(t)) {
          if (!QsSectionFileExist(t - gd.dt)) {
            ReadQsSection(t);
          }
        }
        else {
          Matrix Xstar = gd.ProjMatr * gd.cauchyMatrix[t];
          gd.E[t] = Xstar * gd.C;
          TNum t1 = t;
          // Для борьбы с "Captured variable is modified in the outer scope" (Code Inspection: Access to modified captured variable)
          gd.Qs[t] = ConvexPolytop.CreateFromPoints(gd.Q.Vrep.Select(qPoint => gd.dt * gd.E[t1] * qPoint), false);
          if (needWrite) {
            WriteQsSection(t);
          }
        }


        tPred =  t;
        t     -= gd.dt;

        timer.Restart();
        if (BridgeSectionFileExist(t)) {
          if (!BridgeSectionFileExist(t - gd.dt)) {
            ReadBridgeSection(t);
          }
        }
        else {
          // Формула Пшеничного
          ConvexPolytop? WNext = DoNextSection(W[tPred], gd.Ps[tPred], gd.Qs[tPred]);

          if (WNext is null) {
            Console.WriteLine($"The bridge become degenerate at t = {t}.");
            bridgeIsNotDegenerate = false;
          }
          else {
            W[t] = WNext;
          }
        }
        timer.Stop();
        if (W.TryGetValue(t, out ConvexPolytop? br)) {
          Console.WriteLine($"{TConv.ToDouble(t):F2}) DoNS = {timer.Elapsed.TotalSeconds:F4} sec. Vrep.Count = {br.Vrep.Count}");
        }

        if (needWrite && !BridgeSectionFileExist(t)) {
          timer.Restart();
          WriteBridgeSection(t);
          timer.Stop();
          Console.WriteLine($"\tWrite = {timer.Elapsed.TotalSeconds:F4} sec.");
        }
      }
    }

    public void LoadBridge(TNum t0, TNum T) {
      Debug.Assert(Tools.LT(t0, T), $"The t0 should be less then T. Found t0 = {t0} < T = {T}");

      TNum t = t0;
      do {
        ReadBridgeSection(t);
        t += gd.dt;
      } while (Tools.LT(t, T));
    }

    public void LoadPs(TNum t0, TNum T) {
      Debug.Assert(Tools.LT(t0, T), $"The t0 should be less then T. Found t0 = {t0} < T = {T}");

      TNum t = t0;
      do {
        ReadPsSection(t);
        t += gd.dt;
      } while (Tools.LT(t, T));
    }

    public void LoadQs(TNum t0, TNum T) {
      Debug.Assert(Tools.LT(t0, T), $"The t0 should be less then T. Found t0 = {t0} < T = {T}");

      TNum t = t0;
      do {
        ReadQsSection(t);
        t += gd.dt;
      } while (Tools.LT(t, T));
    }

    public void LoadGame(TNum t0, TNum T) {
      LoadQs(t0, T);
      LoadPs(t0, T);
      LoadBridge(t0, T);

      TNum t = t0;
      do {
        Matrix Xstar = gd.ProjMatr * gd.cauchyMatrix[t];
        gd.D[t] = Xstar * gd.B;
        gd.E[t] = Xstar * gd.C;

        t += gd.dt;
      } while (Tools.LT(t, T));
    }

    /// <summary>
    /// Computes the controls for the first and second players in a linear differential game.
    /// </summary>
    /// <param name="x">Current state vector.</param>
    /// <param name="t">Current time.</param>
    /// <param name="u">
    /// Output control vector for the first player.
    /// If the state is inside the bridge section, the first vertex of <c>P</c> is selected.
    /// Otherwise, the vertex that maximizes the (h-x, pVert) is chosen.
    /// </param>
    /// <param name="v">
    /// Output control vector for the second player.
    /// If the state is outside the bridge section, the first vertex of <c>Q</c> is selected.
    /// Otherwise, the vertex that maximizes the (x-h, qVert) is chosen.
    /// </param>
    public void workOutControl(Vector x, TNum t, out Vector u, out Vector v) {
      Debug.Assert(gd.D.ContainsKey(t), $"Matrix D must be defined for time t = {t}, but not found.");
      Debug.Assert(gd.E.ContainsKey(t), $"Matrix E must be defined for time t = {t}, but not found.");
      Debug.Assert(W.ContainsKey(t), $"Bridge W must be defined for time t = {t}, but not found.");


      u = Vector.Zero(1); // Чтобы компилятор не ругался, что не присвоено значение
      v = Vector.Zero(1); // Чтобы компилятор не ругался, что не присвоено значение

      Vector h = W[t].NearestPoint(x, out bool isInside); // Нашли ближайшую точку на сечении моста

      if (isInside) { // Внутри моста выбираем любой из P, но лучший из Q
        u = gd.P.Vrep.First();

        Vector l       = x - h;
        TNum   extrVal = Tools.NegativeInfinity;
        foreach (Vector qVert in gd.Q.Vrep) {
          TNum val = gd.dt * gd.E[t] * qVert * l;
          if (val > extrVal) {
            extrVal = val;
            v       = qVert;
          }
        }
      }
      else { // Снаружи моста выбираем любой из Q, но лучший из P
        v = gd.P.Vrep.First();

        Vector l       = h - x;
        TNum   extrVal = Tools.NegativeInfinity;
        foreach (Vector pVert in gd.P.Vrep) {
          TNum val = -gd.dt * gd.D[t] * pVert * l;
          if (val > extrVal) {
            extrVal = val;
            u       = pVert;
          }
        }
      }
    }

    /// <summary>
    /// Performs a single Euler integration step for the state vector in a linear differential game.
    /// </summary>
    /// <param name="x">Current state vector.</param>
    /// <param name="u">Control of the first player.</param>
    /// <param name="v">Control of the second player.</param>
    /// <returns>Updated state vector after one Euler step.</returns>
    public Vector EulerStep(Vector x, Vector u, Vector v) => x + gd.dt * (gd.A * x + gd.B * u + gd.C * v);

    /// <summary>
    /// Computes the trajectory of the system using the explicit Euler method
    /// from the initial time t0 to the final time T.
    /// </summary>
    /// <param name="x0">Initial state vector.</param>
    /// <param name="t0">Initial time.</param>
    /// <param name="T">Final time.</param>
    /// <returns>List of state vectors representing the trajectory of the system from t0 to T.</returns>
    public List<Vector> Euler(Vector x0, TNum t0, TNum T) {
      List<Vector> trajectory = new List<Vector> { x0 }; // Начальное состояние

      Vector x = x0;
      for (TNum t = t0; t < T; t += gd.dt) {
        // Вычисляем управления
        workOutControl(x, t, out Vector u, out Vector v);

        // Выполняем шаг Эйлера
        x = EulerStep(x, u, v);
        trajectory.Add(x);
      }

      return trajectory;
    }

    // --------------------------------------------------------------------------------------------------------------------------

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
      writer.WriteLine($"{setType}RectPLeft = {(-left).ToStringBraceAndDelim('{', '}', ',')};");
      writer.WriteLine($"{setType}RectPRight = {right.ToStringBraceAndDelim('{', '}', ',')};");
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
