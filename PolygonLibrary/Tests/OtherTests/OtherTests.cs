using NUnit.Framework;
using DoubleDouble;

// using DG = CGLibrary.Geometry<DoubleDouble.ddouble, Convertors.DDConvertor>;
// using G = CGLibrary.Geometry<double, Convertors.DConvertor>;
namespace OtherTests;

[TestFixture]
public class OtherTests {

  [Test]
  public void Atan2Test() {
    var x = ddouble.Atan2(-1e-15, -1);
    var y = double.Atan2(-1e-15, -1);
    Console.WriteLine(x);
    Console.WriteLine(y);
  }

  [Test]
  public void AcosTest() {
    var x = ddouble.Acos(1);
    var y = double.Acos(1);
    Console.WriteLine(x);
    Console.WriteLine(y);
  }

}