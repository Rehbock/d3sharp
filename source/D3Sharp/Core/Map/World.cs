/*
 * Copyright (C) 2011 D3Sharp Project
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using D3Sharp.Net.Game;
using D3Sharp.Core.Map;
using D3Sharp.Core.Actors;
using D3Sharp.Net.Game.Message.Definitions.ACD;
using D3Sharp.Net.Game.Message.Definitions.Map;
using D3Sharp.Net.Game.Message.Definitions.Scene;
using D3Sharp.Net.Game.Message.Definitions.World;
using D3Sharp.Net.Game.Message.Fields;
using D3Sharp.Net.Game.Messages;
using D3Sharp.Utils;
using D3Sharp.Utils.Extensions;
using D3Sharp.Core.NPC;
using D3Sharp.Core.Actors;
using D3Sharp.Core.Toons;
using D3Sharp.Net.Game.Message.Definitions.Misc;
using D3Sharp.Net.Game.Message.Definitions.Animation;
using D3Sharp.Net.Game.Message.Definitions.Attribute;

namespace D3Sharp.Core.Map
{
    public class World
    {
        static readonly Logger Logger = LogManager.CreateLogger();
        private List<BasicNPC> NPCs;
        private List<Actor> Actors;
        private List<Scene> Scenes;
        private Random rand = new Random();

        public List<EncounterPoint> encounters = new List<EncounterPoint>();
        private int tick = 0;

        public int WorldID;
        public int WorldSNO;

        public World(int ID)
        {
            NPCs = new List<BasicNPC>();
            Actors = new List<Actors.Actor>();
            Scenes = new List<Scene>();
            WorldID = ID;
        }

        public Scene GetScene(int ID)
        {
            for (int x = 0; x < Scenes.Count; x++)
                if (Scenes[x].ID == ID) return Scenes[x];
            return null;
        }

        public Actor GetActor(int ID)
        {
            for (int x = 0; x < Actors.Count; x++)
                if (Actors[x].ID == ID) return Actors[x];
            return null;
        }

        private void CreateEncounter(GameClient client, Type type, int encounterSize, float x, float y, float z)
        {
            EncounterPoint encounter = new EncounterPoint(this, client, type, encounterSize, new Vector3D(x, y, z));
            encounters.Add(encounter);
        }

        public void SendEncounters(GameClient client)
        {
            CreateEncounter(client, typeof(MonsterSkeleton), EncounterPoint.MedEncounter, 2504.14624f, 2864.31787f, 27.1f);
            CreateEncounter(client, typeof(MonsterSkeleton), EncounterPoint.MedEncounter, 2477.483f, 2835.02954f, 27.1000023f);
        }

        public BasicNPC SpawnNPC(Type type)
        {
            BasicNPC npc = (BasicNPC)Activator.CreateInstance(type);

            NPCs.Add(npc);

            return npc;
        }

        public void SpawnMob(GameClient Game, Vector3D position, Monster monster)
        {
            int nId = monster.SpawnID;
            int objectId = monster.ID;

            #region ACDEnterKnown Hittable Zombie
            Game.SendMessage(new ACDEnterKnownMessage()
            {
                Id = 0x003B,
                Field0 = objectId,
                Field1 = nId,
                Field2 = 0x8,
                Field3 = 0x0,
                Field4 = new WorldLocationMessageData()
                {
                    Field0 = 1.35f,
                    Field1 = new PRTransform()
                    {
                        Field0 = new Quaternion()
                        {
                            Field0 = 0.768145f,
                            Field1 = new Vector3D()
                            {
                                Field0 = 0f,
                                Field1 = 0f,
                                Field2 = -0.640276f,
                            },
                        },
                        Field1 = new Vector3D()
                        {
                            Field0 = position.Field0 + 5,
                            Field1 = position.Field1 + 5,
                            Field2 = position.Field2,
                        },
                    },
                    Field2 = 0x772E0000,
                },
                Field5 = null,
                Field6 = new GBHandle()
                {
                    Field0 = 1,
                    Field1 = 1,
                },
                Field7 = 0x00000001,
                Field8 = nId,
                Field9 = 0x0,
                Field10 = 0x0,
                Field11 = 0x0,
                Field12 = 0x0,
                Field13 = 0x0
            });
            Game.SendMessage(new AffixMessage()
            {
                Id = 0x48,
                Field0 = objectId,
                Field1 = 0x1,
                aAffixGBIDs = new int[0]
            });
            Game.SendMessage(new AffixMessage()
            {
                Id = 0x48,
                Field0 = objectId,
                Field1 = 0x2,
                aAffixGBIDs = new int[0]
            });
            Game.SendMessage(new ACDCollFlagsMessage
            {
                Id = 0xa6,
                Field0 = objectId,
                Field1 = 0x1
            });

            Game.SendMessage(new AttributesSetValuesMessage
            {
                Id = 0x4d,
                Field0 = objectId,
                atKeyVals = new NetAttributeKeyValue[15] {
                    new NetAttributeKeyValue {
                        Attribute = GameAttribute.Attributes[214],
                        Int = 0
                    },
                    new NetAttributeKeyValue {
                        Attribute = GameAttribute.Attributes[464],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Field0 = 1048575,
                        Attribute = GameAttribute.Attributes[441],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Field0 = 30582,
                        Attribute = GameAttribute.Attributes[560],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Field0 = 30286,
                        Attribute = GameAttribute.Attributes[560],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Field0 = 30285,
                        Attribute = GameAttribute.Attributes[560],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Field0 = 30284,
                        Attribute = GameAttribute.Attributes[560],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Field0 = 30283,
                        Attribute = GameAttribute.Attributes[560],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Field0 = 30290,
                        Attribute = GameAttribute.Attributes[560],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Field0 = 79486,
                        Attribute = GameAttribute.Attributes[560],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Field0 = 30286,
                        Attribute = GameAttribute.Attributes[460],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Field0 = 30285,
                        Attribute = GameAttribute.Attributes[460],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Field0 = 30284,
                        Attribute = GameAttribute.Attributes[460],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Field0 = 30283,
                        Attribute = GameAttribute.Attributes[460],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Field0 = 30290,
                        Attribute = GameAttribute.Attributes[460],
                        Int = 1
                    }
                }

            });

            Game.SendMessage(new AttributesSetValuesMessage
            {
                Id = 0x4d,
                Field0 = objectId,
                atKeyVals = new NetAttributeKeyValue[9] {
                    new NetAttributeKeyValue {
                        Attribute = GameAttribute.Attributes[86],
<<<<<<< HEAD
                        Float = 100.0f
=======
                        Float = 4.546875f
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
                    },
                    new NetAttributeKeyValue {
                        Field0 = 79486,
                        Attribute = GameAttribute.Attributes[460],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Attribute = GameAttribute.Attributes[84],
<<<<<<< HEAD
                        Float = 100.0f
=======
                        Float = 4.546875f
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
                    },
                    new NetAttributeKeyValue {
                        Attribute = GameAttribute.Attributes[81],
                        Int = 0
                    },
                    new NetAttributeKeyValue {
                        Attribute = GameAttribute.Attributes[77],
<<<<<<< HEAD
                        Float = 100.0f
=======
                        Float = 4.546875f
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
                    },
                    new NetAttributeKeyValue {
                        Attribute = GameAttribute.Attributes[69],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Field0 = 30582,
                        Attribute = GameAttribute.Attributes[460],
                        Int = 1
                    },
                    new NetAttributeKeyValue {
                        Attribute = GameAttribute.Attributes[67],
                        Int = 10
                    },
                    new NetAttributeKeyValue {
                        Attribute = GameAttribute.Attributes[38],
                        Int = 1
                    }
                }

            });


            Game.SendMessage(new ACDGroupMessage
            {
                Id = 0xb8,
                Field0 = objectId,
                Field1 = unchecked((int)0xb59b8de4),
                Field2 = unchecked((int)0xffffffff)
            });

            Game.SendMessage(new ANNDataMessage
            {
                Id = 0x3e,
                Field0 = objectId
            });

            Game.SendMessage(new ACDTranslateFacingMessage
            {
                Id = 0x70,
                Field0 = objectId,
                Field1 = (float)(rand.NextDouble() * 2.0 * Math.PI),
                Field2 = false
            });

            Game.SendMessage(new SetIdleAnimationMessage
            {
                Id = 0xa5,
                Field0 = objectId,
                Field1 = 0x11150
            });

            Game.SendMessage(new SNONameDataMessage
            {
                Id = 0xd3,
                Field0 = new SNOName
                {
                    Field0 = 0x1,
                    Field1 = nId
                }
            });
            #endregion
            /*
            _client.packetId += 30 * 2;
            Game.SendMessage(new DWordDataMessage()
            {
                Id = 0x89,
                Field0 = _client.packetId,
            });

            _client.tick += 20;
            Game.SendMessage(new EndOfTickMessage()
            {
                Id = 0x008D,
                Field0 = _client.tick - 20,
                Field1 = _client.tick
            });
             * */
        }

        public void Tick(GameClient client)
        {
<<<<<<< HEAD
            bool updateTick = false;
            if (NPCs == null)
                return;

=======
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
            foreach (BasicNPC npc in NPCs)
            {
                if (tick >= npc.nextThink)
                {
<<<<<<< HEAD
                    updateTick = true;
                    npc.Tick();
                }
            }

            if (updateTick)
            {
                
=======
                    npc.Tick();



                }
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
            }

            tick++;
        }

        public bool InWorld()
        {
<<<<<<< HEAD
            if (encounters == null)
                return false;

=======
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
            return encounters.Count > 0;
        }

        //get actor by snoid and x,y,z position - this is a helper function to prune duplicate entries from the packet data
        public Actor GetActor(int snoID, float x, float y, float z)
        {
            for (int i = 0; i < Actors.Count; i++)
                if (Actors[i].snoID == snoID &&
                    Actors[i].PosX==x &&
                    Actors[i].PosY==y &&
                    Actors[i].PosZ==z) return Actors[i];
            return null;
        }

        public void AddScene(string Line)
        {
            string[] data = Line.Split(' ');
            int SceneID=int.Parse(data[46]);

            Scene s = GetScene(SceneID);
            if (s != null) return;

            s = new Scene();
            s.ID = SceneID;

            s.SceneLine = Line;
            s.SceneData = new RevealSceneMessage(data.Skip(2).ToArray(),WorldID);

            Scenes.Add(s);
        }

        public void AddMapScene(string Line)
        {
            string[] data = Line.Split(' ');
            int SceneID = int.Parse(data[2]);

            Scene s = GetScene(SceneID);

            if (s==null) return;

            s.MapLine = Line;
            s.Map = new MapRevealSceneMessage(data.Skip(2).ToArray(), WorldID);
        }

        public void AddActor(string Line)
        {
            string[] data = Line.Split(' ');

            //skip inventory using items as their use is unknown
            if (int.Parse(data[2]) == 0) return;

            int ActorID = int.Parse(data[4]);
            int snoID = int.Parse(data[5]);

            float x, y, z;
            x = float.Parse(data[13], System.Globalization.CultureInfo.InvariantCulture);
            y = float.Parse(data[14], System.Globalization.CultureInfo.InvariantCulture);
            z = float.Parse(data[15], System.Globalization.CultureInfo.InvariantCulture);

            Actor a = GetActor(snoID,x,y,z);
            if (a != null) return;

            a = new Actor();
            a.ID = ActorID;
            a.snoID = snoID;
            a.RevealMessage = new ACDEnterKnownMessage(data.Skip(2).ToArray(),WorldID);
            a.PosX = x;
            a.PosY = y;
            a.PosZ = z;

            Actors.Add(a);
        }

        private int SceneSorter(Scene x, Scene y)
        {
            if (x.SceneData.WorldID != y.SceneData.WorldID) return x.SceneData.WorldID - y.SceneData.WorldID;
            if (x.SceneData.ParentChunkID != y.SceneData.ParentChunkID) return x.SceneData.ParentChunkID - y.SceneData.ParentChunkID;
            return 0;
        }

        public void SortScenes()
        {   
            //this makes sure no scene is referenced before it is revealed to a player
            Scenes.Sort(SceneSorter);
        }

        public void RevealWorld(Toon t)
        {
            //reveal world to player
            t.Owner.LoggedInBNetClient.InGameClient.SendMessage(new RevealWorldMessage()
            {
                Id = 0x0037,
                Field0 = WorldID,
                Field1 = WorldSNO,
            });

            //player enters world
            t.Owner.LoggedInBNetClient.InGameClient.SendMessage(new EnterWorldMessage()
            {
                Id = 0x0033,
                Field0 = new Vector3D()
                {
                    Field0 = t.PosX,
                    Field1 = t.PosY,
                    Field2 = t.PosZ
                },
                Field1 = WorldID,
                Field2 = WorldSNO,
            });

            //just reveal the whole thing to the player for now
            foreach (Scene s in Scenes)
                s.Reveal(t);

            //reveal actors
            foreach (Actor a in Actors)
            {
                if (ActorDB.isBlackListed(a.snoID)) continue;
                if (ActorDB.isNPC(a.snoID)) continue;
                a.Reveal(t);
            }
        }

        void CreateNPC(int objectId)
        {
            //NPCs.Add(new BasicNPC(objectId, ref Client));
        }

        
    }
}
