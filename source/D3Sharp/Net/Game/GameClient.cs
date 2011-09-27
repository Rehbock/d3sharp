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
using System.Linq;
using System.Collections.Generic;
using D3Sharp.Core.Toons;
using D3Sharp.Net.BNet;
using D3Sharp.Net.Game.Message.Definitions.ACD;
using D3Sharp.Net.Game.Message.Definitions.Animation;
using D3Sharp.Net.Game.Message.Definitions.Attribute;
using D3Sharp.Net.Game.Message.Definitions.Map;
using D3Sharp.Net.Game.Message.Definitions.Misc;
using D3Sharp.Net.Game.Message.Definitions.Player;
using D3Sharp.Net.Game.Message.Definitions.Scene;
using D3Sharp.Net.Game.Message.Definitions.World;
using D3Sharp.Net.Game.Message.Fields;
using D3Sharp.Net.Game.Messages;
using D3Sharp.Utils;
using D3Sharp.Core.Map;
using D3Sharp.Core.Skills;
using D3Sharp.Core.NPC;
using D3Sharp.Core.Universe;
using D3Sharp.Core.Helpers;

//using Gibbed.Helpers;

namespace D3Sharp.Net.Game
{
    public sealed class GameClient : IGameClient
    {
        static readonly Logger Logger = LogManager.CreateLogger();

        public IConnection Connection { get; set; }
        public BNetClient BnetClient { get; set; }

        GameBitBuffer _incomingBuffer = new GameBitBuffer(512);
        GameBitBuffer _outgoingBuffer = new GameBitBuffer(ushort.MaxValue);

        //this is the main universe reference to handle most of the player-game interactions and manage game state
        public Universe GameUniverse;
        
        public int packetId = 0x227 + 20;
        public int tick = (int)0x0000009F;
        public  int objectId = 0x78f50114 + 100;

        public IList<int> objectIdsSpawned = null;
        public Vector3D position;

        public Toon Toon;

        public bool IsLoggingOut;

        public int Experience = 1200;


        public GameClient(IConnection connection, Universe GU)
        {
            this.Connection = connection;
            _outgoingBuffer.WriteInt(32, 0);
            GameUniverse = GU;
        }

        public void UpdateExperience(int exp)
        {
            Experience -= exp;

            SendMessage(new AttributeSetValueMessage()
            {
                Id = 0x4c,
                Field0 = 0x789E00E2,
                Field1 = new NetAttributeKeyValue
                {
                    Attribute = GameAttribute.Attributes[34],
                    Int = Experience
                }
            });
        }

        public void Parse(ConnectionDataEventArgs e)
        {
            //Console.WriteLine(e.Data.Dump());

            _incomingBuffer.AppendData(e.Data.ToArray());

            while (_incomingBuffer.IsPacketAvailable())
            {
                int end = _incomingBuffer.Position;
                end += _incomingBuffer.ReadInt(32) * 8;

                while ((end - _incomingBuffer.Position) >= 9)
                {
                    try
                    {
                        GameMessage msg = _incomingBuffer.ParseMessage();
                        if (msg == null) continue;

                        //Logger.LogIncoming(msg);
                        msg.Handle((e.Connection.Client as GameClient));
                    }
                    catch (NotImplementedException)
                    {
                        //Logger.Debug("Unhandled game message: 0x{0:X4} {1}", msg.Id, msg.GetType().Name);
                    }
                }

                _incomingBuffer.Position = end;
            }
            _incomingBuffer.ConsumeData();
            FlushOutgoingBuffer();
        }

        public void SendMessage(GameMessage msg)
        {
            //Logger.LogOutgoing(msg);
            _outgoingBuffer.EncodeMessage(msg);
        }

        public void SendMessageNow(GameMessage msg)
        {
            SendMessage(msg);
            FlushOutgoingBuffer();
        }

        public void FlushOutgoingBuffer()
        {
            if (_outgoingBuffer.Length > 32)
            {
                var data = _outgoingBuffer.GetPacketAndReset();
                Connection.Send(data);
            }
        }

        public World GetLocalWorld()
        {
            return GameUniverse.GetLocalWorld();
        }

        public void EnterInn()
        {
            SendMessage(new RevealWorldMessage()
            {
                Id = 0x037,
                Field0 = 0x772F0001,
                Field1 = 0x0001AB32,
            });

            SendMessage(new WorldStatusMessage()
            {
                Id = 0x0B4,
                Field0 = 0x772F0001,
                Field1 = false,
            });

            SendMessage(new EnterWorldMessage()
            {
                Id = 0x0033,
                Field0 = new Vector3D()
                {
                    Field0 = 83.75f,
                    Field1 = 123.75f,
                    Field2 = 0.2000023f,
                },
                Field1 = 0x772F0001,
                Field2 = 0x0001AB32,
            });

            FlushOutgoingBuffer();

            SendMessage(new RevealSceneMessage()
            {
                Id = 0x0034,
                WorldID = 0x772F0001,
                SceneSpec = new SceneSpecification()
                {
                    Field0 = 0x00000000,
                    Field1 = new IVector2D()
                    {
                        Field0 = 0x00000000,
                        Field1 = 0x00000000,
                    },
                    arSnoLevelAreas = new int[] { 0x0001AB91, unchecked((int)0xFFFFFFFF), unchecked((int)0xFFFFFFFF), unchecked((int)0xFFFFFFFF) },
                    snoPrevWorld = 0x000115EE,
                    Field4 = 0x000000DF,
                    snoPrevLevelArea = unchecked((int)0xFFFFFFFF),
                    snoNextWorld = 0x00015348,
                    Field7 = 0x000000AC,
                    snoNextLevelArea = unchecked((int)0xFFFFFFFF),
                    snoMusic = 0x000206F8,
                    snoCombatMusic = unchecked((int)0xFFFFFFFF),
                    snoAmbient = 0x0002C68A,
                    snoReverb = 0x00021ABA,
                    snoWeather = 0x00017869,
                    snoPresetWorld = 0x0001AB32,
                    Field15 = 0x00000000,
                    Field16 = 0x00000000,
                    Field17 = 0x00000000,
                    Field18 = unchecked((int)0xFFFFFFFF),
                    tCachedValues = new SceneCachedValues()
                    {
                        Field0 = 0x0000003F,
                        Field1 = 0x00000060,
                        Field2 = 0x00000060,
                        Field3 = new AABB()
                        {
                            Field0 = new Vector3D()
                            {
                                Field0 = 120f,
                                Field1 = 120f,
                                Field2 = 26.61507f,
                            },
                            Field1 = new Vector3D()
                            {
                                Field0 = 120f,
                                Field1 = 120f,
                                Field2 = 36.06968f,
                            }
                        },
                        Field4 = new AABB()
                        {
                            Field0 = new Vector3D()
                            {
                                Field0 = 120f,
                                Field1 = 120f,
                                Field2 = 26.61507f,
                            },
                            Field1 = new Vector3D()
                            {
                                Field0 = 120f,
                                Field1 = 120f,
                                Field2 = 36.06968f,
                            }
                        },
                        Field5 = new int[] { 0x00000267, 0x00000000, 0x00000000, 0x00000000, },
                        Field6 = 0x00000001
                    }
                },
                ChunkID = 0x78740120,
                snoScene = 0x0001AB2F,
                Position = new PRTransform()
                {
                    Field0 = new Quaternion()
                    {
                        Field0 = 1f,
                        Field1 = new Vector3D()
                        {
                            Field0 = 0f,
                            Field1 = 0f,
                            Field2 = 0f,
                        }
                    },
                    Field1 = new Vector3D()
                    {
                        Field0 = 0f,
                        Field1 = 0f,
                        Field2 = 0f,
                    }
                },
                ParentChunkID = unchecked((int)0xFFFFFFFF),
                snoSceneGroup = unchecked((int)0xFFFFFFFF),
                arAppliedLabels = new int[0]

            });
            FlushOutgoingBuffer();

            SendMessage(new MapRevealSceneMessage()
            {
                Id = 0x044,
                ChunkID = 0x78740120,
                snoScene = 0x0001AB2F,
                Field2 = new PRTransform()
                {
                    Field0 = new Quaternion()
                    {
                        Field0 = 1f,
                        Field1 = new Vector3D()
                        {
                            Field0 = 0f,
                            Field1 = 0f,
                            Field2 = 0f,
                        }
                    },
                    Field1 = new Vector3D()
                    {
                        Field0 = 0f,
                        Field1 = 0f,
                        Field2 = 0f,
                    }
                },
                Field3 = 0x772F0001,
                MiniMapVisibility = 0
            });
            FlushOutgoingBuffer();

            SendMessage(new ACDEnterKnownMessage
            {
                Id = 0x3B,
                Field0 = 0x7A0800A2,
                Field1 = 0x0000157F,
                Field2 = 8,
                Field3 = 0,
                Field4 = new WorldLocationMessageData
                {
                    Field0 = 1f,
                    Field1 = new PRTransform
                    {
                        Field0 = new Quaternion
                        {
                            Field0 = 0.9909708f,
                            Field1 = new Vector3D
                            {
                                Field0 = 0f,
                                Field1 = 0f,
                                Field2 = 0.1340775f
                            }
                        },
                        Field1 = new Vector3D
                        {
                            Field0 = 82.15131f,
                            Field1 = 122.2867f,
                            Field2 = 0.1000366f
                        }
                    },
                    Field2 = 0x772F0001
                },
                Field6 = new GBHandle
                {
                    Field0 = -1,
                    Field1 = unchecked((int)0xFFFFFFFF)
                },
                Field7 = 0x00000001,
                Field8 = 0x0000157F,
                Field9 = 0,
                Field10 = 0x0,
                Field12 = 0x0001AB8C,
                Field13 = 0x00000000
            });

            FlushOutgoingBuffer();
            SendMessage(new AffixMessage()
            {
                Id = 0x48,
                Field0 = 0x7A0800A2,
                Field1 = 1,
                aAffixGBIDs = new int[0]
            });
            FlushOutgoingBuffer();

            SendMessage(new AffixMessage()
            {
                Id = 0x48,
                Field0 = 0x7A0800A2,
                Field1 = 2,
                aAffixGBIDs = new int[0]
            });
            FlushOutgoingBuffer();

            SendMessage(new PlayerWarpedMessage()
            {
                Id = 0x0B1,
                Field0 = 9,
                Field1 = 0xf,
            });
            FlushOutgoingBuffer();

            SendMessage(new ACDWorldPositionMessage
            {
                Id = 0x3f,
                Field0 = 0x789E00E2,
                Field1 = new WorldLocationMessageData
                {
                    Field0 = 1.43f,
                    Field1 = new PRTransform
                    {
                        Field0 = new Quaternion
                        {
                            Field0 = 0.05940768f,
                            Field1 = new Vector3D
                            {
                                Field0 = 0f,
                                Field1 = 0f,
                                Field2 = 0.9982339f,
                            }
                        },
                        Field1 = new Vector3D
                        {
                            Field0 = 82.15131f,
                            Field1 = 122.2867f,
                            Field2 = 0.1000366f
                        }
                    },
                    Field2 = 0x772F0001
                }
            });

            packetId += 10 * 2;
            SendMessage(new DWordDataMessage()
            {
                Id = 0x89,
                Field0 = packetId,
            });
            FlushOutgoingBuffer();
        }
        public int GetNextValidObjectID()
        {
            return objectId++;
        }
    }
}
