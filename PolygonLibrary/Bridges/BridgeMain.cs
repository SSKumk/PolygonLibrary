using System.Globalization;


namespace Bridges;
// todo: ПОКА ЗАБИЛ НА ВСЯКИЕ ХЕШИ, ПРОВЕРКИ КОРРЕКТНОСТИ И ПРОЧЕЕ!!!

// todo: Если есть файл описания объектов, то с ним работать надо АККУРАТНО! исправляя только то, что изменилось. Возможно стоит втянуть ВСЁ. !!!

public class LDGPathHolder<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly string PathLDG;          // корневая папка для всех данных
  public readonly string PathDynamics;     // Путь к папке с файлами динамик системы
  public readonly string PathPolytopes;    // Путь к папке с файлами многогранников
  public readonly string PathTerminalSets; // Путь к папке с файлами терминальных множеств

  public readonly string PathBr; // Путь к папке, в которой лежат папки с мостами для разных терминальных множеств
  public readonly string PathPs; // Путь к папке, в которой лежат вектограммы первого игрока
  public readonly string PathQs; // Путь к папке, в которой лежат вектограммы второго игрока

  public readonly Dictionary<string, (int dynFileName, string comment)>
    name2dyn; // имя_динамики --> внутреннее имя файла динамики

  public readonly Dictionary<string, (int polFileName, string comment)>
    name2pol; // имя_многогранника --> внутреннее имя файла многогранника

  public readonly Dictionary<string, (int tmsFileName, string comment)>
    name2tms; // имя_терминального_множества --> внутреннее имя файла терминального множества

  public LDGPathHolder(string pathLdg, string outputFolderName) {
    PathLDG          = pathLdg;
    PathDynamics     = Path.Combine(PathLDG, "Dynamics");
    PathPolytopes    = Path.Combine(PathLDG, "Polytopes");
    PathTerminalSets = Path.Combine(PathLDG, "Terminal sets");

    string pathOut = Path.Combine(PathLDG, "_Out", outputFolderName);
    PathBr = Path.Combine(pathOut, "Br");
    PathPs = Path.Combine(pathOut, "Ps");
    PathQs = Path.Combine(pathOut, "Qs");

    // Считываем словари, переводящие имена во внутренние имена файлов
    name2dyn = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathDynamics, "!Dict_dynamics.txt")));
    name2pol = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathPolytopes, "!Dict_polytopes.txt")));
    name2tms = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathTerminalSets, "!Dict_terminalsets.txt")));
  }

  private static Dictionary<string, (int fileName, string comment)> ReadDictionary(Geometry<TNum, TConv>.ParamReader pr) {
    Dictionary<string, (int fileName, string comment)> dict = new Dictionary<string, (int fileName, string comment)>();

    int k = pr.ReadNumber<int>("Qnt");
    for (int i = 0; i < k; i++) {
      string key      = pr.ReadString("Key");
      int    fileName = pr.ReadNumber<int>("Value");
      string comment  = pr.ReadString("Comment");
      dict.Add(key, (fileName, comment));
    }

    return dict;
  }

  public Geometry<TNum, TConv>.ParamReader OpenDynamicsReader(string name)
    => new(Path.Combine(PathDynamics, name2dyn[name].dynFileName + ".gamedata"));

  public Geometry<TNum, TConv>.ParamReader OpenPolytopeReader(string name)
    => new(Path.Combine(PathPolytopes, name2pol[name].polFileName + ".polytope"));

  public Geometry<TNum, TConv>.ParamReader OpenTerminalSetReader(string name)
    => new(Path.Combine(PathTerminalSets, name2tms[name].tmsFileName + ".terminalset"));

}

class BridgeCreator<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

#region Data
  public readonly LDGPathHolder<TNum, TConv>     ph; // пути к основным папкам и словари-связки
  public readonly Geometry<TNum, TConv>.GameData gd; // данные по динамике игры
  public readonly TerminalSet<TNum, TConv>       ts; // данные о терминальном множестве
#endregion

  public BridgeCreator(string pathLDG, string problemFileName) {
    // Предполагаем, что структура папок LDG создана и корректна. Если это не так, вызвать SetUpDirectories.

    var pr = new Geometry<TNum, TConv>.ParamReader(Path.Combine(pathLDG, "Problems", problemFileName) + ".gameconfig");

    // Имя выходной папки совпадает с полем ProblemName в файле задачи
    ph = new LDGPathHolder<TNum, TConv>(pathLDG, pr.ReadString("ProblemName")); // установили пути и прочитали словари-связки

    // Читаем имена динамики и многогранников ограничений на управления игроков
    string                                          dynName      = pr.ReadString("DynamicsName");
    string                                          fpPolName    = pr.ReadString("FPName");
    Geometry<TNum, TConv>.TransformReader.Transform fpTransform  = Geometry<TNum, TConv>.TransformReader.ReadTransform(pr);
    string                                          spPolName    = pr.ReadString("SPName");
    Geometry<TNum, TConv>.TransformReader.Transform spTransform  = Geometry<TNum, TConv>.TransformReader.ReadTransform(pr);
    string                                          tmsName      = pr.ReadString("TSName");
    Geometry<TNum, TConv>.TransformReader.Transform tmsTransform = Geometry<TNum, TConv>.TransformReader.ReadTransform(pr);

    // заполняем динамику и многогранники ограничений на управления игроков
    gd =
      new Geometry<TNum, TConv>.GameData
        (
         ph.OpenDynamicsReader(dynName)
       , ph.OpenPolytopeReader(fpPolName)
       , ph.OpenPolytopeReader(spPolName)
       , fpTransform
       , spTransform
        );

    ts = new TerminalSet<TNum, TConv>(tmsName, ph, ref gd, tmsTransform);
  }

  public void Solve() {
    int i = 1;
    while (ts.GetNextTerminalSet(out Geometry<TNum, TConv>.ConvexPolytop? tms)) {
      Geometry<TNum, TConv>.SolverLDG slv =
        new Geometry<TNum, TConv>.SolverLDG(Path.Combine(ph.PathBr, i.ToString()), ph.PathPs, ph.PathQs, gd, tms!);
      slv.Solve();
      i++;
    }
  }


  // ---------------------------------------------

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
