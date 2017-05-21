using System.Text;
using SharedDeviceItems;
using SharedDeviceItems.Exceptions;

namespace Hub.Helpers
{
    /// <summary>
    /// Abstracts the created of byte arrays for command requests from cameras
    /// </summary>
    public class CommandBuilder
    {
        private string command = "";

        static CommandBuilder() { }

        public CommandBuilder Request(CameraRequest request)
        {
            command += (int)request;
            return this;
        }

        public CommandBuilder AddParam(string key, string value)
        {
            if (key.Contains(Constants.ParamSeparator)) throw new CommandException("key cannot contain the parameter seperator");
            if (key.Contains(Constants.MessageSeparator)) throw new CommandException("key cannot contain the message seperator");
            if (key.Contains(Constants.EndOfMessage)) throw new CommandException("key cannot contain the end of message key");
            if (key.Contains(Constants.ParamKeyValueSeparator)) throw new CommandException("key cannot contain the key value seperator key");

            if (value.Contains(Constants.ParamSeparator)) throw new CommandException("value cannot contain the parameter seperator");
            if (value.Contains(Constants.MessageSeparator)) throw new CommandException("value cannot contain the message seperator");
            if (value.Contains(Constants.EndOfMessage)) throw new CommandException("value cannot contain the end of message key");
            if (value.Contains(Constants.ParamKeyValueSeparator)) throw new CommandException("value cannot contain the key value seperator key");

            command += Constants.ParamSeparator + key + Constants.ParamKeyValueSeparator + value;
            return this;
        }

        public byte[] Build()
        {
            command += Constants.EndOfMessage;
            return Encoding.ASCII.GetBytes(command);
        }
    }
}
