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
using System.Text;

using D3Sharp.Core.NPC;
using D3Sharp.Core.Helpers;
using D3Sharp.Net.Game.Message.Definitions.Animation;
using D3Sharp.Net.Game.Message.Definitions.Attribute;
using D3Sharp.Net.Game.Message.Definitions.Effect;
using D3Sharp.Net.Game.Message.Definitions.Misc;
using D3Sharp.Net.Game.Message.Fields;
using D3Sharp.Net.Game.Messages;

namespace D3Sharp.Net.Game.Message.Definitions.Combat
{
    [IncomingMessage(Opcodes.TargetMessage)]
    public class TargetMessage : GameMessage
    {
        public int Field0;
        public int Field1;
        public WorldPlace Field2;
        public int /* sno */ snoPower;
        public int Field4;
        public int Field5;
        public AnimPreplayData Field6;

        public override void Handle(GameClient client)
        {
            
            if (this.Field1 == 0x77F20036)
            {
                client.EnterInn();
                return;
            }
            for (int i = 0; i < client.GetLocalWorld().encounters.Count; i++)
            {
                Monster monster = client.GetLocalWorld().encounters[i].GetMonsterByID(this.Field1);
                if (monster != null)
                {
                    if (monster.IsDead())
                    {
                        monster.Die();
                        client.GetLocalWorld().encounters[i].RemoveMonster(monster);
                    }
                    else
                    {
                        monster.Pain(18 + RandomHelper.Next(8));

                        if (monster.IsDead())
                        {
                            
                            monster.Die();
                            client.GetLocalWorld().encounters[i].RemoveMonster(monster);
                        }
                    }

                    break;
                }
            }
        }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(2) + (-1);
            Field1 = buffer.ReadInt(32);
            Field2 = new WorldPlace();
            Field2.Parse(buffer);
            snoPower = buffer.ReadInt(32);
            Field4 = buffer.ReadInt(32);
            Field5 = buffer.ReadInt(2);
            if (buffer.ReadBool())
            {
                Field6 = new AnimPreplayData();
                Field6.Parse(buffer);
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(2, Field0 - (-1));
            buffer.WriteInt(32, Field1);
            Field2.Encode(buffer);
            buffer.WriteInt(32, snoPower);
            buffer.WriteInt(32, Field4);
            buffer.WriteInt(2, Field5);
            buffer.WriteBool(Field6 != null);
            if (Field6 != null)
            {
                Field6.Encode(buffer);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("TargetMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8") + " (" + Field0 + ")");
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + Field1.ToString("X8") + " (" + Field1 + ")");
            Field2.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("snoPower: 0x" + snoPower.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Field4: 0x" + Field4.ToString("X8") + " (" + Field4 + ")");
            b.Append(' ', pad); b.AppendLine("Field5: 0x" + Field5.ToString("X8") + " (" + Field5 + ")");
            if (Field6 != null)
            {
                Field6.AsText(b, pad);
            }
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }

}