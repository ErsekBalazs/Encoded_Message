using System.Text;

namespace Encoded_Message_Test
{
    [TestClass]
    public sealed class EncodedTest
    {
        [TestMethod]
        public void TestEncoder()
        {
            Encrypt encrypt = new Encrypt();
            string message = "hello";
            string key = "abcdefg";
            string encrypted = encrypt.Encoder(message, key); 
            Assert.AreNotEqual(message, encrypted);
            string unencrypted = encrypt.Decoder(encrypted, key);
            Assert.AreEqual(message, unencrypted);
        }

        [DataTestMethod]
        [DataRow("hello", "bbbbbb", "ifmmp")]
        [DataRow("early bird", "abcabcabcabc", "ebtlzbbjtd")]
        public void TestEncoder_ValueInputs(string message, string key, string expected)
        {
            Encrypt encrypt = new Encrypt();

            string encrypted = encrypt.Encoder(message, key);
            Assert.AreEqual(expected, encrypted);
            string unencrypted = encrypt.Decoder(encrypted, key);
            Assert.AreEqual(message, unencrypted);
        }

        [DataTestMethod]
        [DataRow("hello", "by", "ifmmp")]
        [DataRow("neighborhood", "participation", "ebtlzbbjtdasd")]
        [DataRow("curiosity", "early bird", "eykscw byg")]
        public void TestKeyFinder(string phrase1, string phrase2, string key)
        {
            Encrypt encrypt = new();
            string encrypt1 = encrypt.Encoder(phrase1, key);
            string encrypt2 = encrypt.Encoder(phrase2, key);
            encrypt.KeyFinder(encrypt1, encrypt2);
            Assert.IsTrue(encrypt.possibleKeys.Count > 0);
            Assert.IsTrue(encrypt.possibleKeys.Contains(key));
        }
    }
}
