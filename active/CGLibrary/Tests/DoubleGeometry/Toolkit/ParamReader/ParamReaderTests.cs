using System.Text;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Toolkit;

[TestFixture]
public class ParamReaderTests {

  private static string WriteTempFile(string content) {
    string path = Path.Combine(Path.GetTempPath(), $"param-reader-{Guid.NewGuid():N}.c");
    File.WriteAllText(path, content, new UTF8Encoding(false));

    return path;
  }

  private static string MainSample => """
    // Path to the folder whereto the result should be written
    path = "some/path";

    // Some boolean data
    flag1 = true;
    flag2 = false;

    /* multi-line comment */

    // The initial instant
    t0 = 0.0;
    T = /* inline */ 2.0;
    dt
        =
            0.05
                ;

    // Dimension of the phase vector
    n = 2;

    boolAr = { true, true, false };
    intAr = { 100, 0, -50 };
    doubleLst = { 1.0, -3.1415, 1.4142 };
    stringAr = { "string1", "", "Hello, world!" };

    A = { { 5.0, -1.5 }, { -3.14, 2.718281828 } };
    B = { { -7.21 }, { 5.999, 2.32 }, { 876, 0, -7.19 } };
    """;

  private static string GeometrySample => """
    vector = { 1.0, -2.0, 3.5 };
    vectors = { { 1.0, 0.0 }, { 0.0, 1.0, 2.0 } };
    hyperPlanes = { { 1.0, 0.0, 5.0 }, { 0.0, 1.0, -2.0 } };
    """;

  [Test]
  public void ReadScalarsArraysAndJaggedData_MatchesLegacyScenario() {
    string pathToFile = WriteTempFile(MainSample);

    try {
      ParamReader pr = new ParamReader(pathToFile);

      string path = pr.ReadString("path");
      Assert.That(
        path,
        Is.EqualTo("some/path"),
        $"Wrong reading of the string parameter 'path': the read value is '{path}', the expected value is 'some/path'"
      );

      bool flag1 = pr.ReadNumber<bool>("flag1");
      bool flag2 = pr.ReadNumber<bool>("flag2");
      Assert.That(
        flag1,
        Is.EqualTo(true),
        $"Wrong reading of the string parameter 'flag1': the read value is '{flag1}', the expected value is 'True'"
      );
      Assert.That(
        flag2,
        Is.EqualTo(false),
        $"Wrong reading of the string parameter 'flag2': the read value is '{flag2}', the expected value is 'False'"
      );

      double t0 = pr.ReadNumber<double>("t0");
      double t = pr.ReadNumber<double>("T");
      double dt = pr.ReadNumber<double>("dt");
      int n = pr.ReadNumber<int>("n");

      Assert.Multiple(() => {
        Assert.That(t0, Is.EqualTo(0.0), $"Wrong reading of the string parameter 't0': the read value is '{t0}', the expected value is '0'");
        Assert.That(t, Is.EqualTo(2.0), $"Wrong reading of the string parameter 'T': the read value is '{t}', the expected value is '2'");
        Assert.That(dt, Is.EqualTo(0.05), $"Wrong reading of the string parameter 'dt': the read value is '{dt}', the expected value is '0.05'");
        Assert.That(n, Is.EqualTo(2), $"Wrong reading of the string parameter 'n': the read value is '{n}', the expected value is '2'");
      });

      Assert.That(pr.Read1DArray<bool>("boolAr", 3), Is.EqualTo(new[] { true, true, false }));
      Assert.That(pr.Read1DArray<int>("intAr", 3), Is.EqualTo(new[] { 100, 0, -50 }));
      Assert.That(pr.ReadList<double>("doubleLst"), Is.EqualTo(new[] { 1.0, -3.1415, 1.4142 }));
      Assert.That(pr.Read1DArray<string>("stringAr", 3), Is.EqualTo(new[] { "string1", "", "Hello, world!" }));

      double[,] a = pr.Read2DArray<double>("A", n, n);
      List<List<double>> b = pr.Read2DJaggedArray<double>("B");

      Assert.Multiple(() => {
        Assert.That(a[0, 0], Is.EqualTo(5.0));
        Assert.That(a[0, 1], Is.EqualTo(-1.5));
        Assert.That(a[1, 0], Is.EqualTo(-3.14));
        Assert.That(a[1, 1], Is.EqualTo(2.718281828));
        Assert.That(b, Has.Count.EqualTo(3));
        Assert.That(b[0], Is.EqualTo(new[] { -7.21 }));
        Assert.That(b[1], Is.EqualTo(new[] { 5.999, 2.32 }));
        Assert.That(b[2], Is.EqualTo(new[] { 876.0, 0.0, -7.19 }));
      });
    }
    finally {
      File.Delete(pathToFile);
    }
  }

  [Test]
  public void PeekString_DoesNotAdvanceReaderPosition() {
    string pathToFile = WriteTempFile("""
      first = "alpha";
      second = "beta";
      """);

    try {
      ParamReader pr = new ParamReader(pathToFile);

      string peeked = pr.PeakString("first");
      string actualFirst = pr.ReadString("first");
      string actualSecond = pr.ReadString("second");

      Assert.Multiple(() => {
        Assert.That(peeked, Is.EqualTo("alpha"));
        Assert.That(actualFirst, Is.EqualTo("alpha"));
        Assert.That(actualSecond, Is.EqualTo("beta"));
      });
    }
    finally {
      File.Delete(pathToFile);
    }
  }

  [Test]
  public void GetSanitizedData_RemovesCommentsAndWhitespaceEverywhere() {
    string pathToFile = WriteTempFile(MainSample);

    try {
      ParamReader pr = new ParamReader(pathToFile);

      string sanitized = pr.GetSanitizedData();

      Assert.That(
        sanitized,
        Is.EqualTo(
          "path=\"some/path\";flag1=true;flag2=false;t0=0.0;T=2.0;dt=0.05;n=2;boolAr={true,true,false};intAr={100,0,-50};doubleLst={1.0,-3.1415,1.4142};stringAr={\"string1\",\"\",\"Hello,world!\"};A={{5.0,-1.5},{-3.14,2.718281828}};B={{-7.21},{5.999,2.32},{876,0,-7.19}};"
        )
      );
    }
    finally {
      File.Delete(pathToFile);
    }
  }

  [Test]
  public void ReadVectorVectorsAndHyperPlanes_ParseStructuredGeometryData() {
    string pathToFile = WriteTempFile(GeometrySample);

    try {
      ParamReader pr = new ParamReader(pathToFile);

      Vector vector = pr.ReadVector("vector");
      List<Vector> vectors = pr.ReadVectors("vectors");
      List<HyperPlane> hyperPlanes = pr.ReadHyperPlanes("hyperPlanes");

      Assert.Multiple(() => {
        Assert.That(vector.SpaceDim, Is.EqualTo(3));
        Assert.That(vector[0], Is.EqualTo(1.0));
        Assert.That(vector[1], Is.EqualTo(-2.0));
        Assert.That(vector[2], Is.EqualTo(3.5));
        Assert.That(vectors, Has.Count.EqualTo(2));
        Assert.That(vectors[0].GetCopyAsArray(), Is.EqualTo(new[] { 1.0, 0.0 }));
        Assert.That(vectors[1].GetCopyAsArray(), Is.EqualTo(new[] { 0.0, 1.0, 2.0 }));
        Assert.That(hyperPlanes, Has.Count.EqualTo(2));
        Assert.That(hyperPlanes[0].Normal.GetCopyAsArray(), Is.EqualTo(new[] { 1.0, 0.0 }));
        Assert.That(hyperPlanes[0].ConstantTerm, Is.EqualTo(5.0));
        Assert.That(hyperPlanes[1].Normal.GetCopyAsArray(), Is.EqualTo(new[] { 0.0, 1.0 }));
        Assert.That(hyperPlanes[1].ConstantTerm, Is.EqualTo(-2.0));
      });
    }
    finally {
      File.Delete(pathToFile);
    }
  }

  [Test]
  public void ReadNumberLine_SkipsLeadingWhitespaceAndParsesExpectedCount() {
    string pathToFile = WriteTempFile("""

      
      1.5  -2.0   3.25
      """);

    try {
      ParamReader pr = new ParamReader(pathToFile);

      double[] values = pr.ReadNumberLine(3);

      Assert.That(values, Is.EqualTo(new[] { 1.5, -2.0, 3.25 }));
    }
    finally {
      File.Delete(pathToFile);
    }
  }

}
