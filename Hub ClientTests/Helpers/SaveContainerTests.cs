using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Hub.Helpers.Tests
{
    [TestFixture]
    public class SaveContainerTests
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
                SaveContainer.Load("thisfileDoesn'tExitstAndIfItDoesThenThisWillFailSoDon'tHaveFileWithThisName2232.yep");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is InvalidDataException);
                ex = e;
            }

            Assert.IsTrue(ex != null);
            ex = null;

            //Other exception for when there is another issue
            try
            {
                SaveContainer.Load(@"C:\scanimage\adminOnly.txt");
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
            SaveContainer.CustomSaveDirectory = null;
            SaveContainer.Conf = new SaveContainer.Data().Default();
            try
            {
                SaveContainer.Save();
                Assert.IsTrue(File.Exists(SaveContainer.DefaultSaveDirectory));

                SaveContainer.Load();
                File.Delete(SaveContainer.DefaultSaveDirectory);
            }
            catch (Exception e)
            {
                Assert.Fail("Shouldn't throw an error, " + e.Message);
            }
            finally
            {
                if (File.Exists(SaveContainer.DefaultSaveDirectory)) File.Delete(SaveContainer.DefaultSaveDirectory);
            }
        }

        [Test]
        public void AlternateSave()
        {
            #region initialise
            SaveContainer.CustomSaveDirectory = SaveContainer.DefaultSaveDirectory;
            SaveContainer.Data testCase = new SaveContainer.Data
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
                SaveContainer.Conf = testCase;
                SaveContainer.Save();
                Assert.IsTrue(File.Exists(SaveContainer.CustomSaveDirectory));

                SaveContainer.Load();
                File.Delete(SaveContainer.CustomSaveDirectory);

                Assert.IsTrue(SaveContainer.Conf.Equals(testCase));
            }
            catch (Exception e)
            {
                Assert.Fail("Shouldn't throw an error, " + e.Message);
            }
            finally
            {
                if (File.Exists(SaveContainer.DefaultSaveDirectory)) File.Delete(SaveContainer.DefaultSaveDirectory);
            }
        }

        [Test]
        public void InvalidLoad()
        {
            string save = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "testSave";
            SaveContainer.CustomSaveDirectory = save;
            try
            {
                if (File.Exists(save)) File.Delete(save);

                SaveContainer.Data config = SaveContainer.Load();
                Assert.True(config.Equals(new SaveContainer.Data().Default()));
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
            SaveContainer.Conf = new SaveContainer.Data()
            {
                Cameras = new CameraConfiguration[0]
            };

            //No camera Exception
            try
            {
                SaveContainer.Save("nope");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ex = e;
            }

            Assert.IsTrue(ex != null);
            ex = null;

            //Invlid path exception
            SaveContainer.Conf = new SaveContainer.Data().Default();
            try
            {
                SaveContainer.Save(null);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is NullReferenceException);
                ex = e;
            }

            Assert.IsTrue(ex != null);
            ex = null;

            //Other exception for when there is another issue
            try
            {
                SaveContainer.Save(@"C:\foob.test");

                //wow, ok that should't work so let's delete that file
                File.Delete(@"C:\foob.test");
            }
            catch (Exception e)
            {
                Assert.IsFalse(e is NullReferenceException);
                ex = e;
            }
            Assert.IsNotNull(ex);
        }

        /// <summary>
        /// Tests the data structure in saveload class
        /// </summary>
        [Test]
        public void DataTest()
        {
            SaveContainer.Data testObject = new SaveContainer.Data();

            //uninitialised
            Assert.IsTrue(testObject.Cameras == null);
            Assert.IsTrue(testObject.CameraCount == 0);

            testObject = new SaveContainer.Data().Default();

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
            SaveContainer.Data data = CreateDummyData(12);
            string fileLocation = @"C:\scanimage\testFile.test";

            //ensure that the file doesn't already exist
            if (File.Exists(fileLocation)) File.Delete(fileLocation);

            SaveContainer.Conf = data;
            SaveContainer.Save(fileLocation);

            Assert.IsTrue(File.Exists(fileLocation));

            //check that the loaded file is identicle
            SaveContainer.Conf = new SaveContainer.Data().Default();
            SaveContainer.Data loadData = SaveContainer.Load(fileLocation);
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

        private SaveContainer.Data CreateDummyData(int entryAmount)
        {
            SaveContainer.Data data = new SaveContainer.Data();
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