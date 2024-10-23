using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using bdtp.blockchain;
using HoshoEthUtil;

// ReSharper disable once CheckNamespace
namespace bdtp
{
    public class Client
    {
        private readonly string _address;
        private readonly Int32 _port;

        public Client(string address, Int32 port)
        {
            _address = address;
            _port = port;
        }

        private static async Task SendData(Stream s, string bdtpAddress, byte[] data)
        {
            var chain = ExtractChain(bdtpAddress);
            var pointer = ExtractNetworkAddress(bdtpAddress, System.Text.Encoding.UTF8.GetString(chain));

            await s.WriteAsync(chain);
            await s.WriteAsync(pointer);
            var l = BitConverter.GetBytes(data.Length).ToArray().Reverse();
            await s.WriteAsync(l.ToArray());
            await s.WriteAsync(data);
        }

        private static async Task<byte[]> Listen(Stream s, string bdtpAddress)
        {
            var respLen = new byte[4];
            await s.ReadAsync(respLen);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(respLen);
                
            var len = BitConverter.ToInt32(respLen);
                
            if (len <= 0)
            {
                Console.WriteLine($"no data at ${bdtpAddress}");
                return null;
            }
            
            var d = new byte[len];
            var dataLen = await s.ReadAsync(d);
            if (dataLen != len)
            {
                            
                Console.WriteLine($"Read {dataLen} bytes out of {len}");
                return null;
            }

            return d;
        }

        private static byte[] ExtractChain(string bdtpAddress)
        {
                var b = Encoding.UTF8.GetBytes(bdtpAddress).ToArray();
                return b.Take(3).ToArray();
        }

        private static byte[] ExtractNetworkAddress(string bdtpAddress, string chain)
        {
            var b = Encoding.UTF8.GetBytes(bdtpAddress).ToArray();
            var pointer = Encoding.UTF8.GetString(b.Skip(3).ToArray());
            
            //TODO:create a factory or something
            if (chain == Blockchain.POL.ToString())
            {
                return Encoding.UTF8.GetBytes(pointer);
            }
            
            return SimpleBase.Base58.Bitcoin.Decode(pointer).ToArray();
        }

        private async Task<byte[]> TcpCall(string bdtpAddress, byte[] data)
        {
             var tcp = new TcpClient();
             try
             {
                 await tcp.ConnectAsync(_address, _port);
                 var stream = tcp.GetStream();
 
                 await SendData(stream, bdtpAddress, data);
                 return await Listen(stream, bdtpAddress);
             }
             catch (Exception e)
             {
                 Console.WriteLine(e);
                 throw;
             }
             finally
             {
                 tcp.Close();
             }
             
        }

        public async Task<byte[]> FetchDataFromBlockchain(string bdtpAddress)
        {
            return await TcpCall(bdtpAddress, new byte[0]);
        }

        public async Task<byte[]> SaveDataToChain(string bdtpAddress, byte[] data)
        {
            return await TcpCall(bdtpAddress, data);
        }
    }
}
