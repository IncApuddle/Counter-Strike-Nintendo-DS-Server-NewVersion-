// SPDX-License-Identifier: MIT
//
// Copyright (c) 2008-2011, 2019, Antonio Niño Díaz
// Copyright (c) 2021-2022, Fewnity - Grégory Machefer (I ported the physics engine for C# language)

// This file is part of Nitro Engine

using System;
using System.Collections.Generic;

namespace Counter_Strike_Server
{
    public class Physics
    {
        /*! \enum NE_OnCollision
 *  \brief Possible actions that can be applied to an object after a collision.
 */
        public enum NE_OnCollision
        {
            NE_ColNothing, /*!< Ignore the object. */
            NE_ColBounce,      /*!< Bounce against the object. */
            NE_ColStop         /*!< Stop. */
        }

        [Serializable]
        public class NE_Physics
        {
            //public NE_PhysicsTypes type;
            public bool enabled;

            // Speed of model. The coordinates are taken from the NE_Model struct
            public Int64 xspeed, yspeed, zspeed;

            // For Axis-Aligned Bounding Boxes
            public int xsize, ysize, zsize;
            public int x, y, z;

            public int gravity, friction;

            // Percentage of energy in an object after bouncing
            public float keptpercent;

            // What to do when a collision is detected
            public NE_OnCollision oncollision;

            // true if a collision was detected during the last update of the model
            public bool iscolliding;
            public bool lastIscolliding;
            public bool iscollidingTrigger;

            // Objects only collide with other objects in the same physicsgroup
            public int physicsgroupCount;
            public int[] physicsgroup = new int[2];
        }

        public class Stairs
        {
            public float xSideA;
            public float xSideB;
            public float zSideA;
            public float zSideB;
            public float startY;
            public float endY;
            public int direction;
        }

        public class Grenade
        {
            public NE_Physics physics;
            public Client launcher;
            public DateTime timer;
            public int id;
        }

        public List<NE_Physics> allStaticPhysics = new();
        public int NE_MIN_BOUNCE_SPEED = (int)(0.01f * (1 << 12));


        public Physics()
        {
            NE_MIN_BOUNCE_SPEED = (int)(0.01f * (1 << 12));

            AddAllWalls();
            AddAllStairs();
        }

        public static Grenade CreateGrenade(Client launcher, int id, float xDirection, float yDirection, float zDirection)
        {
            Grenade newGrenade = new();
            newGrenade.launcher = launcher;
            newGrenade.id = id;
            newGrenade.timer = DateTime.Now.AddSeconds(4);
            NE_Physics newPhysics = new();
            newGrenade.physics = newPhysics;
            newPhysics.oncollision = NE_OnCollision.NE_ColBounce;

            NE_PhysicsSetSizeI(newPhysics, (int)(0.5 * (1 << 12)), (int)(0.5 * (1 << 12)), (int)(0.5 * (1 << 12)));
            newPhysics.physicsgroupCount = 1;
            newPhysics.physicsgroup[0] = 1;

            newPhysics.xspeed = (int)(xDirection * 2200);
            newPhysics.yspeed = (int)(yDirection * 2200);
            newPhysics.zspeed = (int)(-zDirection * 2200);

            newPhysics.x = (launcher.position.x * 2) + (int)(xDirection * 4096);
            newPhysics.y = (launcher.position.y * 2) + (int)(0.7f * 8192 * 2) + (int)(yDirection * 4096);
            newPhysics.z = (-launcher.position.z * 2) + (int)(-zDirection * 4096);

            newPhysics.friction = (int)(1.5f * (1 << 12));
            newPhysics.gravity = (int)(0.0065 * (1 << 12));

            newPhysics.keptpercent = 30;
            NE_PhysicsEnable(newPhysics, true);
            launcher.party.allGrenades.Add(newGrenade);
            return newGrenade;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <param name="zPos"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <param name="zSize"></param>
        /// <param name="Zone">Unsed</param>
        /// <param name="index">Unsed</param>
        public void CreateWall(double xPos, double yPos, double zPos, double xSize, double ySize, double zSize, int Zone, int index)
        {
            NE_Physics newPhysics = new();

            newPhysics.oncollision = NE_OnCollision.NE_ColBounce;
            NE_PhysicsSetSizeI(newPhysics, (int)(xSize * 8192), (int)(ySize * 8192), (int)(zSize * 8192));
            newPhysics.physicsgroupCount = 2;
            newPhysics.physicsgroup[1] = 1;
            newPhysics.x = (int)(xPos * 8192);
            newPhysics.y = (int)(yPos * 8192);
            newPhysics.z = -(int)(zPos * 8192);
            NE_PhysicsEnable(newPhysics, false);
            allStaticPhysics.Add(newPhysics);
        }

        public void NE_PhysicsDelete(NE_Physics pointer)
        {
            allStaticPhysics.Remove(pointer);
        }

        public void NE_PhysicsDeleteAll()
        {
            for (int i = 0; i < allStaticPhysics.Count; i++)
                NE_PhysicsDelete(allStaticPhysics[i]);
        }

        public static void NE_PhysicsSetSpeedI(NE_Physics pointer, int x, int y, int z)
        {
            pointer.xspeed = x;
            pointer.yspeed = y;
            pointer.zspeed = z;
        }

        public static void NE_PhysicsSetSizeI(NE_Physics pointer, int x, int y, int z)
        {
            pointer.xsize = x;
            pointer.ysize = y;
            pointer.zsize = z;
        }

        public static void NE_PhysicsSetGravityI(NE_Physics pointer, int gravity)
        {
            pointer.gravity = gravity;
        }

        public static void NE_PhysicsSetFrictionI(NE_Physics pointer, int friction)
        {
            pointer.friction = friction;
        }

        public static void NE_PhysicsSetBounceEnergy(NE_Physics pointer, int percent)
        {
            pointer.keptpercent = percent;
        }

        public static void NE_PhysicsEnable(NE_Physics pointer, bool value)
        {
            pointer.enabled = value;
        }

        public static void NE_PhysicsSetGroup(NE_Physics physics, int group, int index)
        {
            physics.physicsgroup[index] = group;
        }

        public static void NE_PhysicsOnCollision(NE_Physics physics, NE_OnCollision action)
        {
            physics.oncollision = action;
        }

        public static bool NE_PhysicsIsColliding(NE_Physics pointer)
        {
            return pointer.iscolliding;
        }

        public void NE_PhysicsUpdateOneGrenade(Grenade grenade)
        {
            bool isOnStairs = CheckStairsForGrenades(grenade.physics);
            int totalSpeed = (int)(grenade.physics.xspeed + grenade.physics.yspeed + grenade.physics.zspeed);
            if (totalSpeed == 0 && !isOnStairs)
            {
                NE_PhysicsEnable(grenade.physics, false);
            }
            else
            {
                NE_PhysicsUpdate(grenade.physics);
            }
        }

        public void NE_PhysicsUpdate(NE_Physics pointer)
        {
            if (pointer.enabled == false)
                return;

            pointer.iscolliding = false;

            // We change Y speed depending on gravity.
            pointer.yspeed -= pointer.gravity;

            // Now, let's move the object

            // Used in collision checking to simplify the code
            int posx, posy, posz;
            // Position before movement
            int bposx, bposy, bposz;

            bposx = pointer.x;
            bposy = pointer.y;
            bposz = pointer.z;
            posx = pointer.x = (int)(pointer.x + pointer.xspeed);
            posy = pointer.y = (int)(pointer.y + pointer.yspeed);
            posz = pointer.z = (int)(pointer.z + pointer.zspeed);

            // Gravity and movement have been applied, time to check collisions...
            bool xenabled = true, yenabled = true, zenabled = true;
            if (bposx == posx)
                xenabled = false;
            if (bposy == posy)
                yenabled = false;
            if (bposz == posz)
                zenabled = false;

            for (int i = 0; i < allStaticPhysics.Count; i++)
            {
                // Check that we aren't checking an object with itself
                if (allStaticPhysics[i] == pointer)
                    continue;

                bool NeedContinue = true;
                // Check that both objects are in the same group
                for (int j = 0; j < allStaticPhysics[i].physicsgroupCount; j++)
                {
                    if (allStaticPhysics[i].physicsgroup[j] == pointer.physicsgroup[0])
                    {
                        NeedContinue = false;
                        break;
                    }
                }

                if (NeedContinue)
                {
                    continue;
                }

                NE_Physics otherpointer = allStaticPhysics[i];

                //Get coordinates
                int otherposx = otherpointer.x, otherposy = otherpointer.y, otherposz = otherpointer.z;

                // Both are boxes

                bool collision =
                    ((Math.Abs(posx - otherposx) < (pointer.xsize + otherpointer.xsize) >> 1) &&
                     (Math.Abs(posy - otherposy) < (pointer.ysize + otherpointer.ysize) >> 1) &&
                     (Math.Abs(posz - otherposz) < (pointer.zsize + otherpointer.zsize) >> 1));

                if (!collision)
                {
                    continue;
                }

                pointer.iscolliding = true;

                if (pointer.oncollision == NE_OnCollision.NE_ColBounce)
                {
                    // Used to reduce speed:
                    int temp = (int)(((pointer.keptpercent * (1 << 12)) / (100f * (1 << 12))) * (1 << 12));

                    if ((yenabled) && ((Math.Abs(bposy - otherposy) >= (pointer.ysize + otherpointer.ysize) >> 1)))
                    {
                        yenabled = false;
                        pointer.yspeed += pointer.gravity;

                        if (posy > otherposy)
                            pointer.y = otherposy + ((pointer.ysize + otherpointer.ysize) >> 1);
                        if (posy < otherposy)
                            pointer.y = otherposy - ((pointer.ysize + otherpointer.ysize) >> 1);

                        if (pointer.gravity == 0)
                        {
                            pointer.yspeed =
                                -(temp * pointer.yspeed);
                        }
                        else
                        {
                            if (Math.Abs(pointer.yspeed) > NE_MIN_BOUNCE_SPEED)
                            {
                                pointer.yspeed = -((temp * (pointer.yspeed - pointer.gravity)) / (1 << 12));
                            }
                            else
                            {
                                pointer.yspeed = 0;
                            }
                        }
                    }
                    else if ((xenabled) && ((Math.Abs(bposx - otherposx) >= (pointer.xsize + otherpointer.xsize) >> 1)))
                    {
                        xenabled = false;

                        if (posx > otherposx)
                            pointer.x = otherposx + ((pointer.xsize + otherpointer.xsize) >> 1);
                        if (posx < otherposx)
                            pointer.x = otherposx - ((pointer.xsize + otherpointer.xsize) >> 1);

                        pointer.xspeed = -((temp * pointer.xspeed) / (1 << 12));
                    }
                    else if ((zenabled) && ((Math.Abs(bposz - otherposz) >= (pointer.zsize + otherpointer.zsize) >> 1)))
                    {
                        zenabled = false;

                        if (posz > otherposz)
                            pointer.z = otherposz + ((pointer.zsize + otherpointer.zsize) >> 1);
                        if (posz < otherposz)
                            pointer.z = otherposz - ((pointer.zsize + otherpointer.zsize) >> 1);

                        pointer.zspeed = -((temp * pointer.zspeed) / (1 << 12));
                    }
                }
                else if (pointer.oncollision == NE_OnCollision.NE_ColStop)
                {
                    if ((yenabled) && ((Math.Abs(bposy - otherposy) >= (pointer.ysize + otherpointer.ysize) >> 1)))
                    {
                        yenabled = false;

                        if (posy > otherposy)
                            pointer.y = otherposy + ((pointer.ysize + otherpointer.ysize) >> 1);
                        if (posy < otherposy)
                            pointer.y = otherposy - ((pointer.ysize + otherpointer.ysize) >> 1);
                    }
                    if ((xenabled) && ((Math.Abs(bposx - otherposx) >= (pointer.xsize + otherpointer.xsize) >> 1)))
                    {
                        xenabled = false;

                        if (posx > otherposx)
                            pointer.x = otherposx + ((pointer.xsize + otherpointer.xsize) >> 1);
                        if (posx < otherposx)
                            pointer.x = otherposx - ((pointer.xsize + otherpointer.xsize) >> 1);
                    }
                    if ((zenabled) && ((Math.Abs(bposz - otherposz) >= (pointer.zsize + otherpointer.zsize) >> 1)))
                    {
                        zenabled = false;

                        if (posz > otherposz)
                            pointer.z = otherposz + ((pointer.zsize + otherpointer.zsize) >> 1);
                        if (posz < otherposz)
                            pointer.z = otherposz - ((pointer.zsize + otherpointer.zsize) >> 1);
                    }
                    pointer.xspeed = pointer.yspeed = pointer.zspeed = 0;
                }
            }

            if (!pointer.lastIscolliding && pointer.iscolliding)
                pointer.iscollidingTrigger = true;
            else
                pointer.iscollidingTrigger = false;

            pointer.lastIscolliding = pointer.iscolliding;

            //Now, we get the module of speed in order to apply friction.
            if (pointer.friction != 0)
            {
                pointer.xspeed <<= 10;
                pointer.yspeed <<= 10;
                pointer.zspeed <<= 10;

                Int64 _mod_ = (pointer.xspeed * pointer.xspeed);
                _mod_ += (pointer.yspeed * pointer.yspeed);
                _mod_ += (pointer.zspeed * pointer.zspeed);
                _mod_ = (int)(Math.Sqrt(_mod_));

                //check if module is very small -> speed = 0
                if (_mod_ < pointer.friction)
                {
                    pointer.xspeed = pointer.yspeed = pointer.zspeed = 0;
                }
                else
                {
                    Int64 newmod = _mod_ - pointer.friction;

                    int number = (int)((((float)newmod) / ((float)_mod_)) * (1 << 12));

                    pointer.xspeed = (pointer.xspeed * number) / (1 << 12);
                    pointer.yspeed = (pointer.yspeed * number) / (1 << 12);
                    pointer.zspeed = (pointer.zspeed * number) / (1 << 12);
                    pointer.xspeed >>= 10;
                    pointer.yspeed >>= 10;
                    pointer.zspeed >>= 10;
                }

            }
        }

        public static bool NE_PhysicsCheckCollision(NE_Physics pointer1, NE_Physics pointer2)
        {
            //Get coordinates
            int posx, posy, posz;

            posx = pointer1.x;
            posy = pointer1.y;
            posz = pointer1.z;

            int otherposx, otherposy, otherposz;

            otherposx = pointer2.x;
            otherposy = pointer2.y;
            otherposz = pointer2.z;


            if ((Math.Abs(posx - otherposx) < (pointer1.xsize + pointer2.xsize) >> 1) &&
                (Math.Abs(posy - otherposy) < (pointer1.ysize + pointer2.ysize) >> 1) &&
                (Math.Abs(posz - otherposz) < (pointer1.zsize + pointer2.zsize) >> 1))
            {
                return true;
            }

            return false;
        }

        public void AddAllWalls()
        {
            if(Globals.selectedMap == 0)
            {
                CreateWall(9.38846, -1.4, -8.007592, 35.77919, 1, 33.08558, 3, 0);
                CreateWall(3.991, 0.285, 4.786, 2.423, 2.423, 2.423, 3, 1);
                CreateWall(5.41, 1.107, 5.980854, 0.4, 4, 30.68171, 3, 2);
                CreateWall(-1.4, 1.8, 38.985, 13, 1, 34.67, 4, 3);
                CreateWall(-10.31409, -1.03, 5.18, 17.45819, 1, 7, 1, 4);
                CreateWall(-1.654, 2.288648, 16.47623, 0.8, 6.477295, 19.37246, 4, 5);
                CreateWall(-1.654, 2.05, 2.617579, 0.8, 6, 1.884841, 3, 6);
                CreateWall(24.42, 1.8, 29.52468, 37.5, 1, 67.90935, -1, 7);
                CreateWall(48.3451, 1.8, 1.665, 9.130207, 1, 38.67, 5, 8);
                CreateWall(40.63582, 1.8, -14.36, 5.668356, 1, 7, 5, 9);
                CreateWall(44.5, 5.04, -33.965, 15, 1, 6.67, 5, 10);
                CreateWall(47.75, -3.04, 37.732, 9, 1, 7, 5, 11);
                CreateWall(40.68, 4.23, -23.46, 5.7, 1, 11.2, 5, 12);
                CreateWall(27.58, 4.225, -24.69, 20, 1, 9, 3, 13);
                CreateWall(21.51, 4.225, -12.405, 7, 1, 15.17, 3, 14);
                CreateWall(-30.05, 1.8, -16.65, 24.67, 1, 34, 2, 15);
                CreateWall(-37.12021, 2.6, -30.795, 9.679577, 1, 19.67, 2, 16);
                CreateWall(-38.66, 2.6065, 3.923, 10, 1, 16.17, 1, 17);
                CreateWall(-26.975, 2.6065, 10.75, 12.67, 1, 11.5, 1, 18);
                CreateWall(-31.85, 2.6065, 21.645, 16.8, 1, 9.97, 1, 19);
                CreateWall(-31.85, 1.8, 35.88, 16.8, 1, 16, 0, 20);
                CreateWall(-36.76, 5.03, 60.475, 6.5, 1, 6.67, 0, 21);
                CreateWall(-42.78, 5.03, 58.6, 5, 1, 10, 0, 22);
                CreateWall(-22.91243, 5.03, 53.76, 20.59513, 1, 19, 0, 23);
                CreateWall(-7.165, 5.03, 61.57, 11.17, 1, 10, 4, 24);
                CreateWall(-15.974, 0.591, 13.66, 3.7, 1, 3.37, 1, 25);
                CreateWall(55.00196, 3.415, 23.80224, 3.714117, 1, 17.83263, 5, 26);
                CreateWall(4.846629, 2.989377, -0.481, 0.9097431, 7.831755, 2.423, 3, 27);
                CreateWall(-0.8933716, 2.709331, -0.481, 0.9097431, 7.271662, 2.423, 3, 28);
                CreateWall(0.6405261, 1.114704, 0.3353194, 2.150795, 4.082407, 0.6293912, 3, 29);
                CreateWall(3.354151, 1.114704, -1.250537, 2.145238, 4.082407, 0.6865859, 3, 30);
                CreateWall(4.053317, 1.114704, -0.5631354, 0.6353655, 4.082407, 0.3297293, 3, 31);
                CreateWall(-0.1147662, 1.114704, -0.41167, 0.6778869, 4.082407, 0.3050682, 3, 32);
                CreateWall(-10.94747, 2.889332, -4.495358, 19.35818, 7.631665, 12.90928, -1, 33);
                CreateWall(-1.26794, 2.889332, -23.5255, 12.94932, 7.631665, 2.408997, 3, 34);
                CreateWall(-8.63794, 4.209399, -25.88165, 8.13, 10.2718, 3.561691, 2, 35);
                CreateWall(-15.26169, 4.209399, -28.2181, 11.56, 10.2718, 1.161465, 2, 36);
                CreateWall(-16.24241, 3.05695, -12.16243, 2.29025, 3.088099, 2.408759, 2, 37);
                CreateWall(-20.628, 4.339704, -12.831, 0.6293912, 4.082407, 2.150795, 2, 38);
                CreateWall(-21.486, 4.339704, -16.268, 0.3297293, 4.082407, 0.6353655, 2, 39);
                CreateWall(-22.215, 4.339704, -15.521, 0.6293912, 4.082407, 2.150795, 2, 40);
                CreateWall(-21.352, 4.339704, -12.076, 0.3297293, 4.082407, 0.6353655, 2, 41);
                CreateWall(-17.98957, 2.909, -11.5547, 1.207145, 1.2, 1.211931, 2, 42);
                CreateWall(-21.42, 5.894331, -11.297, 2.423, 7.271662, 0.9097431, 2, 43);
                CreateWall(-21.42, 5.894331, -17.06, 2.423, 7.271662, 0.9097431, 2, 44);
                CreateWall(-21.41365, 5.894331, -21.33369, 1.604465, 7.271662, 8.111914, 2, 45);
                CreateWall(-21.41365, 5.894331, -8.338291, 1.604465, 7.271662, 5.283774, 2, 46);
                CreateWall(-21.41365, 5.894331, -29.9949, 1.604465, 7.271662, 4.181724, 2, 47);
                CreateWall(-17.20958, 4.082152, -26.64717, 9.35, 3.647305, 2.424164, 2, 48);
                CreateWall(-23.02708, 3.912082, -27.25182, 1.616625, 3.231766, 1.284817, 2, 49);
                CreateWall(-23.02901, 4.519009, -25.66659, 1.620493, 1.219922, 1.21636, 2, 50);
                CreateWall(-23.8313, 3.102336, -25.47028, 3.225069, 1.615268, 1.608994, 2, 51);
                CreateWall(9.870321, 1.493199, -8.82, 8.488289, 4.839397, 1, 3, 52);
                CreateWall(19.71741, 1.488101, -10.13818, 11.26746, 4.849592, 1.627839, 3, 53);
                CreateWall(34.23705, 3.871293, -0.4494247, 19.34229, 9.615976, 20.9745, -1, 54);
                CreateWall(8.829233, 0.6898834, -10.13725, 1.602546, 3.232767, 1.615795, 3, 55);
                CreateWall(7.217544, 0.2866741, -16.19459, 2.415168, 2.426349, 2.424468, 3, 56);
                CreateWall(6.816122, 2.310524, -16.59494, 1.612324, 1.596437, 1.616609, 3, 57);
                CreateWall(18.91403, 1.488101, -23.54818, 11.28471, 4.849592, 1, 3, 58);
                CreateWall(9.246572, 1.488101, -20.25092, 8.058916, 4.849592, 5.70549, 3, 59);
                CreateWall(24.96741, 1.488101, -20.2333, 0.8, 4.849592, 5.66225, 3, 60);
                CreateWall(26.58551, 0.4672852, -18.67, 2.418206, 2.457632, 2.39, 3, 61);
                CreateWall(29.00651, 1.064285, -18.67, 2.418206, 2.457632, 2.39, 3, 62);
                CreateWall(26.98624, 2.499154, -19.01415, 1.618732, 1.627894, 1.629699, 3, 63);
                CreateWall(31.60709, 2.795956, -20.02006, 12.55215, 5.453778, 0.4001274, 3, 64);
                CreateWall(40.56831, 3.903672, -17.60006, 6.194584, 3.238344, 0.4001274, 5, 65);
                CreateWall(37.69, 3.903672, -18.81139, 0.4001274, 3.238344, 2.822779, 5, 66);
                CreateWall(43.73696, 3.752151, -23.85887, 0.4140491, 3.541387, 12.91774, 5, 67);
                CreateWall(25.17, 4.305497, -15.58066, 0.4001274, 2.434694, 9.287472, 3, 68);
                CreateWall(38.68263, 5.510857, -19.60757, 1.611956, 1.60449, 3.610546, 5, 69);
                CreateWall(38.68263, 7.150856, -19.01406, 1.611956, 1.60449, 1.619928, 5, 70);
                CreateWall(42.71363, 5.530857, -22.65006, 1.611956, 1.60449, 1.619928, 5, 71);
                CreateWall(27.79171, 7.936794, -32.73777, 19.3618, 6.416366, 8.091341, 3, 72);
                CreateWall(27.78882, 7.936794, -28.30295, 7.260923, 6.416366, 0.8257637, 3, 73);
                CreateWall(15.6907, 7.936794, -13.78543, 4.817293, 6.416366, 29.87009, 3, 74);
                CreateWall(18.42, 7.936794, -20.63499, 1, 6.416366, 7.24859, 3, 75);
                CreateWall(44.73563, 3.656087, -19.84385, 1.611956, 2.038029, 1.555517, 5, 76);
                CreateWall(54.45702, 6.858232, -27.74011, 4.934025, 8.573491, 20.66022, 5, 77);
                CreateWall(44.79, 6.858232, -37.25, 15, 8.573491, 1, 5, 78);
                CreateWall(43.12514, 3.871293, -10.13021, 3.204861, 9.615976, 3.212919, 5, 79);
                CreateWall(43.12514, 3.871293, 9.217745, 3.204861, 9.615976, 3.228837, 5, 80);
                CreateWall(55.61611, 4.50092, -5.702986, 2.438217, 2.458159, 2.414028, 5, 81);
                CreateWall(55.15685, 5.471698, 5.165548, 6.338552, 6.415167, 19.35178, 5, 82);
                CreateWall(58.07824, 5.471698, 7.948711, 2.521341, 6.415167, 50.76546, 5, 83);
                CreateWall(56.02677, 4.719615, 15.67324, 1.615543, 1.62077, 1.627525, 5, 84);
                CreateWall(55.63088, 6.345802, 31.40095, 2.447762, 4.873144, 2.418945, 5, 85);
                CreateWall(47.67071, 6.720139, 36.50803, 13.9532, 8.840049, 7.817391, 5, 86);
                CreateWall(37.47268, 6.720139, 35.01363, 3.218556, 8.840049, 1.644658, 5, 87);
                CreateWall(38.30424, 6.720139, 36.66363, 4.881684, 8.840049, 1.644658, 5, 88);
                CreateWall(47.88821, -0.1699834, 41.53085, 8.275005, 4.7418, 1.703053, 5, 89);
                CreateWall(43.52215, 0.2628939, 31.01274, 0.8107128, 5.686295, 19.40301, 5, 90);
                CreateWall(44.72398, -1.73808, 39.10096, 1.634037, 1.618159, 3.295925, 5, 91);
                CreateWall(51.18998, -1.73808, 35.84166, 1.634037, 1.618159, 3.231308, 5, 92);
                CreateWall(51.18998, -0.1290795, 36.23938, 1.634037, 1.618159, 1.610428, 5, 93);
                CreateWall(52.32014, 0.8512607, 29.83927, 0.6802864, 6.955806, 20.27887, 5, 94);
                CreateWall(52.32679, 3.286222, 15.67575, 0.6669998, 2.08588, 1.603443, 5, 95);
                CreateWall(33.44603, 6.720139, 36.03182, 4.826495, 8.840049, 29.46829, 6, 96);
                CreateWall(22.95895, 6.720139, 21.99475, 3.240649, 8.840049, 24.45416, 6, 97);
                CreateWall(30.64663, 5.934331, 33.42, 0.9097431, 7.271662, 2.423, 6, 98);
                CreateWall(24.90663, 5.934331, 33.42, 0.9097431, 7.271662, 2.423, 6, 99);
                CreateWall(26.44053, 4.339704, 32.63532, 2.150795, 4.082407, 0.6293912, 6, 100);
                CreateWall(29.15415, 4.339704, 34.19246, 2.145238, 4.082407, 0.6865859, 6, 101);
                CreateWall(29.85332, 4.339704, 33.49086, 0.6353655, 4.082407, 0.3297293, 6, 102);
                CreateWall(25.68523, 4.339704, 33.37033, 0.6778869, 4.082407, 0.3050682, 6, 103);
                CreateWall(29.813, 3.51, 24.547, 2.423, 2.423, 2.423, 6, 104);
                CreateWall(24.90663, 5.934331, 22.13, 0.9097431, 7.271662, 2.423, 6, 105);
                CreateWall(30.64663, 5.934331, 22.13, 0.9097431, 7.271662, 2.423, 6, 106);
                CreateWall(26.37633, 6.720139, 57.01281, 10.12095, 8.840049, 13.31212, 6, 107);
                CreateWall(15.38498, 6.720139, 65.11506, 13.95091, 8.840049, 3.702241, 6, 108);
                CreateWall(-18.70116, 6.720139, 67.90365, 54.33317, 8.840049, 2.839417, 0, 109);
                CreateWall(-20.62166, 9.071501, 66.18, 7.271675, 5.883001, 1, 0, 110);
                CreateWall(-33.51166, 9.071501, 66.18, 7.271675, 5.883001, 1, 0, 111);
                CreateWall(19.46931, 6.720139, 16.87856, 9.941291, 8.840049, 23.40178, -1, 112);
                CreateWall(11.42511, 6.720139, 16.45395, 6.092877, 8.840049, 22.55255, -1, 113);
                CreateWall(8.43699, 6.720139, 43.91084, 12.90911, 8.840049, 19.34007, -1, 114);
                CreateWall(15.2, 6.720139, 43.90626, 1, 8.840049, 7.272514, 6, 115);
                CreateWall(9.832866, 6.720139, 33.91, 10.07427, 8.840049, 1, 4, 116);
                CreateWall(3.994344, 3.108447, 33.42598, 1.615686, 1.604893, 1.623959, 4, 117);
                CreateWall(-5.272417, 5.538648, 25.75, 4.824834, 6.477295, 4, 4, 118);
                CreateWall(-5.272417, 5.538648, 36.227, 4.824834, 6.477295, 4, 4, 119);
                CreateWall(-6.90057, 6.730637, 46.3273, 11.26114, 8.861274, 20.97461, -1, 120);
                CreateWall(-7.831, 5.3, 30.99, 1, 6, 6.5, 4, 121);
                CreateWall(5.074815, 4.301765, 56.40557, 13.1601, 4.062151, 0.7663727, 4, 122);
                CreateWall(0.24, 7.568765, 47.13827, 3.5, 4.062151, 12.90346, 4, 123);
                CreateWall(-0.4394904, 4.314489, 43.90987, 1.612981, 4.03266, 3.21767, 4, 124);
                CreateWall(-11.73649, 7.54249, 64.4865, 1.612981, 4.03266, 4.060944, 0, 125);
                CreateWall(-11.92781, 7.54249, 56.60429, 1.995615, 4.03266, 2.005371, 0, 126);
                CreateWall(-14.16297, 7.145098, 55.19912, 3.225281, 3.237878, 3.219673, 0, 127);
                CreateWall(-13.34894, 7.145098, 52.7741, 1.610119, 3.237878, 1.614204, 0, 128);
                CreateWall(-11.92781, 7.54249, 62.85986, 1.995615, 4.03266, 0.8165054, 0, 129);
                CreateWall(-45.35478, 7.54249, 59.61, 1.089561, 4.03266, 15, 0, 130);
                CreateWall(-42.46, 5.924819, 39.06439, 5, 7.268002, 29.11122, 0, 131);
                CreateWall(-40.176, 5.523045, 55.21048, 0.4, 1.619911, 3.189045, 0, 132);
                CreateWall(-38.76395, 4.321394, 25.75254, 2.419907, 2.425211, 2.422911, 0, 133);
                CreateWall(-25.86095, 3.515394, 41.48711, 2.419907, 2.425211, 4.835773, 0, 134);
                CreateWall(-31.50165, 7.140826, 45.92187, 3.236599, 3.233137, 3.219515, 0, 135);
                CreateWall(-32.31604, 6.330677, 48.3438, 1.60783, 1.614568, 1.621651, 0, 136);
                CreateWall(-33.318, 4.250121, 50.35683, 0.4, 4.165757, 12.89635, 0, 137);
                CreateWall(-29.05922, 4.250121, 44.11132, 8.917554, 4.165757, 0.4053345, 0, 138);
                CreateWall(-24.24499, 6.744021, 40.68601, 0.8298302, 8.882464, 7.251068, 0, 139);
                CreateWall(-18.16362, 6.744021, 35.00214, 11.37258, 8.882464, 24.24732, 0, 140);
                CreateWall(-16.79598, 8.345907, 47.44, 8.42, 5.678691, 1, 0, 141);
                CreateWall(-25.05, 7.125232, 19.71861, 7.251068, 8.120045, 6.440567, 1, 142);
                CreateWall(-35.55, 7.125232, 19.71861, 7.251068, 8.120045, 6.440567, 1, 143);
                CreateWall(-37.39254, 7.125231, 23.74257, 5.316154, 8.120045, 1.612665, 0, 144);
                CreateWall(-20.57564, 3.187231, 15.68, 12.98743, 8.120045, 1.612665, 1, 145);
                CreateWall(-38.3665, 5.120971, 14.08765, 9.724064, 4.111524, 4.898634, 1, 146);
                CreateWall(-43.49247, 5.120971, 8.41167, 0.5960007, 4.111524, 6.454079, 1, 147);
                CreateWall(-41.58795, 5.120971, 1.959855, 3.235893, 4.111524, 12.92029, 1, 148);
                CreateWall(-34.34674, 4.740863, 0.3402195, 4.858311, 4.87174, 9.681019, 1, 149);
                CreateWall(-30.24487, 4.740863, 2.731868, 3.448055, 4.87174, 4.779274, 1, 150);
                CreateWall(-19.33138, 5.148415, 5.109592, 21.90529, 4.056636, 6.620091, 1, 151);
                CreateWall(-8.102446, 3.298522, 11.64577, 12.11511, 7.756417, 6.451536, -1, 152);
                CreateWall(-9.72221, 0.6884941, 7.204192, 2.422421, 2.419012, 2.422384, 1, 153);
                CreateWall(-17.38029, 0.2831234, 2.711911, 3.224581, 1.60827, 1.728946, 1, 154);
                CreateWall(-19.47366, 1.283146, 5.166797, 0.9559011, 3.613672, 6.495212, 1, 155);
                CreateWall(-19.01047, 1.429199, 10.02323, 3.239193, 4.145567, 3.21852, 1, 156);
                CreateWall(-25.45899, 5.416459, -2.911342, 6.440163, 6.200087, 6.407383, 2, 157);
                CreateWall(-34.32658, 3.935917, -5.298461, 1.617161, 3.239004, 1.625077, 2, 158);
                CreateWall(-42.02054, 5.132627, -21.06406, 0.8790817, 5.646835, 33.14563, 2, 159);
                CreateWall(-41.19003, 5.970626, -28.69818, 0.7959557, 5.646835, 7.276354, 2, 160);
                CreateWall(-39.97736, 3.099399, -20.8255, 3.224247, 1.623342, 0.3831959, 2, 161);
                CreateWall(-33.71442, 3.099399, -20.83454, 2.830128, 1.623342, 0.4012718, 2, 162);
                CreateWall(-33.92458, 3.504645, -19.42433, 2.409798, 2.433834, 2.412289, 2, 163);
                CreateWall(-32.10999, 3.099732, -25.9817, 0.4169121, 1.624008, 11.07064, 2, 164);
                CreateWall(-29.48116, 3.909713, -23.45477, 1.612955, 3.243968, 1.615158, 2, 165);
                CreateWall(-40.17027, 4.314253, -31.31842, 1.213169, 2.440887, 1.210466, 2, 166);
                CreateWall(-40.78803, 4.711897, -38.37749, 1.635225, 3.236175, 1.613446, 2, 167);
                CreateWall(-37.95386, 4.711897, -39.17149, 4.044044, 3.236175, 1.613446, 2, 168);
                CreateWall(-37.14289, 5.310734, -37.17551, 2.42997, 4.433849, 2.441495, 2, 169);
                CreateWall(-29.07345, 4.917582, -33.95314, 13.76087, 5.220153, 4.064755, 2, 170);
                CreateWall(-40.78566, 3.90744, -21.84602, 1.629536, 1.62726, 1.619009, 2, 171);
                CreateWall(15.65053, 3.52211, -1.713665, 4.895332, 2.395253, 5.782387, 3, 172);
                CreateWall(12.46407, 3.925133, 1.150944, 1.617191, 3.201298, 1.618112, 3, 173);
                CreateWall(5.992119, 5.469345, -0.4590559, 14.66269, 6.289724, 1.618112, 3, 174);
                CreateWall(23.35082, 3.525312, 3.975177, 2.418831, 2.389922, 2.420465, 3, 175);
                CreateWall(23.75113, 6.332403, -9.739486, 1.617455, 3.222512, 1.611141, 3, 176);
                CreateWall(21.54216, 3.514502, -2.880072, 0.4025955, 2.410711, 3.235695, 3, 177);
                CreateWall(-19.39825, 3.717201, -24.99569, 2.410248, 2.821984, 6.29, 2, 178);
                CreateWall(-17.58567, 3.108061, -24.66569, 1.205088, 1.603703, 5.62, 2, 179);
                CreateWall(-11.74238, 1.165568, -22.24077, 1.60723, 2.26469, 1.611542, 2, 180);
                CreateWall(-10.12338, 0.5635681, -23.04477, 1.60723, 2.26469, 1.611542, 2, 181);
                CreateWall(-10.12338, 0.371568, -21.43977, 1.60723, 2.26469, 1.611542, 2, 182);
                CreateWall(-24.23902, 2.723958, -26.694, 0.8139538, 0.8524289, 0.8340797, 2, 183);
                CreateWall(-17.78772, 2.671109, -21.45288, 0.8073368, 0.7361269, 0.775835, 2, 184);
                CreateWall(32.64186, 3.912763, 11.63979, 3.213718, 3.226475, 3.216652, 6, 185);
                CreateWall(35.05, 3.102, 10.828, 1.62, 1.62, 1.62, 5, 186);
                CreateWall(23.153, 3.307, -4.994, 2.82, 2, 1, 3, 187);
                CreateWall(-20.6154, 4.318905, 9.235, 3.226804, 1.613811, 1.6, 1, 188);
                CreateWall(8.033, 2.504, 0.538, 4.05, 0.4, 1.222159, 3, 189);
                CreateWall(18.112, 2.504, 28.781, 4.05, 0.4, 1.222159, 6, 190);
                CreateWall(15.9, 2.504, 43.898, 1.222159, 0.4, 4.05, 6, 191);
                CreateWall(-21.4125, 8.321119, -26.54848, 1.607979, 0.8150377, 4.404967, 2, 192);
                CreateWall(-25.052, 2.665476, -24.29729, 0.8, 0.7450484, 0.7311573, 2, 193);
                CreateWall(51.25, -0.15, 40.724, 1.77, 4.7418, 1.703053, 5, 194);
                CreateWall(43.89105, 0.4453565, 40.36826, 1.712103, 3.551087, 0.9915719, 5, 195);
                CreateWall(56.639, 4.118, 26.56685, 1.222159, 0.4, 4.037703, 5, 196);
                CreateWall(-20.6154, 6.148234, 9.235, 3.226804, 2.025973, 1.6, 1, 197);
                CreateWall(-21.006, 6.454825, 14.67238, 0.8, 1.41279, 0.415242, 1, 198);
                CreateWall(-21.006, 6.713649, 14.14657, 0.8, 0.895143, 0.8783064, 1, 199);
                CreateWall(-2.28019, 2.288647, 25.35139, 2.052381, 6.477295, 1.62215, 4, 200);
                CreateWall(-13.63971, 2.708109, -25.22188, 11.81, 2.43, 2.35, 2, 201);
            }
            else if(Globals.selectedMap == 1)
            {
                CreateWall(0, -1.5, 2, 22.7, 1, 22.7, 0, 0);
                CreateWall(-0.002059221, 0.227705, 1.943192, 2.450016, 2.451308, 2.462128, 0, 1);
                CreateWall(1.890654, -0.321328, 1.942942, 1.352347, 1.353242, 1.352881, 0, 2);
                CreateWall(0, -0.95, -6.720729, 22.7, 1, 5.33, 0, 3);
                CreateWall(-11.84, 3.77, 2, 1, 10, 22.7, 0, 4);
                CreateWall(11.84, 3.77, 2, 1, 10, 22.7, 0, 5);
                CreateWall(0, 3.77, -9.9, 22.7, 10, 1, 0, 6);
                CreateWall(0, 3.77, 13.785, 22.7, 10, 1, 0, 7);
            }
            else if(Globals.selectedMap == 2)
            {
                CreateWall(9.38846, -1.4, -8.007592, 35.77919, 1, 33.08558, 3, 0);
                CreateWall(3.991, 0.285, 4.786, 2.423, 2.423, 2.423, 3, 1);
                CreateWall(5.41, 1.107, 1.770854, 0.4, 4, 39.1, 3, 2);
                CreateWall(-1.4, 1.8, 38.985, 13, 1, 34.67, 4, 3);
                CreateWall(-1.654, 2.288648, 13.15623, 0.8, 6.477295, 26, 4, 4);
                CreateWall(24.42, 1.8, 29.52468, 37.5, 1, 67.90935, -1, 5);
                CreateWall(48.3451, 1.8, 1.665, 9.130207, 1, 38.67, 5, 6);
                CreateWall(40.63582, 1.8, -14.36, 5.668356, 1, 7, 5, 7);
                CreateWall(44.5, 5.04, -33.965, 15, 1, 6.67, 5, 8);
                CreateWall(47.75, -3.04, 37.732, 9, 1, 7, 5, 9);
                CreateWall(40.68, 4.23, -23.46, 5.7, 1, 11.2, 5, 10);
                CreateWall(27.58, 4.225, -24.69, 20, 1, 9, 3, 11);
                CreateWall(21.51, 4.225, -12.405, 7, 1, 15.17, 3, 12);
                CreateWall(-7.165, 5.03, 61.57, 11.17, 1, 10, 4, 13);
                CreateWall(55.00196, 3.415, 23.80224, 3.714117, 1, 17.83263, 5, 14);
                CreateWall(5.79, 1.107, -13.25915, 0.54, 3.71, 7.75, 3, 15);
                CreateWall(9.870321, 1.493199, -8.82, 8.488289, 4.839397, 1, 3, 16);
                CreateWall(19.71741, 1.488101, -10.13818, 11.26746, 4.849592, 1.627839, 3, 17);
                CreateWall(34.23705, 3.871293, -0.4494247, 19.34229, 9.615976, 20.9745, -1, 18);
                CreateWall(8.829233, 0.6898834, -10.13725, 1.602546, 3.232767, 1.615795, 3, 19);
                CreateWall(7.217544, 0.2866741, -16.19459, 2.415168, 2.426349, 2.424468, 3, 20);
                CreateWall(6.816122, 2.310524, -16.59494, 1.612324, 1.596437, 1.616609, 3, 21);
                CreateWall(18.91403, 1.488101, -23.54818, 11.28471, 4.849592, 1, 3, 22);
                CreateWall(9.246572, 1.488101, -20.25092, 8.058916, 4.849592, 5.70549, 3, 23);
                CreateWall(24.96741, 1.488101, -20.2333, 0.8, 4.849592, 5.66225, 3, 24);
                CreateWall(26.58551, 0.4672852, -18.67, 2.418206, 2.457632, 2.39, 3, 25);
                CreateWall(29.00651, 1.064285, -18.67, 2.418206, 2.457632, 2.39, 3, 26);
                CreateWall(26.98624, 2.499154, -19.01415, 1.618732, 1.627894, 1.629699, 3, 27);
                CreateWall(31.60709, 2.795956, -20.02006, 12.55215, 5.453778, 0.4001274, 3, 28);
                CreateWall(40.69431, 3.903672, -17.60006, 6.45, 3.238344, 0.4001274, 5, 29);
                CreateWall(37.69, 3.903672, -18.81139, 0.4001274, 3.238344, 2.822779, 5, 30);
                CreateWall(43.73696, 3.752151, -23.86887, 0.4140491, 3.541387, 12.91774, 5, 31);
                CreateWall(25.17, 4.305497, -15.58066, 0.4001274, 2.434694, 9.287472, 3, 32);
                CreateWall(38.68263, 5.510857, -19.60757, 1.611956, 1.60449, 3.610546, 5, 33);
                CreateWall(38.68263, 7.150856, -19.01406, 1.611956, 1.60449, 1.619928, 5, 34);
                CreateWall(42.71363, 5.530857, -22.65006, 1.611956, 1.60449, 1.619928, 5, 35);
                CreateWall(27.79171, 7.936794, -32.73777, 19.3618, 6.416366, 8.091341, 3, 36);
                CreateWall(27.78882, 7.936794, -28.30295, 7.260923, 6.416366, 0.8257637, 3, 37);
                CreateWall(15.6907, 7.936794, -13.78543, 4.817293, 6.416366, 29.87009, 3, 38);
                CreateWall(18.42, 7.936794, -20.63499, 1, 6.416366, 7.24859, 3, 39);
                CreateWall(44.73563, 3.656087, -19.84385, 1.611956, 2.038029, 1.555517, 5, 40);
                CreateWall(54.45702, 6.858232, -27.74011, 4.934025, 8.573491, 20.66022, 5, 41);
                CreateWall(44.79, 6.858232, -37.25, 15, 8.573491, 1, 5, 42);
                CreateWall(43.12514, 3.871293, -10.13021, 3.204861, 9.615976, 3.212919, 5, 43);
                CreateWall(43.12514, 3.871293, 9.217745, 3.204861, 9.615976, 3.228837, 5, 44);
                CreateWall(55.61611, 4.50092, -5.702986, 2.438217, 2.458159, 2.414028, 5, 45);
                CreateWall(55.15685, 5.471698, 5.165548, 6.338552, 6.415167, 19.35178, 5, 46);
                CreateWall(58.07824, 5.471698, 7.948711, 2.521341, 6.415167, 50.76546, 5, 47);
                CreateWall(56.02677, 4.719615, 15.67324, 1.615543, 1.62077, 1.627525, 5, 48);
                CreateWall(55.63088, 6.345802, 31.40095, 2.447762, 4.873144, 2.418945, 5, 49);
                CreateWall(47.67071, 6.720139, 36.50803, 13.9532, 8.840049, 7.817391, 5, 50);
                CreateWall(37.47268, 6.720139, 35.01363, 3.218556, 8.840049, 1.644658, 5, 51);
                CreateWall(38.30424, 6.720139, 36.66363, 4.881684, 8.840049, 1.644658, 5, 52);
                CreateWall(47.88821, -0.1699834, 41.53085, 8.275005, 4.7418, 1.703053, 5, 53);
                CreateWall(43.52215, 0.2628939, 31.01274, 0.8107128, 5.686295, 19.40301, 5, 54);
                CreateWall(44.72398, -1.73808, 39.10096, 1.634037, 1.618159, 3.295925, 5, 55);
                CreateWall(51.18998, -1.73808, 35.84166, 1.634037, 1.618159, 3.231308, 5, 56);
                CreateWall(51.18998, -0.1290795, 36.23938, 1.634037, 1.618159, 1.610428, 5, 57);
                CreateWall(52.32014, 0.8512607, 29.83927, 0.6802864, 6.955806, 20.27887, 5, 58);
                CreateWall(52.32679, 3.286222, 15.67575, 0.6669998, 2.08588, 1.603443, 5, 59);
                CreateWall(33.44603, 6.720139, 36.03182, 4.826495, 8.840049, 29.46829, 6, 60);
                CreateWall(22.95895, 6.720139, 21.99475, 3.240649, 8.840049, 24.45416, 6, 61);
                CreateWall(30.64663, 5.934331, 33.42, 0.9097431, 7.271662, 2.423, 6, 62);
                CreateWall(24.90663, 5.934331, 33.42, 0.9097431, 7.271662, 2.423, 6, 63);
                CreateWall(26.44053, 4.339704, 32.63532, 2.150795, 4.082407, 0.6293912, 6, 64);
                CreateWall(29.15415, 4.339704, 34.19246, 2.145238, 4.082407, 0.6865859, 6, 65);
                CreateWall(29.85332, 4.339704, 33.49086, 0.6353655, 4.082407, 0.3297293, 6, 66);
                CreateWall(25.68523, 4.339704, 33.37033, 0.6778869, 4.082407, 0.3050682, 6, 67);
                CreateWall(29.813, 3.51, 24.547, 2.423, 2.423, 2.423, 6, 68);
                CreateWall(24.90663, 5.934331, 22.13, 0.9097431, 7.271662, 2.423, 6, 69);
                CreateWall(30.64663, 5.934331, 22.13, 0.9097431, 7.271662, 2.423, 6, 70);
                CreateWall(26.37633, 6.720139, 57.01281, 10.12095, 8.840049, 13.31212, 6, 71);
                CreateWall(15.38498, 6.720139, 65.11506, 13.95091, 8.840049, 3.702241, 6, 72);
                CreateWall(-3.511163, 6.720139, 67.90365, 26.58, 8.840049, 2.839417, 0, 73);
                CreateWall(19.46931, 6.720139, 16.87856, 9.941291, 8.840049, 23.40178, -1, 74);
                CreateWall(11.42511, 6.720139, 16.45395, 6.092877, 8.840049, 22.55255, -1, 75);
                CreateWall(8.43699, 6.720139, 43.91084, 12.90911, 8.840049, 19.34007, -1, 76);
                CreateWall(15.2, 6.720139, 43.90626, 1, 8.840049, 7.272514, 6, 77);
                CreateWall(9.832866, 6.720139, 33.91, 10.07427, 8.840049, 1, 4, 78);
                CreateWall(3.994344, 3.108447, 33.42598, 1.615686, 1.604893, 1.623959, 4, 79);
                CreateWall(-5.272417, 5.538648, 25.75, 4.824834, 6.477295, 4, 4, 80);
                CreateWall(-5.272417, 5.538648, 36.227, 4.824834, 6.477295, 4, 4, 81);
                CreateWall(-6.90057, 6.730637, 46.3273, 11.26114, 8.861274, 20.97461, -1, 82);
                CreateWall(-7.831, 5.3, 30.99, 1, 6, 6.5, 4, 83);
                CreateWall(5.074815, 4.301765, 56.40557, 13.1601, 4.062151, 0.7663727, 4, 84);
                CreateWall(0.24, 7.568765, 47.13827, 3.5, 4.062151, 12.90346, 4, 85);
                CreateWall(-0.4394904, 4.314489, 43.90987, 1.612981, 4.03266, 3.21767, 4, 86);
                CreateWall(-11.73649, 7.54249, 61.2065, 1.612981, 4.03266, 12.21, 0, 87);
                CreateWall(15.65053, 3.52211, -1.713665, 4.895332, 2.395253, 5.782387, 3, 88);
                CreateWall(12.46407, 3.925133, 1.150944, 1.617191, 3.201298, 1.618112, 3, 89);
                CreateWall(5.992119, 3.914345, -0.4590559, 14.66269, 10.87, 1.618112, 3, 90);
                CreateWall(23.35082, 3.525312, 3.975177, 2.418831, 2.389922, 2.420465, 3, 91);
                CreateWall(23.75113, 6.332403, -9.739486, 1.617455, 3.222512, 1.611141, 3, 92);
                CreateWall(21.54216, 3.514502, -2.880072, 0.4025955, 2.410711, 3.235695, 3, 93);
                CreateWall(32.64186, 3.912763, 11.63979, 3.213718, 3.226475, 3.216652, 6, 94);
                CreateWall(35.05, 3.102, 10.828, 1.62, 1.62, 1.62, 5, 95);
                CreateWall(23.153, 3.307, -4.994, 2.82, 2, 1, 3, 96);
                CreateWall(8.033, 2.504, 0.538, 4.05, 0.4, 1.222159, 3, 97);
                CreateWall(18.112, 2.504, 28.781, 4.05, 0.4, 1.222159, 6, 98);
                CreateWall(15.9, 2.504, 43.898, 1.222159, 0.4, 4.05, 6, 99);
                CreateWall(51.25, -0.15, 40.724, 1.77, 4.7418, 1.703053, 5, 100);
                CreateWall(43.89105, 0.4453565, 40.36826, 1.712103, 3.551087, 0.9915719, 5, 101);
                CreateWall(56.639, 4.118, 26.56685, 1.222159, 0.4, 4.037703, 5, 102);
                CreateWall(-2.28019, 2.288647, 25.35139, 2.052381, 6.477295, 1.62215, 4, 103);
            }
            else if(Globals.selectedMap == 3)
            {
                CreateWall(0, -1.5, 0.3719978, 35.8, 1, 60, 0, 0);
                CreateWall(-16.44, 3.77, 0.3719978, 1, 10, 60, 0, 1);
                CreateWall(16.55, 3.77, 0.3719978, 1, 10, 60, 0, 2);
                CreateWall(0, 3.77, -29.248, 33, 10, 1, 0, 3);
                CreateWall(0, 3.77, 29.592, 33, 10, 1, 0, 4);
                CreateWall(-1.4, 1.24, 23.492, 30.5, 5.5, 0.5, 0, 5);
                CreateWall(-1.4, 0.35, 26.372, 30.5, 5.5, 6, 0, 6);
                CreateWall(1.44, 1.24, -23.128, 30.5, 5.5, 0.5, 0, 7);
                CreateWall(1.44, 0.35, -26.188, 30.5, 5.5, 6, 0, 8);
                CreateWall(-14.89, 0.35, -26.353, 2.6, 5.5, 6.17, 0, 9);
                CreateWall(-13.61, -0.497, -20.141, 0.4, 1, 6.51, 0, 10);
                CreateWall(-13.61, 0.53, -21.564, 0.4, 1, 6.51, 0, 11);
                CreateWall(-13.61, 1.56, -23.065, 0.4, 1, 6.51, 0, 12);
                CreateWall(-13.61, 2.616, -22.264, 0.4, 1, 1.74, 0, 13);
                CreateWall(14.99, 0.35, 26.537, 2.76, 5.5, 5.87, 0, 14);
                CreateWall(13.66, -0.54, 20.471, 0.4, 1, 6.51, 0, 15);
                CreateWall(13.66, 0.51, 21.93, 0.4, 1, 6.51, 0, 16);
                CreateWall(13.66, 1.55, 23.46, 0.4, 1, 6.51, 0, 17);
                CreateWall(13.66, 2.597, 22.62, 0.4, 1, 1.77, 0, 18);
                CreateWall(7.937, 3.985, 24.576, 1.8, 1.8, 1.8, 0, 19);
                CreateWall(-9.906, 3.985, 24.576, 1.8, 1.8, 1.8, 0, 20);
                CreateWall(9.952, 3.985, -24.247, 1.8, 1.8, 1.8, 0, 21);
                CreateWall(-7.897, 3.985, -24.245, 1.8, 1.8, 1.8, 0, 22);
                CreateWall(-10.224, -0.042, -14.01, 1.8, 1.9, 1.8, 0, 23);
                CreateWall(-5.55, -0.042, -12.903, 1.8, 1.9, 1.8, 0, 24);
                CreateWall(4.21, -0.13, -13.924, 1.8, 1.8, 1.8, 0, 25);
                CreateWall(2.066, 0.24, -12.954, 2.54, 2.44, 2.14, 0, 26);
                CreateWall(-3.735, 0.826, -11.866, 1.81, 3.56, 1.8, 0, 27);
                CreateWall(-11.989, 0.826, -13.02, 1.81, 3.56, 1.8, 0, 28);
                CreateWall(-2.43, -0.371, -13.31, 1.38, 1.37, 1.15, 0, 29);
                CreateWall(-8.898, -0.5, -14.659, 0.86, 1, 0.9, 0, 30);
                CreateWall(3.79, 0.826, 12.198, 1.81, 3.56, 1.8, 0, 31);
                CreateWall(12.04, 0.826, 13.337, 1.81, 3.56, 1.8, 0, 32);
                CreateWall(-2.008, 0.24, 13.278, 2.54, 2.44, 2.14, 0, 33);
                CreateWall(-4.159, -0.184, 14.257, 1.8, 1.9, 1.8, 0, 34);
                CreateWall(5.58, -0.042, 13.229, 1.8, 1.9, 1.8, 0, 35);
                CreateWall(10.264, -0.042, 14.343, 1.8, 1.9, 1.8, 0, 36);
                CreateWall(8.96, -0.521, 14.982, 0.86, 1, 0.9, 0, 37);
                CreateWall(2.469, -0.343, 13.647, 1.33, 1.32, 1.13, 0, 38);
                CreateWall(16.24, 3.429, 2.05, 2.87, 8.82, 0.9, 0, 39);
                CreateWall(16.22, 3.429, 2.76, 1.52, 8.82, 0.9, 0, 40);
                CreateWall(16.24, 3.429, -2.42, 2.87, 8.82, 0.9, 0, 41);
                CreateWall(16.22, 3.429, -3.13, 1.52, 8.82, 0.9, 0, 42);
                CreateWall(16.08, 3.429, -0.17, 1.52, 8.82, 3.8, 0, 43);
                CreateWall(16.3, 5.28, -0.15, 2.87, 5.62, 3.61, 0, 44);
                CreateWall(-16.29, 3.429, 2.031, 2.87, 8.82, 0.9, 0, 45);
                CreateWall(-16.29, 3.429, -2.44, 2.87, 8.82, 0.9, 0, 46);
                CreateWall(-16.28, 3.429, 2.741, 1.52, 8.82, 0.9, 0, 47);
                CreateWall(-16.28, 3.429, -3.149, 1.52, 8.82, 0.9, 0, 48);
                CreateWall(-16.06, 3.429, -0.13, 1.52, 8.82, 3.7, 0, 49);
                CreateWall(-16.29, 5.28, -0.21, 2.87, 5.62, 3.61, 0, 50);
                CreateWall(16.45, 4.85, 9.15, 1.4, 5.62, 4.1, 0, 51);
                CreateWall(16.45, 4.85, -9.420001, 1.4, 5.62, 4.1, 0, 52);
                CreateWall(-16.38, 4.85, 9.09, 1.4, 5.62, 4.1, 0, 53);
                CreateWall(-16.38, 4.85, -9.48, 1.4, 5.62, 4.1, 0, 54);
                CreateWall(-10.725, -0.872, 20.25, 10.98, 1.1, 5.96, 0, 55);
                CreateWall(-12.68, -0.872, 15, 10.33, 1.1, 5.6, 0, 56);
                CreateWall(-13.58, -0.872, 10.41, 6.48, 1.1, 4.29, 0, 57);
                CreateWall(-5.109, 0.01300001, 22.286, 0.5, 1.1, 2.03, 0, 58);
                CreateWall(-7.56, 0.01300001, 14.786, 0.5, 1.1, 5.29, 0, 59);
                CreateWall(-10.47, 0.01300001, 10.334, 0.5, 1.1, 4.46, 0, 60);
                CreateWall(-6.096, 0.01300001, 17.25, 2.31, 1.1, 0.5, 0, 61);
                CreateWall(-9.027, 0.01300001, 12.339, 2.29, 1.1, 0.5, 0, 62);
                CreateWall(-13.81, 0.01300001, 8.322, 6.04, 1.1, 0.5, 0, 63);
                CreateWall(10.795, -0.872, -19.86, 10.98, 1.1, 5.96, 0, 64);
                CreateWall(11.89, -0.872, -14.51, 8.6, 1.1, 5.13, 0, 65);
                CreateWall(13.56, -0.872, -10.15, 6.15, 1.1, 4.3, 0, 66);
                CreateWall(5.149, 0.01300001, -22.004, 0.5, 1.1, 2.17, 0, 67);
                CreateWall(7.606, 0.01300001, -14.453, 0.5, 1.1, 5.29, 0, 68);
                CreateWall(10.502, 0.01300001, -10.006, 0.5, 1.1, 4.42, 0, 69);
                CreateWall(13.475, 0.01300001, -7.994, 5.41, 1.1, 0.5, 0, 70);
                CreateWall(9.036, 0.01300001, -12.009, 2.24, 1.1, 0.5, 0, 71);
                CreateWall(6.073, 0.01300001, -16.913, 2.4, 1.1, 0.5, 0, 72);
                CreateWall(7.111, 0.353, -17.671, 1.39, 1.31, 1.14, 0, 73);
                CreateWall(8.674, 0.544, -13.088, 1.79, 1.82, 1.81, 0, 74);
                CreateWall(11.584, 0.544, -9.076, 1.79, 1.82, 1.81, 0, 75);
                CreateWall(15.08, 0.811, -19.223, 1.99, 2.18, 2.01, 0, 76);
                CreateWall(12.373, 0.436, -22.116, 1.59, 1.6, 1.57, 0, 77);
                CreateWall(12.97, 0.175, -8.692, 0.99, 1.01, 1, 0, 78);
                CreateWall(14.68, 1.42, -21.56, 3.03, 3.64, 2.66, 0, 79);
                CreateWall(-14.72, 1.42, 21.96, 3.03, 3.64, 2.66, 0, 80);
                CreateWall(-13.027, 0.175, 9.08, 0.99, 1.01, 1, 0, 81);
                CreateWall(-7.153, 0.353, 18.049, 1.39, 1.31, 1.14, 0, 82);
                CreateWall(-8.728, 0.544, 13.485, 1.79, 1.82, 1.81, 0, 83);
                CreateWall(-11.626, 0.544, 9.467, 1.79, 1.82, 1.81, 0, 84);
                CreateWall(-15.167, 0.8099999, 19.62, 2.09, 2.2, 2.03, 0, 85);
                CreateWall(-12.415, 0.438, 22.515, 1.58, 1.6, 1.59, 0, 86);
            }
            else if(Globals.selectedMap == 4)
            {
                CreateWall(-0.1639996, -1.6, 0.3489971, 44, 1, 50, 0, 0);
                CreateWall(-0.1639996, 0.76, -21.98, 42.6, 3.16, 2, 0, 1);
                CreateWall(-0.1639996, 3.94, -23.81, 43, 3.16, 2, 0, 2);
                CreateWall(-0.1639996, 0.76, 22.2, 42.6, 3.16, 2, 0, 3);
                CreateWall(-0.1639996, 3.94, 23.99, 43, 3.16, 2, 0, 4);
                CreateWall(-20.82, 3.1, 0.1269993, 1, 8, 47, 0, 5);
                CreateWall(20.63, 3.1, 0.1269993, 1, 8, 47, 0, 6);
                CreateWall(-0.1639996, 7.41, -23.013, 43, 3.16, 2, 0, 7);
                CreateWall(-0.1639996, 7.41, 23.217, 43, 3.16, 2, 0, 8);
                CreateWall(-17.88, 0.5779999, 13.99, 5.2, 3, 1, 0, 9);
                CreateWall(-17.88, 0.5779999, 4.34, 5.2, 3, 1, 0, 10);
                CreateWall(-17.88, 0.5779999, -4.13, 5.2, 3, 1, 0, 11);
                CreateWall(-17.88, 0.5779999, -13.8, 5.2, 3, 1, 0, 12);
                CreateWall(17.7, 0.5779999, -13.8, 5.2, 3, 1, 0, 13);
                CreateWall(17.7, 0.5779999, -4.12, 5.2, 3, 1, 0, 14);
                CreateWall(17.7, 0.5779999, 4.34, 5.2, 3, 1, 0, 15);
                CreateWall(17.7, 0.5779999, 13.99, 5.2, 3, 1, 0, 16);
                CreateWall(-0.08, 3.04, 8.58, 20.35, 8, 5.08, 0, 17);
                CreateWall(-0.1, 3.04, -8.39, 20.35, 8, 5.08, 0, 18);
                CreateWall(-6.4, 3.04, 0.105, 7.8, 8, 5.1, 0, 19);
                CreateWall(6.32, 3.04, 0.105, 7.6, 8, 5.1, 0, 20);
                CreateWall(-0.1639996, 7.79, 0.3489971, 40, 1, 45.5, 0, 21);
                CreateWall(-19.474, 0.76, 20.512, 1.87, 3.16, 1.6, 0, 22);
                CreateWall(19.329, 0.76, 20.51, 1.87, 3.16, 1.6, 0, 23);
                CreateWall(19.329, 0.76, -20.21, 1.87, 3.16, 1.68, 0, 24);
                CreateWall(-19.476, 0.76, -20.21, 1.87, 3.16, 1.68, 0, 25);
                CreateWall(-20.009, 2.89, -13.773, 0.9, 0.5, 0.65, 0, 26);
                CreateWall(19.841, 2.886, -13.787, 0.9, 0.5, 0.65, 0, 27);
                CreateWall(-20.035, 2.89, 13.977, 0.9, 0.5, 0.65, 0, 28);
                CreateWall(19.841, 2.886, 13.977, 0.9, 0.5, 0.65, 0, 29);
                CreateWall(-16.4, -0.49, -20.21, 3.5, 1, 1.68, 0, 30);
                CreateWall(-17.35, 0.521, -20.21, 1.7, 1, 1.68, 0, 31);
                CreateWall(-18.158, 1.539, -20.21, 0.2, 1, 1.68, 0, 32);
                CreateWall(-16.53, -0.49, 20.544, 3.5, 1, 1.68, 0, 33);
                CreateWall(-17.31, 0.521, 20.544, 1.7, 1, 1.68, 0, 34);
                CreateWall(-18.108, 1.539, 20.544, 0.2, 1, 1.68, 0, 35);
                CreateWall(16.36, -0.49, 20.544, 3.5, 1, 1.68, 0, 36);
                CreateWall(17.17, 0.521, 20.544, 1.7, 1, 1.68, 0, 37);
                CreateWall(17.966, 1.539, 20.544, 0.2, 1, 1.68, 0, 38);
                CreateWall(16.26, -0.49, -20.21, 3.5, 1, 1.68, 0, 39);
                CreateWall(17.04, 0.521, -20.21, 1.7, 1, 1.68, 0, 40);
                CreateWall(17.979, 1.539, -20.21, 0.2, 1, 1.68, 0, 41);
            }
        }

        public void AddAllStairs()
        {
            if(Globals.selectedMap == 0)
            {
                CreateStairs(-1.257, 5.2, 8.419, 21.325, 0, 3.211, 2, 0);
                CreateStairs(-1.252, -0.85, 3.576, 6.8, 0, 0.388, 1, 1);
                CreateStairs(5.3, 6.45, 21.313, 56.32, 3.24, 3.24, 1, 2);
                CreateStairs(41.98, 45.06, 10.7, 21.311, 3.24, 3.24, 2, 3);
                CreateStairs(24.558, 37.4724, -20.03, -10.9489, 0, 3.2112, 3, 4);
                CreateStairs(42.87, 44.44, -17.4, -11.755, 3.24, 3.24, 2, 5);
                CreateStairs(50.37, 56.8304, -17.4011, -4.494, 3.237, 4.8242, 3, 6);
                CreateStairs(43.93, 51.99, -30.31, -17.403, 3.228, 6.451, 0, 7);
                CreateStairs(43.93, 51.99, 21.315, 34.231, -1.613, 3.217, 0, 8);
                CreateStairs(49.58, 52.804, 16.485, 19.704, 3.228, 4.829, 3, 9);
                CreateStairs(18.111, 21.33, -4.495, 0.33, 3.237, 5.64, 0, 10);
                CreateStairs(37.48, 43.51, -30.31, -28.293, 5.647, 6.46, 0, 11);
                CreateStairs(37.13, 38.37, -28.69, -20.22, 5.632, 5.632, 2, 12);
                CreateStairs(18.89, 24.98, -20.98, -19.1, 5.632, 5.632, 3, 13);
                CreateStairs(-17.385, -7.699999, -26.87, -10.9489, 0, 3.211, 1, 14);
                CreateStairs(-38.35, -35.131, -20.628, -19.037, 3.237, 4.026, 0, 15);
                CreateStairs(-39.974, -36.749, -6.12, -4.495998, 3.237, 4.026, 2, 16);
                CreateStairs(-34.28, -33.04, 4.909999, 13.38, 4.02, 4.02, 2, 17);
                CreateStairs(-32.48, -28.09, 15.51, 17.13, 4.02, 4.02, 3, 18);
                CreateStairs(-39.972, -23.841, 26.966, 28.46, 3.228, 4.019, 0, 19);
                CreateStairs(-39.97, -33.52, 43.904, 56.818, 3.23, 6.449, 2, 20);
                CreateStairs(-1.248, 11.659, 56.79, 66.49001, 3.227, 6.441, 1, 21);
                CreateStairs(-33.96, -32.95, 56.824, 63.264, 6.44, 6.44, 0, 22);
                CreateStairs(-40.67, -39.49, 56.824, 63.264, 6.44, 6.44, 0, 23);
                CreateStairs(-13.74, -12.56, 56.824, 63.264, 6.44, 6.44, 0, 24);
                CreateStairs(-44.813, -12.546, 63.2688, 66.496, 6.454, 7.255, 2, 25);
                CreateStairs(-17.383, -14.166, 8.419, 11.642, 0.4100001, 2.006, 2, 26);
                CreateStairs(-20.3, -17.214, 11.644, 14.868, 2.073, 4.018, 1, 27);
                CreateStairs(-20.585, -18.3, -25.084, -23.196, 6.058, 6.893, 0, 28);
                CreateStairs(-22.7, -22.23, -27.867, -26.293, 6.457, 6.843, 3, 29);
                CreateStairs(-23.845, -22.226, -26.263, -25.927, 6.052, 6.439, 0, 30);
            }
            else if(Globals.selectedMap == 1)
            {
                CreateStairs(-11.344, 11.336, -3.7246, -3.15, -0.071, 0.471, 0, 0);
            }
            else if(Globals.selectedMap == 2)
            {
                CreateStairs(-1.257, 5.2, 8.419, 21.325, 0, 3.211, 2, 0);
                CreateStairs(5.3, 6.45, 21.313, 56.32, 3.24, 3.24, 1, 1);
                CreateStairs(41.98, 45.06, 10.7, 21.311, 3.24, 3.24, 2, 2);
                CreateStairs(24.558, 37.4724, -20.03, -10.9489, 0, 3.2112, 3, 3);
                CreateStairs(42.87, 44.44, -17.4, -11.755, 3.24, 3.24, 2, 4);
                CreateStairs(50.37, 56.8304, -17.4011, -4.494, 3.237, 4.8242, 3, 5);
                CreateStairs(43.93, 51.99, -30.31, -17.403, 3.228, 6.451, 0, 6);
                CreateStairs(43.93, 51.99, 21.315, 34.231, -1.613, 3.217, 0, 7);
                CreateStairs(49.58, 52.804, 16.485, 19.704, 3.228, 4.829, 3, 8);
                CreateStairs(18.111, 21.33, -4.495, 0.33, 3.237, 5.64, 0, 9);
                CreateStairs(37.48, 43.51, -30.31, -28.293, 5.647, 6.46, 0, 10);
                CreateStairs(37.13, 38.37, -28.69, -20.22, 5.632, 5.632, 2, 11);
                CreateStairs(18.89, 24.98, -20.98, -19.1, 5.632, 5.632, 3, 12);
                CreateStairs(-1.248, 11.659, 56.79, 66.49001, 3.227, 6.441, 1, 13);
            }
            else if(Globals.selectedMap == 3)
            {
                CreateStairs(-16.2, -13.593, -22.929, -16.86, -0.071, 4.013, 0, 0);
                CreateStairs(13.602, 16.37, 17.217, 23.266, -0.071, 4.01, 2, 1);
                CreateStairs(-4.9, -3.949999, 17.273, 23.23, -0.073, 0.5920001, 1, 2);
                CreateStairs(3.78, 4.97, -22.845, -16.883, -0.061, 0.5910001, 3, 3);
                CreateStairs(-15.324, -14.037, -1.96, 1.600002, -0.073, 0.358, 0, 4);
                CreateStairs(13.981, 15.23, -1.940001, 1.6, -0.06, 0.3430001, 2, 5);
            }
            else if(Globals.selectedMap == 4)
            {
                CreateStairs(-18.233, -12.459, 19.726, 21.343, -0.073, 3.303, 1, 0);
                CreateStairs(12.289, 18.039, -21.078, -19.386, -0.07300002, 3.281, 3, 1);
                CreateStairs(-18.23, -12.492, -21.078, -19.386, -0.073, 3.277, 1, 2);
                CreateStairs(12.25, 18.043, 19.73, 21.34, -0.07300002, 3.298, 3, 3);
            }
        }

        public List<Stairs> AllStairsRef = new();

        public void CreateStairs(double xSideA, double xSideB, double zSideA, double zSideB, double startY, double endY, int direction, int index)
        {
            Stairs newStairs = new()
            {
                xSideA = (float)xSideA,
                xSideB = (float)xSideB,
                zSideA = -(float)zSideB,
                zSideB = -(float)zSideA,
                startY = (float)startY,
                endY = (float)endY,
                direction = direction
            };
            AllStairsRef.Add(newStairs);
        }

        public bool CheckStairsForGrenades(NE_Physics grenade)
        {
            for (int i = 0; i < AllStairsRef.Count; i++)
            {
                float xpos = grenade.x / 8192f;
                float ypos = grenade.y / 8192f;
                float zpos = grenade.z / 8192f;

                if (zpos >= AllStairsRef[i].zSideA && zpos <= AllStairsRef[i].zSideB && xpos >= AllStairsRef[i].xSideA && xpos <= AllStairsRef[i].xSideB)
                {
                    float yVal;
                    if (AllStairsRef[i].direction == 0)
                    {
                        yVal = Program.Map(zpos, AllStairsRef[i].zSideA, AllStairsRef[i].zSideB, AllStairsRef[i].endY, AllStairsRef[i].startY);
                    }
                    else if (AllStairsRef[i].direction == 1)
                    {
                        yVal = Program.Map(xpos, AllStairsRef[i].xSideA, AllStairsRef[i].xSideB, AllStairsRef[i].endY, AllStairsRef[i].startY);
                    }
                    else if (AllStairsRef[i].direction == 2)
                    {
                        yVal = Program.Map(zpos, AllStairsRef[i].zSideA, AllStairsRef[i].zSideB, AllStairsRef[i].startY, AllStairsRef[i].endY);
                    }
                    else
                    {
                        yVal = Program.Map(xpos, AllStairsRef[i].xSideA, AllStairsRef[i].xSideB, AllStairsRef[i].startY, AllStairsRef[i].endY);
                    }

                    if (ypos < yVal && yVal - ypos < 3)
                    {
                        if (grenade.yspeed < 100)
                            grenade.yspeed = 0;

                        grenade.x = (int)(xpos * 8192);
                        grenade.y = (int)((yVal - 0.6) * 8192);
                        grenade.z = (int)(zpos * 8192);

                        if (AllStairsRef[i].startY != AllStairsRef[i].endY)
                        {
                            if (AllStairsRef[i].direction == 0)
                            {
                                grenade.zspeed += 15;
                            }
                            else if (AllStairsRef[i].direction == 1)
                            {
                                grenade.xspeed += 15;
                            }
                            else if (AllStairsRef[i].direction == 2)
                            {
                                grenade.zspeed -= 15;
                            }
                            else
                            {
                                grenade.xspeed -= 15;
                            }
                        }
                    }

                    return true;
                }
            }
            return false;
        }
    }
}
