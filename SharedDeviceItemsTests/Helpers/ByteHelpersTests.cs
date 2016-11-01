using System;
using NUnit.Framework;
using System.IO;

namespace SharedDeviceItems.Helpers.Tests
{
    [TestFixture]
    public class ByteHelpersTests
    {
        [Test]
        public void BasicFileSerialisation()
        {
            string filePath = Path.GetPathRoot(Directory.GetCurrentDirectory()) + "scanimage" + 
                Path.DirectorySeparatorChar + "test.jpg";

            Assert.IsTrue(File.Exists(filePath));

            byte[] bytes = ByteHelpers.FileToBytes(filePath);

            Assert.IsTrue(Hub.Helpers.ByteManipulation.SearchEndOfMessage(bytes, bytes.Length));
            Assert.IsTrue(Hub.Helpers.ByteManipulation.SearchEndOfMessageIndex(bytes, bytes.Length) == 
                bytes.Length - Constants.EndOfMessage.Length-1);
        }

        [Test]
        public void FileSerialisationIncorrectPath()
        {
            string fakePath = Path.GetPathRoot(Directory.GetCurrentDirectory()) + "this" + Path.DirectorySeparatorChar +
                              "path" + Path.DirectorySeparatorChar + "does" + Path.DirectorySeparatorChar + "not" +
                              Path.DirectorySeparatorChar + "exist.txt";
            Assert.IsFalse(File.Exists(fakePath));

            try
            {
                ByteHelpers.FileToBytes(fakePath);
                Assert.Fail("An Exception should have been thrown...");
            }
            catch(IOException)
            {
                Assert.Pass("Exception was thrown, yay");
            }
        }

        [Test]
        public void codeTest()
        {
            byte[] one = new byte[20],
                two = new byte[20],
                three = new byte[40];

            for(int i = 0; i < one.Length; i++)
            {
                one[i] = Convert.ToByte(i);
            }

            for(int i = 0; i < two.Length; i++)
            {
                two[i] = Convert.ToByte(i + one.Length);
            }

            Array.Copy(one, 0, three, 0, one.Length);
            Array.Copy(two, 0, three, 20, two.Length);
        }
    }
}