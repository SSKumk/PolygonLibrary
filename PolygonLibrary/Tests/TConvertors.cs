using CGLibrary;
using DoubleDouble;
using Rationals;
namespace Tests;
//ТЕСТЫ очередного типа TNum: vvvv
/*
 * 1) Acos(-1) == +PI, а не -PI
 * 2) Atan2(y,x) \in (-PI, +PI], то есть включена положительная граница
 */

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

/// <summary>
/// Interface for Rationals-conversions.
/// </summary>
public class RConvertor : INumConvertor<Rational> {

  public static double   ToDouble(Rational from) => (double)from;
  public static Rational FromDouble(double from) => Rational.Approximate(from);
  public static int      ToInt(Rational    from) => (int)from;
  public static Rational FromInt(int       from) => from;
  public static uint     ToUInt(Rational   from) => (uint)from;
  public static Rational FromUInt(uint     from) => from;

}
