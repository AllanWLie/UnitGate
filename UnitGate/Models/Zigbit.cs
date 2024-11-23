using OfficeOpenXml.Attributes;
using OfficeOpenXml.Table;
using System;

namespace UnitGate.Models
{
  [EpplusTable(TableStyle = TableStyles.Light14, PrintHeaders = true, AutofitColumns = true, AutoCalculate = true, ShowLastColumn = true)]
  public class Zigbit
  {
    [EpplusTableColumn(Header = "Serial Number")]
    public string Serial { get; set; }
    [EpplusTableColumn(Header = "Alias")]
    public string Alias { get; set; }
    [EpplusTableColumn(Header = "AC Watt")]
    public double ACWatt { get; set; }
    [EpplusTableColumn(Header = "DC Watt")]
    public double DCWatt { get; set; }
    [EpplusTableColumn(Header = "Lifetime kWh")]
    public double LifeProduction { get; set; }
    [EpplusTableColumn(Header = "Today kWh")]
    public double Wh { get; set; }
    [EpplusTableColumn(Header = "DC Current")]
    public double DCCurrent { get; set; }
    [EpplusTableColumn(Header = "DC Volt")]
    public double DCVolt { get; set; }
    [EpplusTableColumn(Header = "AC Volt")]
    public double ACVolt { get; set; }
    [EpplusTableColumn(Header = "AC Frequency")]
    public double ACFrequency { get; set; }
    [EpplusTableColumn(Header = "Invertet Temp")]
    public double InvertetTemp { get; set; }
    [EpplusTableColumn(Header = "Efficiency")]
    public double Efficiency { get; set; }
    [EpplusTableColumn(Header = "LastUpdated", NumberFormat = "yyyy-MM-dd")]
    public DateTime LastUpdated { get; set; }
    [EpplusIgnore]
    public string LastKey { get; set; }

    public override string ToString()
    {
      return String.Format("Serial: {0}\n Alias: {1}\n ACWatt: {2}\n DCWatt: {3}\n LifeProduction: {4}\n Wh: {5}\n DCCurrent: {6}\n DCVolt: {7}\n ACVolt: {8}\n ACFrequency: {9}\n InverterTemp: {10}\n Efficiency: {11}\n LastUpdated: {12}\n LastKey: {13}\n", Serial, Alias, ACWatt, DCWatt, LifeProduction, Wh, DCCurrent, DCVolt, ACVolt, ACFrequency, InvertetTemp, Efficiency, LastUpdated, LastKey);
    }
  }
}