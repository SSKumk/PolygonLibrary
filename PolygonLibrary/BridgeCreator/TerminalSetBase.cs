using System.Numerics;
using CGLibrary;

namespace BridgeCreator;

public abstract class TerminalSetBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public string TerminalSetName; // имя терминального множества в выходном файле


  protected TerminalSetBase(Geometry<TNum, TConv>.ParamReader pr) { TerminalSetName = pr.ReadString("Name"); }

  public static readonly string NumericalType = typeof(TNum).ToString(); // текущий используемый числовой тип
  public static readonly string Eps = $"{TConv.ToDouble(Geometry<TNum, TConv>.Tools.Eps):e0}"; // текущая точность в библиотеке


  public abstract void Solve(
      string                         baseWorkDir
    , Geometry<TNum, TConv>.GameData gameData
    , int                            projDim
    , int[]                          projInd
    , string                         gameInfoNoTerminalInfo
    , string                         PsInfo
    , string                         QsInfo
    );

  // public          Geometry<TNum, TConv>.ConvexPolytop M; // для случая когда терминальное множество одно

  // public          string                              TSetInfo;


  // public SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> Ms =

  //   new SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>

  //     (Geometry<TNum, TConv>.Tools.TComp); // для случая нарезки по множествам уровня функции платы


  // public TerminalSetBase(Geometry<TNum, TConv>.ParamReader paramReader) {
  //   pr              = paramReader;
  //   TerminalSetName = pr.ReadString("Name");
  //
  //   projDim = pr.ReadNumber<int>("ProjDim");
  //   projInd = pr.Read1DArray<int>("ProjInd", projDim);
  //
  //   ProjInfo = $"{projDim}{string.Join(';', projInd)}";
  //
  //
  //   IsLevelSet = pr.ReadString("IsLevelSet") switch
  //                  {
  //                    "True"  => true
  //                  , "False" => false
  //                  , _       => throw new ArgumentException("GameData.Ctor: IsLevelSet should be \"True\" or \"False\"")
  //                  };
  //
  //   if (!IsLevelSet) {
  //     // todo: пока что не работаем с надграфиком функции, тут надо о чём-то думать
  //     M = Geometry<TNum, TConv>.GameData.ReadExplicitSet(pr, 'M', projDim, out TSetInfo);
  //   }
  //   else {
  //     throw new NotImplementedException("Множество уровня пока не сделано!");
  //   }
  // }

  //
  // private Dictionary<TNum, Geometry<TNum,TConv>.ConvexPolytop> ReadSetLevels() {
  //
  //  }


  // // Случай игры с надграфиком функции платы
  // case Geometry<TNum, TConv>.GameData.GoalType.PayoffEpigraph:
  //   switch (_MType) {
  //     // Явно дано терминальное множество расширенной системы
  //     case MType.Ordinary: {
  //       Geometry<TNum, TConv>.ConvexPolytop readM = ReadExplicitSet('M', d + 1, out string MSetTypeInfo);
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
