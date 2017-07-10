using System.IO;
using CameraServer;
using NUnit.Framework;

namespace CameraServer.Tests
{
    [TestFixture]
    public class CameraSettingsTests
    {
        private string newLocation;
        private string originalLocation;
        private string defaultPath;

        [OneTimeSetUp]
        public void Setup()
        {
            defaultPath = Path.GetPathRoot(Directory.GetCurrentDirectory()) +
                          "scanimage" + Path.DirectorySeparatorChar + "camera.conf";
            if (File.Exists(defaultPath))
            {
                //rename file for the tests so that it can be restored later
                originalLocation = defaultPath;
                newLocation = defaultPath.Insert(defaultPath.Length - 5, "2");
                if(!File.Exists(newLocation))File.Move(defaultPath, newLocation);
            }
        }

        [OneTimeTearDown]
        public void Finish()
        {
            if (newLocation != null)
            {
                ClearSetupFile();

                //move the file back to where it was
                File.Move(newLocation, originalLocation);
            }
        }

        [Test]
        public void InitTest()
        {
            CameraSettings.Init();

            Assert.True(File.Exists(defaultPath), "A configuration file should have been made");
        }

        [Test]
        public void PersistanceTest()
        {
            CameraSettings.Init();

            Assert.True(File.Exists(defaultPath), "A configuration file should have been made");

            CameraSettings.AddSetting("test", "3444");
            CameraSettings.AddSetting("test55", "nAMes and lotS of leTters to make A laRGer File");

            Assert.True(File.Exists(defaultPath), "The camera configuration file should still exist");

            CameraSettings.Reload();

            Assert.True(CameraSettings.GetSetting("test") == "3444");
            Assert.True(CameraSettings.GetSetting("test55") == "nAMes and lotS of leTters to make A laRGer File");
        }

        [TestCase("thing", new[] {"one", "two", "three"})]
        [TestCase("one", new[] {"first", "second", "third"})]
        [TestCase("test", new[] {"213", "213", "32432"})]
        [TestCase("test", new[] {"sgf", "dbf", "cxvs"})]
        public void OverrideTest(string key, string[] values)
        {
            CameraSettings.Init();
            
            Assert.True(CameraSettings.GetSetting(key) == null);

            foreach (string value in values)
            {
                CameraSettings.AddSetting(key, value);
                Assert.True(CameraSettings.GetSetting(key) == value);
            }

            CameraSettings.Reload();
            Assert.True(CameraSettings.GetSetting(key) == values[values.Length-1]);
        }

        [Test]
        public void InvalidAdd()
        {
            CameraSettings.Init();

            Assert.True(CameraSettings.AddSetting("thing", "thing"));
            Assert.False(CameraSettings.AddSetting("blah=", "thing"));

            Assert.True(CameraSettings.GetSetting("thing") == "thing");
            Assert.True(CameraSettings.GetSetting("blah=") == null);

            Assert.True(CameraSettings.AddSetting("param", "no"));
            Assert.False(CameraSettings.AddSetting("paramssss", "thi=ng"));

            Assert.True(CameraSettings.GetSetting("param") == "no");
            Assert.True(CameraSettings.GetSetting("paramssss") == null);

            Assert.False(CameraSettings.AddSetting("thing", "equa=l"));
            Assert.False(CameraSettings.AddSetting("param", "lots of stu=ff"));

            Assert.True(CameraSettings.GetSetting("thing") == "thing");
            Assert.True(CameraSettings.GetSetting("param") == "no");
        }

        [Test]
        public void NoReload()
        {
            CameraSettings.Init();

            CameraSettings.AddSetting("Peach", "Prince");
            CameraSettings.AddSetting("Toad", "blue");

            Assert.True(CameraSettings.GetSetting("Peach") == "Prince");
            Assert.True(CameraSettings.GetSetting("Toad") == "blue");

            ClearSetupFile();
            CameraSettings.Init();

            Assert.True(CameraSettings.GetSetting("Peach") == "Prince");
            Assert.True(CameraSettings.GetSetting("Toad") == "blue");

            CameraSettings.Reload();

            Assert.True(CameraSettings.GetSetting("Peach") == null);
            Assert.True(CameraSettings.GetSetting("Toad") == null);
        }

        [TestCase("nope", "yes", "no")]
        [TestCase("Bowser", "alive", "dead")]
        [TestCase("Command and Conquer", "good", "bad")]
        public void DefaultGetSetting(string key, string standard, string set)
        {
            CameraSettings.Init();

            //value doesn't exist
            Assert.AreEqual(standard, CameraSettings.GetSetting(key, standard));
            CameraSettings.AddSetting(key, set);

            //value now exists
            Assert.AreEqual(set, CameraSettings.GetSetting(key, standard));
            Assert.AreEqual(set, CameraSettings.GetSetting(key));
        }

        [TearDown]
        public void CleanUp()
        {
            ClearSetupFile();
            CameraSettings.Reload();
        }

        private void ClearSetupFile()
        {
            if (File.Exists(defaultPath))
            {
                if(File.Exists(defaultPath)) File.Delete(defaultPath);
            }
        }
    }
}