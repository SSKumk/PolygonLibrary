using Bridges.EpigraphType;
namespace Bridges;

public class EpigraphTS<TNum, TConv> : ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop> ReadTerminalSets(
      Geometry<TNum, TConv>.ParamReader pr
    , LDGPathHolder<TNum, TConv>        ph) {
    IEpiType<TNum, TConv> epiType = EpiTypeFactory<TNum, TConv>.Read(pr, ph);

    TNum k = pr.ReadNumber<TNum>("Constant");
    if (Geometry<TNum, TConv>.Tools.LE(k)) {
      throw new ArgumentException($"Constant must be greater than zero. Found Constant = {k}");
    }
    Geometry<TNum, TConv>.IBall ballType = Geometry<TNum, TConv>.BallFactory.Read(pr);
    switch (epiType) {
      case EpiTypes<TNum, TConv>.DistToPoint d: {
        yield return
          ballType switch
            {
              Geometry<TNum, TConv>.Ball_1    => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPointBall_1(d.Point, k)
            , Geometry<TNum, TConv>.Ball_2 b2 => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPointBall_2(d.Point, b2.PolarDivision, b2.AzimuthsDivisions, k)
            , Geometry<TNum, TConv>.Ball_oo   => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPointBall_oo(d.Point, k)
            };

        break;
      }

      case EpiTypes<TNum, TConv>.DistToPolytope d: {
        yield return
          ballType switch
            {
              Geometry<TNum, TConv>.Ball_1    => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopeBall_1(d.Polytope, k)
            , Geometry<TNum, TConv>.Ball_2 b2 => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopeBall_2(d.Polytope, k, b2.PolarDivision, b2.AzimuthsDivisions)
            , Geometry<TNum, TConv>.Ball_oo   => Geometry<TNum, TConv>.ConvexPolytop.DistanceToPolytopeBall_oo(d.Polytope, k)
            };

        break;
      }
    }
  }

  public EpigraphTS(ref Geometry<TNum, TConv>.GameData gd) {
    // Расширяем систему, если решаем задачу с надграфиком функции цены
    gd.projDim++;
    gd.projInd = new List<int>(gd.projInd)
      {
        gd.n
      }.ToArray(); // новая координата всегда в ответе
    gd.n++;        // размерность стала на 1 больше
    gd.A            = Geometry<TNum, TConv>.Matrix.vcat(gd.A, Geometry<TNum, TConv>.Matrix.Zero(1, gd.n - 1));
    gd.A            = Geometry<TNum, TConv>.Matrix.hcat(gd.A, Geometry<TNum, TConv>.Matrix.Zero(gd.n, 1))!;
    gd.B            = Geometry<TNum, TConv>.Matrix.vcat(gd.B, Geometry<TNum, TConv>.Matrix.Zero(1, gd.pDim));
    gd.C            = Geometry<TNum, TConv>.Matrix.vcat(gd.C, Geometry<TNum, TConv>.Matrix.Zero(1, gd.qDim));
    gd.CauchyMatrix = new Geometry<TNum, TConv>.CauchyMatrix(gd.A, gd.T, gd.dt);
  }

}
