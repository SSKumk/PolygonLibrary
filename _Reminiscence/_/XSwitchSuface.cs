using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Robust
{
  /// <summary>
  /// Class for XML-presentation of a switching line as a list of points
  /// </summary>
  [XmlRoot]
  public class XSwitchLine 
  {
    [XmlElement("Points")]
    public List<Point> points;

    /// <summary>
    /// Empty constructor
    /// </summary>
    public XSwitchLine () 
    {
      points = new List<Point> ();
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="points">The points of the line</param>
    public XSwitchLine (IEnumerable<Point> points)
    {
      if (points == null)
        this.points = null;
      else
        this.points = new List<Point> (points);
    }
  }

  /// <summary>
  /// Class for XML-presentation of a system of switching lines
  /// </summary>
  [XmlRoot]
  public class XSwitchLines
  {
    /// <summary>
    /// Time instant of the system
    /// </summary>
    [XmlElement("t")]
    public double t;

    /// <summary>
    /// List of switching lines
    /// </summary>
    [XmlElement("SwitchLines")]
    public List<XSwitchLine> xLines;

    /// <summary>
    /// Empty constructor
    /// </summary>
    public XSwitchLines ()
    {
      xLines = new List<XSwitchLine> ();
      t = 0.0;
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="lines"></param>
    /// <param name="curT"></param>
    public XSwitchLines (IEnumerable<XSwitchLine> lines, double curT)
    {
      xLines = new List<XSwitchLine> (lines);
      t = curT;
    }
  }

  /// <summary>
  /// Class for XML-presentation of a switching surface as a list of sets of switching lines
  /// </summary>
  [XmlRoot]
  public class XSwitchSurface 
  {
    [XmlElement ("Signature")]
    public string Signature = SignClass.GetSurfaceSignature();

    [XmlElement ("ExampleName")]
    public string ExampleName;

    [XmlElement("SurfSections")]
    public List<XSwitchLines> sections;

    /// <summary>
    /// Empty constructor
    /// </summary>
    public XSwitchSurface () 
    {
      sections = new List<XSwitchLines> ();
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="points">The points of the line</param>
    public XSwitchSurface (string exName, IEnumerable<XSwitchLines> sections) 
    {
      this.sections = new List<XSwitchLines> (sections);
      this.ExampleName = exName;
    }
  }

}
