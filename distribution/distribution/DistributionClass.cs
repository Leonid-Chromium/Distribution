using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Diagnostics;

namespace distribution
{
    class DistributionClass
    {
        static DataTable inDataTable = new DataTable("In data table");
        static DataTable outDataTable = new DataTable("Out data table");

        static DataTable equipments = new DataTable("equipments data table");

        public static void FormingODT()
        {
            outDataTable.Columns.Add("EquipID", typeof(int));
            outDataTable.Columns.Add("EquipName", typeof(string));

            //Делаем столбцы для времени
            for (int a = 0; a < (4 * 12); a++)
            {
                outDataTable.Columns.Add(Convert.ToString(a - 1), typeof(int));
            }

            //Делаем колличество строк равные колличеству станков
            equipments = SQLClass.ReturnDT(@"
SELECT IdEquipment, Name FROM Equipments
ORDER BY IdEquipment ASC
");
            while (equipments.Rows.Count > outDataTable.Rows.Count)
            {
                outDataTable.Rows.Add();
                outDataTable.Rows[outDataTable.Rows.Count - 1].SetField(0, equipments.Rows[outDataTable.Rows.Count - 1].ItemArray[0]);
                outDataTable.Rows[outDataTable.Rows.Count - 1].SetField(1, equipments.Rows[outDataTable.Rows.Count - 1].ItemArray[1]);
            }
        }

        public static bool Fun2(int i, int j, int k)
        {
            for(int a = 0; a < k; a++) //true - чисто, false - занято
            {
                if(outDataTable.Rows[Convert.ToInt32(inDataTable.Rows[i].ItemArray[3])].ItemArray[j + a] == DBNull.Value) //j+a-1
                    return false;
            }
            return true;
        
        }

        public static void Fun()
        {
            int lastTime = 0;
            Trace.WriteLine(lastTime);
            for(int i = 0; i < inDataTable.Rows.Count; i++) // двигаемся по операциям
            {
                Trace.WriteLine("\ti: " + i);
                for(int j = (lastTime + 2); j < (outDataTable.Columns.Count - 2); j++) // двигаемся по времени
                {
                    Trace.WriteLine("\t\tj: " + j);

                    Trace.WriteLine("\t\t\t" + (outDataTable.Rows[Convert.ToInt32(inDataTable.Rows[i].ItemArray[3])].ItemArray[j] == DBNull.Value));
                    Trace.WriteLine("\t\t\t" + Fun2(Convert.ToInt32(inDataTable.Rows[i].ItemArray[3]), j, 1));
                    Trace.WriteLine("\t\t\t" + ((outDataTable.Rows[Convert.ToInt32(inDataTable.Rows[i].ItemArray[3])].ItemArray[j] == DBNull.Value) && Fun2(Convert.ToInt32(inDataTable.Rows[i].ItemArray[3]), j, 1)));

                    if (/*(outDataTable.Rows[Convert.ToInt32(inDataTable.Rows[i].ItemArray[3])].ItemArray[j] == DBNull.Value) && */Fun2(Convert.ToInt32(inDataTable.Rows[i].ItemArray[3]), j, 1))
                    {
                        Trace.WriteLine(Convert.ToInt32(inDataTable.Rows[i/*-1*/].ItemArray[3]));
                        Trace.WriteLine(j+1);

                        outDataTable.Rows[Convert.ToInt32(inDataTable.Rows[i/*-1*/].ItemArray[3])].SetField(j, inDataTable.Rows[i].ItemArray[1]); //  j + 2
                        Trace.WriteLine(Convert.ToInt32(inDataTable.Rows[i].ItemArray[3]) + "\t" + j + "\t" + inDataTable.Rows[i].ItemArray[1]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public static DataTable MainFun()
        {
            inDataTable = SQLClass.ReturnDT(@"
SELECT
Batchs.Priority as 'Приоритет партии',
Batchs.IdBatch as 'ID партии',
--Batchs.Name as 'Название партии',
Operations.Number as 'Номер операции',
--Operations.IdOperation as 'ID операции',
--Operations.IdMSL as 'ID МСЛ',
--Operations.Name as 'Название операций',
Equipments.IdEquipment as 'ID оборудование',
--Equipments.Name as 'Название оборудования',
Devices.IdDevice as 'ID девайса',
--Devices.KeyDevice as 'Код девайса',
Devices.Name as 'Название девайса'
FROM Operations

LEFT JOIN MSLs ON Operations.IdMSL = MSLs.IdMSL
LEFT JOIN Devices ON MSLs.IdDevice = Devices.IdDevice
--Возможно нужно RIGHT JOIN V
LEFT JOIN Batchs ON MSLs.IdMSL = Batchs.IdMSL

LEFT JOIN Routing ON Operations.IdRouting = Routing.IdRouting
--Наверняка можно упростить V
LEFT JOIN TechnologicalMaps ON Routing.IdTM = TechnologicalMaps.IdTM
LEFT JOIN EquipmentsTM ON TechnologicalMaps.IdTM = EquipmentsTM.IdTM
LEFT JOIN EquipmentsCertificates ON EquipmentsTM.IdEquipmentCertificate = EquipmentsCertificates.IdCertificate
LEFT JOIN Equipments ON EquipmentsCertificates.IdEquipment = Equipments.IdEquipment

ORDER BY Batchs.Priority DESC, Batchs.IdBatch ASC, Operations.Number ASC
");

            FormingODT();

            SQLClass.DTtoTrace(inDataTable);

            //SQLClass.DTtoTrace(outDataTable);

            Fun();
            SQLClass.DTtoTrace(outDataTable);

            return outDataTable;
        }
    }
}
