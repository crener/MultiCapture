using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Hub.ResponseSystem;
using NUnit.Framework;

namespace Hub.DesktopInterconnect
{
    class DesktopConnectionTest
    {
        [SetUp]
        public void Setup()
        {
            //reset and reinitialise the desktop thread so that all responders exist again
            DesktopThread.Responders.Clear();
            new TestDesktopThread();
            object thing = DesktopThread.Instance;
        }

        [Test]
        public void CommandRecognition()
        {
            TestConnection response = new TestConnection(null);
            string rawRequest = ((int)ScannerCommands.ApiVersion).ToString();

            response.ExtractRequest(rawRequest);

            Assert.AreEqual(ScannerCommands.ApiVersion, response.processCommand);
        }

        [TestCase(ScannerCommands.Unknown)]
        [TestCase(-2)]
        public void UnknownCommand(int commandNo)
        {
            TestConnection response = new TestConnection(null);

            response.ExtractRequest(commandNo.ToString());

            Assert.AreEqual(ScannerCommands.Unknown, response.processCommand);

            string result = Encoding.ASCII.GetString(response.finalResponse);
            Assert.IsTrue(result.StartsWith(ResponseConstants.FailString));
        }

        [Test]
        public void PlainRequest()
        {
            string[,] parameters = { { "one", "result" }, { "yes", "no" } };

            string rawRequest = ((int)ScannerCommands.ApiVersion).ToString();
            for (int i = 0; i < parameters.Length / 2; i++)
                rawRequest += DesktopConnection.Separator + parameters[i, 0] + DesktopConnection.ParamSeperator +
                              parameters[i, 1];

            TestConnection response = new TestConnection(null);

            response.ExtractRequest(rawRequest);

            Assert.AreEqual(ScannerCommands.ApiVersion, response.processCommand);
            Assert.AreEqual(2, response.processParameters.Count);
        }

        [Test]
        public void ValidRequest()
        {
            TestConnection response = new TestConnection(null);

            response.ExtractRequest(((int)ScannerCommands.ApiVersion).ToString());
            string result = Encoding.ASCII.GetString(response.finalResponse);

            Console.WriteLine(result);
            foreach(KeyValuePair<ScannerCommands, IResponse> pair in DesktopThread.Responders)
            {
                Console.WriteLine(pair.Value.GetType());
            }

            Assert.AreEqual(ScannerCommands.ApiVersion, response.processCommand);
            Assert.IsTrue(result.EndsWith(ResponseConstants.ApiVersion.ToString("N")));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ResponseException(bool exceptions)
        {
            UnknownResponse unknown = new UnknownResponse();
            unknown.throwUnknown = exceptions;
            TestConnection response = new TestConnection(null);

            response.ProcessRequest(ScannerCommands.Unknown, new Dictionary<string, string>());
            string result = Encoding.ASCII.GetString(response.finalResponse);

            Assert.IsTrue(result.StartsWith(ResponseConstants.FailString));
        }

        private class TestConnection : DesktopConnection
        {
            public string extractData;

            public ScannerCommands processCommand;
            public Dictionary<string, string> processParameters;

            public byte[] finalResponse;

            public TestConnection(TcpClient client) : base() { }

            public void ExtractRequest(string instrusction)
            {
                ExtractRequest(instrusction, null);
            }
            public void ProcessRequest(ScannerCommands command, Dictionary<string, string> parameters)
            {
                base.ProcessRequest(command, parameters, null);
            }

            protected override void ExtractRequest(string instruction, NetworkStream stream)
            {
                extractData = instruction;
                base.ExtractRequest(instruction, stream);
            }

            protected override void ProcessRequest(ScannerCommands command, Dictionary<string, string> parameters,
                NetworkStream stream)
            {
                processCommand = command;
                processParameters = parameters;

                base.ProcessRequest(command, parameters, null);
            }

            protected override void SendResponse(NetworkStream stream, byte[] data)
            {
                finalResponse = data;
            }
        }

        private class UnknownResponse : IResponse
        {
            public bool throwUnknown = false;

            public UnknownResponse()
            {
                DesktopThread.Responders.Add(ScannerCommands.Unknown, this);
            }

            public byte[] GenerateResponse(ScannerCommands command, Dictionary<string, string> parameters)
            {
                if(throwUnknown) throw new UnknownResponseException();

                throw new Exception();
            }

            public void Reset()
            {
            }
        }

        /// <summary>
        /// Sets the Desktop Thread instance to null so that it can be reinitialised
        /// </summary>
        private class TestDesktopThread: DesktopThread
        {
            public TestDesktopThread()
            {
                Instance = null;
            }
        }
    }
}
