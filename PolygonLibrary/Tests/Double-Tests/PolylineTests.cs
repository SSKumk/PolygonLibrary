using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
namespace Tests.Double_Tests; 

[TestFixture]
public class PolylineTests {
  private List<Point2D> ps1 = new List<Point2D>() {
      new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1)
    };

  [Test]
  public void PolylineContainsTest1a() {
    Polyline line = new Polyline(ps1, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double  x = 1 - 1 / double.Pow(2, i);
      Point2D p = new Point2D(x, x);
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(p)
                                  , "${i}th test: the polyline does not contain the point, which should contain");
                        Assert.That(line.ContainsPointInside(p)
                                  , "${i}th test: the polyline does not contain the point inside it, which should contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest1b() {
    Polyline line = new Polyline(ps1, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double  x = 1 - 1 / double.Pow(2, i);
      Point2D p = new Point2D(x, 0.1);
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(p)
                                  , "${i}th test: the polyline does not contain the point, which should contain");
                        Assert.That(line.ContainsPointInside(p)
                                  , "${i}th test: the polyline does not contain the point inside it, which should contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest1c() {
    Polyline line = new Polyline(ps1, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double  x = 1 / double.Pow(2, i);
      Point2D p = new Point2D(x, x);
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(p)
                                  , "${i}th test: the polyline does not contain the point, which should contain");
                        Assert.That(line.ContainsPointInside(p)
                                  , "${i}th test: the polyline does not contain the point inside it, which should contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest1d() {
    Polyline line = new Polyline(ps1, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double  x = 1 + 1 / double.Pow(2, i);
      Point2D p = new Point2D(x, x);
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(p), Is.False
                                  , "${i}th test: the polyline does contain the point, which should not contain");
                        Assert.That(line.ContainsPointInside(p)
                                  , Is.False, "${i}th test: the polyline does contain the point inside it, which should not contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest1e() {
    Polyline line = new Polyline(ps1, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double  x = 1 + 1 / double.Pow(2, i);
      Point2D p = new Point2D(x, 0.1);
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(p)
                                  , Is.False, "${i}th test: the polyline does contain the point, which should not contain");
                        Assert.That(line.ContainsPointInside(p)
                                  , Is.False, "${i}th test: the polyline does contain the point inside it, which should not contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest1f() {
    Polyline line = new Polyline(ps1, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double  x = -1 / double.Pow(2, i);
      Point2D p = new Point2D(x, x);
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(p)
                                  , Is.False, "${i}th test: the polyline does contain the point, which should not contain");
                        Assert.That(line.ContainsPointInside(p)
                                  , Is.False, "${i}th test: the polyline does contain the point inside it, which should not contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest1g() {
    Polyline line = new Polyline(ps1, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 0; i < ps1.Count; i++) {
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(ps1[i])
                                  , "${i}th test: the polyline does not contain the point, which should contain");
                        Assert.That(line.ContainsPointInside(ps1[i])
                                  , Is.False, "${i}th test: the polyline does contain the point inside it, which should not contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest1h() {
    Polyline line = new Polyline(ps1, PolylineOrientation.Counterclockwise, false, false);
    Assert.Multiple(() => {
                      Assert.That(line.ContainsPoint(new Point2D(1, 0.5)));
                      Assert.That(line.ContainsPointInside(new Point2D(1, 0.5)), Is.False);
                      Assert.That(line.ContainsPoint(new Point2D(1.0000001, 0.5)), Is.False);
                      Assert.That(line.ContainsPointInside(new Point2D(1.0000001, 0.5)), Is.False);
                    });
  }

  private List<Point2D> ps2 = new List<Point2D>() {
      new Point2D(0, 0), new Point2D(1, 0), new Point2D(1, 1), new Point2D(0, 1), new Point2D(0.1, 0.5)
    };

  [Test]
  public void PolylineContainsTest2a() {
    Polyline line = new Polyline(ps2, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double  x = 1 - 1 / double.Pow(2, i);
      Point2D p = new Point2D(x, x);
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(p)
                                  , "${i}th test: the polyline does not contain the point, which should contain");
                        Assert.That(line.ContainsPointInside(p)
                                  , "${i}th test: the polyline does not contain the point inside, which should contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest2b() {
    Polyline line = new Polyline(ps2, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double  x = 1 - 1 / double.Pow(2, i);
      Point2D p = new Point2D(x, 0.1);
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(p)
                                  , "${i}th test: the polyline does not contain the point, which should contain");
                        Assert.That(line.ContainsPointInside(p)
                                  , "${i}th test: the polyline does not contain the point inside, which should contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest2c() {
    Polyline line = new Polyline(ps2, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double  x = 1 / double.Pow(2, i);
      Point2D p = new Point2D(x, x);
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(p)
                                  , "${i}th test: the polyline does not contain the point, which should contain");
                        Assert.That(line.ContainsPointInside(p)
                                  , "${i}th test: the polyline does not contain the point inside, which should contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest2d() {
    Polyline line = new Polyline(ps2, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double  x = 1 / double.Pow(2, i);
      Point2D p = new Point2D(0.1 + x, 0.5);
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(p)
                                  , "${i}th test: the polyline does not contain the point, which should contain");
                        Assert.That(line.ContainsPointInside(p)
                                  , "${i}th test: the polyline does not contain the point inside, which should contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest2e() {
    Polyline line = new Polyline(ps2, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double  x = 1 + 1 / double.Pow(2, i);
      Point2D p = new Point2D(x, x);
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(p)
                                  , Is.False, "${i}th test: the polyline does contain the point, which should not contain");
                        Assert.That(line.ContainsPointInside(p)
                                  , Is.False, "${i}th test: the polyline does contain the point inside, which should not contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest2f() {
    Polyline line = new Polyline(ps2, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double  x = 1 + 1 / double.Pow(2, i);
      Point2D p = new Point2D(x, 0.1);
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(p)
                                  , Is.False, "${i}th test: the polyline does contain the point, which should not contain");
                        Assert.That(line.ContainsPointInside(p)
                                  , Is.False, "${i}th test: the polyline does contain the point inside, which should not contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest2g() {
    Polyline line = new Polyline(ps2, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double  x = -1 / double.Pow(2, i);
      Point2D p = new Point2D(x, x);
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(p)
                                  , Is.False, "${i}th test: the polyline does contain the point, which should not contain");
                        Assert.That(line.ContainsPointInside(p)
                                  , Is.False, "${i}th test: the polyline does contain the point inside, which should not contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest2h() {
    Polyline line = new Polyline(ps2, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 1; i <= 10; i++) {
      double  x = 1 / double.Pow(2, i);
      Point2D p = new Point2D(0.1 - x, 0.5);
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(p)
                                  , Is.False, "${i}th test: the polyline does contain the point, which should not contain");
                        Assert.That(line.ContainsPointInside(p), Is.False
                                  , "${i}th test: the polyline does contain the point inside, which should not contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest2i() {
    Polyline line = new Polyline(ps2, PolylineOrientation.Counterclockwise, false, false);

    for (int i = 0; i < ps2.Count; i++) {
      Assert.Multiple(() => {
                        Assert.That(line.ContainsPoint(ps2[i])
                                  , "${i}th test: the polyline does not contain the point, which should contain");
                        Assert.That(line.ContainsPointInside(ps2[i])
                                  , Is.False, "${i}th test: the polyline does contain the point inside, which should not contain");
                      });
    }
  }

  [Test]
  public void PolylineContainsTest2j() {
    Polyline line = new Polyline(ps2, PolylineOrientation.Counterclockwise, false, false);

    Assert.Multiple(() => {
                      Assert.That(line.ContainsPoint(new Point2D(0.05, 0.75)));
                      Assert.That(line.ContainsPointInside(new Point2D(0.05, 0.75)), Is.False);
                      Assert.That(line.ContainsPoint(new Point2D(1, 0.5)));
                      Assert.That(line.ContainsPointInside(new Point2D(1, 0.5)), Is.False);
                      Assert.That(line.ContainsPoint(new Point2D(1.0000001, 0.5)), Is.False);
                      Assert.That(line.ContainsPointInside(new Point2D(1.0000001, 0.5)), Is.False);
                    });
  }
}