using System.IO;
using System.Linq;
using DZen.Security.Cryptography;
using NSec.Cryptography;
using HashAlgorithm = NSec.Cryptography.HashAlgorithm;

namespace bdtp.blockchain
{
    public class WavesKeyPair
    {
        public byte[] PrivK { get; }
        public byte[] PubK { get; }
        public Key K;

        public WavesKeyPair(Key k)
        {
            PrivK = k.Export(KeyBlobFormat.RawPrivateKey);
            PubK = k.Export(KeyBlobFormat.RawPublicKey);
            K = k;
        }

        public byte[] GetAddress(BlockchainNet net)
        {
            using (var ms = new MemoryStream(26))
            {
                ms.WriteByte(1);
                ms.WriteByte((byte)net);
                var hash = SecureHash(PubK);
                ms.Write(hash.Take(20).ToArray());

                var b = ms.ToArray();
                var checksum = SecureHash(b);
                
                ms.Write(checksum.Take(4).ToArray());

                return ms.ToArray();
            }
        }

        private static byte[] SecureHash(byte[] b)
        {
            var algo = HashAlgorithm.Blake2b_256;
            var hash = algo.Hash(b).ToArray();

            var sha3 = SHA3.Create();
            sha3.Initialize();
            sha3.UseKeccakPadding = true;
            
            return sha3.ComputeHash(hash);
        }
    }

    public class Waves
    {
        public static (string, byte[], byte[]) GetWavesPointers(bool testnet)
        {
            var keys = GenerateKeyPair();
            var net = testnet ? BlockchainNet.Testnet : BlockchainNet.Mainnet;
            var p = keys.GetAddress(net);
            var pointer = Blockchain.WAV + SimpleBase.Base58.Bitcoin.Encode(p);

            return (pointer, keys.PubK, keys.PrivK);
        }

        
        //TODO: verify signature for these keys.
        public static WavesKeyPair GenerateKeyPair()
        {
            var algo = SignatureAlgorithm.Ed25519;
            
            var param = new KeyCreationParameters();
            param.ExportPolicy = KeyExportPolicies.AllowPlaintextExport;

            using var k = Key.Create(algo, param);
            
            return new WavesKeyPair(k);
        }
    }
}