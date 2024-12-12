namespace Bridges;

public class LevelSetTS<TNum, TConv> : ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  private enum LevelSetType { DistToPoint, DistToPolytope }

  
  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop>
    ReadTerminalSets(Geometry<TNum, TConv>.ParamReader pr
                   , LDGPathHolder<TNum, TConv>        dh
                   , Geometry<TNum, TConv>.GameData    gd) {
    string type = pr.ReadString("Type");
    LevelSetType epiType =
      type switch
        {
          "DistToPoint"    => LevelSetType.DistToPoint
        , "DistToPolytope" => LevelSetType.DistToPolytope
        , _ => throw new ArgumentException
                 (
                  $"Bridges...ReadTerminalSets.LevelSetTS: " +
                  $"Unsupported level set type: '{type}'.\nIn file {pr.filePath}\n" +
                  $"Please refer to the documentation for supported types."
                 )
        };

    Geometry<TNum, TConv>.Vector        point    = Geometry<TNum, TConv>.Vector.Zero(1);
    Geometry<TNum, TConv>.ConvexPolytop polytope = Geometry<TNum, TConv>.ConvexPolytop.Zero();

    switch (epiType) {
      case LevelSetType.DistToPoint:    point    = pr.ReadVector("Point"); break;
      case LevelSetType.DistToPolytope: polytope = ITerminalSetReader<TNum, TConv>.DoPolytope(pr.ReadString("Name"), dh); break;
    }
    
    TNum[] ks = pr.ReadVector("Constants").GetAsArray();
    if (ks.Any(Geometry<TNum,TConv>.Tools.LE)) {
      throw new ArgumentException($"Bridges...ReadTerminalSets.LevelSetTS: " +
                                  $"All 'Constants' must be greater than zero.");

    }
    Geometry<TNum,TConv>.BallType ballTypeStr = 
      Geometry<TNum,TConv>.ReadHelper.ReadBallType(pr, out int? polarDivisions, out int? azimuthsDivisions);

  }
}
