using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolygonLibrary.Segments;
using PolygonLibrary.Basics;
using PolygonLibrary;
using PolygonLibrary.Rearranging;
using PolygonLibrary.Marker;

namespace PolygonLibrary.Build
{
   public class BuildResult
    {
        /// <summary>
        /// Метод возвращающий результат булевских операций над многоугольниками . 
        /// </summary>
        /// <param name="mnog1">Первый многоугольник.</param>
        /// <param name="mnog2">Второй многоугольник.</param>
        /// <param name="operation">Булевская операция над многоугольниками (OR-объединение , AND-пересечение, SUB-разность, SIMSUB-симметрическая разность).</param>

        /// <returns>Результирующий многоугольник .</returns>

       public static void buildResult(List<List<Point2D>> mnog1, List<List<Point2D>> mnog2, string operation, out List<List<EdgeMarker>> res)
         {
             res = new List<List<EdgeMarker>>();/// результат

             List<List<Segment>> res1 = new List<List<Segment>>();
             List<List<Segment>> res2 = new List<List<Segment>>();
             SortedDictionary<Point2D, SortedSet<Descriptor>> PointDictionary = new SortedDictionary<Point2D,SortedSet<Descriptor>>();
             Rearranger.polygonRearranging(mnog1, mnog2, out  res1, out  res2, out  PointDictionary);/// переразбили 
            
             List<EdgeMarker> cont = new List<EdgeMarker>();
             List<СontourMarker> reg1 = new List<СontourMarker>();
             List<СontourMarker> reg2 = new List<СontourMarker>();
             Marker.MarkerBasis.markerMnog(res1, out reg1, res2, PointDictionary, 1);// Промаркировали 
             Marker.MarkerBasis.markerMnog(res2, out reg2, res1, PointDictionary, 2);
             List<List<СontourMarker>> reg = new List<List<СontourMarker>>();
             reg.Add(reg1);
             reg.Add(reg2);
             int[] h = new int[] { 0,0 };
             

           // Пробегаемся по многоугольникам и включаем в результат ребра.
             int i1 = 0; int j1 = 0; int k1 = 0, nap=0; 
while(!(reg[0].Count==h[0]&reg[1].Count==h[1]))
{
    
    int t = 0;
    reg1 = reg[i1];
     while (j1 < reg1.Count)
      {
         //Если контур является внутренним относительно другого многоугольника.

          switch (conRul(reg1[j1], operation))
          {
              case 1:
                  {
                      res.Add(reg1[j1].contour); reg1[j1].r = reg1[j1].contour.Count; h[i1]++; //Добавляем контур в результат с исходной ориентацией.

                      j1++; break;
                  };

              //Если контур является внешним относительно другого многоугольника.

              case -1:
                  {
                      res.Add(сontRev(reg1[j1].contour)); reg1[j1].r = reg1[j1].contour.Count; h[i1]++;
                      j1++; break;
                  };
              default:
                  //Если контур имеет точки пересечения с другим многоугольником и контур имеет не обработанные ребра.              
                  {
                      if (!reg1[j1].marker.Equals("Isected"))
                      {
                          reg1[j1].r = reg1[j1].contour.Count; h[i1]++;
                          j1++; break;
                      }
                      if (reg1[j1].marker.Equals("Isected") & reg1[j1].r != reg1[j1].contour.Count)
                      {//Пробегает по контуру.
                          while (k1 < reg1[j1].contour.Count && k1 >= 0)
                          {
                              //Если ребро не обработано.
                              if (reg1[j1].contour[k1].r == 0)


                                  switch (edgeRul(reg1[j1].contour[k1], operation))
                                  {// Ребро не входит в результат.
                                      case 0:
                                          {
                                              reg1[j1].contour[k1].r = 1; reg1[j1].r++;
                                              if (reg1[j1].r == reg1[j1].contour.Count) // Если ребро последнее, переходим в начало.
                                              { h[i1]++; k1 = 0; t = 1; break; }
                                              if (nap == 0)// Выбираем направление обхода.
                                                  k1++;
                                              else k1--;
                                          } break;
                                      // Ребро входит в результат с обратной ориентацией. 
                                      case -1:
                                          {

                                              nap = 1;// Обратное направление обхода.
                                              Segment a = new Segment(reg1[j1].contour[k1].edge.p2, reg1[j1].contour[k1].edge.p1);
                                              EdgeMarker marc = new EdgeMarker(reg1[j1].contour[k1]);
                                              marc.edge = a;

                                              cont.Add(marc);
                                              reg1[j1].contour[k1].r = 1;
                                              reg1[j1].r++;
                                              if (reg1[j1].r == reg1[j1].contour.Count) { h[i1]++; };//Если контур пройден.
                                              if (reg1[j1].contour[k1].edge.p1.Equals(cont[0].edge.p1)) // Если контур замкнулся.
                                              {
                                                  res.Add(new List<EdgeMarker>(cont)); cont.Clear(); reg[i1] = reg1; k1 = 0; j1 = 0; t = 1; break;
                                              }
                                              if (PointDictionary.ContainsKey(reg1[j1].contour[k1].edge.p1))// Если конец - точка пересечения.
                                              {
                                                  reg[i1] = reg1;
                                                  List<Descriptor> dis = new List<Descriptor>();
                                                  dis = (listOftrue(PointDictionary[reg1[j1].contour[k1].edge.p1], i1, j1, k1));
                                                  foreach (var l in dis)
                                                  {
                                                      reg1 = reg[l.numReg - 1];
                                                      if (reg1[l.numMnog].contour[l.numSeg].r == 0 && Math.Abs(edgeRul(reg1[l.numMnog].contour[l.numSeg], operation)) == 1) { i1 = l.numReg - 1; j1 = l.numMnog; k1 = l.numSeg; t = 1; break; }
                                                  }
                                                  if (t == 1) break;
                                              }

                                              k1--;


                                          } break;
                                      case 1:
                                          {
                                              nap = 0;// Прямое направление обхода.
                                              cont.Add(reg1[j1].contour[k1]);
                                              reg1[j1].contour[k1].r = 1;
                                              reg1[j1].r++;
                                              if (reg1[j1].r == reg1[j1].contour.Count) h[i1]++;//Если контур пройден.
                                              if (reg1[j1].contour[k1].edge.p2.Equals(cont[0].edge.p1))// Если контур замкнулся.
                                              {
                                                  res.Add(new List<EdgeMarker>(cont)); cont.Clear(); reg[i1] = reg1; k1 = 0; j1 = 0; t = 1; break;
                                              }
                                              if (PointDictionary.ContainsKey(reg1[j1].contour[k1].edge.p2))// Если конец - точка пересечения.
                                              {
                                                  reg[i1] = reg1;
                                                  List<Descriptor> dis = new List<Descriptor>();
                                                  dis = (listOftrue(PointDictionary[reg1[j1].contour[k1].edge.p2], i1 + 1, j1, k1));
                                                  foreach (var l in dis)
                                                  {
                                                      reg1 = reg[l.numReg - 1];
                                                      if (reg1[l.numMnog].contour[l.numSeg].r == 0 && Math.Abs(edgeRul(reg1[l.numMnog].contour[l.numSeg], operation)) == 1) { i1 = l.numReg - 1; j1 = l.numMnog; k1 = l.numSeg; t = 1; break; }
                                                  }
                                                  if (t == 1) break;
                                              }

                                              k1++;

                                          } break;

                                  }
                              else k1++;
                              if (t == 1) break;


                          }
                          if (k1 >= reg1[j1].contour.Count | k1 < 0) { if (reg1[j1].r == reg1[j1].contour.Count) { k1 = 0; j1++; } else  k1 = 0; }
                      }
                      else 
                          j1++;
                  } break;
          }
           if (t == 1) break;
                }
                if (j1 >= reg1.Count) {   j1 = 0; if (i1 == 0) i1 = 1; else i1 = 0; }
                     }

                     
                 }
               
                
        
         
         

         /// <summary>
         /// Метод формирует список дескрипторов. 
         /// </summary>
         /// <param name="dis">Исходный список дескрипторов.</param>
       /// <param name="nR">Номер многоугольника, которому принадлежит ребро относительно которого создается новый список дескрипторов.</param>
       /// <param name="nM">Номер контура, которому принадлежит ребро относительно которого создается новый список дескрипторов.</param>
       /// <param name="nS">Номер pебра относительно которого создается новый список дескрипторов.</param>
       /// <returns>Список дескрипторов начинающийся с заданного ребра и отсортированных в направлении против часовой стрелки .</returns>
         public static List<Descriptor> listOftrue(SortedSet<Descriptor> dis, int nR, int nM, int nS) {
             List<Descriptor> res=new List<Descriptor>();
             
             int i=0;
             foreach (var d in dis) {
                 if (d.numMnog == nM && d.numReg == nR && d.numSeg == nS) break;
                 else i++; 
             }
             res.AddRange(dis.Skip(i));
             res.AddRange(dis.Take(i).Reverse());
             return res;

         }



         /// <summary>
         /// Метод изменяющий направление обхода контура. 
         /// </summary>
         /// <param name="mnog">Контур.</param>
         /// <returns>Контур с противоположным направлением обхода.</returns>
         
         public static List<EdgeMarker> сontRev(List<EdgeMarker> mnog)
         {
             List<EdgeMarker> res = new List<EdgeMarker>();
             int i = mnog.Count-1;
             while(i>=0){
                 Segment newseg = new Segment(mnog[i].edge.p2, mnog[i].edge.p1);
                 mnog[i].edge=newseg;
                 res.Add(mnog[i]);
                 i--;
             }
             return res;
         }


         /// <summary>
         /// Метод определяет вхождение ребра в результат. 
         /// </summary>
         /// <param name="operation">Операция.</param>
         /// <param name="seg">Ребро.</param>
         /// <returns>0 - Ребро не входит в результат. 1 - Ребро входит в результат с исходной ориентацией. -1 - Ребро входит в результат с обратной ориентацией.</returns>

         public static int edgeRul(EdgeMarker seg, string operation)
         {

             if (seg.numR == 1 && seg.marker.Equals("Outside"))
             {
                 if (operation.Equals("OR") | operation.Equals("SUB") | operation.Equals("SIMSUB"))
                     return 1;

             }
             if (seg.numR == 2 && seg.marker.Equals("Outside"))
             {
                 if (operation.Equals("OR") | operation.Equals("SIMSUB"))
                     return 1;
             }

             if (seg.numR == 1 && seg.marker.Equals("Inside"))
             {
                 if (operation.Equals("AND"))
                     return 1;
                 if (operation.Equals("SIMSUB"))
                     return -1;
             }

             if (seg.numR == 2 && seg.marker.Equals("Inside"))
             {
                 if (operation.Equals("SUB") | operation.Equals("SIMSUB"))
                     return -1;
                 if (operation.Equals("AND"))
                     return 1;
             }

             if (seg.numR == 1 && seg.marker.Equals("Shared2"))
             {
                 if (operation.Equals("SUB"))
                     return 1;
             }

             if (seg.numR == 1 && seg.marker.Equals("Shared1"))
             {
                 if (operation.Equals("AND") | operation.Equals("OR"))
                     return 1;

             }
             return 0;
         }



         public static int conRul(СontourMarker con, string operation)
         {

             if (con.numC == 1 && con.marker.Equals("Outside"))
             {
                 if (operation.Equals("OR") | operation.Equals("SUB") | operation.Equals("SIMSUB"))
                     return 1;

             }
             if (con.numC == 2 && con.marker.Equals("Outside"))
             {
                 if (operation.Equals("OR") | operation.Equals("SIMSUB"))
                     return 1;
             }

             if (con.numC == 1 && con.marker.Equals("Inside"))
             {
                 if (operation.Equals("AND"))
                     return 1;
                 if (operation.Equals("SIMSUB"))
                     return -1;
             }

             if (con.numC == 2 && con.marker.Equals("Inside"))
             {
                 if (operation.Equals("SUB") | operation.Equals("SIMSUB"))
                     return -1;
                 if (operation.Equals("AND"))
                     return 1;
             }

            
             return 0;
         }
    }
}
