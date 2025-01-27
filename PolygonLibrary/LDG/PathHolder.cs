using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace LDG;

/// <summary>
/// Holds paths to the game data and provides methods to open data files.
/// </summary>
public class LDGPathHolder<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  private readonly string NumType;     // числовой тип
  private readonly string NumAccuracy; // текущая точность вычислений

#region Paths
  public readonly string PathLDG;          // корневая папка для всех данных
  public readonly string PathDynamics;     // Путь к папке с файлами динамик системы
  public readonly string PathPolytopes;    // Путь к папке с файлами многогранников
  public readonly string PathTerminalSets; // Путь к папке с файлами терминальных множеств

  public readonly string PathGame;         // Путь к папке игры
  public readonly string PathTrajectories; // Путь к папке с файлами траекторий
  public readonly string PathTrajConfigs;  // Путь к папке с файлами настроек траекторий

  public          string PathBr(int i) => Path.Combine(PathBrs, i.ToString(), NumType, NumAccuracy);
  public readonly string PathBrs; // Путь к папке, в которой лежат папки с мостами для разных терминальных множеств
  public readonly string PathPs; // Путь к папке, в которой лежат вектограммы первого игрока
  public readonly string PathQs; // Путь к папке, в которой лежат вектограммы второго игрока
#endregion

#region Dicts
  private readonly Dictionary<string, (int dynFileName, string comment)>
    name2dyn; // имя_динамики --> внутреннее имя файла динамики

  private readonly Dictionary<string, (int polFileName, string comment)>
    name2pol; // имя_многогранника --> внутреннее имя файла многогранника

  private readonly Dictionary<string, (int tmsFileName, string comment)>
    name2tms; // имя_терминального_множества --> внутреннее имя файла терминального множества
#endregion

  /// <summary>
  /// Initializes a new instance of the <see cref="LDGPathHolder{TNum, TConv}"/> class.
  /// </summary>
  /// <param name="pathLdg">The root path for all data.</param>
  /// <param name="problemFolderName">The name of the output folder.</param>
  /// <param name="numType">The numerical type.</param>
  /// <param name="numAccuracy">The numerical accuracy.</param>
  public LDGPathHolder(string pathLdg, string problemFolderName, string numType, TNum numAccuracy) {
    NumType     = numType;
    NumAccuracy = $"{TConv.ToDouble(numAccuracy):e0}";

    // глобальные пути
    PathLDG          = pathLdg;
    PathDynamics     = Path.Combine(PathLDG, "Dynamics");
    PathPolytopes    = Path.Combine(PathLDG, "Polytopes");
    PathTerminalSets = Path.Combine(PathLDG, "Terminal sets");

    // пути для данной игры
    PathGame         = Path.Combine(PathLDG, "_Out", problemFolderName);
    PathBrs          = Path.Combine(PathGame, "Br");
    PathPs           = Path.Combine(PathGame, "Ps", NumType, NumAccuracy);
    PathQs           = Path.Combine(PathGame, "Qs", NumType, NumAccuracy);
    PathTrajectories = Path.Combine(PathGame, "Trajectories");
    PathTrajConfigs  = Path.Combine(PathTrajectories, "!Configs");

    // пути к конкретным файлам

    // Считываем словари, переводящие имена во внутренние имена файлов
    name2dyn = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathDynamics, "!Dict_dynamics.txt")));
    name2pol = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathPolytopes, "!Dict_polytopes.txt")));
    name2tms = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathTerminalSets, "!Dict_terminalsets.txt")));
  }

  /// <summary>
  /// Reads a dictionary from a given parameter reader.
  /// </summary>
  /// <param name="pr">The parameter reader to read from.</param>
  /// <returns>A dictionary: Key -> (Value, Comment)</returns>
  private static Dictionary<string, (int fileName, string comment)> ReadDictionary(Geometry<TNum, TConv>.ParamReader pr) {
    Dictionary<string, (int fileName, string comment)> dict = new Dictionary<string, (int fileName, string comment)>();

    int k = pr.ReadNumber<int>("Qnt");
    for (int i = 0; i < k; i++) {
      try {
        string key      = pr.ReadString("Key");
        int    fileName = pr.ReadNumber<int>("Value");
        string comment  = pr.ReadString("Comment");
        dict.Add(key, (fileName, comment));
      }
      catch (Exception e) {
        Console.WriteLine
          (
           $"An error occurred while reading the file {pr.filePath}! Please check the Qnt field. The program read the value = {k}.\n"
          );


        throw;
      }
    }

    return dict;
  }

#region Open reader
  /// <summary>
  /// Opens a reader for the game problem file.
  /// </summary>
  /// <returns>A parameter reader for the game problem file.</returns>
  public Geometry<TNum, TConv>.ParamReader OpenProblemReader() => new(Path.Combine(PathGame, ".gameconfig"));

  /// <summary>
  /// Opens a reader for the game hash file.
  /// </summary>
  /// <returns>A parameter reader for the game hash file.</returns>
  public Geometry<TNum, TConv>.ParamReader OpenGameInfoReader()
    => new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathGame, "game.md5hash"));

  /// <summary>
  /// Opens a reader for a dynamics file.
  /// </summary>
  /// <param name="name">The name of the dynamics.</param>
  /// <returns>A parameter reader for the dynamics file.</returns>
  public Geometry<TNum, TConv>.ParamReader OpenDynamicsReader(string name)
    => new(Path.Combine(PathDynamics, name2dyn[name].dynFileName + ".gamedata"));

  /// <summary>
  /// Opens a reader for a polytope file.
  /// </summary>
  /// <param name="name">The name of the polytope.</param>
  /// <returns>A parameter reader for the polytope file.</returns>
  public Geometry<TNum, TConv>.ParamReader OpenPolytopeReader(string name)
    => new(Path.Combine(PathPolytopes, name2pol[name].polFileName + ".polytope"));

  /// <summary>
  /// Opens a reader for a terminal set file.
  /// </summary>
  /// <param name="name">The name of the terminal set.</param>
  /// <returns>A parameter reader for the terminal set file.</returns>
  public Geometry<TNum, TConv>.ParamReader OpenTerminalSetReader(string name)
    => new(Path.Combine(PathTerminalSets, name2tms[name].tmsFileName + ".terminalset"));

  /// <summary>
  /// Opens a reader for the trajectory config file.
  /// </summary>
  /// <param name="name">The name of the trajectory configuration file.</param>
  /// <returns>A parameter reader for the trajectory config file.</returns>
  public Geometry<TNum, TConv>.ParamReader OpenTrajConfigReader(string name)
    => new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathTrajConfigs, name + ".trajconfig"));
#endregion

#region Load
  public void LoadBridgeSection(SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> bridge, int i, double t)
    => ReadSection(bridge, "W", PathBr(i), t);

  public SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> LoadBridge(int i) {
    var    bridge     = new SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>(Geometry<TNum, TConv>.Tools.TComp);
    string bridgePath = PathBr(i);

    if (!Directory.Exists(bridgePath)) {
      throw new ArgumentException($"Warning: PathHolder.LoadBridge: Bridge does not exist: {bridgePath}");
    }

    string[] sectionFiles = Directory.GetFiles(bridgePath, "*.wsection"); // Получаем все .wsection файлы

    foreach (string sectionFile in sectionFiles) {
      string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sectionFile); // Имя файла без расширения
      if (double.TryParse(fileNameWithoutExtension, NumberStyles.Float, CultureInfo.InvariantCulture, out double t)) {
        LoadBridgeSection(bridge, i, t); // Вызываем LoadBridgeSection для каждого файла
      }
      else {
        Console.WriteLine
          (
           $"Warning: PathHolder.LoadBridge: Invalid filename format for section file: {sectionFile}. Cannot parse time from filename."
          );
      }
    }

    return bridge;
  }

  public List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> LoadBridges() {
    var Ws = new List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>>();

    int k = Directory.GetDirectories(PathBrs).Length;

    for (int i = 0; i < k; i++) {
      Ws.Add(LoadBridge(i));
    }

    return Ws;
  }


  public TNum Load_tMin(int i) {
    Geometry<TNum, TConv>.ParamReader pr = new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathBr(i), ".tmin"));

    return pr.ReadNumber<TNum>("tMin");
  }

  public Dictionary<int, TNum> LoadMinimalTimes() {
    Dictionary<int, TNum> minTimes = new Dictionary<int, TNum>();

    int k = Directory.GetDirectories(PathBrs).Length;

    for (int i = 0; i < k; i++) {
      minTimes.Add(i, Load_tMin(i));
    }

    return minTimes;
  }
#endregion


#region Aux
  /// <summary>
  /// Constructs the file path for the specified section at a given time t.
  /// </summary>
  /// <param name="sectionPrefix">The prefix of the section (e.g., "W", "P", or "Q").</param>
  /// <param name="basePath">The base directory path where the section file is located.</param>
  /// <param name="t">The time instant for which the section file is constructed.</param>
  /// <returns>The full path of the section file corresponding to the given time.</returns>
  /// <exception cref="ArgumentException">Thrown if the sectionPrefix is invalid (i.e., not "W", "P", or "Q").</exception>
  public string GetSectionPath(string sectionPrefix, string basePath, double t) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    string prefix =
      sectionPrefix switch
        {
          "W" => "w"
        , "P" => "p"
        , "Q" => "q"
        , _   => throw new ArgumentException($"Unknown section prefix: '{sectionPrefix}'. Expected 'W', 'P', or 'Q'.")
        };

    return Path.Combine(basePath, $"{t:F2}.{prefix}section");
  }

  /// <summary>
  /// Reads the specified section of a convex polytope for a given time t from a file.
  /// </summary>
  /// <param name="sectionDict">The dictionary where the read convex polytope will be stored for the specified time.</param>
  /// <param name="sectionPrefix">The prefix of the section, e.g. "W", "P", or "Q".</param>
  /// <param name="basePath">The base directory path from which the section is read.</param>
  /// <param name="t">The time instant for which the section is read.</param>
  public void ReadSection(
      SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> sectionDict
    , string                                                      sectionPrefix
    , string                                                      basePath
    , double                                                      t
    ) {
    string filePath = GetSectionPath(sectionPrefix, basePath, t);
    Debug.Assert
      (File.Exists(filePath), $"LDG.PathHolder.ReadSection: There is no {sectionPrefix} section at time {t}. File: {filePath}");

    Geometry<TNum, TConv>.ParamReader prR = new Geometry<TNum, TConv>.ParamReader(filePath);
    sectionDict.Add(TConv.FromDouble(t), Geometry<TNum, TConv>.ConvexPolytop.CreateFromReader(prR));
  }
#endregion

}
