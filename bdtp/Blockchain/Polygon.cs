using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;

namespace bdtp.blockchain
{
    public static class Polygon
    {
        public static (string address, byte[] privateKey, byte[] publicKey) GeneratePolygonPointer()
        {
            var ecKey = EthECKey.GenerateKey();
            var privateKey = ecKey.GetPrivateKeyAsBytes();
            var publicKey = ecKey.GetPubKey();
            var address = Blockchain.POL + ecKey.GetPublicAddress();

            return (address, privateKey, publicKey);
        }
    }
}