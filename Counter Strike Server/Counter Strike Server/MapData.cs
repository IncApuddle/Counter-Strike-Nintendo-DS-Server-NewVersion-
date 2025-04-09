// SPDX-License-Identifier: MIT
//
// Copyright (c) 2021-2022, Fewnity - Grégory Machefer
//
// This file is part of the server of Counter Strike Nintendo DS Multiplayer Edition (CS:DS)

using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

namespace Counter_Strike_Server
{
    public enum mapEnum
    {
        DUST2 = 0,
        TUTORIAL = 1,
        DUST2_2x2 = 2,
        AIM_MAP = 3,
        B2000 = 4
    };


    public class Globals
    {   

        public static int selectedMap = 4; // The map that's going to load on server
        public static bool mapSwitch = true; //Turn on and off the map switch
        public static int MapPointer = 0; // Next map pointer
        public static bool PointerSwitch = true; // Gate for map switch

        //UserInterface Var
        public static bool slotSet = false;
        public static int slotNumber = -2;
        public static int mapNumber = -2;
        public static int numbersOfMaps = Enum.GetNames(typeof(mapEnum)).Length - 1;
        public static int timeToSet = -1;

        public static int MapMinuts = 50; // Time of a map

        public static List<int> MapsToGo = new List<int>{0,2,3,4};

        public static void ChengeMap()
        {
            //Disconnect Users
            ConnectionManager.KickAll();

            if(MapPointer >= MapsToGo.Count)
            {
                MapPointer = 0;
            }

            // Switching Maps
            if(PointerSwitch)
            {
                PointerSwitch = false;
                selectedMap = MapsToGo[MapPointer];
                MapPointer++;
            }

            // Set time for map
            PartyManager.mapTime = new(2000, 1, 1, 0, MapMinuts, 0);
            PointerSwitch = true;
        }
    }
    
    public class MapData
    {
        public mapEnum mapId;
        public int terroristsSpawnsAngle;
        public int counterTerroristsSpawnsAngle;
        public List<Vector3> allTerroristsSpawns = new List<Vector3>();
        public List<Vector3> allCounterTerroristsSpawns = new List<Vector3>();
        public List<BoxCollisions> AllBombsTriggersCollisions = new List<BoxCollisions>();
    }
}
