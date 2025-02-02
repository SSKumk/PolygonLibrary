namespace LDG;

public abstract class ControlFactory<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public static IController<TNum, TConv> ReadFirstPlayer(
      Geometry<TNum, TConv>.ParamReader                                 pr
    , List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> Ws
    , GameData<TNum, TConv>                                             gd
    )
    => ReadControl(pr, "FPControl", true, Ws, gd);

  public static IController<TNum, TConv> ReadSecondPlayer(
      Geometry<TNum, TConv>.ParamReader                                 pr
    , List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> Ws
    , GameData<TNum, TConv>                                             gd
    )
    => ReadControl(pr, "SPControl", false, Ws, gd);

  private static IController<TNum, TConv> ReadControl(
      Geometry<TNum, TConv>.ParamReader                                 pr
    , string                                                            controlKey
    , bool                                                              isFirstPlayer
    , List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> Ws
    , GameData<TNum, TConv>                                             gd
    ) {
    string controlType = pr.ReadString(controlKey);

    return controlType switch
             {
               "Constant" => isFirstPlayer
                               ? new FirstPlayerConstantControl<TNum, TConv>(pr,gd)
                               : new SecondPlayerConstantControl<TNum, TConv>(pr, gd)
             , "Optimal" => isFirstPlayer
                              ? new FirstPlayerOptimalControl<TNum, TConv>(Ws, gd)
                              : new SecondPlayerOptimalControl<TNum, TConv>(Ws, gd)
             , "Random_RectAxisParallel" => isFirstPlayer
                             ? new FirstPlayerRandomControl_RectParallel<TNum, TConv>(pr, gd)
                             : new SecondPlayerRandomControl_RectParallel<TNum, TConv>(pr, gd)

             , _ => throw new ArgumentException
                      (
                       $"Unsupported type of the control: '{controlType}'.\nIn file {pr.filePath}\n" +
                       $"Please refer to the documentation for supported types."
                      )
             };
  }

}
