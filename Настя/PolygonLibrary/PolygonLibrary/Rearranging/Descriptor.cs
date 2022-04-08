using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;
using PolygonLibrary.Segments;

namespace PolygonLibrary.Rearranging
{
       
    public class Descriptor : IComparable<Descriptor>
    {/// <summary>
        /// Класс описывающий структуру данных Descriptor.
        /// </summary>
        public int CompareTo(Descriptor other)
        {
            int res = Tools.CMP(this.ang, other.ang);
            if (res != 0)
                return res;
            else
                return -1;
        }
        /// <summary>
        /// Полярный угол ребра.
        /// </summary>
        public double ang;
        /// <summary>
        /// Ориентация ребра( Входящее 1, Выходящее -1).
        /// </summary>
        public int orien;
        /// <summary>
        /// Ссылка на ребро.
        /// </summary>
        public int numReg;
        public int numMnog;
        public int numSeg;
        
        
        #region Constructors
        /// <summary>
        /// Конструкторы для инициализации нового экземпляра класса.
        /// </summary>
        /// 
        /// <summary>
        /// Конструктор для создание Descriptor с начальными данными.
        /// </summary>
        public Descriptor()
        {
            ang = 0;
            orien = 1;
            numReg = 1;
           
            numMnog = 0;
            numSeg = 0;
        }

        /// <summary>
        /// Конструктор для задания Descriptor.
        /// </summary>
        /// <param name="da">Полярный угол.</param>
        /// <param name="dr">Номер многоугольника(1 или 2).</param>
        ///  <param name="dy">Ориентация ребра.</param>
        /// <param name="dm">Номер контура(нумерация начинается с 0).</param>
        ///  <param name="ds">Номер ребра(нумерация начинается с 0).</param>
        public Descriptor(double da, int dy , int dr, int dm, int ds)
        {
            ang = da;
            numReg = dr;
            orien = dy;
            numMnog = dm;
            numSeg = ds;
            
           
        }

        /// <summary>
        /// Конструктор копирующий поля другого Descriptor.
        /// </summary>
        /// <param name="d">Descriptor</param>
        public Descriptor(Descriptor d)
        {
            ang = d.ang;
            orien = d.orien;
            numReg = d.numReg;
            numMnog = d.numMnog;
            numSeg = d.numSeg;
           
        }
        #endregion


    }
}

