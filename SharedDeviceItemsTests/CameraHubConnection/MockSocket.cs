﻿using System;
using System.Net;
using System.Net.Sockets;
using Hub.Networking;

namespace SharedDeviceItemsTests.CameraHubConnection
{
    class MockSocket : ISocket
    {
        public byte[] SendData { get; set; }

        public byte[] RecieveData
        {
            get { return recieveData; }
            set
            {
                recieveData = value;
                RecievePosition = 0;
            }
        }

        public int RecievePosition { get; private set; }

        public bool DisconnectMidTransmission { get; set; }
        public bool OverridePollFalse { get; set; }
        public bool Closed { get; set; }

        private byte[] recieveData;

        public MockSocket()
        {
            Connected = true;
            OverridePollFalse = false;
        }

        public int ReceiveBufferSize { get; set; }
        public int ReceiveTimeout { get; set; }

        public void Connect(IPEndPoint endPoint)
        {
        }

        public void Send(byte[] data)
        {
            SendData = data;
        }

        public int Receive(byte[] buffer)
        {
            int length = recieveData.Length - RecievePosition;
            if (length > buffer.Length)
            {
                //send small chunk
                length = buffer.Length;
            }

            if (length == 0) throw new Exception("Tried to read data when there was none avaliable");
            if (DisconnectMidTransmission) Connected = false;

            Array.Copy(recieveData, RecievePosition, buffer, 0, length);
            RecievePosition += length;

            return length;
        }

        public void Shutdown(SocketShutdown shutdown)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            Closed = true;
        }

        public bool Connected { get; set; }

        public int Available
        {
            get
            {
                if(recieveData == null || !Connected) return 0;
                return recieveData.Length - RecievePosition;
            }
        }

        public bool Poll(int timout, SelectMode mode)
        {
            if(OverridePollFalse) return false;
            return Connected;
        }

        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback,
            object state)
        {
            throw new NotImplementedException();
        }

        public int EndReceive(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback,
            object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginConnect(EndPoint remoteEp, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public void Bind(IPEndPoint localEndPoint)
        {
            throw new NotImplementedException();
        }

        public void Listen(int i)
        {
            throw new NotImplementedException();
        }

        public ISocket Accept()
        {
            return this;
        }
    }
}
