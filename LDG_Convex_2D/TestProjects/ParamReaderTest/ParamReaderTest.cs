using System.Text;
using NUnit.Framework;
using ParamReaderLibrary;

namespace ParamReaderTest {
  [TestFixtureAttribute]
  public class ParamReaderTest {
    [Test]
    public void SomeExample01() {
      ParamReader pr = new ParamReader("../../../ex0a-simpleMotions-circle.c");

      string path = pr.ReadString("path")
      , aPath = "some/path";
      Assert.That(aPath, Is.EqualTo(path)
      , $"Wrong reading of the string parameter 'path': the read value is '{path}', the expected value is '{aPath}'");

      bool flag1 = pr.ReadBoolean("flag1")
      , aFlag1 = true
      , flag2 = pr.ReadBoolean("flag2")
      , aFlag2 = false;

      Assert.That(aFlag1, Is.EqualTo(flag1)
      , $"Wrong reading of the string parameter 'flag1': the read value is '{flag1}', the expected value is '{aFlag1}'");
      Assert.That(aFlag2, Is.EqualTo(flag2)
      , $"Wrong reading of the string parameter 'flag2': the read value is '{flag2}', the expected value is '{aFlag2}'");

      double t0 = pr.ReadDouble("t0")
      , at0 = 0.0
      , T = pr.ReadDouble("T")
      , aT = 2.0
      , dt = pr.ReadDouble("dt")
      , adt = 0.05;

      Assert.That(t0, Is.EqualTo(at0)
      , $"Wrong reading of the string parameter 't0': the read value is '{t0}', the expected value is '{at0}'");
      Assert.That(T, Is.EqualTo(aT)
      , $"Wrong reading of the string parameter 'T': the read value is '{T}', the expected value is '{aT}'");
      Assert.That(dt, Is.EqualTo(adt)
      , $"Wrong reading of the string parameter 'dt': the read value is '{dt}', the expected value is '{adt}'");

      int n = pr.ReadInt("n")
      , an = 2;

      Assert.That(n, Is.EqualTo(an)
      , $"Wrong reading of the string parameter 'n': the read value is '{n}', the expected value is '{an}'");

      bool[] boolAr = pr.Read1DArray<bool>("boolAr", 3)
      , aBoolAr =
          {
            true
          , true
          , false
          };
      for (int i = 0; i < 3; i++) {
        Assert.That(boolAr[i], Is.EqualTo(aBoolAr[i])
        , $"Wrong reading of the boolean array 'boolAr': the read value of the {i}th element is '{boolAr[i]}', its expected value is '{aBoolAr[i]}'");
      }

      int[] intAr = pr.Read1DArray<int>("intAr", 3)
      , aIntAr =
          {
            100
          , 0
          , -50
          };
      for (int i = 0; i < 3; i++) {
        Assert.That(intAr[i], Is.EqualTo(aIntAr[i])
        , $"Wrong reading of the integer array 'intAr': the read value of the {i}th element is '{intAr[i]}', its expected value is '{aIntAr[i]}'");
      }

      double[] doubleAr = pr.Read1DArray<double>("doubleAr", 3)
      , aDoubleAr =
          {
            1.0
          , -3.1415
          , 1.4142
          };
      for (int i = 0; i < 3; i++) {
        Assert.That(doubleAr[i], Is.EqualTo(aDoubleAr[i])
        , $"Wrong reading of the real array 'doubleAr': the read value of the {i}th element is '{doubleAr[i]}', its expected value is '{aDoubleAr[i]}'");
      }

      string[] stringAr = pr.Read1DArray<string>("stringAr", 3)
      , aStringAr =
          {
            "string1"
          , ""
          , "Hello, world!"
          };
      for (int i = 0; i < 3; i++) {
        Assert.That(stringAr[i], Is.EqualTo(aStringAr[i])
        , $"Wrong reading of the string array 'stringAr': the read value of the {i}th element is '{stringAr[i]}', its expected value is '{aStringAr[i]}'");
      }

      double[,] A = pr.Read2DArray<double>("A", n, n)
      , aA =
          {
            {
              5.0
            , -1.5
            }
           ,
            {
              -3.14
            , 2.718281828
            }
          };
      for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
          Assert.That(A[i, j], Is.EqualTo(aA[i, j])
          , $"Wrong reading of the two-dimensional real array 'A': the read value of the element at the position [{i},{j}] is '{A[i, j]}', its expected value is '{aA[i, j]}'");
        }
      }
    }
  }
}