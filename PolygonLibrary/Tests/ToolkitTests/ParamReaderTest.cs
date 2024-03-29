using System.Text;
using DoubleDouble;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.ToolkitTests;

[TestFixture]
public class ParamReaderTest {

  [Test]
  public void SomeExample01() {
    ParamReader pr = new ParamReader("../../../ToolkitTests/someFile.c");

    string path = pr.ReadString("path"), aPath = "some/path";
    Assert.That
      (
       aPath
     , Is.EqualTo(path)
     , $"Wrong reading of the string parameter 'path': the read value is '{path}', the expected value is '{aPath}'"
      );

    bool flag1 = pr.ReadNumber<bool>("flag1"), aFlag1 = true, flag2 = pr.ReadNumber<bool>("flag2"), aFlag2 = false;

    Assert.That
      (
       aFlag1
     , Is.EqualTo(flag1)
     , $"Wrong reading of the string parameter 'flag1': the read value is '{flag1}', the expected value is '{aFlag1}'"
      );
    Assert.That
      (
       aFlag2
     , Is.EqualTo(flag2)
     , $"Wrong reading of the string parameter 'flag2': the read value is '{flag2}', the expected value is '{aFlag2}'"
      );

    double t0 = pr.ReadNumber<double>
              ("t0")
          , at0 = 0.0
          , T   = pr.ReadNumber<double>("T")
          , aT  = 2.0
          , dt  = pr.ReadNumber<double>("dt")
          , adt = 0.05;

    Assert.That
      (
       t0
     , Is.EqualTo(at0)
     , $"Wrong reading of the string parameter 't0': the read value is '{t0}', the expected value is '{at0}'"
      );
    Assert.That
      (T, Is.EqualTo(aT), $"Wrong reading of the string parameter 'T': the read value is '{T}', the expected value is '{aT}'");
    Assert.That
      (
       dt
     , Is.EqualTo(adt)
     , $"Wrong reading of the string parameter 'dt': the read value is '{dt}', the expected value is '{adt}'"
      );

    int n = pr.ReadNumber<int>("n"), an = 2;

    Assert.That
      (n, Is.EqualTo(an), $"Wrong reading of the string parameter 'n': the read value is '{n}', the expected value is '{an}'");

    bool[] boolAr = pr.Read1DArray<bool>("boolAr", 3), aBoolAr = { true, true, false };
    for (int i = 0; i < 3; i++) {
      Assert.That
        (
         boolAr[i]
       , Is.EqualTo(aBoolAr[i])
       , $"Wrong reading of the boolean array 'boolAr': the read value of the {i}th element is '{boolAr[i]}', its expected value is '{aBoolAr[i]}'"
        );
    }

    int[] intAr = pr.Read1DArray<int>("intAr", 3), aIntAr = { 100, 0, -50 };
    for (int i = 0; i < 3; i++) {
      Assert.That
        (
         intAr[i]
       , Is.EqualTo(aIntAr[i])
       , $"Wrong reading of the integer array 'intAr': the read value of the {i}th element is '{intAr[i]}', its expected value is '{aIntAr[i]}'"
        );
    }

    List<double> doubleLst = pr.ReadList<double>("doubleLst");
    double[]     aDoubleAr = { 1.0, -3.1415, 1.4142 };
    for (int i = 0; i < 3; i++) {
      Assert.That
        (
         doubleLst[i]
       , Is.EqualTo(aDoubleAr[i])
       , $"Wrong reading of the real list 'doubleLst': the read value of the {i}th element is '{doubleLst[i]}', its expected value is '{aDoubleAr[i]}'"
        );
    }

    string[] stringAr = pr.Read1DArray<string>("stringAr", 3), aStringAr = { "string1", "", "Hello, world!" };
    for (int i = 0; i < 3; i++) {
      Assert.That
        (
         stringAr[i]
       , Is.EqualTo(aStringAr[i])
       , $"Wrong reading of the string array 'stringAr': the read value of the {i}th element is '{stringAr[i]}', its expected value is '{aStringAr[i]}'"
        );
    }

    double[,] A = pr.Read2DArray<double>("A", n, n), aA = { { 5.0, -1.5 }, { -3.14, 2.718281828 } };
    for (int i = 0; i < n; i++) {
      for (int j = 0; j < n; j++) {
        Assert.That
          (
           A[i, j]
         , Is.EqualTo(aA[i, j])
         , $"Wrong reading of the two-dimensional real array 'A': the read value of the element at the position [{i},{j}] is '{A[i, j]}', its expected value is '{aA[i, j]}'"
          );
      }
    }

    List<List<double>> B = pr.Read2DJaggedArray<double>("B");
    List<List<double>> bB = new List<List<double>>()
      {
        new List<double>() { -7.21 }, new List<double>() { 5.999, 2.32 }, new List<double>() { 876, 0, -7.19 }
      };
    for (int i = 0; i < bB.Count; i++) {
      for (int j = 0; j < bB[i].Count; j++) {
        Assert.That
          (
           B[i][j]
         , Is.EqualTo(bB[i][j])
         , $"Wrong reading of the list of lists 'B': the read value of the element at the position [{i},{j}] is '{B[i][j]}', its expected value is '{bB[i][j]}'"
          );
      }
    }
  }

}
