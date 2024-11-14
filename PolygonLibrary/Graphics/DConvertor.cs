using CGLibrary;

namespace Graphics;

/// <summary>
/// Interface for double-conversions.
/// </summary>
public class DConvertor : INumConvertor<double> {

  public static double ToDouble(double   from) => from;
  public static double FromDouble(double from) => from;
  public static int    ToInt(double      from) => (int)from;
  public static double FromInt(int       from) => from;
  public static uint   ToUInt(double     from) => (uint)from;
  public static double FromUInt(uint     from) => from;

}
