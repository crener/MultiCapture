using System;
using System.Collections.Generic;
using Camera_Server;
using NUnit.Framework;
using Hub.Helpers;
using SharedDeviceItems;
using SharedDeviceItems.Exceptions;

namespace SharedDeviceItemsTests.CommandBuilders
{
    [TestFixture]
    public class CommandBuildRead
    {
        [Test]
        public void BuildAndRead()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("camera", "yerp");
            param.Add("trumpet", "yellow");

            CommandBuilder builder = new CommandBuilder().Request(CameraRequest.SendFullResImage);

            foreach (var parm in param) builder.AddParam(parm.Key, parm.Value);

            byte[] data = builder.Build();

            CommandReader interp = new CommandReader(data);

            Assert.True(interp.Request == CameraRequest.SendFullResImage);
            Assert.True(interp.Parameters.Count == param.Count);

            foreach(KeyValuePair<string, string> pair in param)
            {
                Assert.True(interp.Parameters.ContainsKey(pair.Key));
                Assert.True(interp.Parameters[pair.Key] == pair.Value);
            }
        }

        [TestCase(Constants.ParamSeperator)]
        [TestCase(Constants.MessageSeperator)]
        [TestCase(Constants.EndOfMessage)]
        [TestCase(Constants.ParamKeyValueSeperator)]
        public void builderParamException(string constant)
        {
            CommandBuilder test = new CommandBuilder().AddParam("legal", "entry");
            try
            {
                test.AddParam("thing" + constant + "more", "value");
                Assert.Fail("should have got an exception");
            }
            catch(CommandException)
            {
                
            }

            try
            {
                test.AddParam("key", "thing" + constant + "more");
                Assert.Fail("should have got an exception");
            }
            catch (CommandException)
            {

            }
        }
    }
}
