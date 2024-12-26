using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace LDG;

/// <summary>
/// Creates bridges for the game based on the given problem definition.
/// </summary>
public class BridgeCreator<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

#region Data
  public readonly LDGPathHolder<TNum, TConv> ph; // пути к основным папкам и словари-связки
  public readonly GameData<TNum, TConv>      gd; // данные по динамике игры
  public readonly TerminalSet<TNum, TConv>   ts; // данные о терминальном множестве
#endregion

  /// <summary>
  ///  Creates the main object responsible for constructing bridges.
  /// </summary>
  /// <param name="pathLDG">The root path for all data.</param>
  /// <param name="problemFileName">The name of the problem configuration file.</param>
  public BridgeCreator(string pathLDG, string problemFileName) {
    // Предполагаем, что структура папок LDG создана и корректна. Если это не так, вызвать SetUpDirectories.

    var problemReader = new Geometry<TNum, TConv>.ParamReader(Path.Combine(pathLDG, "Problems", problemFileName) + ".gameconfig");

    // Имя выходной папки совпадает с полем ProblemName в файле задачи
    ph =
      new LDGPathHolder<TNum, TConv>
        (pathLDG, problemReader.ReadString("ProblemName")); // установили пути и прочитали словари-связки

    // Читаем имена динамики и многогранников ограничений на управления игроков
    string dynName      = problemReader.ReadString("DynamicsName");
    string fpPolName    = problemReader.ReadString("FPName");
    var    fpTransform  = TransformReader<TNum, TConv>.ReadTransform(problemReader);
    string spPolName    = problemReader.ReadString("SPName");
    var    spTransform  = TransformReader<TNum, TConv>.ReadTransform(problemReader);
    string tmsName      = problemReader.ReadString("TSName");
    var    tmsTransform = TransformReader<TNum, TConv>.ReadTransform(problemReader);

    // заполняем динамику и многогранники ограничений на управления игроков
    Geometry<TNum, TConv>.ParamReader dynamicsReader   = ph.OpenDynamicsReader(dynName);
    Geometry<TNum, TConv>.ParamReader fpPolytopeReader = ph.OpenPolytopeReader(fpPolName);
    Geometry<TNum, TConv>.ParamReader spPolytopeReader = ph.OpenPolytopeReader(spPolName);
    gd = new GameData<TNum, TConv>(dynamicsReader, fpPolytopeReader, spPolytopeReader, fpTransform, spTransform);

    ts = new TerminalSet<TNum, TConv>(tmsName, ph, ref gd, tmsTransform);

    // Считаем и пишем хеш игры
    Geometry<TNum, TConv>.ParamWriter pw = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathGame, "game.md5hash"));
    pw.WriteString("Problem", RemoveComment(problemReader.GetSanitizedData()));
    pw.WriteString("Dynamic", RemoveComment(dynamicsReader.GetSanitizedData()));
    pw.WriteString("P", RemoveComment(fpPolytopeReader.GetSanitizedData()));
    pw.WriteString("Q", RemoveComment(spPolytopeReader.GetSanitizedData()));
    pw.WriteString("TerminalSet", RemoveComment(ph.OpenTerminalSetReader(tmsName).GetSanitizedData()));
    pw.Close();
  }

  /// <summary>
  /// Solves the game by creating and solving bridges for each terminal set.
  /// </summary>
  public void Solve() {
    StreamWriter sw = new StreamWriter(ph.PathBr + "!times.txt");
    while (ts.GetNextTerminalSet(out Geometry<TNum, TConv>.ConvexPolytop? tms)) {
      SolverLDG<TNum, TConv> slv =
        new SolverLDG<TNum, TConv>
          (
           ph,
           Path.Combine(ph.PathBr, ts.CurrI.ToString())
         , gd
         , tms!
          );
      slv.Solve();

      sw.WriteLine(slv.tMin);
      sw.Flush();
    }
    sw.Close();
  }


  /// <summary>
  /// Removes the part of the string that matches Comment="...";
  /// </summary>
  /// <param name="str">The input string to process.</param>
  /// <returns>The string with the 'Comment="...";' part removed.</returns>
  public static string RemoveComment(string str) { return Regex.Replace(str, "Comment=\".*?\";", ""); }

  // ---------------------------------------------

  /// <summary>
  /// Sets up the required directories for the game if they do not exist.
  /// </summary>
  /// <param name="ldgDir">The root directory where all data is stored.</param>
  public static void SetUpDirectories(string ldgDir) {
    Directory.CreateDirectory(Path.Combine(ldgDir, "_Out"));

    Directory.CreateDirectory(Path.Combine(ldgDir, "Dynamics"));
    string dynPath = Path.Combine(ldgDir, "Dynamics", "!Dict_dynamics.txt");
    if (!File.Exists(dynPath)) { File.Create(dynPath); }

    Directory.CreateDirectory(Path.Combine(ldgDir, "Polytopes"));
    string polPath = Path.Combine(ldgDir, "Polytopes", "!Dict_polytopes.txt");
    if (!File.Exists(polPath)) { File.Create(polPath); }

    Directory.CreateDirectory(Path.Combine(ldgDir, "Problems"));

    Directory.CreateDirectory(Path.Combine(ldgDir, "Terminal sets"));
    string tmsPath = Path.Combine(ldgDir, "Terminal sets", "!Dict_terminalsets.txt");
    if (!File.Exists(tmsPath)) { File.Create(tmsPath); }
  }
  // ---------------------------------------------

}
