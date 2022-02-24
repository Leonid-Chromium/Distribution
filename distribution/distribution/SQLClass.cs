using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace distribution
{
    class SQLClass
    {
        public static string standartConString = @"Data Source=DESKTOP-LEONID\SQLEXPRESS;Initial Catalog=DispatcherV3;Persist Security Info=True;User ID=Admin;Password=Admin";

        public static DataTable ReturnDT(string Query, string ConStr)
        {
            DataTable dataTable = new DataTable();

            try
            {
                Trace.WriteLine("ConString = " + ConStr);
                Trace.WriteLine("Query = " + Query);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(Query, ConStr);
                sqlDataAdapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return dataTable;
        }

        public static DataTable ReturnDT(string Query)
        {
            return ReturnDT(Query, standartConString);
        }

        public static int NoReturn(string Query, string ConStr)
        {
            try
            {
                Trace.WriteLine("Connection sring = " + ConStr);
                Trace.WriteLine("Query = " + Query);

                using (SqlConnection connection = new SqlConnection(ConStr))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(Query, connection);
                    command.ExecuteNonQuery(); //Выполнение запроса без возращения данных
                    connection.Close();
                }
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 1;
            }
        }

        public static int NoReturn(string Query)
        {
            try
            {
                return NoReturn(Query, standartConString);
            }
            catch
            {
                return 1;
            }
        }

        public static string ArrayToValue(string[] valueName, string[] value) // Используется для формирования строки-перечисления пар ([название столбца] = [значение])
        {
            List<string> valueСollection = new List<string>();
            for (int i = 0; i < valueName.Length; i++)
                valueСollection.Add(String.Concat(valueName[i], "=", value[i]));
            string resultStr = String.Join(", ", valueСollection);
            Trace.WriteLine(resultStr);
            return resultStr;
        }

        public static void DTtoTrace(DataTable dataTable)
        {
            Trace.WriteLine("");
            Trace.WriteLine("Общая информация");
            Trace.WriteLine(String.Format("x = " + dataTable.Columns.Count));
            Trace.WriteLine(String.Format("y = " + dataTable.Rows.Count));

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                Trace.Write("|");
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    Trace.Write(String.Format("{0,3}", dataTable.Rows[i].ItemArray[j].ToString()));
                    Trace.Write("|");
                }
                Trace.WriteLine("");
            }

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                Trace.WriteLine(String.Format(dataTable.Columns[i].ColumnName + " " + dataTable.Columns[i].DataType));
            }
        }
    }
}
