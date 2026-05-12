using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolygonLibrary.Segments;
using PolygonLibrary.Basics;
using PolygonLibrary;


namespace PolygonLibrary.Rearranging
{
    public class Rearranger
    {  /// <summary>
        ///  Метод переразбивает отрезок, добавляя в него точки пересечения, и создает список дескрипторов для каждой точки пересечения.
        /// </summary>
        /// <param name="m">Номер многоугольника.</param>
        /// <param name="n">Номер контура.</param>
        ///  <param name="k">Номер ребра.</param>
        /// <param name="s1">Ребро. </param>
        /// <param name="listOfPoint">Список точек пересечения.</param>
        ///  <returns>Список точек переразбитого ребра.</returns>
        public static void divSegment(int m,int n,int k,Segment s1, IEnumerable<SegmentCrosser2.CrossData> listOfPoint ,
            ref SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary, out List<Point2D> res)
        {
             res = new List<Point2D>();
            List<Point2D> res1 = new List<Point2D>();
            SortedSet<Descriptor> l = new SortedSet<Descriptor>();
            foreach (SegmentCrosser2.CrossData s in listOfPoint)
            {
                if (!res.Contains(s.p))
                {
                    res.Add(s.p);
                    if (s.p.Equals(s1.p1) | s.p.Equals(s1.p2))
                        l = new SortedSet<Descriptor> { newDescrip1(s.p, s1, m, n, k) };
                    else
                    { l = newDescrip(s.p, s1, m, n, k); k++; }

                        if (PointDictionary.ContainsKey(s.p))
                                      PointDictionary[s.p].UnionWith(l);
                                                else
                                      PointDictionary.Add(s.p, l);
                  }
            }
            if (!res[0].Equals(s1.p1))
            {
                res1.Add(s1.p1);
                res1.AddRange(res);
                res = res1;
            }
            if (!res[res.Count - 1].Equals(s1.p2))
                res.Add(s1.p2);
            
           
        }
        /// <summary>
        /// Процедура представления многоугольника в виде списка ребер  и создает исключения для работы алгоритма Бентли-Оттмана.
        /// </summary>
        /// <param name="region">Контур заданный точками</param>
        ///<returns>
        ///<param name="res">Контур заданный отрезками.</param>
        ///<param name="neighs">Список ребер,исключения для работы алгоритма Бентли-Оттмана. </param>
        ///</returns>
        public static void SegmentAndException(List<Point2D> region, out List<Segment> res, out SortedSet<SegmentPair> neighs)
        {

            res = new List<Segment>();
            neighs = new SortedSet<SegmentPair>();
            Point2D p1 = (Point2D)region[0];
            Point2D a;
            Point2D b;
            for (int i = 1; i < region.Count; i++)
            {
                a = region[i - 1];
                b = region[i];
                res.Add(new Segment(a, b));
                if (i > 1)
                    neighs.Add(new SegmentPair(res[i - 1], res[i - 2]));
            }
            a = region[region.Count - 1];
            b = region[0];

            res.Add(new Segment(a, b));
            neighs.Add(new SegmentPair(res[res.Count - 2], res[res.Count - 1]));
            neighs.Add(new SegmentPair(res[0], res[res.Count - 1]));
        }
        /// <summary>
        /// Процедура представления многоугольника в виде списка ребер  
        /// </summary>
        /// <param name="region">Контур заданный точками</param>
        ///<returns>
        ///<param name="res">Контур заданный отрезками.</param>
        
        ///</returns>
        public static List<Segment> PointToSegment(List<Point2D> region)
        {

            List<Segment> res = new List<Segment>();

           
            Point2D a;
            Point2D b;
            for (int i = 1; i < region.Count; i++)
            {
                a = region[i - 1];
                b = region[i];
                res.Add(new Segment(a, b));

            }

            a = region[region.Count - 1];
            b = region[0];

            res.Add(new Segment(a, b));
            return res;
        }



        
        /// <summary>
        /// Процедура переразбиения многоугольника . 
        /// </summary>
        /// <param name="n">Номер многоугольника.</param>
        /// <param name="bentley">Результат работы алгоритма Бентли-Оттмана.</param>
        /// <param name="mnog">Многоугольник заданный точками.</param>
        ///  <returns>
        ///<param name="res1">Переразбитый многоугольник заданный отрезками.</param>
        ///<param name="PointDictionary">Словарь дескрипторов. Ключом являются точка пересечения. Значение - список дескрипторов, отсортированных по возрастанию полярного угла соответствующих ребер.</param>
        ///</returns>

        public static void rearranging(int n,SegmentCrosser2 bentley, List<List<Point2D>> mnog,
            out List<List<Segment>> res1, ref SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary)
        {
            List<Segment> segs = new List<Segment>();
            res1 = new List<List<Segment>>();
           
            List<Segment> temp1 = new List<Segment>();
            for (int i = 0; i < mnog.Count; i++)
            {
                List<Segment> res = new List<Segment>();
                res.Clear();
                segs = PointToSegment(mnog[i]);
                for (int j = 0; j < segs.Count; j++)
                {
                    int k;
                    for (k = 0; k < bentley.crosses.Count; k++)
                        if (bentley.crosses[k].Count > 0 && segs[j].Equals(bentley.crosses[k].Min.s1)) break;

                    if (k < bentley.crosses.Count)
                    {
                        // Нашли !!!
                        // Переразбиение...
                        Segment a = new Segment(bentley.crosses[k].Min.s1);

                        List<Point2D> lPoint = new List<Point2D>();

                            divSegment(n,i,res.Count, a, bentley.crosses[k], ref PointDictionary, out lPoint);


                            temp1 = PointToSegment(lPoint);
                            temp1.RemoveAt(temp1.Count - 1);
                            res.AddRange(temp1);
                        //}
                    }
                    else
                        // Не нашли...  :(
                        res.Add(segs[j]);
                }

                res1.Add(res);

            }


        }


        /// <summary>
        /// Процедура переразбиения многоугольников . 
        /// </summary>
        
        /// <param name="mnog1">Первый многоугольник заданный точками.</param>
        /// <param name="mnog2">Второй многоугольник заданный точками.</param>
        ///  <returns>
        ///<param name="res1">Первый переразбитый многоугольник заданный отрезками.</param>
        ///<param name="res2">Второй переразбитый многоугольник заданный отрезками.</param>
        ///<param name="PointDictionary">Словарь дескрипторов. Ключом являются точка пересечения. Значение - список дескрипторов, отсортированных по возрастанию полярного угла соответствующих ребер.</param>
        ///</returns>
        public static void polygonRearranging(List<List<Point2D>> mnog1, List<List<Point2D>> mnog2,
            out List<List<Segment>> res1, out List<List<Segment>> res2, 
            out SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary)
        {
            res1 = new List<List<Segment>>();
            res2 = new List<List<Segment>>();
            PointDictionary = new SortedDictionary<Point2D, SortedSet<Descriptor>>();
            
            
            List<Segment> segs = new List<Segment>();

            SortedSet<SegmentPair> neighs = new SortedSet<SegmentPair>();
            List<Segment> segs1;
            SortedSet<SegmentPair> neighs1;
            for (int i = 0; i < mnog1.Count; i++)
            {
                SegmentAndException(mnog1[i], out segs1, out neighs1);
                segs.AddRange(segs1);
                neighs.UnionWith(neighs1);
            }
            for (int i = 0; i < mnog2.Count; i++)
            {
                SegmentAndException(mnog2[i], out segs1, out neighs1);
                segs.AddRange(segs1);
                neighs.UnionWith(neighs1);
            }
            SegmentCrosser2 bentley = new SegmentCrosser2(segs, neighs);
            rearranging(1,bentley, mnog1, out res1, ref PointDictionary);
            rearranging(2, bentley, mnog2, out res2, ref PointDictionary);
           
        }
      /// <summary>
        ///  Процедура создания  двух дескрипторов для точки пересечения, которая не является концевой.
        /// </summary>
        /// <param name="p">Точка пересечения.</param>
        /// <param name="m">Номер многоугольника.</param>
        /// <param name="n">Номер контура.</param>
        ///  <param name="k">Номер ребра.</param>
        /// <param name="s1">Ребро. </param>
        ///  <returns>Список дескрипторов, отсортированных по возрастанию полярного угла. При расчете полярного угла начальной точкой отрезка считается точка пересечения.</returns>
          
          
             
          
            
             
             

        public static SortedSet<Descriptor> newDescrip(Point2D p, Segment s1, int m, int n, int k)
        {
            
            Point2D v2 = new Point2D(s1.p2.x - s1.p1.x, s1.p2.y - s1.p1.y);
            Descriptor d1 = new Descriptor();
            Descriptor d2 = new Descriptor();
            if (v2.PolarAngle < 0)
                d1.ang = v2.PolarAngle + 2*Math.PI;
            else
                d1.ang = v2.PolarAngle;

            d1.orien = -1;
            
            if (d1.ang + Math.PI<2*Math.PI)
            d2.ang = d1.ang + Math.PI;
            else
            d2.ang = d1.ang - Math.PI;
            d2.orien = 1;



            d1.numReg = m;
            d2.numReg = m;
            d1.numMnog = n;
            d2.numMnog = n;
            d2.numSeg = k;
            d1.numSeg = k+1;
            SortedSet<Descriptor> l = new SortedSet<Descriptor>();
            l.Add(d1);
            l.Add(d2);


            return l;
        }

        /// <summary>
        ///  Процедура создания дескриптора  для концевой точки пересечения.
        /// </summary>
        /// <param name="p">Точка пересечения.</param>
        /// <param name="m">Номер многоугольника.</param>
        /// <param name="n">Номер контура.</param>
        ///  <param name="k">Номер ребра.</param>
        /// <param name="s1">Ребро. </param>
        ///  <returns>  Дескриптор ребра, для которого точка пересечения является концевой. </returns>

        public static Descriptor newDescrip1(Point2D p, Segment s1, int m, int n, int k)
        {
            Point2D v2;
            if (p.Equals(s1.p1))
             v2 = new Point2D(s1.p2.x - s1.p1.x, s1.p2.y - s1.p1.y);
            else
            v2 = new Point2D(s1.p1.x - s1.p2.x, s1.p1.y - s1.p2.y);
            Descriptor d1 = new Descriptor();
            
            if (v2.PolarAngle < 0)
                d1.ang = v2.PolarAngle + 2 * Math.PI;
            else
                d1.ang = v2.PolarAngle;

            if (s1.p1.Equals(p))

            d1.orien = -1;
            else
            d1.orien = 1;

             d1.numReg = m;
             d1.numMnog = n;
            d1.numSeg = k;
            return d1;
        }


    }
}
