

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Shared.Models;

namespace Shared.Services
{
    public class ZigbitService
    {

        
        private int retryCount;
        public Zigbit CollectData(string zigbeeData)
        {
            try
            {
                zigbeeData = zigbeeData.Replace("\r", "");
                Zigbit zigbit = null;
                if (zigbeeData.StartsWith("WS") && zigbeeData.Length == 57)
                {
                    retryCount = 0;
                    zigbit = ProcessString(zigbeeData);
                }
                return zigbit;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Zigbit ProcessString(string zigbeeData, char letter = 'A')
        {
            //Inverter data
            string orgData = zigbeeData;
            //Remove WS= from string
            zigbeeData = zigbeeData.Remove(0, 3);
            //Convert base64 to decimal
            var zigbeeDec = FromBase64ToDec(letter + zigbeeData);
             
            //Convert decimal to hex
            var zigbeeHex = zigbeeDec.ToString("x4");
           
            if (zigbeeHex.Length == 82)
            {
                zigbeeHex = zigbeeHex.Remove(0, 1);
            }
            //Get inverter serial number
            var hexId = zigbeeHex.Substring(0, 8);
            var littlEndianUInt32 = Convert.ToUInt32("0x" + hexId, 16);
            var serial = ReverseBytes(littlEndianUInt32).ToString();
            //Get DC Watt
            var dcWatt = int.Parse(zigbeeHex.Substring(50, 4), NumberStyles.HexNumber);
            //Get DC Current in mA units
            var dcCurrent = int.Parse(zigbeeHex.Substring(46, 4), NumberStyles.HexNumber) * 0.025;
            //Get Efficiency in fraction
            var efficency = int.Parse(zigbeeHex.Substring(54, 4), NumberStyles.HexNumber) * 0.001;
            if (efficency > 1 && retryCount == 0)
            {
                retryCount++;
                ProcessString(orgData, 'B');
                return null;
            }
            if (efficency > 1 && retryCount == 1)
            {
                retryCount++;
                ProcessString(orgData, 'C');
                return null;
            }
            //Get AC Watt
            var acWatt = dcWatt * efficency;
            //Get AC frequency
            double acFrequency = int.Parse(zigbeeHex.Substring(58, 2), NumberStyles.HexNumber);
            //Get AC Volt
            double acVolt = int.Parse(zigbeeHex.Substring(60, 4), NumberStyles.HexNumber);
            //Get Temperature
            double temperature = int.Parse(zigbeeHex.Substring(64, 2), NumberStyles.HexNumber);
            //Get Today Wh
            double todayWh = int.Parse(zigbeeHex.Substring(66, 4), NumberStyles.HexNumber);
            //Get kWh
            double kWh = int.Parse(zigbeeHex.Substring(70, 4), NumberStyles.HexNumber);
            //Get LifekWh
            var lifekWh = (0.001 * todayWh) + kWh;
            //Get DCVolt
            var dcVolt = Math.Round((dcWatt / dcCurrent), 2);


            var zigbit = new Zigbit
            {
                Serial = serial,
                ACWatt = acWatt,
                DCWatt = dcWatt,
                LifeProduction = lifekWh,
                Wh = todayWh,
                DCCurrent = dcCurrent,
                DCVolt = dcVolt,
                ACVolt = acVolt,
                ACFrequency = acFrequency,
                Efficiency = efficency,
                InvertetTemp = temperature,
                LastUpdated = DateTime.Now,
                LastKey = orgData
            };
            return zigbit;
            // UpdateZigbit(zigbit);
        }

        private uint ReverseBytes(uint value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

        private BigInteger FromBase64ToDec(string s)
        {
            var alphabet = new Dictionary<char, int>
            {
                {'-', 62},
                {'1', 53},
                {'0', 52},
                {'3', 55},
                {'2', 54},
                {'5', 57},
                {'4', 56},
                {'7', 59},
                {'6', 58},
                {'9', 61},
                {'8', 60},
                {'A', 0},
                {'C', 2},
                {'B', 1},
                {'E', 4},
                {'D', 3},
                {'G', 6},
                {'F', 5},
                {'I', 8},
                {'H', 7},
                {'K', 10},
                {'J', 9},
                {'M', 12},
                {'L', 11},
                {'O', 14},
                {'N', 13},
                {'Q', 16},
                {'P', 15},
                {'S', 18},
                {'R', 17},
                {'U', 20},
                {'T', 19},
                {'W', 22},
                {'V', 21},
                {'Y', 24},
                {'X', 23},
                {'Z', 25},
                {'_', 63},
                {'a', 26},
                {'c', 28},
                {'b', 27},
                {'e', 30},
                {'d', 29},
                {'g', 32},
                {'f', 31},
                {'i', 34},
                {'h', 33},
                {'k', 36},
                {'j', 35},
                {'m', 38},
                {'l', 37},
                {'o', 40},
                {'n', 39},
                {'q', 42},
                {'p', 41},
                {'s', 44},
                {'r', 43},
                {'u', 46},
                {'t', 45},
                {'w', 48},
                {'v', 47},
                {'y', 50},
                {'x', 49},
                {'z', 51}
            };

            var bigInt = new BigInteger(0);
            for (var i = 0; i < s.Length; i++)
            {
                var c = s[i]; //chrAt(i);
                bigInt = BigInteger.Add(BigInteger.Multiply(bigInt, 64), alphabet[c]);
            }
            return bigInt;
        }
    }
}