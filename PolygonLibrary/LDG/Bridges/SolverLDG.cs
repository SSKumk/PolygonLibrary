using System.Diagnostics;
using System.Globalization;

namespace LDG;

/// <summary>
/// The solver responsible for solving a single game problem.
/// The solution includes computing the bridges, vectograms, and writing them to files.
/// Assume the correctness of the already existing files in the working directory: the problem is solved for the same dynamics, P, Q, and M as before, if it happened.
/// </summary>
public class SolverLDG<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Provides access to directories where bridge, Ps and Qs files will be located.
  /// </summary>
  public readonly LDGPathHolder<TNum, TConv> ph;

  /// <summary>
  /// Directory where the bridges are stored.
  /// </summary>
  public readonly string BrDir;

  /// <summary>
  /// Directory where the first player's vectograms are stored.
  /// </summary>
  public readonly string PsDir;

  /// <summary>
  /// Directory where the second player's vectograms are stored.
  /// </summary>
  public readonly string QsDir;

  /// <summary>
  /// The game data for the current problem.
  /// </summary>
  public readonly GameData<TNum, TConv> gd;

  /// <summary>
  /// The bridge of the game.
  /// </summary>
  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> W;

  /// <summary>
  /// Vectograms of the first player.
  /// </summary>
  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> Ps =
    new SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>(Geometry<TNum, TConv>.Tools.TComp);

  /// <summary>
  /// Vectograms of the second player.
  /// </summary>
  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> Qs =
    new SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>(Geometry<TNum, TConv>.Tools.TComp);

  /// <summary>
  /// The minimum time (greater than or equal to t0) when the bridge is successfully constructed.
  /// </summary>
  public TNum tMin;


  /// <summary>
  /// Reading the necessary parameters and setting up directories for solving the game.
  /// </summary>
  /// <param name="pathHolder">The path holder providing path to an all-game files.</param>
  /// <param name="bridgeDir">The directory where bridges are stored.</param>
  /// <param name="gameData">The game data used in the problem.</param>
  /// <param name="M">The terminal set of the game.</param>
  public SolverLDG(
      LDGPathHolder<TNum, TConv>          pathHolder
    , string                              bridgeDir
    , GameData<TNum, TConv>               gameData
    , Geometry<TNum, TConv>.ConvexPolytop M
    ) {
    ph = pathHolder;

    BrDir = bridgeDir;
    PsDir = ph.PathPs;
    QsDir = ph.PathQs;
    gd    = gameData;

    Debug.Assert
      (
       gd.ProjMatrix.Cols == gd.CauchyMatrix[gd.T].Rows
     , $"SolverLDG.Ctor: The dimensions of ProjMatrix and CauchyMatrix should coincides."
      );

    W       = new SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>(Geometry<TNum, TConv>.Tools.TComp);
    W[gd.T] = M;

    Directory.CreateDirectory(BrDir);
    Directory.CreateDirectory(PsDir);
    Directory.CreateDirectory(QsDir);

    WriteBridgeSection(gd.T);
  }

#region Read-Write
  // Reads the corresponding section for the given time section.

  private void ReadBridgeSection(TNum t) => ph.ReadSection(W, "W", BrDir, TConv.ToDouble(t));
  private void ReadPsSection(TNum     t) => ph.ReadSection(Ps, "P", PsDir, TConv.ToDouble(t));
  private void ReadQsSection(TNum     t) => ph.ReadSection(Qs, "Q", QsDir, TConv.ToDouble(t));

  // Write the corresponding section for the given time section.

  private void WriteBridgeSection(TNum t) => WriteSection("W", W, BrDir, t, Geometry<TNum, TConv>.ConvexPolytop.Rep.FLrep);
  private void WritePsSection(TNum     t) => WriteSection("P", Ps, PsDir, t, Geometry<TNum, TConv>.ConvexPolytop.Rep.FLrep);
  private void WriteQsSection(TNum     t) => WriteSection("Q", Qs, QsDir, t, Geometry<TNum, TConv>.ConvexPolytop.Rep.Vrep);

  // Checks if the corresponding section file is correct for the given time instant.

  private bool BridgeSectionFileCorrect(TNum t) => SectionFileExist("W", BrDir, t);
  private bool PsSectionFileExist(TNum       t) => SectionFileExist("P", PsDir, t);
  private bool QsSectionFileCorrect(TNum     t) => SectionFileExist("Q", QsDir, t);

  /// <summary>
  /// Writes the specified section of a convex polytope for a given time t.
  /// </summary>
  /// <param name="sectionPrefix">The prefix of the section, e.g. "W", "P", or "Q".</param>
  /// <param name="sectionDict">The dictionary containing the convex polytopes for each time instant.</param>
  /// <param name="basePath">The base directory path where the section is written.</param>
  /// <param name="t">The time instant for which the section is written.</param>
  /// <param name="repType">The representation type to be used for writing the convex polytope.</param>
  private void WriteSection(
      string                                                      sectionPrefix
    , SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> sectionDict
    , string                                                      basePath
    , TNum                                                        t
    , Geometry<TNum, TConv>.ConvexPolytop.Rep                     repType
    ) {
    Debug.Assert(sectionDict.ContainsKey(t), $"SolverLDG.WriteSection: There is no {sectionPrefix} section at time {t}.");
    using Geometry<TNum, TConv>.ParamWriter prW =
      new Geometry<TNum, TConv>.ParamWriter(ph.GetSectionPath(sectionPrefix, basePath, TConv.ToDouble(t)));
    sectionDict[t].WriteIn(prW, repType);
  }
#endregion

#region Aux
  /// <summary>
  /// Checks if the file for the specified section exists at the given time t.
  /// </summary>
  /// <param name="sectionPrefix">The prefix of the section (e.g., "W", "P", or "Q").</param>
  /// <param name="basePath">The base directory path where the section file is stored.</param>
  /// <param name="t">The time instant for which the section file is checked.</param>
  /// <returns><c>true</c> if the section file exists, <c>false</c> otherwise.</returns>
  private bool SectionFileExist(string sectionPrefix, string basePath, TNum t)
    => File.Exists(ph.GetSectionPath(sectionPrefix, basePath, TConv.ToDouble(t)));
#endregion


  /// <summary>
  /// Computes the next section of a convex polytope by the second Pontryagin's method.
  /// </summary>
  /// <param name="predSec">The previous section of the bridge.</param>
  /// <param name="currP">The first convex polytope (P) used in the Minkowski sum.</param>
  /// <param name="currQ">The second convex polytope (Q) used in the Minkowski difference.</param>
  /// <returns>The next section of the stable bridge, or null if the operation results in an invalid polytope.</returns>
  public Geometry<TNum, TConv>.ConvexPolytop? DoNextSection(
      Geometry<TNum, TConv>.ConvexPolytop predSec
    , Geometry<TNum, TConv>.ConvexPolytop currP
    , Geometry<TNum, TConv>.ConvexPolytop currQ
    ) {
    Debug.Assert
      (
       predSec.SpaceDim == currP.SpaceDim
     , $"SolverLDG.DoNextSection: The dimensions of spaces of polytopes should be the same! Found predSec = {predSec.SpaceDim}, currP = {currP.SpaceDim}!"
      );
    Debug.Assert
      (
       predSec.SpaceDim == currQ.SpaceDim
     , $"SolverLDG.DoNextSection: The dimensions of spaces of the polytopes should be the same! Found predSec = {predSec.SpaceDim}, currQ = {currQ.SpaceDim}!"
      );
    Geometry<TNum, TConv>.ConvexPolytop  sum  = Geometry<TNum, TConv>.MinkowskiSum.BySandipDas(predSec, currP, true);
    Geometry<TNum, TConv>.ConvexPolytop? next = Geometry<TNum, TConv>.MinkowskiDiff.Geometric(sum, currQ);

    return next;
  }

  /// <summary>
  /// Processes the section of the first player's vectogram (Ps) at the given time t.
  /// If the section file does not exist, it computes the required values and writes the new section file.
  /// </summary>
  /// <param name="t">The time at which the first player's vectogram section is processed.</param>
  private void ProcessPsSection(TNum t) {
    if (!PsSectionFileExist(t)) {
      TNum t1 = t;
      Ps[t] = Geometry<TNum, TConv>.ConvexPolytop.CreateFromPoints(gd.P.Vrep.Select(pPoint => -gd.dt * gd.D[t1] * pPoint), true);
      WritePsSection(t);
    }
  }

  /// <summary>
  /// Processes the section of the second player's vectogram (Qs) at the given time t.
  /// If the section file does not exist, it computes the required values and writes the new section file.
  /// </summary>
  /// <param name="t">The time at which the second player's vectogram section is processed.</param>
  private void ProcessQsSection(TNum t) {
    if (!QsSectionFileCorrect(t)) {
      TNum t1 = t;
      Qs[t] = Geometry<TNum, TConv>.ConvexPolytop.CreateFromPoints(gd.Q.Vrep.Select(qPoint => gd.dt * gd.E[t1] * qPoint), false);
      WriteQsSection(t);
    }
  }

  /// <summary>
  /// Processes the bridge section at the given time t by checking if the section is valid and not degenerate.
  /// It calculates the new bridge section if necessary and writes the new section file .
  /// </summary>
  /// <param name="t">The current time for which the bridge section is processed.</param>
  /// <param name="tPred">The previous time, used to calculate the next bridge section.</param>
  /// <param name="bridgeIsNotDegenerate">Indicates whether the bridge is not degenerate. Set to <c>false</c> if the bridge becomes degenerate.</param>
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
      Geometry<TNum, TConv>.ConvexPolytop? WNext = DoNextSection(W[tPred], Ps[t], Qs[t]);
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
  /// Solves the problem by iteratively processing the sections of the bridge at each time step.
  /// The process stops when the bridge becomes degenerate or when the time reaches an initial time.
  /// </summary>
  public void Solve() {
    Stopwatch timer = new Stopwatch();

    TNum t = gd.T;
    tMin = gd.T;
    TNum tPred;
    bool bridgeIsNotDegenerate = true;

    // Обрабатываем терминальный момент времени
    ProcessPsSection(t);
    ProcessQsSection(t);

    // Основной цикл
    while (Geometry<TNum, TConv>.Tools.GT(t, gd.t0) && bridgeIsNotDegenerate) {
      tPred =  t;
      tMin  =  t;
      t     -= gd.dt;
      Geometry<TNum, TConv>.ParamWriter prW = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(BrDir, ".tmin"));
      prW.WriteNumber("tMin", tMin);
      prW.Close();

      timer.Restart();
      ProcessPsSection(t);
      ProcessQsSection(t);
      ProcessBridgeSection(t, tPred, ref bridgeIsNotDegenerate);
      timer.Stop();

      if (W.TryGetValue(t, out Geometry<TNum, TConv>.ConvexPolytop? br)) {
        Console.WriteLine($"{TConv.ToDouble(t):F2}) DoNS = {timer.Elapsed.TotalSeconds:F4} sec. |Vrep| = {br.Vrep.Count}. |Hrep| = {br.Hrep.Count}");
      }
    }

    tMin -= gd.dt;

    {
      using Geometry<TNum, TConv>.ParamWriter prW = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(BrDir, ".tmin"));
      prW.WriteNumber("tMin", tMin);
    }
  }

}
