using NUnit.Framework;
using System.Text;
using SharedDeviceItems;

namespace Hub.Helpers.Tests
{
    [TestFixture]
    public class ByteManipulationTests
    {
        /// <summary>
        /// Tests the correct byte arrays return true as expected
        /// </summary>
        [Test]
        public void SeperateDataTestCorrect()
        {
            string name = "This is a name";
            byte[] data = { 0, 23, 243, 231, 23, 234, 234, 23, 4, 4, 4, 0, 0, 234 };

            //create byte array for seperation
            byte[] byteName = Encoding.ASCII.GetBytes(name + Constants.MessageSeparator);
            byte[] testData = new byte[byteName.Length + data.Length];
            byteName.CopyTo(testData, 0);
            data.CopyTo(testData, byteName.Length);

            //return data
            string returnName;
            byte[] returnBytes;

            Assert.IsTrue(ByteManipulation.SeparateData(out returnName, testData, out returnBytes));
            Assert.IsTrue(returnName.Equals(name));
            Assert.IsTrue(data.Length == returnBytes.Length);

            for (int i = 0; i < data.Length; i++)
                Assert.IsTrue(data[i] == returnBytes[i]);
        }

        /// <summary>
        /// Tests the incorrect byte arrays returns false as expected
        /// </summary>
        [Test]
        public void SeperateDataTestIncorrect()
        {
            string name = "This is a really long string that needs to be parsed one by one";
            byte[] data = { 0, 23, 243, 231 };

            //create byte array for seperation
            byte[] byteName = Encoding.ASCII.GetBytes(name + Constants.EndOfMessage);
            byte[] testData = new byte[byteName.Length + data.Length];
            byteName.CopyTo(testData, 0);
            data.CopyTo(testData, byteName.Length);

            //return data
            string returnName;
            byte[] returnBytes;

            Assert.IsFalse(ByteManipulation.SeparateData(out returnName, testData, out returnBytes));
            Assert.IsTrue(returnName == "");
            Assert.IsTrue(returnBytes.Length == 0);
        }

        /// <summary>
        /// Test ensures that the seperator will continue to function when most of the 
        /// seperation character are there but not all of them
        /// </summary>
        [Test]
        public void SeperateDataHalfSeperated()
        {
            string name = "String";
            byte[] data = { 12, 34, 211, 179 };

            //create byte array for seperation
            string almostSeperator = "";
            for (int i = 0; i < Constants.MessageSeparator.Length - 1; i++)
            {
                almostSeperator += Constants.MessageSeparator[i];
            }

            name += almostSeperator;

            byte[] byteName = Encoding.ASCII.GetBytes(name + Constants.MessageSeparator);
            byte[] testData = new byte[byteName.Length + data.Length];
            byteName.CopyTo(testData, 0);
            data.CopyTo(testData, byteName.Length);

            //return data
            string returnName;
            byte[] returnBytes;

            Assert.IsTrue(ByteManipulation.SeparateData(out returnName, testData, out returnBytes));
            Assert.IsTrue(returnName.Equals(name));
            Assert.IsTrue(data.Length == returnBytes.Length);

            for (int i = 0; i < data.Length; i++)
                Assert.IsTrue(data[i] == returnBytes[i]);
        }

        /// <summary>
        /// Test that a byte array that contains an end of message string returns true
        /// </summary>
        [Test]
        public void SearchEndOfMessageTrue()
        {
            byte[] garbageData = { 12, 233, 67, 255, 186, 243, 14, 15, 241, 178 };
            byte[] endOfMessage = Encoding.ASCII.GetBytes(Constants.EndOfMessage);

            byte[] testData = new byte[garbageData.Length + endOfMessage.Length];
            garbageData.CopyTo(testData, 0);
            endOfMessage.CopyTo(testData, garbageData.Length);

            Assert.True(ByteManipulation.SearchEndOfMessage(testData, testData.Length));
            Assert.False(ByteManipulation.SearchEndOfMessageIndex(testData, testData.Length) > 10);
            Assert.False(ByteManipulation.SearchEndOfMessageStartIndex(testData, testData.Length) > 10);
        }

        /// <summary>
        /// Test that a byte array that contains an end of message string returns true, with extra entries
        /// after the end of string text
        /// </summary>
        [Test]
        public void SearchEndOfMessageTrueOverhang()
        {
            byte[] garbageData = { 12, 233, 67, 255, 186, 243, 14, 15, 241, 178 };
            byte[] endOfMessage = Encoding.ASCII.GetBytes(Constants.EndOfMessage);
            byte[] extra = {23,23,23,23,23,23};

            byte[] testData = new byte[garbageData.Length + endOfMessage.Length + extra.Length];
            garbageData.CopyTo(testData, 0);
            endOfMessage.CopyTo(testData, garbageData.Length);
            extra.CopyTo(testData, garbageData.Length + endOfMessage.Length);

            Assert.True(ByteManipulation.SearchEndOfMessage(testData, testData.Length));
            Assert.False(ByteManipulation.SearchEndOfMessageIndex(testData, testData.Length) > 10);
            Assert.False(ByteManipulation.SearchEndOfMessageStartIndex(testData, testData.Length) > 10);
        }

        /// <summary>
        /// Test that a byte array that contains an end of message string returns true, with extra entries 
        /// and a partial end of message 
        /// </summary>
        [Test]
        public void SearchEndOfMessageTruePartialOverhang()
        {
            byte[] garbageData = { 12, 233, 67, 255, 186, 243, 14, 15, 241, 178 };
            byte[] endOfMessage = Encoding.ASCII.GetBytes(Constants.EndOfMessage);
            byte[] extra = { 23, 23, 23, 23, 23, 23 };
            byte[] partialMessage = Encoding.ASCII.GetBytes(Constants.EndOfMessage.Substring(1));

            byte[] testData = new byte[garbageData.Length + endOfMessage.Length + extra.Length + partialMessage.Length];
            garbageData.CopyTo(testData, 0);
            endOfMessage.CopyTo(testData, garbageData.Length);
            extra.CopyTo(testData, garbageData.Length + endOfMessage.Length);
            partialMessage.CopyTo(testData, garbageData.Length + endOfMessage.Length + extra.Length);

            Assert.True(ByteManipulation.SearchEndOfMessage(testData, testData.Length));
            Assert.False(ByteManipulation.SearchEndOfMessageIndex(testData, testData.Length) > 10 );
            Assert.False(ByteManipulation.SearchEndOfMessageStartIndex(testData, testData.Length) > 10 );
        }

        /// <summary>
        /// Test that a byte array that DOESN'T contain a valid end of message string returns false
        /// </summary>
        [Test]
        public void SearchEndOfMessageFalse()
        {
            byte[] garbageData = { 12, 233, 67, 255, 186, 243, 14, 15, 241, 178 };
            byte[] endOfMessage = Encoding.ASCII.GetBytes(Constants.EndOfMessage.Substring(1));

            byte[] testData = new byte[garbageData.Length + endOfMessage.Length];
            garbageData.CopyTo(testData, 0);
            endOfMessage.CopyTo(testData, garbageData.Length);

            Assert.False(ByteManipulation.SearchEndOfMessage(testData, testData.Length));
            Assert.True(ByteManipulation.SearchEndOfMessageIndex(testData, testData.Length) == -1);
            Assert.True(ByteManipulation.SearchEndOfMessageStartIndex(testData, testData.Length) == -1);
        }

        /// <summary>
        /// Test that a byte array that DOESN'T contain a valid end of message string returns false, even with
        /// extra values at the end
        /// </summary>
        [Test]
        public void SearchEndOfMessageFalseOverhang()
        {
            byte[] garbageData = { 12, 233, 67, 255, 186, 243, 14, 15, 241, 178 };
            byte[] endOfMessage = Encoding.ASCII.GetBytes(Constants.EndOfMessage.Substring(1));
            byte[] extra = { 23, 23, 23, 23, 23, 23 };

            byte[] testData = new byte[garbageData.Length + endOfMessage.Length + extra.Length];
            garbageData.CopyTo(testData, 0);
            endOfMessage.CopyTo(testData, garbageData.Length);
            extra.CopyTo(testData, garbageData.Length + endOfMessage.Length);

            Assert.False(ByteManipulation.SearchEndOfMessage(testData, testData.Length));
            Assert.True(ByteManipulation.SearchEndOfMessageIndex(testData, testData.Length) == -1);
            Assert.True(ByteManipulation.SearchEndOfMessageStartIndex(testData, testData.Length) == -1);
        }

        [Test]
        public void SearchEndOfMessageNoData()
        {
            byte[] testData = new byte[0];

            Assert.False(ByteManipulation.SearchEndOfMessage(testData, testData.Length));
            Assert.True(ByteManipulation.SearchEndOfMessageIndex(testData, testData.Length) == -1);
            Assert.True(ByteManipulation.SearchEndOfMessageStartIndex(testData, testData.Length) == -1);
        }

        [Test]
        public void LengthGreaterThanData()
        {
            byte[] data = Encoding.ASCII.GetBytes( "random words and stuff" + Constants.EndOfMessage);

            Assert.IsTrue(ByteManipulation.SearchEndOfMessage(data, data.Length + 3));
            Assert.AreEqual(22, ByteManipulation.SearchEndOfMessageIndex(data, data.Length + 3));
            Assert.AreEqual(22, ByteManipulation.SearchEndOfMessageStartIndex(data, data.Length + 3));
        }
    }
}