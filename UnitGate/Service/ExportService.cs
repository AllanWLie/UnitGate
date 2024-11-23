/*
  Application Name:     Unitgate  
  Created by:           Allan Wamming Lie
  Created:              2014-2025
  Company:              Lie Consulting
  E-mail:               unitgate@allanlie.dk 
  Web:                  unitgate.eu
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using OfficeOpenXml;
using UnitGate.Models;


namespace UnitGate.Service
{
  internal static class ExportService
  {
    public static void ExportToExcel(List<Zigbit> zigbits)
    {
      try
      {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        
        var worksheet = package.Workbook.Worksheets.Add("Sheet1");

        // Load data into worksheet
        worksheet.Cells["A1"].LoadFromCollection(zigbits, true);

        SaveFileDialog save = new SaveFileDialog
        {
          Filter = "xlsx|*.xlsx"
        };

        bool? accepted = save.ShowDialog();
        if (accepted.HasValue && accepted.Value == true && !string.IsNullOrEmpty(save.FileName))
        {
          // Save to file
          FileInfo fileInfo = new FileInfo(save.FileName);
          package.SaveAs(fileInfo);

          MessageBox.Show(string.Format("The file '{0}' is saved successfully!", save.FileName));
        }

      }
      catch
      {
        MessageBox.Show("An error has occured. Please check if the document is open");
      }
    }

    public static void ExportRawData(List<Zigbit> zigbits)
    {
      try
      {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(String.Format("****Raw Data export from {0} Exported on {1} ****", Environment.MachineName, DateTime.UtcNow));
        foreach (Zigbit zigbit in zigbits)
        {
          sb.AppendLine("**** Inverter Data ****");
          sb.AppendLine("Serial: " + zigbit.Serial);
          sb.AppendLine("Alias: " + zigbit.Alias);
          sb.AppendLine("ACWatt: " + zigbit.ACWatt);
          sb.AppendLine("DCWatt: " + zigbit.DCWatt);
          sb.AppendLine("LifeProduction: " + zigbit.LifeProduction);
          sb.AppendLine("Wh: " + zigbit.Wh);
          sb.AppendLine("DcCurrent: " + zigbit.DCCurrent);
          sb.AppendLine("DcVolt: " + zigbit.DCVolt);
          sb.AppendLine("ACVolt: " + zigbit.ACVolt);
          sb.AppendLine("AcFrequency: " + zigbit.ACFrequency);
          sb.AppendLine("Efficency: " + zigbit.Efficiency);
          sb.AppendLine("LastUpdated: " + zigbit.LastUpdated);
          sb.AppendLine("LastKey: " + zigbit.LastKey);
          sb.AppendLine(zigbit.ToString());
          sb.AppendLine("**** End Inverter Data ****");
        }
        SaveFileDialog save = new SaveFileDialog();

        save.Filter = "txt|*.txt";

        bool? accepted = save.ShowDialog();

        if (accepted == true)
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