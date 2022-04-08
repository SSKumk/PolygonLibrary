using System;
using System.Collections.Generic;

namespace Robust
{
  public class XmlBridgeConvert
  {
    public static XBridge Convert (string exName, DPolyComplex[] bridge, Time time, double c)
    {
      XPolyComplex[] xpolycomplex = new XPolyComplex[bridge.Length];
      int i, j, k;

// Loop on time instants
      for (i = 0; i < bridge.Length; i++)
      {
        if (bridge[i].isEmpty)
          xpolycomplex[i] = new XPolyComplex (null, time[i], true);
        else
        {
          XPoly[] xpoly = new XPoly[bridge[i].nPoly];
// Loop on line components 
          for (j = 0; j < xpoly.Length; j++)
          {
            XPoint[] xpoint = new XPoint[bridge[i][j].nPoints];
// Loop on points in the line component
            for (k = 0; k < xpoint.Length; k++)
              xpoint[k] = new XPoint (bridge[i][j][k].x, bridge[i][j][k].y, 
                bridge[i][j][k].num, bridge[i][j][k].parent);
            xpoly[j] = new XPoly (xpoint);
          }
          xpolycomplex[i] = new XPolyComplex (xpoly, time[i], false);

        }
      }
      
      return new XBridge (exName, xpolycomplex, c);
    }

    public static void ConvertBack (XBridge xBr, out DPolyComplex[] bridge)
    {
      int n = xBr.xPolyComplex.Length;
      bridge = new DPolyComplex[n];
      for (int i = 0; i < n; i++)
      {
        if (!xBr.xPolyComplex[i].empty)
        {
          int m = xBr.xPolyComplex[i].xPoly.Length;
          DPoly[] polies = new DPoly[m];

          for (int j = 0; j < m; j++)
          {
            int l = xBr.xPolyComplex[i].xPoly[j].xPoint.Length;
            Point[] p = new Point[l];

            for (int k = 0; k < l; k++)
              p[k] = new Point (xBr.xPolyComplex[i].xPoly[j].xPoint[k].x,
                xBr.xPolyComplex[i].xPoly[j].xPoint[k].y,
                xBr.xPolyComplex[i].xPoly[j].xPoint[k].num,
                xBr.xPolyComplex[i].xPoly[j].xPoint[k].parent);

            polies[j] = new DPoly (p);
          }

          bridge[i] = new DPolyComplex (polies);
        }
        else
          bridge[i] = new DPolyComplex ();
      }
    }
  }

  public class XmlSwitchLinesConvert
  {
    public static XSwitchSurface Convert (string exName, Point[][][] surface, Time time)
    {
      int i, j;
      XSwitchLines[] res = new XSwitchLines[time.Count];

      for (i = 0; i < time.Count; i++)
      {
        XSwitchLine[] section = new XSwitchLine[surface.Length];

        for (j = 0; j < surface.Length; j++)
          section[j] = new XSwitchLine (surface[j][i]);

        res[i] = new XSwitchLines (section, time[i]);
      }

      return new XSwitchSurface (exName, res);
    }

    public static Point[][][] ConvertBack (XSwitchSurface surface)
    {
      Point[][][] res = new Point[surface.sections[0].xLines.Count][][];
      int i, j, k;

      for (i = 0; i < surface.sections[0].xLines.Count; i++)
      {
        res[i] = new Point[surface.sections.Count][];
        for (j = 0; j < surface.sections.Count; j++)
        {
          res[i][j] = new Point[surface.sections[j].xLines[i].points.Count];
          for (k = 0; k < surface.sections[j].xLines[i].points.Count; k++)
            res[i][j][k] = surface.sections[j].xLines[i].points[k];
        }
      }

      return res;
    }
  }
}
