using DoubleDouble;

namespace Bridges;

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

/// <summary>
/// Interface for ddouble-conversions.
/// </summary>
public class DDConvertor : INumConvertor<ddouble> {

  public static double  ToDouble(ddouble  from) => (double)from;
  public static ddouble FromDouble(double from) => from;
  public static int     ToInt(ddouble     from) => (int)from;
  public static ddouble FromInt(int       from) => from;
  public static uint    ToUInt(ddouble    from) => (uint)from;
  public static ddouble FromUInt(uint     from) => from;

}