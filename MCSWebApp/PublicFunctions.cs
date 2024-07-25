using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;

namespace MCSWebApp
{
    public class PublicFunctions
    {
        public static string ExcelFolder = @"\excel";
        public static int ExcelDefaultColumnWidth = 20;
        
        public static string IsNullCell(ICell cell)
        {
            if (cell != null) return cell.ToString().Trim();
            else return "";
        }

        public static string IsNullCell(ICell cell, dynamic defaultvalue)
        {
            if (cell != null) return cell.ToString().Trim();
            else return Convert.ToString(defaultvalue);
        }

        public static dynamic DynCell(dynamic cell, dynamic defaultvalue)
        {
            if (cell != null) return cell;
            else return defaultvalue;
        }

        public static int Bulat(dynamic cell)
        {
            if (cell != null)
            {
                int n = 0;
                bool isNumeric = int.TryParse(cell.ToString().Trim(), out n);
                return n;
            }
            else return 0;
        }

        public static decimal Desimal(dynamic cell)
        {
            if (cell != null)
            {
                decimal n = 0;
                bool isNumeric = decimal.TryParse(cell.ToString().Trim(), out n);
                return n;
            }
            else return 0;
        }

        public static double Pecahan(dynamic cell)
        {
            if (cell != null)
            {
                double n = 0;
                bool isNumeric = double.TryParse(cell.ToString().Trim(), out n);
                return n;
            }
            else return 0;
        }

        public static Boolean BenarSalah(dynamic cell)
        {
            if (cell != null)
            {
                bool b = false;
                bool isBool = bool.TryParse(cell.ToString().Trim(), out b);
                return b;
            }
            else return false;
        }

        public static DateTime? TanggalNull(dynamic cell)
        {
            if (cell == null)
                return null;
            if (cell != null && cell.CellType == CellType.Numeric)
            {
                return DateTime.FromOADate(cell.NumericCellValue);
            }

            DateTime tgl;
            bool isDate = DateTime.TryParse(cell.ToString().Trim(), out tgl);
            return tgl;
        }

        public static DateTime Tanggal(dynamic cell)
        {
            if (cell == null)
            {
                DateTime tgl1 = new DateTime(1900, 1, 1, 0, 0, 0);
                return tgl1;
            }
            if (cell != null && cell.CellType == CellType.Numeric)
            {
                return DateTime.FromOADate(cell.NumericCellValue);
            }

            DateTime tgl;
            bool isDate = DateTime.TryParse(cell.ToString().Trim(), out tgl);
            return tgl;
        }

        public static DateTime Waktu(dynamic cell)
        {
            if (cell != null)
            {
                DateTime jam;
                bool isDate = DateTime.TryParse(cell.ToString().Trim(), out jam);
                return jam;
            }
            else return Convert.ToDateTime("00:00:00");
        }

    }
}
