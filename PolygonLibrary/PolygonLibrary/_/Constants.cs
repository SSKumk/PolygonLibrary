using System;

namespace Robust
{
	internal class Constants
	{
		public const double DifStep = .5;
		public const double FloatPrec = 1e-9;
		// error messages
		public const string AIsNull = "������� A �� ����������";
		public const string BIsNull = "������� B �� ����������";
		public const string CIsNull = "������� C �� ����������";
		public const string MuIsNull = "������ Mu �� ���������";
		public const string NuIsNull = "������ Nu �� ���������";
		public const string PIsNull = "P is null";
		public const string QIsNull = "Q is null";
		public const string MIsNull = "������������ ��������� M �� ����������";
		public const string ASizes = "������� A ������ ���� ����������";
		public const string ABSizes = "������� A � B ������ ����� ���������� ����� �����";
		public const string ACSizes = "������� A � C ������ ����� ���������� ����� �����";
		public const string MuBSizes = "����� �������� � ������� B � ����������� ������� Mu ������ ���������";
		public const string NuCSizes = "����� �������� � ������� C � ����������� ������� Nu ������ ���������";
		public const string PSmall = "P has less than 2 elements";
		public const string QSmall = "Q has less than 2 elements";
		public const string PSizes = "P elements have various dimensions";
		public const string QSizes = "Q elements have various dimensions";
		public const string PBSizes = "P elements and matrix B have various dimensions";
		public const string QCSizes = "Q elements and matrix C have various dimensions";
		public const string RowsEqual = "Row1 � Row2 ������ ���� ��������";
		public const string Row1Wrong = "����������� ����� Row1";
		public const string Row2Wrong = "����������� ����� Row2";
		public const string TimeWrong = "������� ������ �����";
		public const string TimeDeltaWrong = "������� ����� ��� �������";
		public const string PointSize = "����������� ������ �����";
		public const string RobustBridgeBreak = "�������� ���� �� ������ ����������";
		public const string SwitchBridgeBreak = "All extremal control bridges are ended";
		public const string SinusWrong = "����������� ������ ��������� � ����� ����";
        public const string DPolyComplex = "��������� ����������� �� ����� ������� ��������������";
	}
}
