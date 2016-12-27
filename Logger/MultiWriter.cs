using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Logger
{
    class MultiWriter : TextWriter
    {
        TextWriter primary;
        ICollection<TextWriter> collection;

        public MultiWriter(TextWriter primary, ICollection<TextWriter> collection)
        {
            this.primary = primary;
            this.collection = collection;
        }

        public override Encoding Encoding
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Close()
        {
            primary.Close();
            foreach (TextWriter writer in collection) writer.Close();
        }

        public override void Flush()
        {
            primary.Flush();
            foreach (TextWriter writer in collection) writer.Flush();
        }

        public override Task FlushAsync()
        {
            foreach (TextWriter writer in collection) writer.FlushAsync();
            return base.FlushAsync();
        }

        public override IFormatProvider FormatProvider
        {
            get
            {
                return primary.FormatProvider;
            }
        }

        public override string NewLine
        {
            get
            {
                return primary.NewLine;
            }

            set
            {
                foreach (TextWriter writer in collection) writer.NewLine = value;
                base.NewLine = value;
            }
        }

        public override void Write(bool value)
        {
            primary.Write(value);
            foreach (TextWriter writer in collection) writer.Write(value);
        }

        public override void Write(char value)
        {
            primary.Write(value);
            foreach (TextWriter writer in collection) writer.Write(value);
        }

        public override void Write(char[] buffer)
        {
            primary.Write(buffer);
            foreach (TextWriter writer in collection) writer.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            primary.Write(buffer, index, count);
            foreach (TextWriter writer in collection) writer.Write(buffer, index, count);
        }

        public override void Write(decimal value)
        {
            primary.Write(value);
            foreach (TextWriter writer in collection) writer.Write(value);
        }

        public override void Write(double value)
        {
            primary.Write(value);
            foreach (TextWriter writer in collection) writer.Write(value);
        }

        public override void Write(float value)
        {
            primary.Write(value);
            foreach (TextWriter writer in collection) writer.Write(value);
        }

        public override void Write(object value)
        {
            primary.Write(value);
            foreach (TextWriter writer in collection) writer.Write(value);
        }

        public override void Write(long value)
        {
            primary.Write(value);
            foreach (TextWriter writer in collection) writer.Write(value);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            primary.Write(format, arg0, arg1, arg2);
            foreach (TextWriter writer in collection) writer.Write(format, arg0, arg1, arg2);
        }

        public override void Write(int value)
        {
            primary.Write(value);
            foreach (TextWriter writer in collection) writer.Write(value);
        }

        public override void Write(string format, object arg0)
        {
            primary.Write(format, arg0);
            foreach (TextWriter writer in collection) writer.Write(format, arg0);
        }

        public override void Write(string value)
        {
            primary.Write(value);
            foreach (TextWriter writer in collection) writer.Write(value);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            primary.Write(format, arg0, arg1);
            foreach (TextWriter writer in collection) writer.Write(format, arg0, arg1);
        }

        public override void Write(string format, params object[] arg)
        {
            primary.Write(format, arg);
            foreach (TextWriter writer in collection) writer.Write(format, arg);
        }

        public override void Write(uint value)
        {
            primary.Write(value);
            foreach (TextWriter writer in collection) writer.Write(value);
        }

        public override void Write(ulong value)
        {
            primary.Write(value);
            foreach (TextWriter writer in collection) writer.Write(value);
        }

        public override Task WriteAsync(char value)
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(string value)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine()
        {
            primary.WriteLine();
            foreach (TextWriter writer in collection) writer.WriteLine();
        }

        public override void WriteLine(bool value)
        {
            primary.WriteLine(value);
            foreach (TextWriter writer in collection) writer.WriteLine(value);
        }

        public override void WriteLine(char value)
        {
            primary.WriteLine(value);
            foreach (TextWriter writer in collection) writer.WriteLine(value);
        }

        public override void WriteLine(char[] buffer)
        {
            primary.WriteLine(buffer);
            foreach (TextWriter writer in collection) writer.WriteLine(buffer);
        }

        public override void WriteLine(decimal value)
        {
            primary.WriteLine(value);
            foreach (TextWriter writer in collection) writer.WriteLine(value);
        }

        public override void WriteLine(double value)
        {
            primary.WriteLine(value);
            foreach (TextWriter writer in collection) writer.WriteLine(value);
        }

        public override void WriteLine(float value)
        {
            primary.WriteLine(value);
            foreach (TextWriter writer in collection) writer.WriteLine(value);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            primary.WriteLine(buffer, index, count);
            foreach (TextWriter writer in collection) writer.WriteLine(buffer, index, count);
        }

        public override void WriteLine(int value)
        {
            primary.WriteLine(value);
            foreach (TextWriter writer in collection) writer.WriteLine(value);
        }

        public override void WriteLine(long value)
        {
            primary.WriteLine(value);
            foreach (TextWriter writer in collection) writer.WriteLine(value);
        }

        public override void WriteLine(object value)
        {
            primary.WriteLine(value);
            foreach (TextWriter writer in collection) writer.WriteLine(value);
        }

        public override void WriteLine(string format, object arg0)
        {
            primary.WriteLine(format, arg0);
            foreach (TextWriter writer in collection) writer.WriteLine(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            primary.WriteLine(format, arg0, arg1);
            foreach (TextWriter writer in collection) writer.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            primary.WriteLine(format, arg0, arg1, arg2);
            foreach (TextWriter writer in collection) writer.WriteLine(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string value)
        {
            primary.WriteLine(value);
            foreach (TextWriter writer in collection) writer.WriteLine(value);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            primary.WriteLine(format, arg);
            foreach (TextWriter writer in collection) writer.WriteLine(format, arg);
        }

        public override void WriteLine(ulong value)
        {
            primary.WriteLine(value);
            foreach (TextWriter writer in collection) writer.WriteLine(value);
        }

        public override void WriteLine(uint value)
        {
            primary.WriteLine(value);
            foreach (TextWriter writer in collection) writer.WriteLine(value);
        }

        public override Task WriteLineAsync(char value)
        {
            throw new NotImplementedException();
        }

        public override Task WriteLineAsync()
        {
            throw new NotImplementedException();
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override Task WriteLineAsync(string value)
        {
            throw new NotImplementedException();
        }
    }
}
