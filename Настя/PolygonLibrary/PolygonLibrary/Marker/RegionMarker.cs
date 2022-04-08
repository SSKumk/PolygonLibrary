using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolygonLibrary.Segments;
using PolygonLibrary.Basics;
using PolygonLibrary;

namespace PolygonLibrary.Marker
{ /// <summary>
    /// Класс описывающий структуру данных СontourMarker. Структура содержит в себе маркер контура.
    /// </summary>
    public class СontourMarker
    { /// <summary>
        /// Контур с промаркированными ребрами.
        /// </summary>
        public List<EdgeMarker> contour;
        /// <summary>
        /// Маркер контура ( "Inside", "Outside","Isected").
        /// </summary>
        public String marker;
        /// <summary>
        ///Индикатор обработки контура (1- контур пройден, 0 - контур не пройден).
        /// </summary>
        public int r;
        ///номер многоугольника (1- первый мнгоугольник, 2 - второй многоугольник).
        /// </summary>
        public int numC;

        /// <summary>
        /// Конструктор для создание СontourMarker с начальными данными.
        /// </summary>
        public СontourMarker()
        {
            contour = new List<EdgeMarker>();
            marker = "";
            r = 0;
            numC = 0;
        }

        /// <summary>
        /// Конструктор для задания СontourMarker .
        /// </summary>
        /// <param name="c">Контур.</param>
        ///  <param name="str">Маркер контура ( "Inside", "Outside","Isected").</param>
        
        public СontourMarker(List<EdgeMarker> c, String str)
        {
            contour = c;
            marker = str;
            r = 0;
            numC = 0;
        }
        /// <summary>
        /// Конструктор для задания СontourMarker .
        /// </summary>
        /// <param name="c">Контур.</param>
        ///  <param name="str">Маркер контура ( "Inside", "Outside","Isected").</param>
        /// <param name="re">Индикатор обработки контура (1- контур пройден, 0 - контур не пройден).</param>
        public СontourMarker(List<EdgeMarker> c, String str, int re, int n)
        {
            contour = c;
            marker = str;
            r = re;
            numC = n;
        }

      



    }
}
