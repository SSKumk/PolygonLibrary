using System.Text;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Toolkit;

[TestFixture]
public class ParamWriterTests {

  private static string CreateTempPath() => Path.Combine(Path.GetTempPath(), $"param-writer-{Guid.NewGuid():N}.c");

  [Test]
  public void WriteNumberAndString_RoundTripSimpleValuesAndFormatting() {
    string path = CreateTempPath();

    try {
      using (ParamWriter writer = new ParamWriter(path)) {
        writer.WriteNumber("scale", 1.23456, "F2");
        writer.WriteString("name", "MyCircle");
      }

      ParamReader reader = new ParamReader(path);

      double scale = reader.ReadNumber<double>("scale");
      string name = reader.ReadString("name");

      Assert.Multiple(() => {
        Assert.That(scale, Is.EqualTo(1.23));
        Assert.That(name, Is.EqualTo("MyCircle"));
      });
    }
    finally {
      File.Delete(path);
    }
  }

  [Test]
  public void WriteArraysAndVectors_RoundTripNumericCollections() {
    string path = CreateTempPath();

    try {
      using (ParamWriter writer = new ParamWriter(path)) {
        writer.Write1DArray("weights", new[] { 1.0, -2.5, 3.25 });
        writer.WriteVector("shift", new Vector(new[] { 2.0, -1.0, 0.5 }, false));
        writer.WriteVectors("points", new[] {
          new Vector(new[] { 1.0, 2.0 }, false),
          new Vector(new[] { -3.0, 4.5 }, false)
        });
      }

      ParamReader reader = new ParamReader(path);

      double[] weights = reader.Read1DArray<double>("weights", 3);
      Vector shift = reader.ReadVector("shift");
      List<Vector> points = reader.ReadVectors("points");

      Assert.Multiple(() => {
        Assert.That(weights, Is.EqualTo(new[] { 1.0, -2.5, 3.25 }));
        Assert.That(shift.GetCopyAsArray(), Is.EqualTo(new[] { 2.0, -1.0, 0.5 }));
        Assert.That(points, Has.Count.EqualTo(2));
        Assert.That(points[0].GetCopyAsArray(), Is.EqualTo(new[] { 1.0, 2.0 }));
        Assert.That(points[1].GetCopyAsArray(), Is.EqualTo(new[] { -3.0, 4.5 }));
      });
    }
    finally {
      File.Delete(path);
    }
  }

  [Test]
  public void Write2DArrayAndHyperPlanes_RoundTripStructuredGeometry() {
    string path = CreateTempPath();

    try {
      using (ParamWriter writer = new ParamWriter(path)) {
        writer.Write2DArray("matrixRows", new[] {
          new[] { 1.0, 2.0 },
          new[] { -3.0, 4.5 }
        });
        writer.WriteHyperPlanes("HPs", new[] {
          new HyperPlane(new Vector(new[] { 1.0, 0.0 }, false), 5.0),
          new HyperPlane(new Vector(new[] { 0.0, 1.0 }, false), -2.0)
        });
      }

      ParamReader reader = new ParamReader(path);

      List<List<double>> matrixRows = reader.Read2DJaggedArray<double>("matrixRows");
      List<HyperPlane> hyperPlanes = reader.ReadHyperPlanes("HPs");

      Assert.Multiple(() => {
        Assert.That(matrixRows, Has.Count.EqualTo(2));
        Assert.That(matrixRows[0], Is.EqualTo(new[] { 1.0, 2.0 }));
        Assert.That(matrixRows[1], Is.EqualTo(new[] { -3.0, 4.5 }));
        Assert.That(hyperPlanes, Has.Count.EqualTo(2));
        Assert.That(hyperPlanes[0].Normal.GetCopyAsArray(), Is.EqualTo(new[] { 1.0, 0.0 }));
        Assert.That(hyperPlanes[0].ConstantTerm, Is.EqualTo(5.0));
        Assert.That(hyperPlanes[1].Normal.GetCopyAsArray(), Is.EqualTo(new[] { 0.0, 1.0 }));
        Assert.That(hyperPlanes[1].ConstantTerm, Is.EqualTo(-2.0));
      });
    }
    finally {
      File.Delete(path);
    }
  }

  [Test]
  public void AppendConstructor_PreservesExistingContent() {
    string path = CreateTempPath();

    try {
      using (ParamWriter writer = new ParamWriter(path)) {
        writer.WriteNumber("first", 1);
      }

      using (ParamWriter writer = new ParamWriter(path, true)) {
        writer.WriteNumber("second", 2);
      }

      string content = File.ReadAllText(path, Encoding.UTF8);
      ParamReader reader = new ParamReader(path);

      Assert.Multiple(() => {
        Assert.That(reader.ReadNumber<int>("first"), Is.EqualTo(1));
        Assert.That(reader.ReadNumber<int>("second"), Is.EqualTo(2));
        Assert.That(content, Does.Contain("first = 1;"));
        Assert.That(content, Does.Contain("second = 2;"));
      });
    }
    finally {
      File.Delete(path);
    }
  }

}
