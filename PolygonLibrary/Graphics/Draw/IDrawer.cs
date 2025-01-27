using static CGLibrary.Geometry<double, Graphics.DConvertor>;

namespace Graphics.Draw;

public interface IDrawer {



  public void SaveFrame(string path, IEnumerable<Vector> vertices, IEnumerable<VisTools.FacetColor> facets);

  public void SaveFrame(string path, IEnumerable<Vector> vertices, IEnumerable<VisTools.Facet> facets);


}
