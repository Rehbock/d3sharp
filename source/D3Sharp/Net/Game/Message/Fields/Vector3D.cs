using System;
using System.Text;
using D3Sharp.Net.Game.Messages;

namespace D3Sharp.Net.Game.Message.Fields
{
    public class Vector3D
    {
        public float Field0;
        public float Field1;
        public float Field2;

        public Vector3D()
        {

        }

        public float DotProduct(Vector3D v)
        {
            return Field0 * v.Field0 + Field1 * v.Field1 + Field2 * v.Field2;
        }

        public float Distance(Vector3D v)
        {
            return (float)Math.Sqrt(((v.Field0 - Field0) * (v.Field0 - Field0)) +
                              ((v.Field1 - Field1) * (v.Field1 - Field1)) +
                              ((v.Field2 - Field2) * (v.Field2 - Field2)));

        }

        public Vector3D(float x, float y, float z)
        {
            Field0 = x;
            Field1 = y;
            Field2 = z;
        }

        public void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadFloat32();
            Field1 = buffer.ReadFloat32();
            Field2 = buffer.ReadFloat32();
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteFloat32(Field0);
            buffer.WriteFloat32(Field1);
            buffer.WriteFloat32(Field2);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("Vector3D:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Field0: " + Field0.ToString("G"));
            b.Append(' ', pad);
            b.AppendLine("Field1: " + Field1.ToString("G"));
            b.Append(' ', pad);
            b.AppendLine("Field2: " + Field2.ToString("G"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}