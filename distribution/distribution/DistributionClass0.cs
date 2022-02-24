using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace distribution
{
    class DistributionClass0
    {
        public void Fun0()
        {
            DataTable locationTable = new DataTable("Location");
            // Add two columns
            locationTable.Columns.Add("State");
            locationTable.Columns.Add("ZipCode");

            // Add data
            locationTable.Rows.Add("Washington", "98052");
            locationTable.Rows.Add("California", "90001");
            locationTable.Rows.Add("Hawaii", "96807");
            locationTable.Rows.Add("Hawaii", "96801");
            locationTable.AcceptChanges();

            Trace.WriteLine("Rows in original order\n State \t\t ZipCode");
            foreach (DataRow row in locationTable.Rows)
            {
                Trace.WriteLine(" " + row["State"] + " \t " + row["ZipCode"]);
            }

            // Create DataView
            DataView view = new DataView(locationTable);

            // Sort by State and ZipCode column in descending order
            view.Sort = "State ASC, ZipCode ASC";

            Trace.WriteLine("\nRows in sorted order\n State \t\t ZipCode");
            foreach (DataRowView row in view)
            {
                Trace.WriteLine(" " + row["State"] + " \t " + row["ZipCode"]);
            }
        }

        public void DTtoTrace(DataTable dataTable)
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

        DataTable equipments = new DataTable("Equipments DataTable");
        public void FillEquipments()
        {
            equipments.Columns.Add("Id", typeof(int));
            equipments.Columns.Add("Name", typeof(string));
            equipments.Rows.Add(1, "Equip1");
            equipments.Rows.Add(2, "Equip2");
            equipments.Rows.Add(3, "Equip3");
            equipments.Rows.Add(4, "Equip4");
            equipments.Rows.Add(5, "Equip5");
            equipments.Rows.Add(6, "Equip6");
            equipments.Rows.Add(7, "Equip7");
        }

        DataTable device = new DataTable("Devices DataTable");
        public void FillDevice()
        {
            device.Columns.Add("Id", typeof(int));
            device.Columns.Add("Name", typeof(string));
            device.Columns.Add("Priority", typeof(int));
            device.Rows.Add(1, "Device1", 100);
            device.Rows.Add(2, "Device1", 90);
            device.Rows.Add(3, "Device1", 80);
            device.Rows.Add(4, "Device1", 70);
            device.Rows.Add(5, "Device1", 60);
            device.Rows.Add(6, "Device1", 50);
            device.Rows.Add(7, "Device1", 40);
            device.Rows.Add(8, "Device1", 30);
            device.Rows.Add(9, "Device1", 20);
        }

        DataTable operation = new DataTable("Operations DataTable");
        public void FillOperation()
        {
            operation.Columns.Add("IdDevice", typeof(int));
            operation.Columns.Add("NOperation", typeof(int));
            operation.Columns.Add("IdEquipment", typeof(int));
            operation.Columns.Add("Time", typeof(int));
            operation.Rows.Add(0, 0, 0, 10);
            operation.Rows.Add(0, 1, 1, 5);
            operation.Rows.Add(0, 2, 2, 6);
            operation.Rows.Add(0, 3, 3, 8);
            operation.Rows.Add(1, 0, 0, 9);
            operation.Rows.Add(1, 1, 1, 2);
            operation.Rows.Add(2, 0, 0, 3);
            operation.Rows.Add(2, 1, 1, 4);
            operation.Rows.Add(2, 2, 2, 6);
            operation.Rows.Add(3, 0, 0, 9);
            operation.Rows.Add(3, 1, 1, 7);
            operation.Rows.Add(3, 2, 2, 2);
            operation.Rows.Add(4, 0, 0, 3);
        }

        public void Sort()
        {
            //подготовка к сортировкам
            DataView dataView = new DataView();

            //Сортировка девайсов по приоритету
            dataView.Table = device;
            dataView.Sort = "Priority DESC";
            device = dataView.ToTable();
            DTtoTrace(device);

            //Сортировка операций по IdDevice
            dataView.Table = operation;
            dataView.Sort = "IdDevice ASC";
            operation = dataView.ToTable();
            DTtoTrace(operation);
        }

        DataTable outDataTable = new DataTable("Out DataTable");
        public void FillOutDataTable()
        {
            outDataTable.Columns.Add("Equipment", typeof(string));

            //Делаем столбцы для времени
            for (int a = 0; a < (4 * 12); a++)
            {
                outDataTable.Columns.Add(Convert.ToString(a - 1), typeof(string));
            }

            //Делаем колличество строк равные колличеству станков
            while (equipments.Rows.Count > outDataTable.Rows.Count)
            {
                outDataTable.Rows.Add();
                outDataTable.Rows[outDataTable.Rows.Count - 1].SetField(0, equipments.Rows[outDataTable.Rows.Count - 1].ItemArray[1]);
            }
        }

        public void Fun1()
        {
            //Можно наверняка читать подряд операции
            for (int d = 0; d < device.Rows.Count; d++)
            {
                int lastTime = 0;
                for (int o = 0; o < operation.Rows.Count; o++)
                {
                    Trace.WriteLine("--------------------");
                    Trace.WriteLine(d + " " + o);
                    Trace.WriteLine(device.Rows[d].ItemArray[0] + " " + operation.Rows[o].ItemArray[0]);

                    if (Convert.ToInt32(device.Rows[d].ItemArray[0]) == Convert.ToInt32(operation.Rows[o].ItemArray[0])) // находим операцию которая используется для данного девайса
                    {
                        Trace.WriteLine("Успех" + operation.Rows[o].ItemArray[2]);
                        //operation.Rows[o].ItemArray[2]; //нужное оборудования

                        for (int e = 0; e < equipments.Rows.Count; e++) //движемся по оборудованию
                        {
                            if (Convert.ToInt32(operation.Rows[o].ItemArray[2]) == Convert.ToInt32(equipments.Rows[e].ItemArray[0])) // находим оборудование подходящее по операциям
                            {
                                /*
                                for(int i = 0; i < outDataTable.Rows.Count; i++) //двигаемся по оборудованию
                                {
                                    if(Convert.ToInt32() == Convert.ToInt32()) //находим нужное оборудование среди выходной таблицы
                                }
                                */

                                Trace.WriteLine(outDataTable.Rows[e].ItemArray[0]);
                            }
                        }
                    }
                }
            }
        }

        public void Fun2()
        {
            DataView dataView = new DataView();

            //Сортировка операций по оборудованиям
            dataView.Table = operation;
            dataView.Sort = "IdEquipment ASC";
            operation = dataView.ToTable();
            DTtoTrace(operation);

        }

        public int distributionF()
        {
            FillEquipments();
            FillDevice();
            FillOperation();
            Sort();
            FillOutDataTable();

            Fun2();

            DTtoTrace(outDataTable);

            return 0;
        }
    }
}
