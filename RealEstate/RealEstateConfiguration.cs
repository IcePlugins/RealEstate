using ExtraConcentratedJuice.RealEstate.Entities;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraConcentratedJuice.RealEstate
{
    public class RealEstateConfiguration : IRocketPluginConfiguration
    {
        public string currencySymbol;
        public int defaultMaxHomes;
        public int paymentCheckIntervalInSeconds;
        public int feePaymentTimeInMinutes;
        public bool destroyStructuresOnEviction;
        public bool disableBuildingIfNotInHome;
        public List<House> homes;
        public List<DisplayName> displayNames;

        public void LoadDefaults()
        {
            currencySymbol = "$";
            defaultMaxHomes = 1;
            paymentCheckIntervalInSeconds = 10;
            feePaymentTimeInMinutes = 1440;
            destroyStructuresOnEviction = false;
            disableBuildingIfNotInHome = false;

            homes = new List<House>();
            displayNames = new List<DisplayName>();
        }
    }
}
