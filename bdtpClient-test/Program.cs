using System;
using System.Text;
using System.Threading.Tasks;
using bdtp;
using bdtp.blockchain;
using Base58 = SimpleBase.Base58;
using Utils = bdtp.Utils;

namespace bdtpClient_test
{
    class Program
    {
        private static async Task TestFetch()
        {
            var c = new Client("localhost", 4444);
            var bdtpadd = "WAV3MpsYmSQJsNZ2zi3Me2Jm8MGxG3QuWhFdni";
            var d = await c.FetchDataFromBlockchain(bdtpadd);
            if (d != null)
                Console.WriteLine(Encoding.UTF8.GetString(d));
        }

        private static async Task TestPost()
        {
            var c = new Client("localhost", 4444);

            var (pointer, pubK, privK) = Utils.GenerateBlockchainPointer(Blockchain.WAV, true);
            var b = await c.SaveDataToChain(pointer, new byte[0]);
            if (b != null)
                Console.WriteLine(Encoding.UTF8.GetString(b));
        }

        static async Task Main(string[] args)
        {
            //await TestFetch();

            //await TestPost();
            //var s = Waves.GeneratePrivateKey(new byte[0], 0);
            //Console.WriteLine(s.Length);
            //Console.WriteLine(SimpleBase.Base58.Bitcoin.Encode(s));
            var k = Waves.GenerateKeyPair();
            var add = k.GetAddress(BlockchainNet.Testnet);
            Console.WriteLine(Base58.Bitcoin.Encode(k.PrivK));
            Console.WriteLine(Base58.Bitcoin.Encode(k.PubK));
            Console.WriteLine(Base58.Bitcoin.Encode(add));
        }
    }
}