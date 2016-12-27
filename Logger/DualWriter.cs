using System;
using System.Text;
using System.IO;

namespace Logger
{
    class DualWriter : TextWriter, IDisposable
    {
        TextWriter primary, secondary;

        public DualWriter(TextWriter primary, TextWriter secondary)
        {
            this.primary = primary;
            this.secondary = secondary;
        }

        public override Encoding Encoding
        {
            get
            {
                return primary.Encoding;
            }
        }

        public override void Write(string text)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text);
            secondary.Write(text);
        }

        public override void Write(char text)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text);
            secondary.Write(text);
        }

        public override void Write(int text)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text);
            secondary.Write(text);
        }

        public override void Write(char[] text)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text);
            secondary.Write(text);
        }

        public override void Write(char[] text, int num, int num2)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text, num, num2);
            secondary.Write(text, num, num2);
        }

        public override void Write(Int64 text)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text);
            secondary.Write(text);
        }

        public override void Write(double text)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text);
            secondary.Write(text);
        }

        public override void Write(string text, Object obj)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text, obj);
            secondary.Write(text, obj);
        }

        public override void Write(string text, Object[] obj)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text, obj);
            secondary.Write(text, obj);
        }

        public override void Write(string text, Object obj, Object obj2)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text, obj, obj2);
            secondary.Write(text, obj, obj2);
        }

        public override void Write(string text, Object obj, Object obj2, Object obj3)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text, obj, obj2, obj3);
            secondary.Write(text, obj, obj2, obj3);
        }

        public override void Write(Object text)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text);
            secondary.Write(text);
        }

        public override void Write(Single text)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text);
            secondary.Write(text);
        }

        public override void Write(bool text)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text);
            secondary.Write(text);
        }

        public override void Write(UInt64 text)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text);
            secondary.Write(text);
        }

        public override void Write(uint text)
        {
            primary.Write(DateTime.Now.ToString("hh:mm:ss") + " - " + text);
            secondary.Write(text);
        }

        public override void WriteLine()
        {
            primary.WriteLine();
            secondary.WriteLine();
        }

        public override void WriteLine(bool value)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + value);
            secondary.WriteLine(value);
        }

        public override void WriteLine(char value)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + value);
            secondary.WriteLine(value);
        }

        public override void WriteLine(char[] buffer)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + buffer);
            secondary.WriteLine(buffer);
        }

        public override void WriteLine(decimal value)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + value);
            secondary.WriteLine(value);
        }

        public override void WriteLine(double value)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + value);
            secondary.WriteLine(value);
        }

        public override void WriteLine(float value)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + value);
            secondary.WriteLine(value);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + buffer, index, count);
            secondary.WriteLine(buffer, index, count);
        }

        public override void WriteLine(int value)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + value);
            secondary.WriteLine(value);
        }

        public override void WriteLine(long value)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + value);
            secondary.WriteLine(value);
        }

        public override void WriteLine(object value)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + value);
            secondary.WriteLine(value);
        }

        public override void WriteLine(string format, object arg0)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + format, arg0);
            secondary.WriteLine(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + format, arg0, arg1);
            secondary.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + format, arg0, arg1, arg2);
            secondary.WriteLine(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string value)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + value);
            secondary.WriteLine(value);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + format, arg);
            secondary.WriteLine(format, arg);
        }

        public override void WriteLine(ulong value)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + value);
            secondary.WriteLine(value);
        }

        public override void WriteLine(uint value)
        {
            primary.WriteLine(DateTime.Now.ToString("hh:mm:ss") + " - " + value);
            secondary.WriteLine(value);
        }

        public override void Close()
        {
            primary.Close();
            secondary.Close();
        }

        public void Dispose()
        {
            primary.Dispose();
            secondary.Dispose();
        }
    }
}
