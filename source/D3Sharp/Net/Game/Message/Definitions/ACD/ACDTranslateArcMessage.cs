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
using D3Sharp.Net.Game.Message.Fields;
using D3Sharp.Net.Game.Messages;

namespace D3Sharp.Net.Game.Message.Definitions.ACD
{
    public class ACDTranslateArcMessage : GameMessage
    {
        public int Field0;
        public Vector3D Field1;
        public Vector3D Field2;
        public int Field3;
        public int Field4;
        public int Field5;
        public float Field6;
        public int /* sno */ Field7;
        public float Field8;


        public override void Handle(GameClient client)
        {
            throw new NotImplementedException();
        }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            Field1 = new Vector3D();
            Field1.Parse(buffer);
            Field2 = new Vector3D();
            Field2.Parse(buffer);
            Field3 = buffer.ReadInt(24);
            Field4 = buffer.ReadInt(21) + (-1);
            Field5 = buffer.ReadInt(21) + (-1);
            Field6 = buffer.ReadFloat32();
            Field7 = buffer.ReadInt(32);
            Field8 = buffer.ReadFloat32();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            Field1.Encode(buffer);
            Field2.Encode(buffer);
            buffer.WriteInt(24, Field3);
            buffer.WriteInt(21, Field4 - (-1));
            buffer.WriteInt(21, Field5 - (-1));
            buffer.WriteFloat32(Field6);
            buffer.WriteInt(32, Field7);
            buffer.WriteFloat32(Field8);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDTranslateArcMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8"));
            Field1.AsText(b, pad);
            Field2.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("Field3: 0x" + Field3.ToString("X8") + " (" + Field3 + ")");
            b.Append(' ', pad); b.AppendLine("Field4: 0x" + Field4.ToString("X8") + " (" + Field4 + ")");
            b.Append(' ', pad); b.AppendLine("Field5: 0x" + Field5.ToString("X8") + " (" + Field5 + ")");
            b.Append(' ', pad); b.AppendLine("Field6: " + Field6.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Field7: 0x" + Field7.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Field8: " + Field8.ToString("G"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}