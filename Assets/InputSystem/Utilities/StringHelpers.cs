using System;
using System.Runtime.InteropServices;

namespace ISX.Utilities
{
    internal static class StringHelpers
    {
        public static int CountOccurrences(this string str, char ch)
        {
            if (str == null)
                return 0;

            var length = str.Length;
            var index = 0;
            var count = 0;

            while (index < length)
            {
                var nextIndex = str.IndexOf(ch, index);
                if (nextIndex == -1)
                    break;

                ++count;
                index = nextIndex + 1;
            }

            return count;
        }

        // Parse an int at the given position in the string.
        // Unlike int.Parse(), does not require allocating a new string containing only
        // the substring with the number.
        public static int ParseInt(string str, int pos)
        {
            var multiply = 1;
            var result = 0;
            var length = str.Length;

            while (pos < length)
            {
                var ch = str[pos];
                var digit = ch - '0';
                if (digit < 0 || digit > 9)
                    break;

                result = result * multiply + digit;

                multiply *= 10;
                ++pos;
            }

            return result;
        }

        public static unsafe bool WriteStringToBuffer(string text, IntPtr buffer, int bufferSize, ref uint offset)
        {
            if (buffer == IntPtr.Zero)
                throw new ArgumentNullException("buffer");

            var length = string.IsNullOrEmpty(text) ? 0 : text.Length;
            var endOffset = offset + sizeof(char) * length + sizeof(int);
            if (endOffset > bufferSize)
                return false;

            var ptr = ((byte*)buffer) + offset;
            *((int*)ptr) = length;
            ptr += sizeof(int);

            for (var i = 0; i < length; ++i, ptr += sizeof(char))
                *((char*)ptr) = text[i];

            offset = (uint)endOffset;
            return true;
        }

        public static unsafe string ReadStringFromBuffer(IntPtr buffer, int bufferSize, ref uint offset)
        {
            if (buffer == IntPtr.Zero)
                throw new ArgumentNullException("buffer");

            if (offset + sizeof(int) > bufferSize)
                return null;

            var ptr = ((byte*)buffer) + offset;
            var length = *((int*)ptr);
            ptr += sizeof(int);

            var endOffset = offset + sizeof(char) * length + sizeof(int);
            if (endOffset > bufferSize)
                return null;

            var text = Marshal.PtrToStringUni(new IntPtr(ptr), length);

            offset = (uint)endOffset;
            return text;
        }
    }
}
