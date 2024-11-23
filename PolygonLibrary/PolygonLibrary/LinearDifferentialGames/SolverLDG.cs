using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using PolygonLibrary.Toolkit;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  // солвер будет ответственен за решение ОДНОЙ задачи.
  // Под решением понимается вычисление мостов, векторграмм и запись их в файл
  //  в файле хранится хеш задачи, которую решал солвер
  public class SolverLDG {

    public readonly string WorkDir; // Эта папка, в которую пишутся файлы


    public readonly GameData gd;


    /// <summary>
    /// Projection matrix, which extracts two necessary rows of the Cauchy matrix
    /// </summary>
    public readonly Matrix ProjMatrix;

    /// <summary>
    /// Collection of matrices D for the instants from the time grid
    /// </summary>
    public readonly SortedDictionary<TNum, Matrix> D = new SortedDictionary<TNum, Matrix>(Tools.TComp);

    /// <summary>
    /// Collection of matrices E for the instants from the time grid
    /// </summary>
    public readonly SortedDictionary<TNum, Matrix> E = new SortedDictionary<TNum, Matrix>(Tools.TComp);

    /// <summary>
    /// The bridge of the game
    /// </summary>
    public readonly SortedDictionary<TNum, ConvexPolytop> W;

    /// <summary>
    /// Vectograms of the first player
    /// </summary>
    public readonly SortedDictionary<TNum, ConvexPolytop> Ps = new SortedDictionary<TNum, ConvexPolytop>(Tools.TComp);

    /// <summary>
    /// Vectograms of the second player
    /// </summary>
    public readonly SortedDictionary<TNum, ConvexPolytop> Qs = new SortedDictionary<TNum, ConvexPolytop>(Tools.TComp);

    public readonly string GameHash;
    public readonly string PHash;
    public readonly string QHash;

    public TNum tMin;


    public SolverLDG(
        string        workDir
      , GameData      gameData
      , int           projDim
      , int[]         projInd
      , ConvexPolytop M
      , string        gameInfo
      , string        PsInfo
      , string        QsInfo
      ) {
      CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

      WorkDir = workDir;
      gd      = gameData;

      TNum[,] ProjMatrixArr = new TNum[projDim, gd.n];
      for (int i = 0; i < projDim; i++) {
        ProjMatrixArr[i, projInd[i]] = Tools.One;
      }

      GameHash = Hashes.GetMD5Hash(gameInfo);
      PHash    = Hashes.GetMD5Hash(PsInfo);
      QHash    = Hashes.GetMD5Hash(QsInfo);

      ProjMatrix = new Matrix(ProjMatrixArr);
      W          = new SortedDictionary<TNum, ConvexPolytop>(Tools.TComp);
      W[gd.T]    = M;
    }


    private static string GetSectionPath(string sectionPrefix, string basePath, TNum t) {
      string prefix = sectionPrefix switch
                        {
                          "W" => "w"
                        , "P" => "p"
                        , "Q" => "q"
                        , _ => throw new ArgumentException
                                 ($"Unknown section prefix: '{sectionPrefix}'. Expected 'W', 'P', or 'Q'.")
                        };

      return Path.Combine(basePath, $"{TConv.ToDouble(t):F3}){sectionPrefix}.{prefix}section");
    }


    private void ReadSection(SortedDictionary<TNum, ConvexPolytop> sectionDict, string sectionPrefix, string basePath, TNum t) {
      string filePath = GetSectionPath(sectionPrefix, basePath, t);
      Debug.Assert
        (File.Exists(filePath), $"SolverLDG.ReadSection: There is no {sectionPrefix} section at time {t}. File: {filePath}");

      ParamReader prR      = new ParamReader(filePath);
      string      taskHash = prR.ReadString("md5");

      bool hashIsCorrect = sectionPrefix switch
                             {
                               "W" => taskHash == GameHash
                             , "P" => taskHash == PHash
                             , "Q" => taskHash == QHash
                             , _   => throw new ArgumentException($"Unknown section prefix: {sectionPrefix}")
                             };
      Debug.Assert
        (
         hashIsCorrect
       , $"SolverLDG.ReadSection: The hash in the file ({taskHash}) does not match the expected hash ({sectionPrefix}Hash)."
        );

      sectionDict.Add(t, ConvexPolytop.CreateFromReader(prR));
    }

    private static void WriteSection(
        string                                sectionPrefix
      , SortedDictionary<TNum, ConvexPolytop> sectionDict
      , string                                basePath
      , TNum                                  t
      , string                                hash
      , ConvexPolytop.Rep                     repType
      ) {
      Debug.Assert(sectionDict.ContainsKey(t), $"SolverLDG.WriteSection: There is no {sectionPrefix} section at time {t}.");
      using ParamWriter prW = new ParamWriter(GetSectionPath(sectionPrefix, basePath, t));
      prW.WriteString("md5", hash);
      sectionDict[t].WriteIn(prW, repType);
    }

    public void ReadBridgeSection(TNum t) => ReadSection(W, "W", WorkDir, t);
    public void ReadPsSection(TNum     t) => ReadSection(Ps, "P", WorkDir, t);
    public void ReadQsSection(TNum     t) => ReadSection(Qs, "Q", WorkDir, t);

    public void WriteBridgeSection(TNum t)
      => WriteSection
        (
         "W"
       , W
       , WorkDir
       , t
       , GameHash
       , ConvexPolytop.Rep.FLrep
        );

    public void WritePsSection(TNum t)
      => WriteSection
        (
         "P"
       , Ps
       , WorkDir
       , t
       , PHash
       , ConvexPolytop.Rep.FLrep
        );

    public void WriteQsSection(TNum t)
      => WriteSection
        (
         "Q"
       , Qs
       , WorkDir
       , t
       , QHash
       , ConvexPolytop.Rep.Vrep
        );


    private bool SectionFileCorrect(string sectionPrefix, string basePath, TNum t, string expectedHash) {
      string filePath = GetSectionPath(sectionPrefix, basePath, t);
      if (!File.Exists(filePath)) {
        return false;
      }

      ParamReader prR = new ParamReader(filePath);

      string fileHash = prR.ReadString("md5");

      return fileHash == expectedHash;
    }

    public bool BridgeSectionFileCorrect(TNum t) => SectionFileCorrect("W", WorkDir, t, GameHash);
    public bool PsSectionFileCorrect(TNum     t) => SectionFileCorrect("P", WorkDir, t, PHash);
    public bool QsSectionFileCorrect(TNum     t) => SectionFileCorrect("Q", WorkDir, t, QHash);


    /// <summary>
    /// Computes the next section of a convex polytope by the second Pontryagin's method.
    /// </summary>
    /// <param name="predSec">The previous section of the bridge.</param>
    /// <param name="currP">The first convex polytope (P) used in the Minkowski sum.</param>
    /// <param name="currQ">The second convex polytope (Q) used in the Minkowski difference.</param>
    /// <returns>The next section of the stable bridge, or null if the operation results in an invalid polytop.</returns>
    public ConvexPolytop? DoNextSection(ConvexPolytop predSec, ConvexPolytop currP, ConvexPolytop currQ) {
      ConvexPolytop  sum  = MinkowskiSum.BySandipDas(predSec, currP, true);
      ConvexPolytop? next = MinkowskiDiff.Geometric(sum, currQ);

      return next;
    }

    private void ProcessPsSection(TNum t) {
      if (!PsSectionFileCorrect(t)) {
        Matrix Xstar = ProjMatrix * gd.cauchyMatrix[t];
        D[t] = Xstar * gd.B;
        TNum t1 = t;
        Ps[t] = ConvexPolytop.CreateFromPoints(gd.P.Vrep.Select(pPoint => -gd.dt * D[t1] * pPoint), true);
        WritePsSection(t);
      }
    }

    private void ProcessQsSection(TNum t) {
      if (!QsSectionFileCorrect(t)) {
        Matrix Xstar = ProjMatrix * gd.cauchyMatrix[t];
        E[t] = Xstar * gd.C;
        TNum t1 = t;
        Qs[t] = ConvexPolytop.CreateFromPoints(gd.Q.Vrep.Select(qPoint => gd.dt * E[t1] * qPoint), false);
        WriteQsSection(t);
      }
    }

    private void ProcessBridgeSection(TNum t, TNum tPred, ref bool bridgeIsNotDegenerate) {
      if (BridgeSectionFileCorrect(t)) {
        if (!BridgeSectionFileCorrect(t - gd.dt)) {
          ReadBridgeSection(t);
        }
      }
      else {
        if (!Ps.ContainsKey(t)) {
          ReadPsSection(t);
        }
        if (!Qs.ContainsKey(t)) {
          ReadQsSection(t);
        }
        ConvexPolytop? WNext = DoNextSection(W[tPred], Ps[t], Qs[t]);
        if (WNext is null) {
          Console.WriteLine($"The bridge become degenerate at t = {t}.");
          bridgeIsNotDegenerate = false;
        }
        else {
          W[t] = WNext;
          WriteBridgeSection(t);
        }
      }
    }

    /// <summary>
    /// It computes the time sections of the maximal stable bridge of  the game.
    /// </summary>
    public TNum Solve() {
      Stopwatch timer = new Stopwatch();

      Directory.CreateDirectory(WorkDir);

      TNum t = gd.T;
      tMin = gd.T;
      TNum tPred;
      bool bridgeIsNotDegenerate = true;

      // Обрабатываем терминальный момент времени
      ProcessPsSection(t);
      ProcessQsSection(t);

      // Основной цикл
      while (Tools.GT(t, gd.t0) && bridgeIsNotDegenerate) {
        tPred =  t;
        tMin  =  t;
        t     -= gd.dt;
        using ParamWriter prW = new ParamWriter(Path.Combine(WorkDir, "tMin.txt"));
        prW.WriteNumber("tMin", tMin);

        timer.Restart();
        ProcessPsSection(t);
        ProcessQsSection(t);
        ProcessBridgeSection(t, tPred, ref bridgeIsNotDegenerate);
        timer.Stop();

        if (W.TryGetValue(t, out ConvexPolytop? br)) {
          Console.WriteLine($"{TConv.ToDouble(t):F2}) DoNS = {timer.Elapsed.TotalSeconds:F4} sec. Vrep.Count = {br.Vrep.Count}");
        }
      }

      tMin -= gd.dt;

      {
        using ParamWriter prW = new ParamWriter(Path.Combine(WorkDir, "tMin.txt"));
        prW.WriteNumber("tMin", tMin);
      }

      return tMin;
    }


    public void LoadBridge(TNum t0, TNum T) {
      Debug.Assert(Tools.LT(t0, T), $"The t0 should be less then T. Found t0 = {t0} < T = {T}");

      TNum t = t0;
      do {
        if (!W.ContainsKey(t)) {
          ReadBridgeSection(t);
        }
        t += gd.dt;
      } while (Tools.LE(t, T));
    }

    public void LoadPs(TNum t0, TNum T) {
      Debug.Assert(Tools.LT(t0, T), $"The t0 should be less then T. Found t0 = {t0} < T = {T}");

      TNum t = t0;
      do {
        if (!Ps.ContainsKey(t)) {
          ReadPsSection(t);
        }
        t += gd.dt;
      } while (Tools.LE(t, T));
    }

    public void LoadQs(TNum t0, TNum T) {
      Debug.Assert(Tools.LT(t0, T), $"The t0 should be less then T. Found t0 = {t0} < T = {T}");

      TNum t = t0;
      do {
        if (!Qs.ContainsKey(t)) {
          ReadQsSection(t);
        }
        t += gd.dt;
      } while (Tools.LE(t, T));
    }

    public void LoadGame(TNum t0, TNum T) {
      LoadQs(t0, T);
      LoadPs(t0, T);
      LoadBridge(t0, T);

      TNum t = t0;
      do {
        Matrix Xstar = ProjMatrix * gd.cauchyMatrix[t];
        D[t] = Xstar * gd.B;
        E[t] = Xstar * gd.C;

        t += gd.dt;
      } while (Tools.LE(t, T));
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
    public void WorkOutControl(Vector x, TNum t, out Vector u, out Vector v) {
      Debug.Assert(D.ContainsKey(t), $"Matrix D must be defined for time t = {t}, but not found.");
      Debug.Assert(E.ContainsKey(t), $"Matrix E must be defined for time t = {t}, but not found.");
      Debug.Assert(W.ContainsKey(t), $"Bridge W must be defined for time t = {t}, but not found.");


      u = Vector.Zero(1); // Чтобы компилятор не ругался, что не присвоено значение
      v = Vector.Zero(1); // Чтобы компилятор не ругался, что не присвоено значение

      Vector h = W[t].NearestPoint(x, out bool isInside); // Нашли ближайшую точку на сечении моста

      if (isInside) { // Внутри моста выбираем u так, чтобы тянул систему к "центру" моста
        // Vector strongInner = W[t].Vrep.Aggregate((acc, v) => acc + v) / TConv.FromInt(W[t].Vrep.Count);
        u = gd.P.Vrep.First(); //todo: прицеливается на капец внутреннюю точку моста

        Vector l       = h - x;
        TNum   extrVal = Tools.NegativeInfinity;
        foreach (Vector qVert in gd.Q.Vrep) {
          TNum val = gd.dt * E[t] * qVert * l;
          if (val > extrVal) {
            extrVal = val;
            v       = qVert;
          }
        }
      }
      else { // лучший из P
        Vector l       = h - x;
        TNum   extrVal = Tools.NegativeInfinity;
        foreach (Vector pVert in gd.P.Vrep) {
          TNum val = gd.dt * D[t] * pVert * l;
          if (val > extrVal) {
            extrVal = val;
            u       = pVert;
          }
        }
        extrVal = Tools.NegativeInfinity;
        foreach (Vector qVert in gd.Q.Vrep) {
          TNum val = -gd.dt * E[t] * qVert * l;
          if (val > extrVal) {
            extrVal = val;
            v       = qVert;
          }
        }
      }
    }

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
      LoadGame(t0, T);                                   // Загружаем информацию об игре на заданном промежутке

      Vector x = x0;
      for (TNum t = t0; Tools.LT(t, T); t += gd.dt) {
        // Вычисляем управления
        WorkOutControl(x, t, out Vector u, out Vector v);

        // Выполняем шаг Эйлера
        x = x + gd.dt * (gd.A * x + gd.B * u + gd.C * v);
        trajectory.Add(x);
      }

      return trajectory;
    }

    // --------------------------------------------------------------------------------------------------------------------------

    // public static void WriteSimplestTask_TerminalSet_GameItself(int dim, string folderPath) {
    //   Vector vP = Vector.Ones(dim);
    //   Vector vQ = TConv.FromDouble(0.5) * Vector.Ones(dim);
    //   Vector vM = Tools.Two * Vector.Ones(dim);
    //   // Matrix
    //   using (StreamWriter writer = new StreamWriter(folderPath + "simplestGame_" + dim + "D.c")) {
    //     writer.WriteLine("// Name of the problem");
    //     writer.WriteLine($"ProblemName = \"Cubes{dim}D\";");
    //     writer.WriteLine();
    //     writer.WriteLine();
    //     writer.WriteLine("// ==================================================");
    //
    //     WriteSimplestDynamics(dim, writer);
    //
    //     WriteConstraintBlock(writer, "P", vP, vP);
    //     WriteConstraintBlock(writer, "Q", vQ, vQ);
    //
    //     writer.WriteLine("// The goal type of the game");
    //     writer.WriteLine("// \"Itself\" - the game itself");
    //     writer.WriteLine("// \"Epigraph\" - the game with epigraphic of the payoff function");
    //     writer.WriteLine("GoalType = \"Itself\";");
    //     writer.WriteLine();
    //
    //     writer.WriteLine("// The type of the M");
    //     writer.WriteLine
    //       (
    //        "// \"TerminalSet\" - the explicit terminal set assigment. In Rd if goal type is \"Itself\", in R{d+1} if goal type is \"Epigraph\""
    //       );
    //     writer.WriteLine("// \"DistToOrigin\" - the game with epigraph of the payoff function as distance to the origin.");
    //     writer.WriteLine
    //       ("// \"DistToPolytop\" - the game with epigraph of the payoff function as distance to the given polytop.");
    //     writer.WriteLine("MType = \"TerminalSet\";");
    //     writer.WriteLine();
    //
    //     WriteConstraintBlock(writer, "M", vM, vM);
    //   }
    // }
    //
    // public static void WriteSimplestTask_Payoff_Supergraphic_2D(string folderPath) {
    //   Vector vP = Vector.Ones(2);
    //   Vector vQ = TConv.FromDouble(0.5) * Vector.Ones(2);
    //   using (StreamWriter writer = new StreamWriter(folderPath + "simplestSupergraphic.c")) {
    //     writer.WriteLine("// Name of the problem");
    //     writer.WriteLine($"ProblemName = \"Cubes2D\";");
    //     writer.WriteLine();
    //     writer.WriteLine();
    //     writer.WriteLine("// ==================================================");
    //
    //     WriteSimplestDynamics(2, writer);
    //
    //     WriteConstraintBlock(writer, "P", vP, vP);
    //     WriteConstraintBlock(writer, "Q", vQ, vQ);
    //
    //     writer.WriteLine("// The goal type of the game");
    //     writer.WriteLine("// \"Itself\" - the game itself");
    //     writer.WriteLine("// \"Epigraph\" - the game with epigraphic of the payoff function");
    //     writer.WriteLine("GoalType = \"Epigraph\";");
    //     writer.WriteLine();
    //
    //     writer.WriteLine("// The type of the M");
    //     writer.WriteLine
    //       (
    //        "// \"TerminalSet\" - the explicit terminal set assigment. In Rd if goal type is \"Itself\", in R{d+1} if goal type is \"Epigraph\""
    //       );
    //     writer.WriteLine("// \"DistToOrigin\" - the game with epigraph of the payoff function as distance to the origin.");
    //     writer.WriteLine
    //       ("// \"DistToPolytop\" - the game with epigraph of the payoff function as distance to the given polytop.");
    //     writer.WriteLine("MType = \"DistToOrigin\";");
    //     writer.WriteLine();
    //
    //     writer.WriteLine("MBallType = \"Ball_1\";");
    //     writer.WriteLine("MCMax = 5;");
    //     writer.WriteLine();
    //     writer.WriteLine("// ==================================================");
    //   }
    // }
    //
    // private static void WriteConstraintBlock(TextWriter writer, string setType, Vector left, Vector right) {
    //   writer.WriteLine($"{setType}SetType = \"RectParallel\";");
    //   writer.WriteLine($"{setType}RectPLeft = {(-left).ToStringBraceAndDelim('{', '}', ',')};");
    //   writer.WriteLine($"{setType}RectPRight = {right.ToStringBraceAndDelim('{', '}', ',')};");
    //   writer.WriteLine();
    //   writer.WriteLine("// ==================================================");
    // }
    //
    // private static void WriteSimplestDynamics(int dim, TextWriter writer) {
    //   writer.WriteLine();
    //   writer.WriteLine("// Block of data defining the dynamics of the game");
    //   writer.WriteLine("// Dimension of the phase vector");
    //   writer.WriteLine($"n = {dim};");
    //   writer.WriteLine();
    //   writer.WriteLine("// The main matrix");
    //   writer.WriteLine($"A = {Matrix.Zero(dim)};");
    //   writer.WriteLine();
    //   writer.WriteLine("// Dimension of the useful control");
    //   writer.WriteLine($"pDim = {dim};");
    //   writer.WriteLine();
    //   writer.WriteLine("// The useful control matrix");
    //   writer.WriteLine($"B = {Matrix.Eye(dim)};");
    //   writer.WriteLine();
    //   writer.WriteLine("// Dimension of the disturbance");
    //   writer.WriteLine($"qDim = {dim};");
    //   writer.WriteLine();
    //   writer.WriteLine("// The disturbance matrix");
    //   writer.WriteLine($"C = {Matrix.Eye(dim)};");
    //   writer.WriteLine();
    //   writer.WriteLine("// The initial instant");
    //   writer.WriteLine("t0 = 0;");
    //   writer.WriteLine();
    //   writer.WriteLine("// The final instant");
    //   writer.WriteLine("T = 1;");
    //   writer.WriteLine();
    //   writer.WriteLine("// The time step");
    //   writer.WriteLine("dt = 0.2;");
    //   writer.WriteLine();
    //   writer.WriteLine("// The dimension of projected space");
    //   writer.WriteLine($"d = {dim};");
    //   writer.WriteLine();
    //   writer.WriteLine("// The indices to project onto");
    //   writer.Write("projJ = {");
    //   for (int i = 0; i < dim - 1; i++) {
    //     writer.Write($"{i}, ");
    //   }
    //   writer.Write($"{dim - 1}}};\n");
    //   writer.WriteLine();
    //
    //   writer.WriteLine("// ==================================================");
    // }

  }

}
