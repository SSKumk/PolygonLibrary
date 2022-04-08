using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolygonLibrary.Segments;
using PolygonLibrary.Basics;
using PolygonLibrary;
using PolygonLibrary.Rearranging;
namespace PolygonLibrary.Marker
{
    public class MarkerBasis
    {
        
        /// <summary>
        /// Метод считает уголол между векторами (p1; seg.p1) и (p1; seg.p2). 
        /// </summary>
        /// <param name="p1">Точка</param>
        /// <param name="seg">Ребро.</param>
        /// <returns>Угол между векторами (p1; seg.p1) и (p1; seg.p2). </returns>
        public static double Argument(Point2D p1, Segment seg)
        {
            Point2D v1 = new Point2D(seg.p1.x - p1.x, seg.p1.y - p1.y);// начало
            Point2D v2 = new Point2D(seg.p2.x - p1.x, seg.p2.y - p1.y);// конец
            double pv1, pv2;
            if (v1.PolarAngle < 0 | (v2.PolarAngle < 0 & v1.PolarAngle == 0))
                pv1 = v1.PolarAngle + 2 * Math.PI;
            else
                pv1 = v1.PolarAngle;
            if (v2.PolarAngle < 0 | (v1.PolarAngle < 0 & v2.PolarAngle == 0))
                pv2 = v2.PolarAngle + 2 * Math.PI;
            else
                pv2 = v2.PolarAngle;
            if (pv2 - pv1 > Math.PI)
                return -2 * Math.PI + (pv2 - pv1);
            else
                if (pv2 - pv1 < -Math.PI)
                    return 2 * Math.PI + pv2 - pv1;
                else
                    return pv2 - pv1;
        }
       
        /// <summary>
        /// Метод проверяет лежит ли точка внутри многоугольниуа.
        /// </summary>
        /// <param name="p1">Точка</param>
        /// <param name="seg">Контур.</param>
        /// <returns>Возвращает целое число (1, -1 или 0) . 1 - Точка лежит внутри контура и обход происходит против часовой стрелки. 0 - точка лежит снаружи. -1 - Точка лежит внутри контура, но обход происходит по часовой стрелки.</returns>
        public static int Inside(Point2D p, List<Segment> cont)
        {
            //List<double> L = new List<double>();
            double P = 0;
            foreach (Segment seg in cont)
            {
                double l = Argument(p, seg);
                P += l;
                // L.Add(l);
            }
            if (P > 6)
                return 1;
            else
                if (P < -6) return -1;
                else
                    return 0;
        }
        /// <summary>
        /// Проверяет принадлежность точки многоугольнику.
        /// </summary>
        /// <param name="p1">Точка</param>
        /// <param name="seg">Многоугольник.</param>
        /// <returns>Логическое значение</returns>
        
        public static bool Belong(Point2D p, List<Segment> mnog)
        {
            bool b = false;
            foreach (var n in mnog)
            {
                if (n.p1 == p)
                { b = true; break; }
            }
            return b;
        }


        /// <summary>
        /// Метод маркирующий контур.
        /// </summary>
        /// <param name="cont">Маркируемый контур.</param>
        /// <param name="mnog">Многоугольник не содержащий в себе маркируемый контур.</param>
        /// <param name="PointDictionary">Словарь дескрипторов. Ключом являются точка пересечения. Значение - список дескрипторов, отсортированных по возрастанию полярного угла соответствующих ребер.</param>
        /// <param name="m">Номер многоугольника который содержит в себе маркируемый контур.</param>
        /// <returns>Промаркированный контур.</returns>
        public static СontourMarker markerContour(List<Segment> cont, List<List<Segment>> mnog, 
            SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary, int m)
        {
            bool a = true;
            foreach (var n in cont)
            {
                if (PointDictionary.ContainsKey(n.p1) || PointDictionary.ContainsKey(n.p2))
                {
                    a = false;
                    break;
                }
            }
            if (a)
            {
                bool b = false;
                for (int i = 0; i < mnog.Count; i++)
                {
                    if (Inside(cont[0].p1, mnog[i]) == 1) b = true;
                    if (Inside(cont[0].p1, mnog[i]) == -1) { b = false; break; }

                }
                if (b)
                    return new СontourMarker(markerEdge(cont,mnog,PointDictionary,m), "Inside",0,m);
                else
                    return new СontourMarker(markerEdge(cont, mnog, PointDictionary, m), "Outside",0,m);
            }
                else
                    return new СontourMarker(markerEdge(cont, mnog, PointDictionary, m), "Isected",0,m);

        }

        
        
        /// <summary>
        /// Определяет лежит ли ребро внутри маркирующего угла
        /// </summary>
        /// <param name="seg">Отрезок</param>
        /// <param name="listOfDes">Список дескрипторов.</param>
        /// <param name="m">Номер многоугольника.</param>
        ///  <param name="l">1 - точка начала, 2 -  точка конца отрезка</param>
        /// <returns>Логическое значение</returns>
        public static bool insideCorner(Segment seg, SortedSet<Descriptor> listOfDes, int m, int r)
        {
            Descriptor l = new Descriptor();
            Descriptor l1 = new Descriptor();
            
            bool h = false;
            Point2D poin ;
            if (r==1)
                 poin = new Point2D(seg.p2.x - seg.p1.x, seg.p2.y - seg.p1.y);
            else
                 poin = new Point2D(seg.p1.x - seg.p2.x, seg.p1.y - seg.p2.y);
            double arg;
            // вычисляем полярный угол отрезка.
            if (poin.PolarAngle < 0)
                arg = poin.PolarAngle + 2 * Math.PI;
            else arg = poin.PolarAngle;

            List<Descriptor> list = new List<Descriptor>();

            // выбираем только дескрипторы принадлежащие другому многоугольнику.
            foreach (var n in listOfDes)
            {
                if (n.numReg != m)
                {
                    list.Add(n);
                }
            }
           
            for (int i=0; i<list.Count;i++)
            {if (i == 0 & list[i].orien == 1 & list[i].ang > arg) { h = true; break; }
             if (i == list.Count-1 & list[i].orien == -1 & list[i].ang < arg) { h = true; break; }
                    if (l1.orien==-1&  list[i].ang < arg) { h = true; break; }
                    if (i+1 % 2 == 1)
                    { l = list[i];  }
                    else
                    { l1 = list[i];  }
                    if (l.orien == 1 & l.ang > arg & l1.ang < arg) { break; }
                    if (l.orien == -1 & l.ang < arg & l1.ang > arg) { h = true; break; }
                }
            
            
            return h;
        }
        //public static bool insideCorner2(Segment seg, SortedSet<Descriptor> listOfDes, int m)
        //{
        //    Descriptor l = new Descriptor();
        //    Descriptor l1 = new Descriptor();
        //    int k = 1;
        //    bool h = false;
        //    Point2D poin = new Point2D(seg.p1.x - seg.p2.x, seg.p1.y - seg.p2.y);
        //    double arg;
        //    if (poin.PolarAngle < 0)
        //        arg = poin.PolarAngle + 2 * Math.PI;
        //    else arg = poin.PolarAngle;

        //    List<Descriptor> list = new List<Descriptor>();


        //    foreach (var n in listOfDes)
        //    {
        //        if (n.numReg != m)
        //        {
        //            list.Add(n);
        //        }
        //    }

        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        if (i == 0 & list[i].orien == 1 & list[i].ang > arg) { h = true; break; }
        //        if (i == list.Count - 1 & list[i].orien == -1 & list[i].ang < arg) { h = true; break; }
        //        if (l1.orien == -1 & list[i].ang < arg) { h = true; break; }
        //        if (k % 2 == 1)
        //        { l = list[i]; k++; }
        //        else
        //        { l1 = list[i]; k++; }
        //        if (l.orien == 1 & l.ang > arg & l1.ang < arg) { break; }
        //        if (l.orien == -1 & l.ang < arg & l1.ang > arg) { h = true; break; }
        //    }


        //    return h;
        //}

        /// <summary>
        /// Метод маркирующий ребра контура.
        /// </summary>
        /// <param name="cont">Маркируемый контур</param>
        /// <param name="mnog">Многоугольник не содержащий в себе маркируемый контур.</param>
        /// <param name="PointDictionary">Словарь дескрипторов. Ключом являются точка пересечения. Значение - список дескрипторов, отсортированных по возрастанию полярного угла соответствующих ребер.</param>
        /// <param name="m">Номер многоугольника который содержит в себе маркируемый контур.</param>
        /// <returns>Контур с промаркированными ребрами</returns>
        


        public static List<EdgeMarker> markerEdge(List<Segment> cont, 
            List<List<Segment>> mnog, SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary, int m)
        {
            List<EdgeMarker> newCont = new List<EdgeMarker>();
            for (int i = 0; i < cont.Count; i++)
            {

                //пункт 2.1  ////////////////////////////////////////////////////////////////////////////////////////    
                // Концы не являются точками пересечения.
                if (!PointDictionary.ContainsKey(cont[i].p1) & !PointDictionary.ContainsKey(cont[i].p2))
                {
                    if (i == 0)
                    {
                        // первое ребро
                        bool b = false;
                        for (int j = 0; j < mnog.Count; j++)
                        {
                            if (Inside(cont[i].p1, mnog[j]) == 1) b = true;
                            if (Inside(cont[i].p1, mnog[j]) == -1) { b = false; break; }

                        }
                        if (b)
                            newCont.Add(new EdgeMarker(cont[i], "Inside",m));// Лежит внутри другого многоугольника
                        else
                            newCont.Add(new EdgeMarker(cont[i], "Outside", m));//  не лежит внутри другого многоугольника
                    }
                    else
                    {
                        // копируем метку у предыдущего ребра
                        EdgeMarker mar = new EdgeMarker(cont[i], newCont[i - 1].marker,m);
                        newCont.Add(mar);
                    }
                }
                
                //пункт 2.2//////////////////////////////////////////////////////////////////////////////////////////////
                // 2.2 а б /////////////////////////////////////////////////////////////////////////////////////////////

                    if (PointDictionary.ContainsKey(cont[i].p1) & PointDictionary.ContainsKey(cont[i].p2))
                    {// оба конца точки пересечения
                        Segment rev = new Segment(cont[i].p2, cont[i].p1);
                        int j = 0;

                        while (j < mnog.Count)
                        {
                            if (mnog[j].Contains(cont[i]))
                            {
                                newCont.Add(new EdgeMarker(cont[i], "Shared1",m));// В другом многоугольнике есть ребро сонаправленное с данным.
                                break;
                            }
                            if (mnog[j].Contains(rev))
                            {
                                newCont.Add(new EdgeMarker(cont[i], "Shared2", m));// В другом многоугольнике есть ребро противоположно направленный  с данным.
                                break;
                            }
                            j++;
                        }
                        // Если ребро не нашли.
                        if (j == mnog.Count)
                        {
                            if (insideCorner(cont[i], PointDictionary[cont[i].p1], m,1))
                                newCont.Add(new EdgeMarker(cont[i], "Inside",m));
                            else
                                newCont.Add(new EdgeMarker(cont[i], "Outside",m));
                        }
                    }
                    

//////////////////Если начало точка пересечения/////////////////////////////

                        if (PointDictionary.ContainsKey(cont[i].p1)&!PointDictionary.ContainsKey(cont[i].p2))
                        {
                            int j = 0;
                            // проверяем содержит ли эту точку другой многоугольник, чтобы исключить самокасание 
                            while (j < mnog.Count)
                            {
                                if (Belong(cont[i].p1, mnog[j])) break;
                                j++;
                            }

                            if (j < mnog.Count)
                            {
                                if (insideCorner(cont[i], PointDictionary[cont[i].p1], m,1))
                                    newCont.Add(new EdgeMarker(cont[i], "Inside",m));// лежит внутри маркируещего угла
                                else
                                    newCont.Add(new EdgeMarker(cont[i], "Outside", m));// не лежит внутри маркируещего угла

                            }


                            else
                            { // произошло самокасание
                                if (i == 0)
                                {
                                    bool b = false;
                                    for (int q = 0; q < mnog.Count; q++)
                                    {
                                        if (Inside(cont[i].p1, mnog[q]) == 1) b = true;
                                        if (Inside(cont[i].p1, mnog[q]) == -1) { b = false; break; }

                                    }
                                    if (b)
                                        newCont.Add(new EdgeMarker(cont[i], "Inside",m));
                                    else
                                        newCont.Add(new EdgeMarker(cont[i], "Outside",m));
                                }
                                else
                                {
                                    newCont.Add(new EdgeMarker(cont[i], newCont[i - 1].marker,m));
                                }
                            }

                        }
                   /////////////если конец точка пересечения//////////////////////////////////
                        if (!PointDictionary.ContainsKey(cont[i].p1)&PointDictionary.ContainsKey(cont[i].p2))
                        {// проверяем содержит ли эту точку другой многоугольник, чтобы исключить самокасание
                            int j = 0;
                            while (j < mnog.Count)
                            {
                                if (Belong(cont[i].p2, mnog[j])) break;
                                j++;
                            }

                            if (j < mnog.Count)
                            {
                                if (insideCorner(cont[i], PointDictionary[cont[i].p2], m,2))
                                    newCont.Add(new EdgeMarker(cont[i], "Inside", m));// лежит внутри маркируещего угла
                                else
                                    newCont.Add(new EdgeMarker(cont[i], "Outside", m));// не лежит внутри маркируещего угла

                            }


                            else
                            {
                                // произошло самокасание, действуем как в пункте 1
                                if (i == 0)
                                {
                                    bool b = false;
                                    for (int q = 0; q < mnog.Count; q++)
                                    {
                                        if (Inside(cont[i].p2, mnog[q]) == 1) b = true;
                                        if (Inside(cont[i].p2, mnog[q]) == -1) { b = false; break; }

                                    }
                                    if (b)
                                        newCont.Add(new EdgeMarker(cont[i], "Inside",m));
                                    else
                                        newCont.Add(new EdgeMarker(cont[i], "Outside",m));
                                }
                                else
                                {
                                    newCont.Add(new EdgeMarker(cont[i], newCont[i - 1].marker,m));
                                }
                            }

                        }


                    }

            return newCont;
        }
        /// <summary>
        /// Метод маркирующий многоугольник.
        /// </summary>
        /// <param name="mnog1">Маркируемый многоугольник.</param>
        /// <param name="mnog2">Многоугольник относительно которого маркируется первый многоугольник</param>
        /// <param name="PointDictionary">Словарь дескрипторов. Ключом являются точка пересечения. Значение - список дескрипторов, отсортированных по возрастанию полярного угла соответствующих ребер.</param>
        /// <param name="m">Номер маркируемого многоугольника .</param>
        /// <returns>Промаркированный многоугольник.</returns>

        public static void markerMnog(List<List<Segment>> mnog1, out List<СontourMarker> res1, List<List<Segment>> mnog2, 
            SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary, int m)
        {
            res1 = new List<СontourMarker>();
            for (int i = 0; i < mnog1.Count; i++)
            {
                СontourMarker mar = markerContour(mnog1[i], mnog2, PointDictionary, m);
                res1.Add(mar);
            }

        }

        internal static СontourMarker markerContour(СontourMarker СontourMarker, List<List<Segment>> mnog2, SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary, int m)
        {
            throw new NotImplementedException();
        }
    }
}
        
   

