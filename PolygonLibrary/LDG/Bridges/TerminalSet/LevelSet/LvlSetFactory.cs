namespace LDG;

public class LvlSetFactory<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {
  public static ILvlSetType<TNum,TConv> Read(Geometry<TNum,TConv>.ParamReader pr, LDGPathHolder<TNum,TConv> ph) {
    string lvlSet = pr.ReadString("Type");
    ILvlSetType<TNum,TConv> lvlSetType =
      lvlSet switch
        {
          "DistToPoint"    => new LvlSetTypes<TNum,TConv>.DistToPoint()
        , "DistToPolytope" => new LvlSetTypes<TNum,TConv>.DistToPolytope()
        , _ => throw new ArgumentException
                 (
                  $"Unsupported level set type: '{lvlSet}'.\nIn file {pr.filePath}\n" +
                  $"Please refer to the documentation for supported types."
                 )
        };
    
    lvlSetType.ReadParameters(pr, ph);
    return lvlSetType;
  }
}
