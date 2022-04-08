using System;
using System.Xml.Serialization;

namespace Robust
{
  /// <summary>
  /// A class for storing XML-signatures of objects
  /// </summary>
  public class SignClass
  {
    /// <summary>
    /// Getting signature for a bridge object
    /// </summary>
    /// <returns>Signature for a bridge object</returns>
    public static string GetBridgeSignature ()
    {
      return "Le Menec Problem / 2012-02-23";
    }

    /// <summary>
    /// Getting signature for a switching surface object
    /// </summary>
    /// <returns>Signature for a bridge object</returns>
    public static string GetSurfaceSignature ()
    {
      return "Le Menec Problem (surface) / 2012-02-23";
    }

    /// <summary>
    /// Getting signature for a switching surface object
    /// </summary>
    /// <returns>Signature for a bridge object</returns>
    public static string GetTrajectorySignature ()
    {
      return "Le Menec Problem (trajectory) / 2012-02-23";
    }
  }

  [XmlRoot]
  public class XBridge
  {
    [XmlElement ("Signature")]
    public string Signature;

    [XmlElement ("ExampleName")]
    public string ExampleName;

    [XmlElement ("XPolyComplex")]
    public XPolyComplex[] xPolyComplex;

    [XmlAttribute ("c")]
    public double c;

    public XBridge () { }

    public XBridge (string exName, XPolyComplex[] xpolycomplex, double c)
    {
      this.xPolyComplex = xpolycomplex;
      this.c = c;
      this.Signature = SignClass.GetBridgeSignature ();
      this.ExampleName = exName;
    }
  }

  public class XPolyComplex
  {
    [XmlElement ("XPoly")]
    public XPoly[] xPoly;

    [XmlAttribute ("t")]
    public double t;

    [XmlAttribute ("empty")]
    public bool empty;

    public XPolyComplex () { }

    public XPolyComplex (XPoly[] xpoly, double t, bool empty)
    {
      this.xPoly = xpoly;
      this.t = t;
      this.empty = empty;
    }
  }

  public class XPoly
  {
    [XmlElement ("XPoint")]
    public XPoint[] xPoint;

    public XPoly () { }

    public XPoly (XPoint[] xpoint)
    {
      this.xPoint = xpoint;
    }
  }

  public class XPoint
  {
    [XmlAttribute ("x")]
    public double x;

    [XmlAttribute ("y")]
    public double y;

    [XmlAttribute ("num")]
    public int num;

    [XmlAttribute ("parent")]
    public int parent;

    public XPoint ()
    {
    }

    public XPoint (double x, double y, int num, int parent)
    {
      this.x = x;
      this.y = y;
      this.num = num;
      this.parent = parent;
    }
  }
}
