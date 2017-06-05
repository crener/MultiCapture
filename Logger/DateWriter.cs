using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    class DateWriter : TextWriter
    {
        public override Encoding Encoding { get; }

        private TextWriter writer;

        public DateWriter(TextWriter realWriter)
        {
            writer = realWriter;
            Encoding = realWriter.Encoding;
        }

        public override void Close()
        {
            writer.Close();
        }

        protected override void Dispose(bool disposing)
        {
            writer.Dispose();
        }

        public override void Flush()
        {
            writer.Flush();
        }

        public override void Write(char value)
        {
            writer.Write(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void Write(bool value)
        {
            writer.Write(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void Write(int value)
        {
            writer.Write(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void Write(uint value)
        {
            writer.Write(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void Write(long value)
        {
            writer.Write(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void Write(ulong value)
        {
            writer.Write(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void Write(float value)
        {
            writer.Write(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void Write(double value)
        {
            writer.Write(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void Write(decimal value)
        {
            writer.Write(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void Write(string value)
        {
            writer.Write(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void Write(object value)
        {
            writer.Write(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void WriteLine()
        {
            writer.WriteLine();
        }

        public override void WriteLine(char value)
        {
            writer.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void WriteLine(bool value)
        {
            writer.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void WriteLine(int value)
        {
            writer.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void WriteLine(uint value)
        {
            writer.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void WriteLine(long value)
        {
            writer.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void WriteLine(ulong value)
        {
            writer.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void WriteLine(float value)
        {
            writer.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void WriteLine(double value)
        {
            writer.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void WriteLine(decimal value)
        {
            writer.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void WriteLine(string value)
        {
            writer.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override void WriteLine(object value)
        {
            writer.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override Task WriteAsync(char value)
        {
            return writer.WriteAsync(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override Task WriteAsync(string value)
        {
            return writer.WriteAsync(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override Task WriteLineAsync(char value)
        {
            return writer.WriteLineAsync(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override Task WriteLineAsync(string value)
        {
            return base.WriteLineAsync(DateTime.Now.ToString("HH:mm:ss") + " - " + value);
        }

        public override Task WriteLineAsync()
        {
            return writer.WriteLineAsync();
        }

        public override Task FlushAsync()
        {
            return writer.FlushAsync();
        }
    }
}
