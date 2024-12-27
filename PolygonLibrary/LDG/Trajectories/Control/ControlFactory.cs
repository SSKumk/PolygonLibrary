namespace LDG;

public abstract class ControlFactory<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public static IController<TNum, TConv> ReadFirstPlayer(
      Geometry<TNum, TConv>.ParamReader                                 pr
    , List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> Ws
    )
    => ReadControl(pr, "FPControl", true, Ws);

  public static IController<TNum, TConv> ReadSecondPlayer(
      Geometry<TNum, TConv>.ParamReader                                 pr
    , List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> Ws
    )
    => ReadControl(pr, "SPControl", false, Ws);

  private static IController<TNum, TConv> ReadControl(
      Geometry<TNum, TConv>.ParamReader                                 pr
    , string                                                            controlKey
    , bool                                                              isFirstPlayer
    , List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> Ws
    ) {
    string controlType = pr.ReadString(controlKey);

    return controlType switch
             {
               "Constant" => new ConstantControl<TNum, TConv>(pr)
             , "Optimal" => isFirstPlayer
                              ? new FirstPlayerOptimalControl<TNum, TConv>(Ws)
                              : new SecondPlayerOptimalControl<TNum, TConv>(Ws)
             , _ => throw new ArgumentException
                      (
                       $"Unsupported type of the control: '{controlType}'.\nIn file {pr.filePath}\n" +
                       $"Please refer to the documentation for supported types."
                      )
             };
  }

}
