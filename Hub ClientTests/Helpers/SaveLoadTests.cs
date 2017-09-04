using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Hub.Helpers.Tests
{
    [TestFixture]
    public class SaveLoadTests
    {
        /// <summary>
        /// test exception is thrown when loading invlid data
        /// </summary>
        [Test]
        public void TestLoadException()
        {
            Exception ex = null;

            //Invalid Data Exception when the path isn't valid
            try
            {
                SaveLoad.Load("thisfileDoesn'tExitstAndIfItDoesThenThisWillFailSoDon'tHaveFileWithThisName2232.yep");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is FileNotFoundException, "Exception was not a FileNotFoundException", new object[0] { });
                ex = e;
            }

            Assert.IsTrue(ex != null);
            ex = null;

            //Other exception for when there is another issue
            try
            {
                SaveLoad.Load(@"C:\scanimage\adminOnly.txt");
            }
            catch (Exception e)
            {
                Assert.IsFalse(e is InvalidDataException);
                ex = e;
            }
            Assert.IsNotNull(ex);
        }

        [Test]
        public void StandardLoadSave()
        {
            SaveLoad.CustomSaveDirectory = null;
            SaveLoad.Conf = SaveLoad.Data.Default();
            try
            {
                SaveLoad.Save();
                Assert.IsTrue(File.Exists(SaveLoad.DefaultSavePath));

                SaveLoad.Load();
                File.Delete(SaveLoad.DefaultSavePath);
            }
            catch (Exception e)
            {
                Assert.Fail("Shouldn't throw an error, " + e.Message);
            }
            finally
            {
                if (File.Exists(SaveLoad.DefaultSavePath)) File.Delete(SaveLoad.DefaultSavePath);
            }
        }

        [Test]
        public void AlternateSave()
        {
            #region initialise
            SaveLoad.CustomSaveDirectory = "Alt.conf";
            SaveLoad.Data testCase = new SaveLoad.Data
            {
                Cameras = new[]
                {
                    new CameraConfiguration()
                    {
                        Address = 3556734,
                        CamFileIdentity = "test1",
                        Id = 67,
                        Port = 3165
                    },
                    new CameraConfiguration()
                    {
                        Address = 7489,
                        CamFileIdentity = "test1WE",
                        Id = 562,
                        Port = 673
                    }
                }
            };
            #endregion

            try
            {
                SaveLoad.Conf = testCase;
                SaveLoad.Save();
                Assert.IsTrue(File.Exists(SaveLoad.CustomSaveDirectory));

                SaveLoad.Load();
                File.Delete(SaveLoad.CustomSaveDirectory);

                Assert.IsTrue(SaveLoad.Conf.Equals(testCase));
            }
            catch (Exception e)
            {
                Assert.Fail("Shouldn't throw an error, " + e.Message);
            }
            finally
            {
                if (File.Exists(SaveLoad.DefaultSavePath)) File.Delete(SaveLoad.DefaultSavePath);
            }
        }

        [Test]
        public void InvalidLoad()
        {
            SaveLoad.CustomSaveDirectory = "testSave";
            string save = SaveLoad.CustomSaveDirectory;
            try
            {
                if (File.Exists(save)) File.Delete(save);

                SaveLoad.Data config = SaveLoad.Load();
                Assert.True(config.Equals(SaveLoad.Data.Default()));
            }
            catch (Exception)
            {
                Assert.Fail("Shouldn't throw an error");
            }
            finally
            {
                if (File.Exists(save)) File.Delete(save);
            }
        }

        /// <summary>
        /// test exception is thrown when saving a file to an invalid location
        /// </summary>
        [Test]
        public void SaveException()
        {
            Exception ex = null;

            //Invlid path exception
            SaveLoad.Conf = SaveLoad.Data.Default();
            try
            {
                SaveLoad.Save(null);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is NullReferenceException);
                ex = e;
            }

            Assert.IsTrue(ex != null);
        }

        /// <summary>
        /// Tests the data structure in saveload class
        /// </summary>
        [Test]
        public void DataTest()
        {
            SaveLoad.Data testObject = new SaveLoad.Data();

            //uninitialized
            Assert.IsTrue(testObject.Cameras == null);
            Assert.IsTrue(testObject.CameraCount == 0);

            testObject = SaveLoad.Data.Default();

            //size test
            //Assert.IsTrue(testObject.Cameras.Length == 1);
            Assert.AreEqual(testObject.Cameras.Length, testObject.CameraCount);
        }

        /// <summary>
        /// Tests standard save load functionality without having any intended issues
        /// </summary>
        [Test]
        public void SaveandLoadNormally()
        {
            SaveLoad.Data data = CreateDummyData(12);
            string fileLocation = @"C:\scanimage\testFile.test";

            //ensure that the file doesn't already exist
            if (File.Exists(fileLocation)) File.Delete(fileLocation);

            SaveLoad.Conf = data;
            SaveLoad.Save(fileLocation);

            Assert.IsTrue(File.Exists(fileLocation));

            //check that the loaded file is identicle
            SaveLoad.Conf = SaveLoad.Data.Default();
            SaveLoad.Data loadData = SaveLoad.Load(fileLocation);
            File.Delete(fileLocation);

            Assert.IsTrue(loadData.CameraCount == data.CameraCount);
            for (int i = 0; i < loadData.CameraCount; i++)
            {
                Assert.IsTrue(data.Cameras[i].Address == loadData.Cameras[i].Address);
                Assert.IsTrue(data.Cameras[i].Id == loadData.Cameras[i].Id);
                Assert.IsTrue(data.Cameras[i].Port == loadData.Cameras[i].Port);
                Assert.IsTrue(data.Cameras[i].CamFileIdentity == loadData.Cameras[i].CamFileIdentity);
            }
        }

        private SaveLoad.Data CreateDummyData(int entryAmount)
        {
            SaveLoad.Data data = new SaveLoad.Data();
            List<CameraConfiguration> camConfigs = new List<CameraConfiguration>();

            Random rand = new Random();
            for (int i = 0; i < 12; i++)
            {
                CameraConfiguration newConfig = new CameraConfiguration();
                newConfig.Address = (long)rand.Next(200000, int.MaxValue) * rand.Next(200000, int.MaxValue);
                newConfig.Id = rand.Next();
                newConfig.Port = rand.Next();
                newConfig.CamFileIdentity = "testCam" + i;

                camConfigs.Add(newConfig);
            }

            data.Cameras = camConfigs.ToArray();
            return data;
        }
    }
}