#region License
//-----------------------------------------------------------------------
// <copyright>
// The MIT License (MIT)
// 
// Copyright (c) 2014 Kirk S Woll
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------
#endregion

using System;
using System.Runtime.WootzJs;

namespace WootzJs.Compiler.Tests
{
    [TestFixture]
    public class NumberTests
    {
        [Test]
        public void ToHex()
        {
            var number = 20;
            QUnit.AreEqual(number.ToString("X4"), "0014");
        }

        [Test]
        public void IntTryParse()
        {
            var s = "1";
            int i;
            QUnit.IsTrue(int.TryParse(s, out i));
            QUnit.AreEqual(i, 1);
        }

        [Test]
        public void TruncFloatViaCast()
        {
            var f = 1.234f;
            var i = (int)f;
            QUnit.AreEqual(i, 1);
        }

        [Test]
        public void ToLocaleString()
        {
            var s = 1.234.As<JsNumber>().toLocaleString();
            QUnit.AreEqual(s, "1.234");
        }

        [Test]
        public void FormatException()
        {
            try
            {
                int.Parse("a");
                QUnit.IsTrue(false);
            }
            catch (FormatException e)
            {
                QUnit.IsTrue(true);                
            }
        }
    }
}
