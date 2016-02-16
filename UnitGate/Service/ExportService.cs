/*
  Application Name:     Unitgate  
  Created by:           Allan Wamming Lie
  Created:              2014-2016
  Company:              Lie Development
  E-mail:               unitgate@allanlie.dk 
  Web:                  unitgate.eu
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Shared.Models;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace UnitGate.Service
{
    internal static class ExportService
    {
        public static void ExportToExcel(DataGrid dgDisplay)
        {
            dgDisplay.SelectAllCells();
            dgDisplay.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dgDisplay);
            String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
            String result = (string)Clipboard.GetData(DataFormats.Text);
            dgDisplay.UnselectAllCells();
            SaveFileDialog save = new SaveFileDialog();

            save.Filter = "xls|*.xls";

            save.ShowDialog();
            if (!string.IsNullOrEmpty(save.FileName))
            {
                using (StreamWriter writer = new StreamWriter(save.FileName))
                {
                    writer.WriteLine(result);
                }
            }

            MessageBox.Show(" Exporting DataGrid data to Excel file created");
        }

        public static void ExportToExcelWithInterOp(List<Zigbit> zigbits)
        {
            Application excel = new Application();

            excel.Workbooks.Add();
            _Worksheet workSheet = excel.ActiveSheet;
            try
            {
                workSheet.Cells[1, "A"] = "Serial Number";
                workSheet.Cells[1, "B"] = "Alias";
                workSheet.Cells[1, "C"] = "AC Watt";
                workSheet.Cells[1, "D"] = "DC Watt";
                workSheet.Cells[1, "E"] = "Lifetime kWh";
                workSheet.Cells[1, "F"] = "DC Current";
                workSheet.Cells[1, "G"] = "DC Volt";
                workSheet.Cells[1, "H"] = "Inverter Temp";
                workSheet.Cells[1, "I"] = "Efficiency";
                workSheet.Cells[1, "J"] = "Last updated";

                //Populate from input
                int row = 2; //1 is for header
                foreach (Zigbit zig in zigbits)
                {
                    workSheet.Cells[row, "A"] = zig.Serial;
                    workSheet.Cells[row, "B"] = zig.Alias;
                    workSheet.Cells[row, "C"] = zig.ACWatt.ToString();
                    workSheet.Cells[row, "D"] = zig.DCWatt.ToString();
                    workSheet.Cells[row, "E"] = zig.LifeProduction.ToString();
                    workSheet.Cells[row, "F"] = zig.DCCurrent.ToString();
                    workSheet.Cells[row, "G"] = zig.DCVolt.ToString();
                    workSheet.Cells[row, "H"] = zig.InvertetTemp.ToString();
                    workSheet.Cells[row, "I"] = zig.Efficiency.ToString();
                    workSheet.Cells[row, "J"] = zig.LastUpdated.ToString("g");
                    row++;
                }
                workSheet.Cells[row, "A"] = "Sum";
                workSheet.Cells[row, "C"] = string.Format("=sum(C2:C{0})", row - 1);
                workSheet.Cells[row, "D"] = string.Format("=sum(D2:D{0})", row - 1);
                workSheet.Cells[row, "E"] = string.Format("=sum(E2:E{0})", row - 1);
                workSheet.Cells[row, "J"] = string.Format("Inverters: {0}", zigbits.Count);

                
                SaveFileDialog save = new SaveFileDialog();

                save.Filter = "xlsx|*.xlsx";

                bool? accepted = save.ShowDialog();
                if(accepted == true)
                {
                    string fileName = save.FileName;
                    workSheet.SaveAs(fileName);
                    MessageBox.Show(string.Format("The file '{0}' is saved successfully!", fileName));
                }
   
            }
            catch
            {
                MessageBox.Show("An error has occured. Please check if the document is open");
            }
            finally
            {
                // Quit Excel application
                excel.Quit();

                // Release COM objects (very important!)
                if (excel != null)
                    Marshal.ReleaseComObject(excel);

                if (workSheet != null)
                    Marshal.ReleaseComObject(workSheet);

                // Empty variables
                excel = null;
                workSheet = null;

                // Force garbage collector cleaning
                GC.Collect();
            }

        }

        public static void ExportRawData(List<Zigbit> zigbits)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(String.Format("****Raw Data export from {0} Exported on {1} ****", Environment.MachineName, DateTime.UtcNow));
                foreach(Zigbit zigbit in zigbits)
                {
                    sb.AppendLine("**** Inverter Data ****");
                    //sb.AppendLine("Serial: " + zigbit.Serial);
                    //sb.AppendLine("Alias: "+ zigbit.Alias);
                    //sb.AppendLine("ACWatt: " + zigbit.ACWatt);
                    //sb.AppendLine("DCWatt: " + zigbit.DCWatt);
                    //sb.AppendLine("LifeProduction: " + zigbit.LifeProduction);
                    //sb.AppendLine("Wh: " + zigbit.Wh);
                    //sb.AppendLine("DcCurrent: " + zigbit.DCCurrent);
                    //sb.AppendLine("DcVolt: " + zigbit.DCVolt);
                    //sb.AppendLine("ACVolt: " + zigbit.ACVolt);
                    //sb.AppendLine("AcFrequency: " + zigbit.ACFrequency);
                    //sb.AppendLine("Efficency: " + zigbit.Efficiency);
                    //sb.AppendLine("LastUpdated: " + zigbit.LastUpdated);
                    //sb.AppendLine("LastKey: " + zigbit.LastKey);
                    sb.AppendLine(zigbit.ToString());
                    sb.AppendLine("**** End Inverter Data ****");
                }
                SaveFileDialog save = new SaveFileDialog();

                save.Filter = "txt|*.txt";

                bool? accepted = save.ShowDialog();

                if(accepted == true)
                {
                    string fileName = save.FileName;
                    File.WriteAllText(fileName, sb.ToString());
                    MessageBox.Show(string.Format("The file '{0}' is saved successfully!", fileName));
                }              
            }
            catch
            {
                MessageBox.Show("Something went wrong, please close the program and try again");
            }
        }

        
    }
}