using CGLibrary;
using static CGLibrary.Geometry<double, Graphics.DConvertor>;

namespace Graphics.Draw;

public class PlyDrawer : IDrawer {

  public void SaveFrame(string path, IEnumerable<Vector> vertices, IEnumerable<Visualization.Facet> facets) {
    using ParamWriter pw = new ParamWriter(path + ".ply");

    List<Vector>              VList = vertices.ToList();
    List<Visualization.Facet> FList = facets.ToList();

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
    pw.WriteLine("end_header");
    // вершины
    foreach (Vector v in VList) {
      pw.WriteLine(v.ToStringBraceAndDelim(null, null, ' '));
    }
    // грани
    foreach (Visualization.Facet F in FList) {
      pw.Write($"{F.Vertices.Count()} ");
      foreach (Vector vertex in F.Vertices) {
        pw.Write($"{VList.IndexOf(vertex)} ");
      }
      pw.Write($"{F.color.red} {F.color.green} {F.color.blue}");
      pw.WriteLine();
    }
  }

}

  public class PythonArrayDrawer : IDrawer {

    // public void SaveFrame(string path, IEnumerable<Vector> vertices, IEnumerable<Visualization.Facet> facets) {
    //   using ParamWriter pw = new ParamWriter(path + ".txt");
    //
    //   List<Vector>              VList = vertices.ToList();
    //   List<Visualization.Facet> FList = facets.ToList();
    //
    //   // Пишем питоновские массивы
    //   // вершины
    //   pw.Write("vertices = [");
    //   for (int i = 0; i < VList.Count - 1; i++) {
    //     pw.Write(VList[i].ToStringBraceAndDelim('(', ')', ','));
    //     pw.Write(",");
    //   }
    //   pw.Write(VList[^1].ToStringBraceAndDelim('(', ')', ','));
    //   pw.WriteLine("]");
    //
    //   // грани
    //   pw.Write("faces = [");
    //   List<double> inds = new List<double>();
    //   for (int i = 0; i < FList.Count - 1; i++) {
    //     inds.Clear();
    //     foreach (Vector vertex in FList[i].Vertices) {
    //       inds.Add(VList.IndexOf(vertex));
    //     }
    //     pw.Write(new Vector(inds.ToArray()).ToStringBraceAndDelim('(', ')', ','));
    //     pw.Write(",");
    //   }
    //   inds.Clear();
    //   foreach (Vector vertex in FList[^1].Vertices) {
    //     inds.Add(VList.IndexOf(vertex));
    //   }
    //   pw.Write(new Vector(inds.ToArray()).ToStringBraceAndDelim('(', ')', ','));
    //   pw.WriteLine("]");
    // }

    public void SaveFrame(string path, IEnumerable<Vector> vertices, IEnumerable<Visualization.Facet> facets) {
      using ParamWriter pw = new ParamWriter(path + ".txt");

      List<Vector>              VList = vertices.ToList();
      List<Visualization.Facet> FList = facets.ToList();

      // Запись вершин
      WriteCollection(pw, "vertices", VList, v => v.ToString());

      // Запись граней
      WriteCollection
        (
         pw
       , "faces"
       , FList
       , facet
           => {
           var indices = facet.Vertices.Select(vertex => (double)VList.IndexOf(vertex)).ToList();

           return new Vector(indices.ToArray()).ToString();
         }
        );
    }

// Общий метод для записи коллекции
    public void WriteCollection<T>(ParamWriter pw, string name, List<T> collection, Func<T, string> toStringFunc) {
      pw.Write($"{name} = [");
      for (int i = 0; i < collection.Count - 1; i++) {
        pw.Write(toStringFunc(collection[i]));
        pw.Write(",");
      }
      pw.Write(toStringFunc(collection[^1]));
      pw.WriteLine("]");
    }

  }
