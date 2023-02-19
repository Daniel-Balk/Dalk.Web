using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Dalk.Web.HttpServer.WebSockets
{
    public class WebSocket
    {
        public event EventHandler<WebSocketMessage> OnMessage;

        private TcpClient sender;
        private Random rand = new Random();

        internal WebSocket(TcpClient sender)
        {
            this.sender = sender;
            HttpListener.Intercept(sender, Message);
        }

        byte ConvertToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("bits");
            }
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }

        void Reverse(BitArray array)
        {
            int length = array.Length;
            int mid = (length / 2);

            for (int i = 0; i < mid; i++)
            {
                bool bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }
        }

        private void Message(byte[] bytes)
        {
            int usedBytes = 2;
            var byte0 = new BitArray(new byte[] { bytes[0] });
            var byte1 = new BitArray(new byte[] { bytes[1] });
            Reverse(byte0);
            Reverse(byte1);
            bool fin = byte0[0];
            bool mask = byte1[0];
            var oa = new BitArray(new bool[] { false, false, false, false, byte0[4], byte0[5], byte0[6], byte0[7] });
            Reverse(oa);
            var opcode = ConvertToByte(oa);
            var pl = new BitArray(new bool[] { false, byte1[1], byte1[2], byte1[3], byte1[4], byte1[5], byte1[6], byte1[7] });
            Reverse(pl);
            ulong payloadLenght = ConvertToByte(pl);

            if(payloadLenght == 126)
            {
                byte[] b = new byte[] { bytes[usedBytes++], bytes[usedBytes++] };
#if !NETCOREAPP3_1_OR_GREATER
                payloadLenght = BitConverter.ToUInt16(b, 0);
#else
                payloadLenght = BitConverter.ToUInt16(b);
#endif
            }

            if(payloadLenght == 127)
            {
                byte[] b = new byte[] { bytes[usedBytes++], bytes[usedBytes++], bytes[usedBytes++], bytes[usedBytes++], bytes[usedBytes++], bytes[usedBytes++], bytes[usedBytes++], bytes[usedBytes++] };
#if !NETCOREAPP3_1_OR_GREATER
                payloadLenght = BitConverter.ToUInt64(b, 0);
#else
                payloadLenght = BitConverter.ToUInt64(b);
#endif            
            }

            byte[] maskingKey = new byte[] {};

            if (mask)
            {
                maskingKey = new byte[] { bytes[usedBytes++], bytes[usedBytes++], bytes[usedBytes++], bytes[usedBytes++] };
            }

            var payloadL = bytes.ToList();
            payloadL.RemoveRange(0, usedBytes);

            var payload = HttpListener.TrimEnd(payloadL.ToArray());

            if (mask)
            {
                for (int i = 0; i < payload.Length; i++)
                {
                    payload[i] = (byte)(payload[i] ^ maskingKey[i % 4]);
                }
            }

            if (opcode == 8)
                sender.Close();

            var text = Encoding.UTF8.GetString(payload);

            var msg = new WebSocketMessage() { Payload = payload, Type = (WebSocketOpCode)opcode };
            OnMessage?.Invoke(this, msg);
        }

        public void Send(bool fin, byte opcode, byte[] payload)
        {
            List<byte> bytes = new List<byte>();

            var oc = new BitArray(new byte[] {opcode});
            BitArray byte0 = new BitArray(new bool[] { fin, false, false, false, oc[3], oc[2], oc[1], oc[0] });
            Reverse(byte0);
            bytes.Add(ConvertToByte(byte0));

            var payloadLenght = 0;
            var len = payload.Length;
            payloadLenght = len;
            if(len > 125)
            {
                if(len < ushort.MaxValue)
                {
                    payloadLenght = 126;
                }
                else
                {
                    payloadLenght = 127;
                }
            }

            BitArray byte1 = new BitArray(new byte[] { (byte)payloadLenght });
            byte1[7] = true;
            bytes.Add(ConvertToByte(byte1));

            if (len > 125)
            {
                if (len < ushort.MaxValue)
                {
                    bytes.AddRange(BitConverter.GetBytes((ushort)len));
                }
                else
                {
                    bytes.AddRange(BitConverter.GetBytes((ulong)len));
                }
            }

            byte[] mask = new byte[4];
            rand.NextBytes(mask);

            bytes.AddRange(mask);
            var pl = payload;

            for (int i = 0; i < pl.Length; i++)
            {
                pl[i] = (byte)(pl[i] ^ mask[i % 4]);
            }

            bytes.AddRange(payload);

            var stream = sender.GetStream();
            var b = bytes.ToArray();
            try
            {
                stream.Write(b, 0, b.Length);
            }
            catch (Exception ex)
            {

            }
        }

        public void Send(string text)
        {
            Send(true, 1, Encoding.UTF8.GetBytes(text));
        }

        public void Send(byte[] bytes)
        {
            Send(true, 2, bytes);
        }
    }
}
