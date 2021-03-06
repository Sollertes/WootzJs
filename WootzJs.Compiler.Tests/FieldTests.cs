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
    public class FieldTests
    {
        [Test]
        public void FieldsDoNotCollide()
        {
            var subClass = new SubClass();
            QUnit.AreEqual(subClass.GetFoo1(), "base");
            QUnit.AreEqual(subClass.GetFoo2(), "sub");
        }

        [Test]
        public void StaticFieldsAreStatic()
        {
            StaticFieldClass.MyField = "foo";
            QUnit.AreEqual(StaticFieldClass.MyField, "foo");
        }

        [Test]
        public void ReferencedField()
        {
            var reference = new ReferenceClass();
            reference.Target = new TargetClass();
            reference.Target.Foo = "foo";

            QUnit.AreEqual(reference.Target.Foo, "foo");
        }

        [Test]
        public void PrimitiveFields()
        {
            var primitiveFields = new PrimitiveFieldsClass();
            QUnit.AreEqual(primitiveFields.Boolean, false);
            QUnit.AreEqual(primitiveFields.Byte, 0);
            QUnit.AreEqual(primitiveFields.SByte, 0);
            QUnit.AreEqual(primitiveFields.Short, 0);
            QUnit.AreEqual(primitiveFields.UShort, 0);
            QUnit.AreEqual(primitiveFields.Int, 0);
            QUnit.AreEqual(primitiveFields.UInt, 0);
            QUnit.AreEqual(primitiveFields.Long, 0);
            QUnit.AreEqual(primitiveFields.ULong, 0);
            QUnit.AreEqual(primitiveFields.Enum, 0);
        }

        [Test]
        public void ConstField()
        {
            var value = ConstFieldClass.MyString;
            QUnit.AreEqual(value, "foo");
        }

        public class StaticFieldClass
        {
            public static string MyField;
        }

        public class ConstFieldClass
        {
            public const string MyString = "foo";
        }

        public class BaseClass : Object
        {
            private string foo;

            public BaseClass()
            {
                foo = "base";
            }

            public string GetFoo1()
            {
                return foo;
            }
        }

        public class SubClass : BaseClass
        {
            private string foo;

            public SubClass()
            {
                System.Runtime.CompilerServices.ExtensionAttribute x = null;
                foo = "sub";
            }

            public string GetFoo2()
            {
                GetFoo1();
                return foo;
            }
        }

        public class ReferenceClass
        {
            public TargetClass Target { get; set; }
        }

        public class TargetClass
        {
            public string Foo { get; set; }
        }

        public class PrimitiveFieldsClass
        {
            public bool Boolean;
            public byte Byte;
            public short Short;
            public int Int;
            public long Long;
            public sbyte SByte;
            public ushort UShort;
            public uint UInt;
            public ulong ULong;
            public TestEnum Enum;
        }

        public enum TestEnum {}
    }
}
