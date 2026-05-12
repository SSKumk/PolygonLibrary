using CGLibrary;
using static CGLibrary.Geometry<double, Graphics.DConvertor>;

namespace Graphics.Draw;

public class PlyDrawer : IDrawer {

  public void SaveFrame(string path, IEnumerable<Vector> vertices, IEnumerable<VisTools.FacetColor> facets) {
    using ParamWriter pw = new ParamWriter(path + ".ply");

    List<Vector>         VList = vertices.ToList();
    List<VisTools.FacetColor> FList = facets.ToList();

    // Пишем в файл в формате .ply
    // шапка
    pw.WriteLine("ply");
    pw.WriteLine("format ascii 1.0");
    pw.WriteLine($"element vertex {VList.Count}");
    pw.WriteLine("property float x");
    pw.WriteLine("property float y");
    pw.WriteLine("property float z");
    pw.WriteLine($"element face {FList.Count}");
    pw.WriteLine("property list uchar int vertex_index");
    pw.WriteLine("property uchar red");
    pw.WriteLine("property uchar green");
    pw.WriteLine("property uchar blue");
    pw.WriteLine("property uchar alpha");
    pw.WriteLine("end_header");
    // вершины
    foreach (Vector v in VList) {
      pw.WriteLine(v.ToStringBraceAndDelim(null, null, ' '));
    }
    // грани
    foreach (VisTools.FacetColor F in FList) {
      pw.Write($"{F.Vertices.Count()} ");
      foreach (Vector vertex in F.Vertices) {
        pw.Write($"{VList.IndexOf(vertex)} ");
      }
      pw.Write($"{F.color.red} {F.color.green} {F.color.blue} 255");
      pw.WriteLine();
    }
  }

  public void SaveFrame(string path, IEnumerable<Vector> vertices, IEnumerable<VisTools.Facet> facets) {
    using ParamWriter pw = new ParamWriter(path + ".ply");

    List<Vector>              VList = vertices.ToList();
    List<VisTools.Facet> FList = facets.ToList();

    // Пишем в файл в формате .ply
    // шапка
    pw.WriteLine("ply");
    pw.WriteLine("format ascii 1.0");
    pw.WriteLine($"element vertex {VList.Count}");
    pw.WriteLine("property float x");
    pw.WriteLine("property float y");
    pw.WriteLine("property float z");
    pw.WriteLine($"element face {FList.Count}");
    pw.WriteLine("property list uchar int vertex_index");
    pw.WriteLine("end_header");
    // вершины
    foreach (Vector v in VList) {
      pw.WriteLine(v.ToStringBraceAndDelim(null, null, ' '));
    }
    // грани
    foreach (VisTools.Facet F in FList) {
      pw.Write($"{F.Vertices.Count()} ");
      foreach (Vector vertex in F.Vertices) {
        pw.Write($"{VList.IndexOf(vertex)} ");
      }
      pw.WriteLine();
    }
  }

}

