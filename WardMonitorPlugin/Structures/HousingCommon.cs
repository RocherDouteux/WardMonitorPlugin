using System;
using System.IO;

namespace WardMonitorPlugin.Structures {
    public class LandIdent {
        public short LandId;
        public short TerritoryTypeId;
        public short WardNumber;
        public short WorldId;

        public static LandIdent ReadFromBinaryReader(BinaryReader binaryReader) {
            var landIdent = new LandIdent
            {
                LandId = binaryReader.ReadInt16(),
                WardNumber = binaryReader.ReadInt16(),
                TerritoryTypeId = binaryReader.ReadInt16(),
                WorldId = binaryReader.ReadInt16()
            };
            return landIdent;
        }
    }

    public class HouseInfoEntry {
        public string EstateOwnerName;
        public sbyte[] HouseAppeals;
        public uint HousePrice;
        public HousingFlags InfoFlags;
    }

    [Flags]
    public enum HousingFlags : byte {
        PlotOwned = 1 << 0,
        VisitorsAllowed = 1 << 1,
        HasSearchComment = 1 << 2,
        HouseBuilt = 1 << 3,
        OwnedByFC = 1 << 4
    }

    public enum PurchaseType : byte {
        Unavailable = 0,
        FCFS = 1,
        Lottery = 2
    }
    
    public enum TenantType : byte {
        FreeCompany = 1,
        Personal = 2
    }
}
