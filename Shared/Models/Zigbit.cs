using System;

namespace Shared.Models
{

    public class Zigbit
    {
        public string Serial { get; set; }
        public string Alias { get; set; }
        public double ACWatt { get; set; }
        public double DCWatt { get; set; }
        public double LifeProduction { get; set; }
        public double Wh { get; set; }
        public double DCCurrent { get; set; }
        public double DCVolt { get; set; }
        public double ACVolt { get; set; }
        public double ACFrequency { get; set; }
        public double InvertetTemp { get; set; }
        public double Efficiency { get; set; }
        public DateTime LastUpdated { get; set; }
        public string LastKey { get; set; }
        public bool Include { get; set; }

        public override string ToString()
        {
            return String.Format("Serial: {0}\n Alias: {1}\n ACWatt: {2}\n DCWatt: {3}\n LifeProduction: {4}\n Wh: {5}\n DCCurrent: {6}\n DCVolt: {7}\n ACVolt: {8}\n ACFrequency: {9}\n InverterTemp: {10}\n Efficiency: {11}\n LastUpdated: {12}\n LastKey: {13}\n", Serial, Alias, ACWatt, DCWatt, LifeProduction, Wh, DCCurrent, DCVolt, ACVolt, ACFrequency, InvertetTemp, Efficiency, LastUpdated, LastKey);
        }
    }
}