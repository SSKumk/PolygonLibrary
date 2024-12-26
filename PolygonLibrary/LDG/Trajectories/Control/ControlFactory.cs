namespace LDG;

public abstract class ControlFactory<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  // todo: Как сделать тут красиво? Типа у нас есть fp и sp и два параметра optimal и constant,
  // причём constant общий, а оптимальный индивидуальный хочется чтобы это всё был просто IController, но как правильно создавать классы?

  public static IController<TNum, TConv> Read(string keyName, Geometry<TNum, TConv>.ParamReader pr, LDGPathHolder<TNum, TConv> ph) {
    string                   controlType = pr.ReadString(keyName);
    IController<TNum, TConv> controller;

    if (controlType == "Optimal") {

    }
    else {
      return controlType switch
                   {
                     "Constant" => new ConstantControl<TNum, TConv>(),
                     _ => throw new ArgumentException
                            (
                             $"Unsupported type of the control: '{controlType}'.\nIn file {pr.filePath}\n" +
                             $"Please refer to the documentation for supported types."
                            )
                   };
    }

  }

}
