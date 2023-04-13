using System;

namespace Robust
{
	internal class Constants
	{
		public const double DifStep = .5;
		public const double FloatPrec = 1e-9;
		// error messages
		public const string AIsNull = "Матрица A не определена";
		public const string BIsNull = "Матрица B не определена";
		public const string CIsNull = "Матрица C не определена";
		public const string MuIsNull = "Вектор Mu не определен";
		public const string NuIsNull = "Вектор Nu не определен";
		public const string PIsNull = "P is null";
		public const string QIsNull = "Q is null";
		public const string MIsNull = "Терминальное множество M не определено";
		public const string ASizes = "Матрица A должна быть квадратной";
		public const string ABSizes = "Матрицы A и B должны иметь одинаковое число строк";
		public const string ACSizes = "Матрицы A и C должны иметь одинаковое число строк";
		public const string MuBSizes = "Число столбцов в матрицы B и размерность вектора Mu должны совпадать";
		public const string NuCSizes = "Число столбцов в матрицы C и размерность вектора Nu должны совпадать";
		public const string PSmall = "P has less than 2 elements";
		public const string QSmall = "Q has less than 2 elements";
		public const string PSizes = "P elements have various dimensions";
		public const string QSizes = "Q elements have various dimensions";
		public const string PBSizes = "P elements and matrix B have various dimensions";
		public const string QCSizes = "Q elements and matrix C have various dimensions";
		public const string RowsEqual = "Row1 и Row2 должны быть различны";
		public const string Row1Wrong = "Неправильно задан Row1";
		public const string Row2Wrong = "Неправильно задан Row2";
		public const string TimeWrong = "Неверно задано время";
		public const string TimeDeltaWrong = "Неверно задан шаг времени";
		public const string PointSize = "Неправильно задана точка";
		public const string RobustBridgeBreak = "Основной мост не должен обрываться";
		public const string SwitchBridgeBreak = "All extremal control bridges are ended";
		public const string SinusWrong = "Неправильно заданы амплитуды и длины волн";
        public const string DPolyComplex = "Несколько пересечений на одном отрезке многоугольника";
	}
}
