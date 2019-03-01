// Original author: Nicolas Gadenne (contact@gaddygames.com) 
// https://github.com/snozbot/StringBuilder

// MIT License
// 
// Copyright (c) 2017 snozbot
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections;
using System.Collections.Generic;

namespace Apkd
{
    public sealed class StringBuilder : IList<char>, IReadOnlyList<char>
    {
        string _cachedString = "";
        char[] _buffer = null;
        int _bufferPos = 0;
        List<char> _temp;

        public StringBuilder(int capacity = 64)
            => _buffer = new char[capacity];

        public bool IsEmpty() => _bufferPos == 0;

        public int Length => _bufferPos;
        public int Capacity => _buffer.Length;

        [System.Runtime.CompilerServices.IndexerName("Chars")]
        public char this[int index]
        {
            get => _buffer[index];
            set
            {
                _buffer[index] = value;
                _cachedString = null;
            }
        }

        /// <summary> Return the string result </summary>
        public override string ToString()
        {
            if (_cachedString == null)
                _cachedString = new string(_buffer, 0, _bufferPos);

            return _cachedString;
        }

        /// <summary> Clears the StringBuilder instance (preserving allocated capacity) </summary>
        public StringBuilder Clear()
        {
            _bufferPos = 0;
            _cachedString = null;
            return this;
        }

        /// <summary> Insert string at given index </summary>
        public StringBuilder Insert(int index, string text)
        {
            AddLength(text.Length);
            _temp = _temp ?? new List<char>(Capacity);
            _temp.Clear();
            _temp.AddRange(_buffer);
            _temp.RemoveRange(_bufferPos, _temp.Count - _bufferPos);
            for (int i = 0; i < text.Length; ++i)
                _temp.Insert(index + i, text[i]);
            _temp.CopyTo(_buffer);
            _bufferPos += text.Length;
            _cachedString = null;

            return this;
        }

        /// <summary> Insert character at given index </summary>
        public StringBuilder Insert(int index, char character)
        {
            AddLength(1);
            _temp = _temp ?? new List<char>(Capacity);
            _temp.Clear();
            _temp.AddRange(_buffer);
            _temp.RemoveRange(_bufferPos, _temp.Count - _bufferPos);
            _temp.Insert(index, character);
            _temp.CopyTo(_buffer);
            _bufferPos += 1;
            _cachedString = null;

            return this;
        }

        /// <summary> Remove characters starting from specified index </summary>
        public StringBuilder Remove(int index, int count)
        {
            _temp = _temp ?? new List<char>(Capacity);
            _temp.Clear();
            _temp.AddRange(_buffer);
            _temp.RemoveRange(_bufferPos, _temp.Count - _bufferPos);
            _temp.RemoveRange(index, count);
            _temp.CopyTo(_buffer);
            _bufferPos -= count;
            _cachedString = null;

            return this;
        }

        /// <summary> Append the content of another StringBuilder instance </summary>
        public StringBuilder Append(StringBuilder other)
        {
            if (other._bufferPos == 0)
                return this;

            AddLength(other._buffer.Length);
            other._buffer.CopyTo(_buffer, _bufferPos);
            _cachedString = null;
            _bufferPos += other._bufferPos;

            return this;
        }

        /// <summary> Append a string </summary>
        public StringBuilder Append(string value)
        {
            if (string.IsNullOrEmpty(value))
                return this;

            int n = value.Length;
            AddLength(n);
            for (int i = 0; i < n; i++)
                _buffer[_bufferPos + i] = value[i];
            _bufferPos += n;
            _cachedString = null;

            return this;
        }

        /// <summary> Append a substring of a string </summary>
        public StringBuilder Append(string value, int valueStartIndex = 0, int? valueLength = default)
        {
            if (string.IsNullOrEmpty(value))
                return this;

            if (valueLength == 0)
                return this;

            int n = System.Math.Min(value.Length, valueLength ?? value.Length - valueStartIndex);

            if (valueStartIndex < 0)
                throw new System.ArgumentOutOfRangeException(nameof(valueStartIndex));

            if (valueLength < 0)
                throw new System.ArgumentOutOfRangeException(nameof(valueLength));

            AddLength(n);
            for (int i = 0; i < n; ++i)
                _buffer[_bufferPos + i] = value[valueStartIndex + i];
            _bufferPos += n;
            _cachedString = null;

            return this;
        }

        /// <summary> Append a character </summary>
        public StringBuilder Append(char c)
        {
            AddLength(1);
            _buffer[_bufferPos] = c;
            _bufferPos += 1;
            _cachedString = null;
            return this;
        }
        
        /// <summary> Append a character </summary>
        public StringBuilder Append(char c, int repeat)
        {
            AddLength(repeat);
            for (int i = 0; i < repeat; i++)
                _buffer[_bufferPos + i] = c;
            _bufferPos += repeat;
            _cachedString = null;
            return this;
        }

        /// <summary> Append an object (calls .ToString()) </summary>
        public StringBuilder Append(object value)
        {
            Append(value.ToString());
            return this;
        }

        /// <summary> Append an int without memory allocation </summary>
        public StringBuilder Append(int value)
        {
            // Allocate enough memory to handle any int number
            AddLength(16);

            // Handle the negative case
            if (value < 0)
            {
                value = -value;
                _buffer[_bufferPos++] = '-';
            }

            // Copy the digits in reverse order
            int nbChars = 0;
            do
            {
                _buffer[_bufferPos++] = (char)('0' + value % 10);
                value /= 10;
                nbChars++;
            } while (value != 0);

            // Reverse the result
            for (int i = nbChars / 2 - 1; i >= 0; i--)
            {
                char c = _buffer[_bufferPos - i - 1];
                _buffer[_bufferPos - i - 1] = _buffer[_bufferPos - nbChars + i];
                _buffer[_bufferPos - nbChars + i] = c;
            }
            _cachedString = null;
            return this;
        }

        /// <summary> Append a float without memory allocation. </summary>
        public StringBuilder Append(float valueF)
        {
            double value = valueF;
            _cachedString = null;
            AddLength(32); // Check we have enough buffer allocated to handle any float number

            // Handle the 0 case
            if (value == 0)
            {
                _buffer[_bufferPos++] = '0';
                return this;
            }

            // Handle the negative case
            if (value < 0)
            {
                value = -value;
                _buffer[_bufferPos++] = '-';
            }

            // Get the 7 meaningful digits as a long
            int nbDecimals = 0;
            while (value < 1000000)
            {
                value *= 10;
                nbDecimals++;
            }
            long valueLong = (long)System.Math.Round(value);

            // Parse the number in reverse order
            int nbChars = 0;
            bool isLeadingZero = true;
            while (valueLong != 0 || nbDecimals >= 0)
            {
                // We stop removing leading 0 when non-0 or decimal digit
                if (valueLong % 10 != 0 || nbDecimals <= 0)
                    isLeadingZero = false;

                // Write the last digit (unless a leading zero)
                if (!isLeadingZero)
                    _buffer[_bufferPos + (nbChars++)] = (char)('0' + valueLong % 10);

                // Add the decimal point
                if (--nbDecimals == 0 && !isLeadingZero)
                    _buffer[_bufferPos + (nbChars++)] = '.';

                valueLong /= 10;
            }
            _bufferPos += nbChars;

            // Reverse the result
            for (int i = nbChars / 2 - 1; i >= 0; i--)
            {
                char c = _buffer[_bufferPos - i - 1];
                _buffer[_bufferPos - i - 1] = _buffer[_bufferPos - nbChars + i];
                _buffer[_bufferPos - nbChars + i] = c;
            }

            return this;
        }

        /// <summary> Replace all occurences of a character </summary>
        public StringBuilder Replace(char a, char b)
        {
            for (int i = 0; i < _bufferPos; ++i)
                if (_buffer[i] == a)
                    _buffer[i] = b;

            _cachedString = null;
            return this;
        }

        /// <summary> Replace all occurences of a string by another one </summary>
        public StringBuilder Replace(string oldStr, string newStr)
        {
            if (newStr == null)
                throw new System.ArgumentNullException(nameof(oldStr));

            if (newStr == null)
                throw new System.ArgumentNullException(nameof(newStr));

            if (_bufferPos == 0)
                return this;

            _temp = _temp ?? new List<char>(Capacity);
            _temp.Clear();

            // Create the new string into _temp
            for (int i = 0; i < _bufferPos; i++)
            {
                bool isToReplace = false;
                if (_buffer[i] == oldStr[0]) // If first character found, check for the rest of the string to replace
                {
                    int k = 1;
                    while (k < oldStr.Length && _buffer[i + k] == oldStr[k])
                        k++;
                    isToReplace = (k >= oldStr.Length);
                }
                if (isToReplace) // Do the replacement
                {
                    i += oldStr.Length - 1;
                    for (int k = 0; k < newStr.Length; k++)
                        _temp.Add(newStr[k]);
                }
                else // No replacement, copy the old character
                {
                    _temp.Add(_buffer[i]);
                }
            }

            // Copy back the new string into m_chars
            AddLength(_temp.Count - _bufferPos);
            _temp.CopyTo(_buffer);
            _bufferPos = _temp.Count;
            _cachedString = null;
            return this;
        }

        public bool StartsWith(string value)
             => StartsWith(value, 0, false);

        public bool StartsWith(string value, bool ignoreCase)
            => StartsWith(value, 0, ignoreCase);

        public bool StartsWith(string value, int startIndex = 0, bool ignoreCase = false)
        {
            int length = value.Length;
            int n = startIndex + length;
            if (ignoreCase == false)
            {
                for (int i = startIndex; i < n; i++)
                    if (_buffer[i] != value[i - startIndex])
                        return false;
            }
            else
            {
                for (int j = startIndex; j < n; j++)
                    if (char.ToLower(_buffer[j]) != char.ToLower(value[j - startIndex]))
                        return false;
            }
            return true;
        }

        /// <summary> Increase the buffer capacity if necessary </summary>
        void AddLength(int charsToAdd)
        {
            if (_bufferPos + charsToAdd <= Capacity)
                return;

            int newCapacity = System.Math.Max(_bufferPos + charsToAdd, Capacity * 2);
            char[] newBuffer = new char[newCapacity];
            _buffer.CopyTo(newBuffer, 0);
            _buffer = newBuffer;
        }

        public int IndexOf(char value)
             => IndexOf(value, 0);

        public int IndexOf(char value, int startIndex)
        {
            for (int i = startIndex, n = _bufferPos; i < n; i++)
                if (_buffer[i] == value)
                    return i;
            return -1;
        }

        public int IndexOf(string value)
             => IndexOf(value, 0, false);

        public int IndexOf(string value, int startIndex)
            => IndexOf(value, startIndex, false);

        public int IndexOf(string value, bool ignoreCase)
            => IndexOf(value, 0, ignoreCase);

        public int IndexOf(string value, int startIndex, bool ignoreCase)
        {
            int length = value.Length;
            int lengthDelta = (_bufferPos - length) + 1;
            if (ignoreCase == false)
            {
                for (int i = startIndex; i < lengthDelta; i++)
                {
                    if (_buffer[i] == value[0])
                    {
                        int n = 1;
                        while ((n < length) && (_buffer[i + n] == value[n]))
                            n++;

                        if (n == length)
                            return i;
                    }
                }
            }
            else
            {
                for (int j = startIndex; j < lengthDelta; j++)
                {
                    if (char.ToLower(_buffer[j]) == char.ToLower(value[0]))
                    {
                        int n = 1;
                        while ((n < length) && (char.ToLower(_buffer[j + n]) == char.ToLower(value[n])))
                            n++;

                        if (n == length)
                            return j;
                    }
                }
            }
            return -1;
        }

        int ICollection<char>.Count => _bufferPos;

        int IReadOnlyCollection<char>.Count => _bufferPos;

        bool ICollection<char>.IsReadOnly => false;

        void IList<char>.RemoveAt(int index) => Remove(index, 1);

        void IList<char>.Insert(int index, char item) => Insert(index, item);

        void ICollection<char>.Clear() => Clear();

        void ICollection<char>.Add(char character) => Append(character);

        bool ICollection<char>.Contains(char item) => IndexOf(item) != -1;

        public void CopyTo(char[] array, int arrayIndex)
            => System.Array.Copy(
                sourceArray: _buffer,
                sourceIndex: 0,
                destinationArray: array,
                destinationIndex: arrayIndex,
                length: _bufferPos);

        bool ICollection<char>.Remove(char item)
        {
            int index = IndexOf(item);
            if (index == -1)
                return false;
            Remove(index, 1);
            return true;
        }

        public List<char>.Enumerator GetEnumerator()
        {
            _temp = _temp ?? new List<char>(Capacity);
            _temp.Clear();
            _temp.AddRange(_buffer);
            _temp.RemoveRange(_bufferPos, _temp.Count - _bufferPos);
            return _temp.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
            => (this as IEnumerable<char>).GetEnumerator();

        IEnumerator<char> IEnumerable<char>.GetEnumerator()
            => GetEnumerator();

        public static implicit operator string(StringBuilder builder)
            => builder.ToString();
    }
}
