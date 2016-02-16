using System;
using System.Collections.Generic;
using Shared.Models;

namespace Shared.DemoData
{
    public static class ZigbitDemoData
    {

        public static List<Zigbit> LoadDemoData()
        {
            List<Zigbit> zigbits = new List<Zigbit>();
            zigbits.Add(new Zigbit
            {
                Serial = "110006693",
                Alias = "Off",
                DCWatt = 128.00,
                ACWatt = 120.44,
                LifeProduction = 747.94,
                Wh = 550.00,
                InvertetTemp = 24,
                Efficiency = 94.10,
                LastUpdated = DateTime.UtcNow
            });
            zigbits.Add(new Zigbit
            {
                Serial = "110006823",
                Alias = "Off",
                DCWatt = 198.00,
                ACWatt = 187.50,
                LifeProduction = 740.93,
                Wh = 550.00,
                InvertetTemp = 25,
                Efficiency = 94.70,
                LastUpdated = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(1))
            });
            zigbits.Add(new Zigbit
            {
                Serial = "110007188",
                Alias = "Off",
                DCWatt = 205.00,
                ACWatt = 193.52,
                LifeProduction = 620.93,
                Wh = 550.00,
                InvertetTemp = 25,
                Efficiency = 94.40,
                LastUpdated = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(2))
            });
            zigbits.Add(new Zigbit
            {
                Serial = "110007566",
                Alias = "Off",
                DCWatt = 202.00,
                ACWatt = 191.49,
                LifeProduction = 737.12,
                Wh = 568.00,
                InvertetTemp = 24,
                Efficiency = 94.80,
                LastUpdated = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(3))
            });
            zigbits.Add(new Zigbit
            {
                Serial = "110007590",
                Alias = "Off",
                DCWatt = 180.00,
                ACWatt = 170.46,
                LifeProduction = 699.19,
                Wh = 572.00,
                InvertetTemp = 27,
                Efficiency = 94.70,
                LastUpdated = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(4))
            });
            zigbits.Add(new Zigbit
            {
                Serial = "110007719",
                Alias = "Off",
                DCWatt = 200.00,
                ACWatt = 189.40,
                LifeProduction = 727.87,
                Wh = 540.00,
                InvertetTemp = 23,
                Efficiency = 94.70,
                LastUpdated = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5))
            });
            zigbits.Add(new Zigbit
            {
                Serial = "110007904",
                Alias = "Off",
                DCWatt = 209.00,
                ACWatt = 197.50,
                LifeProduction = 737.60,
                Wh = 569.00,
                InvertetTemp = 26,
                Efficiency = 94.50,
                LastUpdated = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(6))
            });
            zigbits.Add(new Zigbit
            {
                Serial = "110008125",
                Alias = "Off",
                DCWatt = 191.00,
                ACWatt = 180.49,
                LifeProduction = 737.31,
                Wh = 570.00,
                InvertetTemp = 24,
                Efficiency = 94.50,
                LastUpdated = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(7))
            });

            return zigbits;
        }



    }
}
