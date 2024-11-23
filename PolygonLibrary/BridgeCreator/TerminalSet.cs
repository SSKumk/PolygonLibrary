using System.Numerics;
using CGLibrary;

namespace BridgeCreator;

public class TerminalSet<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  // public enum TSType {
  //
  //   Ordinary
  // , Payoff_DistToOrigin
  // , Payoff_DistToPolytop
  // , SetLevel_Payoff
  // , Minkowski
  //
  // }

  public readonly Geometry<TNum, TConv>.GameData.SetType TStype;
  public readonly bool                                   IsLevelSet;

  public readonly int   projDim;
  public readonly int[] projInd;

  public readonly string ProjInfo;

  public readonly string                              TSName;
  public          Geometry<TNum, TConv>.ConvexPolytop M; // для случая когда терминальное множество одно
  public          string                              TSetInfo;

  public SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> Ms =
    new SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>
      (Geometry<TNum, TConv>.Tools.TComp); // для случая нарезки по множествам уровня функции платы

  public readonly Geometry<TNum, TConv>.ParamReader pr;

  // Формат записи в файле
  // Name = "some_name";
  // ProjDim = 2;
  // ProjInd = [0, 1];
  // IsLevelSet = False;
  // Дальше устройство записи зависит от ответа на IsLevelSet
  /*
   * Если IsLevelSet == False, то
   * MSetType = ; "ConvexPolytope" / "RectParallel" / "Sphere" / "Ellipsoid"
   * Далее для каждого из этих типов идут свои настройки
   */
  // "ConvexPolytope"
  /*
   * Тут проще всего, просто формат, в котором он пишется в файл библиотекой
   * Rep = ; "FLrep" / "Vrep" / "Hrep"
   * ...
   */
  // "RectParallel"
  /*
   * Количество координат должно совпадать с ProjDim
   * MRectPLeft = {-1,-1,-1}; Координаты левой нижней точки
   * MRectPRight = {1, 1, 1}; Координаты правой верхней точки
   */
  // Sphere
  /*
   * MTheta = 7; количество точек по зенитному углу. [0, Pi] (в 2д ни на что не влияет)
   * MPhi = 12;  количество точек по каждому азимутному углу. [0, 2*Pi)
   * MCenter = {0,0,0}; Координаты центра
   * MRadius = 0.9; Значение радиуса
   */
  // Ellipsoid
  /*
   * MTheta = 7; количество точек по зенитному углу. [0, Pi] (в 2д ни на что не влияет)
   * MPhi = 12;  количество точек по каждому азимутному углу. [0, 2*Pi)
   * MCenter = {0,0,0}; Координаты центра
   * SemiaxesLength = {1,2,3}; Длины каждой из осей
   */
  // todo: описать другие множества
  // Если IsLevelSet == True
  /*
   * todo: А чего множество уровня?
   *
   */

  public TerminalSet(Geometry<TNum, TConv>.ParamReader paramReader) {
    pr     = paramReader;
    TSName = pr.ReadString("Name");

    projDim = pr.ReadNumber<int>("ProjDim");
    projInd = pr.Read1DArray<int>("ProjInd", projDim);

    ProjInfo = $"{projDim}{string.Join(';', projInd)}";


    IsLevelSet = pr.ReadString("IsLevelSet") switch
                   {
                     "True"  => true
                   , "False" => false
                   , _       => throw new ArgumentException("GameData.Ctor: IsLevelSet should be \"True\" or \"False\"")
                   };

    if (!IsLevelSet) {
      // todo: пока что не работаем с надграфиком функции, тут надо о чём-то думать
      M = Geometry<TNum, TConv>.GameData.ReadSet(pr, 'M', projDim, out TSetInfo);
    }
    else {
      throw new NotImplementedException("Множество уровня пока не сделано!");
    }
  }

  //
  // private Dictionary<TNum, Geometry<TNum,TConv>.ConvexPolytop> ReadSetLevels() {
  //
  //  }


  // // Случай игры с надграфиком функции платы
  // case Geometry<TNum, TConv>.GameData.GoalType.PayoffEpigraph:
  //   switch (_MType) {
  //     // Явно дано терминальное множество расширенной системы
  //     case MType.Ordinary: {
  //       Geometry<TNum, TConv>.ConvexPolytop readM = ReadSet('M', d + 1, out string MSetTypeInfo);
  //       describeTS += MSetTypeInfo;
  //
  //       return readM;
  //     }
  //
  //
  //     // Множество в виде расстояния до начала координат
  //     case MType.Payoff_DistToOrigin: {
  //       describeTS += "DtnOrigin_";
  //       string BallType = _pr.ReadString("MBallType");
  //       describeTS += BallType;
  //       int Theta = 10, Phi = 10;
  //       if (BallType == "Ball_2") {
  //         Theta = _pr.ReadNumber<int>("MTheta");
  //         Phi   = _pr.ReadNumber<int>("MPhi");
  //
  //         describeTS += $"-T{Theta}-P{Phi}_";
  //       }
  //       TNum CMax = _pr.ReadNumber<TNum>("MCMax");
  //       describeTS += $"-CMax{CMax}";
  //
  //       return BallType switch
  //                {
  //                  "Ball_1"  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToOriginBall_1(d, CMax)
  //                , "Ball_2"  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToOriginBall_2(d, Theta, Phi, CMax)
  //                , "Ball_oo" => Geometry<TNum, TConv>.ConvexPolytop.DistanceToOriginBall_oo(d, CMax)
  //                , _         => throw new ArgumentOutOfRangeException($"Wrong type of the ball! Found {BallType}")
  //                };
  //     }
  //
  //     // Множество в виде расстояния до заданного выпуклого многогранника в Rd
  //     case MType.Payoff_distToPolytop: {
  //       describeTS += "DtnPolytop_";
  //       int                       VsQnt    = _pr.ReadNumber<int>("MVsQnt");
  //       TNum[,]                   Vs       = _pr.Read2DArray<TNum>("MPolytop", VsQnt, d);
  //       Geometry<TNum, TConv>.ConvexPolytop Polytop  = Geometry<TNum, TConv>.ConvexPolytop.CreateFromPoints(Array2DToSortedSet(Vs, VsQnt, d));
  //       string                    BallType = _pr.ReadString("MBallType");
  //       describeTS += $"Vs-Qnt{VsQnt}_{BallType}";
  //       int Theta = 10, Phi = 10;
  //       if (BallType == "Ball_2") {
  //         Theta = _pr.ReadNumber<int>("MTheta");
  //         Phi   = _pr.ReadNumber<int>("MPhi");
  //       }
  //       TNum CMax = _pr.ReadNumber<TNum>("MCMax");
  //       describeTS += $"-CMax{CMax}";
  //
  //       return BallType switch
  //                {
  //                  "Ball_1"  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_1(Polytop, CMax)
  //                , "Ball_2"  => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_2(Polytop, Theta, Phi, CMax)
  //                , "Ball_oo" => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopBall_oo(Polytop, CMax)
  //                , _         => throw new ArgumentException($"Wrong type of the ball! Found {BallType}")
  //                };
  //     }
  //
  //
  //     // Другие варианты функции платы
  //     // case :
  //
  //     default: throw new ArgumentException("Другие варианты непредусмотрены!");
  //   }

  //

}
