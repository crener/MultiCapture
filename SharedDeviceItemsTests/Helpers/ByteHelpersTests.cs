using System;
using System.Text;
using NUnit.Framework;
using System.IO;
using Hub.Helpers;

namespace SharedDeviceItems.Helpers.Tests
{
    [TestFixture]
    public class ByteHelpersTests
    {
        [Test]
        public void BasicFileSerialisation()
        {
            string filePath = Path.GetPathRoot(Directory.GetCurrentDirectory()) + "scanimage" +
                Path.DirectorySeparatorChar + "test2.jpg";

            if (!File.Exists(filePath))
            {
                byte[] data = new byte[100];
                new Random().NextBytes(data);
                byte[] eom = Encoding.ASCII.GetBytes(Constants.EndOfMessage);

                Array.Copy(eom, 0, data, data.Length - eom.Length, eom.Length);

                File.WriteAllBytes(filePath, data);
             }

            Assert.IsTrue(File.Exists(filePath));

            byte[] bytes = ByteHelpers.FileToBytes(filePath);

            Assert.IsTrue(ByteManipulation.SearchEndOfMessage(bytes, bytes.Length));
            Assert.IsTrue(ByteManipulation.SearchEndOfMessageIndex(bytes, bytes.Length) ==
                bytes.Length - Constants.EndOfMessage.Length);

            File.Delete(filePath);
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
            catch (IOException)
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

            for (int i = 0; i < one.Length; i++)
            {
                one[i] = Convert.ToByte(i);
            }

            for (int i = 0; i < two.Length; i++)
            {
                two[i] = Convert.ToByte(i + one.Length);
            }

            Array.Copy(one, 0, three, 0, one.Length);
            Array.Copy(two, 0, three, 20, two.Length);
        }


        //Has permission issues under standard visual studio tests
        [Test]
        public void ReadFile()
        {
            Random rand = new Random();
            int size = rand.Next(514875, 1048576);
            byte[] fakeData = new byte[size];
            rand.NextBytes(fakeData);

            string path = Constants.DefualtHubSaveLocation() + "readfile.txt";

            if (File.Exists(path)) File.Delete(path);

            using (FileStream file = new FileStream(path, FileMode.CreateNew))
            {
                for (int i = 0; i < fakeData.Length; i++)
                {
                    file.WriteByte(fakeData[i]);
                }
            }

            byte[] methodData = ByteHelpers.FileToBytes(path), fileData = new byte[size];
            string name = "";
            Assert.True(ByteManipulation.SeparateData(out name, methodData, out methodData, Constants.MessageSeperator));
            Array.Copy(methodData, fileData, methodData.Length - Constants.EndOfMessage.Length);

            Assert.AreEqual("readfile.txt", name);
            Assert.True(fileData.Length == fakeData.Length);

            for (int i = 0; i < fakeData.Length; i++)
            {
                Assert.True(fakeData[i] == methodData[i]);
            }

            File.Delete(path);
        }
    }
}