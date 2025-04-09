// SPDX-License-Identifier: MIT
//
// Copyright (c) 2021-2022, Fewnity - Grégory Machefer
//
// This file is part of the server of Counter Strike Nintendo DS Multiplayer Edition (CS:DS)

using System;

namespace Counter_Strike_Server
{
    static class UserInterfaceManager
    {
        /// <summary>
        /// Read for input in the console
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static void ReadConsole()
        {
            //Split the user input
            string[] userInput = Console.ReadLine().ToLower().Split(" ");

            Console.ForegroundColor = ConsoleColor.White;
            if(Globals.slotSet == false)
            {
                if (userInput[0] == "help") //Info about commands on server
                {
                    PrintCommandsList();
                }

                else if (userInput[0] == "stop") //Stop the server
                {
                    Environment.Exit(0);
                }

                else if (userInput[0] == "chm") //Change map on server
                {
                    if(userInput[1] == "0")
                    {
                        Globals.ChengeMap();
                        Globals.selectedMap = 0;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Map set to : "+ Globals.selectedMap +"\r\n");
                    }
                    else if(userInput[1] == "1")
                    {
                        Globals.ChengeMap();
                        Globals.selectedMap = 1;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Map set to : "+ Globals.selectedMap +"\r\n");
                    }
                    else if(userInput[1] == "2")
                    {
                        Globals.ChengeMap();
                        Globals.selectedMap = 2;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Map set to : "+ Globals.selectedMap +"\r\n");
                    }
                    else if(userInput[1] == "3")
                    {
                        Globals.ChengeMap();
                        Globals.selectedMap = 3;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Map set to : "+ Globals.selectedMap +"\r\n");
                    }
                    else if(userInput[1] == "4")
                    {
                        Globals.ChengeMap();
                        Globals.selectedMap = 4;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Map set to : "+ Globals.selectedMap +"\r\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Wrong map number\r\n");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

                else if (userInput[0] == "maplist") //Show the queue of maps
                {
                    Console.WriteLine("");
                    for(int iCount = 0; iCount < Globals.MapsToGo.Count; iCount++)
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write("Map list: ");

                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("Slot Number (");

                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(iCount);

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(") Map Number (");

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(Globals.MapsToGo[iCount]);

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine(")");

                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Maps corresponding numbers :\r\n(");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("0");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(") - Dust2, \r\n(");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("1");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(") - Tutorial, \r\n(");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("2");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(") - Dust2_2x2, \r\n(");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("3");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(") - Aim_Map\r\n(");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("4");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(") - $2000$\r\n");
                }

                else if (userInput[0] == "mapslot") //Adjust the queue of maps
                {
                    if(userInput[1] == "add")
                    {
                        Globals.MapsToGo.Add(0);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\r\nNew map slot was added");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if(userInput[1] == "remove")
                    {
                        if(Globals.MapsToGo.Count > 2)
                        {
                            Globals.MapsToGo.RemoveAt(Globals.MapsToGo.Count-1);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\r\nLast map slot was removed");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("\r\nCan't removed last two slots");
                        }
                    }
                    else if(userInput[1] == "set")
                    {
                        Globals.slotSet = true;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("\r\nChoose ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("slot :");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Wrong command argument");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

                else if (userInput[0] == "mapswitch") //Turn on and off map switching
                {
                    if(userInput[1] == "on")
                    {
                        Globals.mapSwitch = true;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("\r\nMap switching is ON");
                    }

                    else if(userInput[1] == "off")
                    {
                        Globals.mapSwitch = false;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("\r\nMap switching is OFF");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\r\nWrong command argument");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

                else if (userInput[0] == "settime") //Set map switch time
                {
                    if(Globals.timeToSet == -1)
                    {
                        while (Globals.timeToSet <= 0 || Globals.timeToSet > 59)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine("\r\nType number from 1 to 59 to set time for map");
                            Console.ForegroundColor = ConsoleColor.White;
                            var input = Console.ReadLine();
                            int.TryParse(input, out Globals.timeToSet);
                        }
                    }
                    if(Globals.timeToSet > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("\r\nTime was set to : "+ Globals.timeToSet);
                        Console.ForegroundColor = ConsoleColor.White;
                        PartyManager.mapTime = new(2000, 1, 1, 0, Globals.timeToSet, 0);
                        Globals.timeToSet = -1;
                    }
                }

                else if (userInput[0] == "maptime") //Check map time
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("\r\n"+PartyManager.mapTime);
                }


                else if (userInput[0] == "status") //Change the server status
                {
                    if (userInput[1] == "online")
                    {
                        Settings.serverStatus = ServerStatus.ONLINE;
                    }
                    else if (userInput[1] == "maintenance")
                    {
                        Settings.serverStatus = ServerStatus.MAINTENANCE;
                    }
                    else
                    {
                        int status = int.Parse(userInput[1]);
                        if (status < 0 || status >= Settings.ServerStatusCount)
                            throw new Exception("Wrong status id");

                        Settings.serverStatus = (ServerStatus)int.Parse(userInput[1]);
                        Console.WriteLine($"Server status set to : {Enum.GetName(typeof(ServerStatus), Settings.serverStatus)}");
                    }
                }
                else if (userInput[0] == "disable" || userInput[0] == "enable") //Change the server status
                {
                    bool enable = userInput[0] == "enable";
                    if (userInput[1] == "logging")
                    {
                        Settings.ENABLE_LOGGING = enable;
                    }
                    else if (userInput[1] == "security")
                    {
                        Settings.ENABLE_SECURITY_KEY = enable;
                    }
                    else if (userInput[1] == "console")
                    {
                        Settings.ENABLE_CONSOLE_PRINT = enable;
                    }
                    else
                    {
                        throw new Exception("Wrong command argument");
                    }
                }
                else
                {
                    PrintAskHelp();
                }
            }
            if(Globals.slotSet == true)
            {
                int slotsCount = Globals.MapsToGo.Count-1;
                if (Globals.slotNumber == -2)
                {
                    while (Globals.slotNumber <= -1 || Globals.slotNumber > Globals.MapsToGo.Count -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Avaloble slots : From 0 to " + slotsCount);
                        Console.ForegroundColor = ConsoleColor.White;
                        var input = Console.ReadLine();
                        int.TryParse(input, out Globals.slotNumber);
                    }
                }
                if (Globals.slotNumber >= -1)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("\r\nSelected");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(" slot : " + Globals.slotNumber);
                    
                    if(Globals.mapNumber == -2)
                    {
                        while (Globals.mapNumber <= -1 || Globals.mapNumber > Globals.numbersOfMaps)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Now choose map :");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("0 - Dust2,\r\n1 - Tutorial,\r\n2 - Dust2_2x2,\r\n3 - Aim_Map");
                            Console.ForegroundColor = ConsoleColor.White;
                            var input = Console.ReadLine();
                            int.TryParse(input, out Globals.mapNumber);
                        }
                    }
                    if(Globals.mapNumber >= -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("Map (");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(Globals.mapNumber);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(") was set in (");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(Globals.slotNumber);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine(") slot.");
                        Console.ForegroundColor = ConsoleColor.White;

                        Globals.MapsToGo[Globals.slotNumber] = Globals.mapNumber;
                        Globals.mapNumber = -2;
                        Globals.slotNumber = -2;

                        Globals.slotSet = false;
                    }
                }
            }
        }

        /// <summary>
        /// Print some informations after the server startup
        /// </summary>
        /// <param name="ServerIp"></param>
        /// <param name="serverPort"></param>
        public static void PrintFirstMessage(string ServerIp, int serverPort)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Server has started on {ServerIp}:{serverPort}.\nWaiting for connections...");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("For debug (if enabled):");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("client.id -> Data sent by the client");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("client.id <- Data sent to the client");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Map Switch is : "+ Globals.mapSwitch);

            Console.ForegroundColor = ConsoleColor.White;
            UserInterfaceManager.PrintAskHelp();
        }


        /// <summary>
        /// Print the help message
        /// </summary>
        public static void PrintAskHelp()
        {
            Console.WriteLine("Type 'help' to get commands list");
        }

        /// <summary>
        /// Print commands list
        /// </summary>
        public static void PrintCommandsList()
        {
            Console.WriteLine("\ncommand_name [param] : Utility.\n" +
                           "stop : Stop the server.\n" +
                           "status [online/maintenance or 0/1] : Set the server status.\n" +
                           "disable/enable [logging/security/console] : Disable or enable a setting.\n" +
                           "chm [1/2/3]... : Change map. \n" +
                           "maplist : Shows map queue and available maps with ID.\n" +
                           "mapswitch [on/off] : Turns on and off the map switch.\n" +
                           "maptime : Shows time left for the map.\n" +
                           "settime [Number from 1 to 59]: Sets time for current and other maps.\n" +
                           "mapslot [add/remove/set]: \n" +
                           "  add: Adds new map slot for the queue.\n" +
                           "  remove: Removes last map slot from the queue.\n" +
                           "  set: Opens menu for you to choose Map Slot first and then ask a Map Index to set in the chosen Map Slot.\n" +
                           "\n");
        }

        /// <summary>
        /// Print a message in the console
        /// </summary>
        /// <param name="data">Message</param>
        public static void PrintMessage(string data)
        {
            if (!Settings.ENABLE_CONSOLE_PRINT)
                return;//Disable text to improve performance

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[{DateTime.Now}] - {data}");
        }

        /// <summary>
        /// Print incoming data in the console
        /// </summary>
        /// <param name="data">Received data</param>
        public static void PrintInData(Client client, string data)
        {
            if (!Settings.ENABLE_CONSOLE_PRINT)
                return;//Disable text to improve performance

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[{DateTime.Now}] {client.id} -> {data}");
        }

        /// <summary>
        /// Print sent data
        /// </summary>
        /// <param name="data">Sent data</param>
        public static void PrintOutData(Client client, string data)
        {
            if (!Settings.ENABLE_CONSOLE_PRINT)
                return;//Disable text to improve performance

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[{DateTime.Now}] {client.id} <- {data}");
        }

        /// <summary>
        /// Print error
        /// </summary>
        /// <param name="errorText">Error text</param>
        public static void PrintError(string errorText)
        {
            Logger.LogErrorInFile(errorText);

            if (!Settings.ENABLE_CONSOLE_PRINT)
                return;//Disable text to improve performance

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now}] {errorText}\n");
        }
    }
}
