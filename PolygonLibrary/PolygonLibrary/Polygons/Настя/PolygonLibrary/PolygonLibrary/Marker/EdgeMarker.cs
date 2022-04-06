using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolygonLibrary.Segments;
using PolygonLibrary.Basics;
using PolygonLibrary;

namespace PolygonLibrary.Marker
{
    /// <summary>
    /// Класс описывающий структуру данных EdgeMarker.
    /// </summary>
   public class   EdgeMarker
    {
        /// <summary>
        /// Ребро.
        /// </summary>
        public Segment edge;
        /// <summary>
        /// Маркер ребра ( "Inside", "Outside","Shared1","Shared2").
        /// </summary>
        public String marker;
        /// <summary>
        /// Номер многоугольника (1 или 2).
        /// </summary>
        public int numR;
        /// <summary>
        /// Индикатор обработки ребра (1- ребро пройдено, 0 - ребро не пройдено).
        /// </summary>
        public int r;
        /// <summary>
        /// Конструкторы для инициализации нового экземпляра класса.
        /// </summary>
        /// 
        /// <summary>
        /// Конструктор для создание EdgeMarker с начальными данными.
        /// </summary>
        public EdgeMarker()
        {
            edge = new Segment(0,0,0,0);
            marker = "";
            numR = 0;
            r = 0;
        }
        /// <summary>
        ///  Конструктор для создание EdgeMarker по ребру.
        /// </summary>
        /// <param name="c">Ребро.</param>
        public EdgeMarker(Segment c)
        {
            edge = c;
            marker = "";
            numR = 0;
            r = 0;
        }
        /// <summary>
        /// Конструктор для задания EdgeMarker .
        /// </summary>
        /// <param name="c">Ребро.</param>
        /// <param name="m">Номер многоугольника(1 или 2).</param>
        ///  <param name="str">Маркер ребра ( "Inside", "Outside","Shared1","Shared2").</param>
        /// <param name="re">Индикатор обработки ребра (1- ребро пройдено, 0 - ребро не пройдено).</param>
        
        public EdgeMarker (Segment c, String str, int m,int re)
        {
            edge = c;
            marker = str;
            numR = m;
            r = re;
        }
        /// <summary>
        /// Конструктор для задания EdgeMarker .
        /// </summary>
        /// <param name="c">Ребро.</param>
        /// <param name="m">Номер многоугольника(1 или 2).</param>
        ///  <param name="str">Маркер ребра ( "Inside", "Outside","Shared1","Shared2").</param>
        
        public EdgeMarker(Segment c, String str, int m)
        {
            edge = c;
            marker = str;
            numR = m;
            r = 0;
        }
        /// <summary>
        /// Конструктор копирующий поля другого EdgeMarker.
        /// </summary>
        /// <param name="d">EdgeMarker</param>
        public EdgeMarker(EdgeMarker mar)
        {
            edge = mar.edge
                ;
            marker = mar.marker;
            numR = mar.numR;
            r = mar.r;
        }
    }
}
