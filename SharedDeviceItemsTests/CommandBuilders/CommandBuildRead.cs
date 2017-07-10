using System;
using System.Collections.Generic;
using System.Text;
using CameraServer;
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

            foreach (KeyValuePair<string, string> pair in param)
            {
                Assert.True(interp.Parameters.ContainsKey(pair.Key));
                Assert.True(interp.Parameters[pair.Key] == pair.Value);
            }

            interp = new CommandReader(Encoding.ASCII.GetString(data));

            Assert.True(interp.Request == CameraRequest.SendFullResImage);
            Assert.True(interp.Parameters.Count == param.Count);

            foreach (KeyValuePair<string, string> pair in param)
            {
                Assert.True(interp.Parameters.ContainsKey(pair.Key));
                Assert.True(interp.Parameters[pair.Key] == pair.Value);
            }
        }

        [TestCase(Constants.ParamSeparator)]
        [TestCase(Constants.MessageSeparator)]
        [TestCase(Constants.EndOfMessage)]
        [TestCase(Constants.ParamKeyValueSeparator)]
        public void BuilderParamException(string constant)
        {
            CommandBuilder test = new CommandBuilder().AddParam("legal", "entry");
            try
            {
                test.AddParam("thing" + constant + "more", "value");
                Assert.Fail("should have got an exception");
            }
            catch (CommandException)
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

        [Test]
        public void StandardPicRequest()
        {
            CommandBuilder builder = new CommandBuilder().Request(CameraRequest.SendFullResImage);
            builder.AddParam("id", "0");

            byte[] request = builder.Build();
            byte[] rawRequest = new byte[request.Length + Constants.EndOfMessage.Length];
            request.CopyTo(rawRequest, 0);
            Encoding.ASCII.GetBytes(Constants.EndOfMessage).CopyTo(rawRequest, request.Length);

            CommandReader result = new CommandReader(rawRequest);

            Assert.True(result.Request == CameraRequest.SendFullResImage);
            Assert.True(result.Parameters["id"] == "0");
            Assert.True(result.Parameters.Count == 1);

            result = new CommandReader(Encoding.ASCII.GetString(rawRequest));

            Assert.True(result.Request == CameraRequest.SendFullResImage);
            Assert.True(result.Parameters["id"] == "0");
            Assert.True(result.Parameters.Count == 1);
        }

        [Test]
        public void StandardPicRequestWithEmpty()
        {
            byte[] request = new CommandBuilder().Request(CameraRequest.SendFullResImage).Build();
            byte[] rawRequest = new byte[request.Length + 12];
            request.CopyTo(rawRequest, 0);

            CommandReader result = new CommandReader(rawRequest);

            Assert.True(result.Request == CameraRequest.SendFullResImage);
            Assert.True(result.Parameters.Count == 0);

            result = new CommandReader(Encoding.ASCII.GetString(rawRequest));

            Assert.True(result.Request == CameraRequest.SendFullResImage);
            Assert.True(result.Parameters.Count == 0);
        }

        [Test]
        public void StandardPicRequestWithPartial()
        {
            byte[] request = new CommandBuilder().Request(CameraRequest.SendFullResImage).AddParam("thing", "xtbb").Build();
            byte[] rawRequest = new byte[request.Length + 12];
            request.CopyTo(rawRequest, 0);

            CommandReader result = new CommandReader(rawRequest);

            Assert.True(result.Request == CameraRequest.SendFullResImage);
            Assert.True(result.Parameters.Count == 1);

            result = new CommandReader(Encoding.ASCII.GetString(rawRequest));

            Assert.True(result.Request == CameraRequest.SendFullResImage);
            Assert.True(result.Parameters.Count == 1);
        }

        [Test]
        public void StandardPicRequestWithoutEndMssg()
        {
            byte[] request = new CommandBuilder().Request(CameraRequest.SendFullResImage).AddParam("thing", "xtbb").Build();
            byte[] rawRequest = new byte[request.Length - Constants.EndOfMessage.Length];
            Array.Copy(request, rawRequest, rawRequest.Length);

            CommandReader result = new CommandReader(rawRequest);

            Assert.True(result.Request == CameraRequest.SendFullResImage);
            Assert.True(result.Parameters.Count == 1);

            result = new CommandReader(Encoding.ASCII.GetString(rawRequest));

            Assert.True(result.Request == CameraRequest.SendFullResImage);
            Assert.True(result.Parameters.Count == 1);
        }

        [Test]
        public void SimpleRequest()
        {
            byte[] request = new CommandBuilder().Request(CameraRequest.Alive).Build();
            CommandReader reader = new CommandReader(request);

            Assert.AreEqual(reader.Request, CameraRequest.Alive);
            Assert.AreEqual(0, reader.Parameters.Count);

            reader = new CommandReader(Encoding.ASCII.GetString(request));

            Assert.AreEqual(CameraRequest.Alive, reader.Request);
            Assert.AreEqual(0, reader.Parameters.Count);
        }
    }
}
