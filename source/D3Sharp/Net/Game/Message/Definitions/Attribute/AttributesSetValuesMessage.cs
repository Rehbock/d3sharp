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

namespace D3Sharp.Net.Game.Message.Definitions.Attribute
{
    public class AttributesSetValuesMessage : GameMessage
    {
        public int Field0;
        // MaxLength = 15
        public NetAttributeKeyValue[] atKeyVals;


        public override void Handle(GameClient client)
        {
            throw new NotImplementedException();
        }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            atKeyVals = new NetAttributeKeyValue[buffer.ReadInt(4)];
            for (int i = 0; i < atKeyVals.Length; i++) { atKeyVals[i] = new NetAttributeKeyValue(); atKeyVals[i].Parse(buffer); }
            for (int i = 0; i < atKeyVals.Length; i++) { atKeyVals[i].ParseValue(buffer); }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            buffer.WriteInt(4, atKeyVals.Length);
            for (int i = 0; i < atKeyVals.Length; i++) { atKeyVals[i].Encode(buffer); }
            for (int i = 0; i < atKeyVals.Length; i++) { atKeyVals[i].EncodeValue(buffer); }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("AttributesSetValuesMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8") + " (" + Field0 + ")");
            b.Append(' ', pad); b.AppendLine("atKeyVals:");
            b.Append(' ', pad); b.AppendLine("{");
            for (int i = 0; i < atKeyVals.Length; i++) { atKeyVals[i].AsText(b, pad + 1); b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}