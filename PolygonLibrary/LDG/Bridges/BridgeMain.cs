using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CGLibrary.Toolkit;


namespace LDG;

/// <summary>
/// Creates bridges for the game based on the given problem definition.
/// </summary>
public class BridgeCreator<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

#region Data
  public readonly string NumType; // числовой тип

  public readonly LDGPathHolder<TNum, TConv> ph; // пути к основным папкам и словари-связки
  public readonly GameData<TNum, TConv>      gd; // данные по динамике игры
  public readonly TerminalSet<TNum, TConv>   ts; // данные о терминальном множестве

  public readonly TNum eps;    // точность
  public readonly TNum epsOld; // точность до вызова функций
#endregion

  /// <summary>
  ///  Creates the main object responsible for constructing bridges.
  /// </summary>
  /// <param name="pathLDG">The root path for all data.</param>
  /// <param name="problemFolderName">The name of the problem folder on _Out directory.</param>
  /// <param name="precision">The CGlibrary precision used in calculations.</param>
  public BridgeCreator(string pathLDG, string problemFolderName, TNum precision) {
    // Предполагаем, что структура папок LDG создана и корректна. Если это не так, вызвать SetUpDirectories.
    eps                             = precision;
    epsOld                          = Geometry<TNum, TConv>.Tools.Eps;
    Geometry<TNum, TConv>.Tools.Eps = eps;

    NumType = typeof(TNum).ToString();

    ph = new LDGPathHolder<TNum, TConv>(pathLDG, problemFolderName, NumType, precision); // установили пути и прочитали словари-связки
    Geometry<TNum, TConv>.ParamReader problemReader = ph.OpenProblemReader();

    // создаём нужные папки, если их нет
    Directory.CreateDirectory(ph.PathBrs);
    Directory.CreateDirectory(ph.PathTrajectories);
    Directory.CreateDirectory(ph.PathTrajConfigs);


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

    // получаем информацию об игре
    string problemInfo = GetInfo(problemReader);
    string dynamicInfo = GetInfo(dynamicsReader);
    string fpInfo      = GetInfo(fpPolytopeReader);
    string spInfo      = GetInfo(spPolytopeReader);
    string tmsInfo     = GetInfo(ph.OpenTerminalSetReader(tmsName));


    string gameInfoPath = Path.Combine(ph.PathGame, "game.md5hash");
    if (File.Exists(gameInfoPath)) {
      Geometry<TNum, TConv>.ParamReader pr              = ph.OpenGameInfoReader();
      string                            readProblemInfo = pr.ReadString("Problem");
      string                            readDynamicInfo = pr.ReadString("Dynamic");
      string                            readFpInfo      = pr.ReadString("P");
      string                            readSpInfo      = pr.ReadString("Q");
      string                            readTmsInfo     = pr.ReadString("TerminalSet");

      if (readProblemInfo != problemInfo) {
        throw new ArgumentException
          (
           $"BridgeCreator.Ctor: The problem information read from the file {gameInfoPath} does not match the corresponding data from the file {problemReader.filePath}."
          );
      }
      if (readDynamicInfo != dynamicInfo) {
        throw new ArgumentException
          (
           $"BridgeCreator.Ctor: The dynamic information read from the file {gameInfoPath} does not match the corresponding data from the file {dynamicsReader.filePath}."
          );
      }

      if (readFpInfo != fpInfo) {
        throw new ArgumentException
          (
           $"BridgeCreator.Ctor: The first player information read from the file {gameInfoPath} does not match the corresponding data from the file {fpPolytopeReader.filePath}."
          );
      }

      if (readSpInfo != spInfo) {
        throw new ArgumentException
          (
           $"BridgeCreator.Ctor: The second player information read from the file {gameInfoPath} does not match the corresponding data from the file {spPolytopeReader.filePath}."
          );
      }

      if (readTmsInfo != tmsInfo) {
        throw new ArgumentException
          (
           $"BridgeCreator.Ctor: The terminal set information read from the file {gameInfoPath} does not match the corresponding data from the file {ph.OpenTerminalSetReader(tmsName).filePath}."
          );
      }
    }
    else { // иначе пишем информацию об игре
      Geometry<TNum, TConv>.ParamWriter pw = new Geometry<TNum, TConv>.ParamWriter(gameInfoPath);
      pw.WriteString("Problem", problemInfo);
      pw.WriteString("Dynamic", dynamicInfo);
      pw.WriteString("P", fpInfo);
      pw.WriteString("Q", spInfo);
      pw.WriteString("TerminalSet", tmsInfo);
      pw.Close();
    }
    Geometry<TNum, TConv>.Tools.Eps = epsOld;
  }

  /// <summary>
  /// Solves the game by creating and solving bridges for each terminal set.
  /// </summary>
  public void Solve() {
    Geometry<TNum, TConv>.Tools.Eps = eps;
    while (ts.GetNextTerminalSet(out Geometry<TNum, TConv>.ConvexPolytop? tms)) { // tms -- (t)er(m)inal(s)et
      SolverLDG<TNum, TConv> slv = new SolverLDG<TNum, TConv>(ph, ph.PathBr(ts.CurrI), gd, tms!);
      slv.Solve();
      // try {
      // }
      // catch (Exception e) {
      //   Console.WriteLine($"BridgeMain.Solve: {e}");
      // }
    }
    Geometry<TNum, TConv>.Tools.Eps = epsOld;
  }


  /// <summary>
  /// Removes the part of the string that matches Comment="...";
  /// </summary>
  /// <param name="str">The input string to process.</param>
  /// <returns>The string with the 'Comment="...";' part removed.</returns>
  public static string RemoveComment(string str) { return Regex.Replace(str, "Comment=\".*?\";", ""); }

  /// <summary>
  /// Gets the information from a given param reader in uniform way. 
  /// </summary>
  /// <param name="pr">The param reader from which info has gotten.</param>
  /// <returns>The string without comments and space symbols.</returns>
  public static string GetInfo(Geometry<TNum, TConv>.ParamReader pr) => Hashes.GetMd5Hash(RemoveComment(pr.GetSanitizedData()));
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

    Directory.CreateDirectory(Path.Combine(ldgDir, "Terminal sets"));
    string tmsPath = Path.Combine(ldgDir, "Terminal sets", "!Dict_terminalsets.txt");
    if (!File.Exists(tmsPath)) { File.Create(tmsPath); }
  }
  // ---------------------------------------------

}
