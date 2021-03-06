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
    public class TypeTests
    {
        [Test]
        public void FullName()
        {
            var type = typeof(TestClass);
            QUnit.AreEqual(type.FullName, "WootzJs.Compiler.Tests.TypeTests.TestClass");
        }                  

        [Test]
        public void InnerClassName()
        {
            var type = typeof(ClassWithInnerClass.InnerClass);
            QUnit.AreEqual(type.Name, "InnerClass");
        }                  

        [Test]
        public void BaseType()
        {
            var type = typeof(SubClass);
            QUnit.AreEqual(type.FullName, "WootzJs.Compiler.Tests.TypeTests.SubClass");
            QUnit.AreEqual(type.BaseType.FullName, "WootzJs.Compiler.Tests.TypeTests.TestClass");
        }                  

        [Test]
        public void IsAssignableFrom()
        {
            var subClass = typeof(SubClass);
            var baseClass = typeof(TestClass);
            QUnit.IsTrue(baseClass.IsAssignableFrom(subClass));
        }                  

        [Test]
        public void IsInstanceOfType()
        {
            var subClass = new SubClass();
            var baseClass = typeof(TestClass);
            QUnit.IsTrue(baseClass.IsInstanceOfType(subClass));
        }                  

        [Test]
        public void GetFields()
        {
            var fields = typeof(FieldsClass).GetFields();
            QUnit.AreEqual(fields.Length, 1);
            var field = fields[0];
            QUnit.AreEqual(field.Name, "MyString");
        }        
        
        [Test]
        public void GetTypeByName()
        {
            var type = Type.GetType("WootzJs.Compiler.Tests.TypeTests.TestClass");
            QUnit.IsTrue(type != null);
        }

        [Test]
        public void TypeOfIntArray()
        {
            var type = typeof(int[]);
            QUnit.AreEqual(type.Name, "Int32[]");
        }

        [Test]
        public void Casts()
        {
            object o = null;
            o = (int)o;
            o = (string)o;
            o = (int[])o;
            o = (float[])o;
            o = (bool[])o;
            QUnit.IsTrue(true);
        }

        public class TestClass
        {
        }

        public class SubClass : TestClass
        {
        }

        public class FieldsClass
        {
            public string MyString;
        }

        public class ClassWithInnerClass
        {
            public class InnerClass
            {
            }
        }
    }
}
