﻿// SPDX-License-Identifier: MIT
//
// Copyright (c) 2021-2022, Fewnity - Grégory Machefer
//
// This file is part of the server of Counter Strike Nintendo DS Multiplayer Edition (CS:DS)

//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Counter_Strike_Server
{
    static class MapManager
    {
        public static List<MapData> allMaps = new List<MapData>();

        /// <summary>
        /// Load maps data
        /// </summary>
        public static void LoadMapsData()
        {
            //I don't know how to use a json lib with Mono for Linux
            /*if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\CounterStrikeDsServer"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\CounterStrikeDsServer");

            string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\CounterStrikeDsServer\\mapsData.json";
            Console.WriteLine(SettingsPath);
            StreamReader Reader = new StreamReader(SettingsPath);
            allMaps = JsonConvert.DeserializeObject<List<MapData>>(Reader.ReadToEnd());
            Console.WriteLine(allMaps[(int)mapEnum.DUST2].mapId);
            Console.WriteLine(allMaps[(int)mapEnum.TUTORIAL].mapId);*/
            
            MapData newMap = new MapData();
            newMap.mapId = mapEnum.DUST2;

            newMap.allTerroristsSpawns.Add(new Vector3(-3, 6.43f, 65));
            newMap.allTerroristsSpawns.Add(new Vector3(-6, 6.43f, 65));
            newMap.allTerroristsSpawns.Add(new Vector3(-9, 6.43f, 65));
            newMap.allTerroristsSpawns.Add(new Vector3(-3, 6.43f, 62.5f));
            newMap.allTerroristsSpawns.Add(new Vector3(-6, 6.43f, 62.5f));
            newMap.allTerroristsSpawns.Add(new Vector3(-9, 6.43f, 62.5f));

            newMap.allCounterTerroristsSpawns.Add(new Vector3(16, 0, -22));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(19.5f, 0, -22));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(23, 0, -22));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(16, 0, -19.5f));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(19.5f, 0, -19.5f));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(23, 0, -19.5f));

            newMap.AllBombsTriggersCollisions.Add(SetBombZone(40.8f, -20.8f, 5, 5));
            newMap.AllBombsTriggersCollisions.Add(SetBombZone(-28.03f, -27.07f, 4.46785f, 4.578236f));

            newMap.terroristsSpawnsAngle = 0;
            newMap.counterTerroristsSpawnsAngle = 256;

            allMaps.Add(newMap);
            newMap = new MapData();

            newMap.mapId = mapEnum.TUTORIAL;

            newMap.allTerroristsSpawns.Add(new Vector3(0, 0, 0));
            newMap.allTerroristsSpawns.Add(new Vector3(0, 0, 0));
            newMap.allTerroristsSpawns.Add(new Vector3(0, 0, 0));
            newMap.allTerroristsSpawns.Add(new Vector3(0, 0, 0));
            newMap.allTerroristsSpawns.Add(new Vector3(0, 0, 0));

            newMap.allCounterTerroristsSpawns.Add(new Vector3(0, 0, 0));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(0, 0, 0));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(0, 0, 0));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(0, 0, 0));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(0, 0, 0));

            newMap.AllBombsTriggersCollisions.Add(SetBombZone(-8.21f, -1.59f, 4, 4));

            newMap.terroristsSpawnsAngle = 0;
            newMap.counterTerroristsSpawnsAngle = 0;

            allMaps.Add(newMap);
            newMap = new MapData();
            
            newMap.mapId = mapEnum.DUST2_2x2;

            newMap.allTerroristsSpawns.Add(new Vector3(-3, 6.43f, 65));
            newMap.allTerroristsSpawns.Add(new Vector3(-6, 6.43f, 65));
            newMap.allTerroristsSpawns.Add(new Vector3(-9, 6.43f, 65));
            newMap.allTerroristsSpawns.Add(new Vector3(-3, 6.43f, 62.5f));
            newMap.allTerroristsSpawns.Add(new Vector3(-6, 6.43f, 62.5f));
            newMap.allTerroristsSpawns.Add(new Vector3(-9, 6.43f, 62.5f));

            newMap.allCounterTerroristsSpawns.Add(new Vector3(16, 0, -22));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(19.5f, 0, -22));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(23, 0, -22));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(16, 0, -19.5f));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(19.5f, 0, -19.5f));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(23, 0, -19.5f));

            newMap.AllBombsTriggersCollisions.Add(SetBombZone(40.8f, -20.8f, 5, 5));

            newMap.terroristsSpawnsAngle = 0;
            newMap.counterTerroristsSpawnsAngle = 256;

            allMaps.Add(newMap);
            newMap = new MapData();

            newMap.mapId = mapEnum.AIM_MAP;

            newMap.allTerroristsSpawns.Add(new Vector3(4, 0, 16.2f));
            newMap.allTerroristsSpawns.Add(new Vector3(-2.5f, 0, 16.2f));
            newMap.allTerroristsSpawns.Add(new Vector3(12, 0, 16.2f));
            newMap.allTerroristsSpawns.Add(new Vector3(4, 0, 18.5f));
            newMap.allTerroristsSpawns.Add(new Vector3(-2.5f, 0, 18.5f));
            newMap.allTerroristsSpawns.Add(new Vector3(12, 0, 18.5f));

            newMap.allCounterTerroristsSpawns.Add(new Vector3(-3.7f, 0, -15.44f));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(-11.82f, 0, -15.44f));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(2.71f, 0, -15.44f));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(-3.7f, 0, -17.74f));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(-11.82f, 0, -17.74f));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(2.71f, 0, -17.74f));

            newMap.AllBombsTriggersCollisions.Add(SetBombZone(22.22f, 0.12f, 4, 4));

            newMap.terroristsSpawnsAngle = 0;
            newMap.counterTerroristsSpawnsAngle = 256;

            allMaps.Add(newMap);
            newMap = new MapData();

            newMap.mapId = mapEnum.B2000;

            newMap.allTerroristsSpawns.Add(new Vector3(0, 0, -14.3f));
            newMap.allTerroristsSpawns.Add(new Vector3(-7, 0, -14.3f));
            newMap.allTerroristsSpawns.Add(new Vector3(7, 0, -14.3f));
            newMap.allTerroristsSpawns.Add(new Vector3(0, 0, -17.3f));
            newMap.allTerroristsSpawns.Add(new Vector3(-7, 0, -17.3f));
            newMap.allTerroristsSpawns.Add(new Vector3(7, 0, -17.3f));

            newMap.allCounterTerroristsSpawns.Add(new Vector3(-0, 0, 14.3f));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(-7, 0, 14.3f));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(7, 0, 14.3f));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(0, 0, 17.3f));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(-7, 0, 17.3f));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(7, 0, 17.3f));

            newMap.AllBombsTriggersCollisions.Add(SetBombZone(-1.76f, -28.5f, 4, 4));

            newMap.terroristsSpawnsAngle = 256;
            newMap.counterTerroristsSpawnsAngle = 0;

            allMaps.Add(newMap);
            newMap = new MapData();

            newMap.mapId = mapEnum.MIRAGEA;

            newMap.allTerroristsSpawns.Add(new Vector3(13, 2.6f, 41));
            newMap.allTerroristsSpawns.Add(new Vector3(16, 2.6f, 41));
            newMap.allTerroristsSpawns.Add(new Vector3(10, 2.6f, 41));
            newMap.allTerroristsSpawns.Add(new Vector3(13, 2.6f, 44));
            newMap.allTerroristsSpawns.Add(new Vector3(16, 2.6f, 44));
            newMap.allTerroristsSpawns.Add(new Vector3(10, 2.6f, 44));

            newMap.allCounterTerroristsSpawns.Add(new Vector3(-28, 0.15f, -29));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(-25.5f, 0.15f, -29));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(-30.5f, 0.15f, -29));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(-28, 0.15f, -33));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(-25.5f, 0.15f, -33));
            newMap.allCounterTerroristsSpawns.Add(new Vector3(-30.5f, 0.15f, -33));

            newMap.AllBombsTriggersCollisions.Add(SetBombZone(-36.61f, -0.39f, 5, 6.6f));

            newMap.terroristsSpawnsAngle = 0;
            newMap.counterTerroristsSpawnsAngle = 256;

            allMaps.Add(newMap);
            newMap = new MapData();
        }

        /// <summary>
        /// Set bomb's zone position and size (2D)
        /// </summary>
        /// <param name="xPos">X position</param>
        /// <param name="zPos">Z position</param>
        /// <param name="xSize">X size</param>
        /// <param name="zSize">Z size</param>
        /// <returns></returns>
        static BoxCollisions SetBombZone(float xPos, float zPos, float xSize, float zSize)
        {
            BoxCollisions newBoxCollisions = new BoxCollisions();
            newBoxCollisions.corner1 = (xPos + xSize / 2.0f) * 4096;
            newBoxCollisions.corner2 = (xPos - xSize / 2.0f) * 4096;
            newBoxCollisions.corner3 = (zPos + zSize / 2.0f) * 4096;
            newBoxCollisions.corner4 = (zPos - zSize / 2.0f) * 4096;

            return newBoxCollisions;
        }

        /// <summary>
        /// Set bomb's defuse zone position and size (2D)
        /// </summary>
        /// <param name="party">Party</param>
        public static void SetBombDefuseZone(Party party)
        {
            BoxCollisions newBoxCollisions = new BoxCollisions();
            newBoxCollisions.corner1 = party.bombPosition.x + 4096;
            newBoxCollisions.corner2 = party.bombPosition.x - 4096;
            newBoxCollisions.corner3 = party.bombPosition.z + 4096;
            newBoxCollisions.corner4 = party.bombPosition.z - 4096;

            party.defuseZoneCollisions = newBoxCollisions;
        }

        /// <summary>
        /// Set bomb's drop zone position and size (2D)
        /// </summary>
        /// <param name="party">Party</param>
        public static void SetBombDropZone(Party party)
        {
            BoxCollisions newBoxCollisions = new BoxCollisions();
            newBoxCollisions.corner1 = party.bombPosition.x + 3277;
            newBoxCollisions.corner2 = party.bombPosition.x - 3277;
            newBoxCollisions.corner3 = party.bombPosition.z + 3277;
            newBoxCollisions.corner4 = party.bombPosition.z - 3277;

            party.defuseZoneCollisions = newBoxCollisions;
        }
    }
}
