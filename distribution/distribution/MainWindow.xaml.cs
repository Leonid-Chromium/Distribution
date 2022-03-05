using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using DistributionLib;
using SQLLib;

namespace distribution
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			UpdateFunA1();
			TestFun();
			//testNullDateTimeFromDB();
		}

		public void UpdateFunD1()
        {
            InDG.ItemsSource = SQLClass.ReturnDT(@"
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
").DefaultView;
            OutDG.ItemsSource = Discret1.MainFun(SQLClass.ReturnDT("SELECT IdEquipment, Name FROM Equipments ORDER BY IdEquipment ASC"), SQLClass.ReturnDT(@"SELECT Batchs.Priority as 'Приоритет партии', Batchs.IdBatch as 'ID партии', Operations.Number as 'Номер операции', Equipments.IdEquipment as 'ID оборудование', Devices.IdDevice as 'ID девайса', Devices.Name as 'Название девайса' FROM Operations LEFT JOIN MSLs ON Operations.IdMSL = MSLs.IdMSL LEFT JOIN Devices ON MSLs.IdDevice = Devices.IdDevice LEFT JOIN Batchs ON MSLs.IdMSL = Batchs.IdMSL LEFT JOIN Routing ON Operations.IdRouting = Routing.IdRouting LEFT JOIN TechnologicalMaps ON Routing.IdTM = TechnologicalMaps.IdTM LEFT JOIN EquipmentsTM ON TechnologicalMaps.IdTM = EquipmentsTM.IdTM LEFT JOIN EquipmentsCertificates ON EquipmentsTM.IdEquipmentCertificate = EquipmentsCertificates.IdCertificate LEFT JOIN Equipments ON EquipmentsCertificates.IdEquipment = Equipments.IdEquipment ORDER BY Batchs.Priority DESC, Batchs.IdBatch ASC, Operations.Number ASC")).DefaultView;
        }

		public void UpdateFunA1()
		{
			string ex;

			DataTable InDT = new DataTable("Input data table");
			InDT = SQL.ReturnDT(@"
SELECT

Batchs.IdBatch AS 'ID партии'
,Equipments.IdEquipment AS 'ID оборудование'
,Equipments.Name AS 'Название оборудования'
,Operations.Number AS 'Порядковый номер операции'
,Operations.IdOperation AS 'ID операции'
,Operations.Name AS 'Название операции'
,LastOperation = CASE WHEN (LastOperations.maxNum IS NULL) THEN 0 ELSE LastOperations.maxNum END
,LastCount = CASE WHEN LastOperations.LastCount IS NULL THEN Batchs.StartCount ELSE LastOperations.LastCount END
,TRIM(Routing.TimeFormula) AS 'Формула расчёта необходимого времени'
--,LastOperations.StartDateTime
--,LastOperations.EndDateTime
,MyStartDateTime = CASE WHEN LastOperations.IdOperation = Operations.IdOperation THEN LastOperations.StartDateTime ELSE NULL END
,MyEndDateTime = CASE WHEN LastOperations.IdOperation = Operations.IdOperation THEN LastOperations.EndDateTime ELSE NULL END


/********************************************************/
--Нужно добавить информацию из ТК,КК

--,Operations.IdRouting
,Operations.InteroperativeTime AS 'Межоперационное время'
,Operations.TypeOfProcessing AS 'Тип обработки'
,Operations.ScanIn AS 'Сканирование на вход'
,Operations.ScanOut AS 'Сканирование на выход'
,MSLs.MSLKey AS 'Ключ МСЛ'
,Devices.IdDevice AS 'Id прибора'
,Devices.Name AS 'Название прибора'
,Batchs.Name AS 'Название партии'
,Batchs.StartCount AS 'Количество пластин на старте'
,Batchs.Priority AS 'Приоритет партии'
,Batchs.Note AS 'Примечание к партии'
,TechnologicalMaps.IdTM AS 'Id технологической карты'
,TechnologicalMaps.NameTM AS 'Название технологической карты'
,Equipments.Capacity AS 'Вместимость оборудования'

FROM
Operations
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

--Получение столбца для выборки последней сделанной операции на партии
LEFT JOIN (
	SELECT
	OperationHistory.IdBatch
	,OperationHistory.IdRecording
	,OperationHistory.IdOperation
	,OperationHistory.LastCount
	,OperationHistory.StartDateTime
	,OperationHistory.EndDateTime
	,Operations.Number
	,maxNum = CASE WHEN (LastOperations.maxNum IS NULL) THEN 1 ELSE LastOperations.maxNum END
	FROM OperationHistory
	LEFT JOIN Operations ON OperationHistory.IdOperation = Operations.IdOperation
	LEFT JOIN (
		SELECT
		OperationHistory.IdBatch
		,OperationHistory.IdRecording
		,OperationHistory.IdOperation
		,OperationHistory.LastCount
		,Operations.Number
		,maxNum = CASE WHEN (LastOperations.maxNum IS NULL) THEN 1 ELSE LastOperations.maxNum END
		FROM OperationHistory
		LEFT JOIN Operations ON OperationHistory.IdOperation = Operations.IdOperation
		LEFT JOIN (
			SELECT
				maxNum = CASE
					WHEN MAX(Operations.Number) = null
					THEN 0
					ELSE MAX(Operations.Number)
					end
				,OperationHistory.IdBatch
			FROM (
				SELECT
				*
				FROM
				OperationHistory
				WHERE (OperationHistory.EndDateTime IS NOT NULL) OR (StartDateTime IS NOT NULL)) AS OperationHistory
			LEFT JOIN Operations on Operations.IdOperation = OperationHistory.IdOperation
			Group by IdBatch
		) AS LastOperations ON LastOperations.IdBatch = OperationHistory.IdBatch AND LastOperations.maxNum = Operations.Number

		--ограничение на последние операции
		WHERE Operations.Number >= maxNum

		--ORDER BY OperationHistory.IdBatch
	) AS LastOperations ON (LastOperations.IdBatch = OperationHistory.IdBatch AND LastOperations.maxNum = Operations.Number)

	--ограничение на последние операции
	WHERE
	Operations.Number >= maxNum)
AS LastOperations ON Batchs.IdBatch = LastOperations.IdBatch

WHERE Operations.Number >= CASE WHEN (LastOperations.maxNum IS NULL) THEN 1 ELSE LastOperations.maxNum END

ORDER BY Batchs.Priority DESC, Batchs.IdBatch ASC, Operations.Number ASC
", @"Data Source=DESKTOP-LEONID\SQLEXPRESS;Initial Catalog=DispatcherV3;Persist Security Info=True;User ID=Admin;Password=Admin", out ex);
			if (!String.IsNullOrEmpty(ex))
				MessageBox.Show(ex);
			InDG.ItemsSource = InDT.DefaultView;
			SQLClass.DTtoTrace(InDT);

			Dictionary<string, decimal> variableDictionary = new Dictionary<string, decimal> ()
			{
				{"x", 1 },
				{"y", 2 },
				{"z", 3 }
			};

			Dictionary<String, int> colDictionary = new Dictionary<string, int>()
			{
				{"IdBatch", 0 },
				{"IdEquipment", 1 },
				{"EquipmentsName", 2 },
				{"LastCount", 7 },
				{"TimeFormula", 8 },
				{"StartDateTime", 9 },
				{"EndDateTime", 10 }
			};

			//Время до которого будет вестись расчёт
			DateTime maxDateTime = new DateTime();
            maxDateTime = DateTime.Now;
            maxDateTime = maxDateTime.AddDays(3);
			Analog1 analog1 = new Analog1();
            DataTable OutDT = analog1.MainFun(InDT, variableDictionary, colDictionary, 15, maxDateTime, out ex);
            OutDG.ItemsSource = OutDT.DefaultView;
			analog1.TraceExceptionList();
		}

		public void testNullDateTimeFromDB()
		{
			string ex;
			DataTable dataTable = SQL.ReturnDT(@"SELECT StartDateTime FROM OperationHistory WHERE IdRecording = 1", @"Data Source=DESKTOP-LEONID\SQLEXPRESS;Initial Catalog=DispatcherV3;Persist Security Info=True;User ID=Admin;Password=Admin", out ex);
			Trace.WriteLine(dataTable.Columns[0].DataType);
			string str;
			str = dataTable.Rows[0].ItemArray[0].ToString();
			Trace.WriteLine("str = " + str);
			DateTime dateTime = new DateTime();
			if (!String.IsNullOrEmpty(str))
			{
				dateTime = Convert.ToDateTime(str);
			}
			Trace.WriteLine(dateTime.ToString());
			Trace.WriteLine(default(DateTime));
			Nullable<DateTime> dateTime1 = Convert.ToDateTime(dataTable.Rows[0].ItemArray[0].ToString());
			Trace.WriteLine(dateTime1.ToString());
		}
		public void TestFun()
		{
			bool a = (false && true);
		}
    }
}
