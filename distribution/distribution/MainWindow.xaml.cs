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
            //distributionF();
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
            OutDG.ItemsSource = Discret1.DurationMainFun(SQLClass.ReturnDT("SELECT IdEquipment, Name FROM Equipments ORDER BY IdEquipment ASC"), SQLClass.ReturnDT(@"SELECT Batchs.Priority as 'Приоритет партии', Batchs.IdBatch as 'ID партии', Operations.Number as 'Номер операции', Equipments.IdEquipment as 'ID оборудование', Devices.IdDevice as 'ID девайса', Devices.Name as 'Название девайса' FROM Operations LEFT JOIN MSLs ON Operations.IdMSL = MSLs.IdMSL LEFT JOIN Devices ON MSLs.IdDevice = Devices.IdDevice LEFT JOIN Batchs ON MSLs.IdMSL = Batchs.IdMSL LEFT JOIN Routing ON Operations.IdRouting = Routing.IdRouting LEFT JOIN TechnologicalMaps ON Routing.IdTM = TechnologicalMaps.IdTM LEFT JOIN EquipmentsTM ON TechnologicalMaps.IdTM = EquipmentsTM.IdTM LEFT JOIN EquipmentsCertificates ON EquipmentsTM.IdEquipmentCertificate = EquipmentsCertificates.IdCertificate LEFT JOIN Equipments ON EquipmentsCertificates.IdEquipment = Equipments.IdEquipment ORDER BY Batchs.Priority DESC, Batchs.IdBatch ASC, Operations.Number ASC")).DefaultView;
        }
    }
}
