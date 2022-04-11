// ReSharper disable once CheckNamespace

using System.Text;
using bdtp.blockchain;

namespace bdtp
{
    public class Utils
    {
        public static (string, byte[], byte[]) GenerateBlockchainPointer(Blockchain chain, bool testnet)
        {
            switch (chain)
            {
                case Blockchain.WAV:
                    return Waves.GetWavesPointers(testnet);
                default:
                    return ("", new byte[0], new byte[0]);
            }
        }


    }
}