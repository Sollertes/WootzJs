﻿using System;
using System.Runtime.WootzJs;

namespace WootzJs.Compiler.Tests
{
    [TestFixture]
    public class StringTests
    {
        [Test]
        public void ToUpper()
        {
            var s = "foo";
            s = s.ToUpper();
            QUnit.AreEqual(s, "FOO");
        }

        [Test]
        public void ToLower()
        {
            var s = "FOO";
            s = s.ToLower();
            QUnit.AreEqual(s, "foo");
        }

        [Test]
        public void Length()
        {
            var s = "FOO";
            int length = s.Length;
            QUnit.AreEqual(length, 3);
        }

        [Test]
        public void EndsWith()
        {
            var s = "HelloWorld";
            QUnit.IsTrue(s.EndsWith("World"));
        }

        [Test]
        public void StartsWith()
        {
            var s = "HelloWorld";
            QUnit.IsTrue(s.StartsWith("Hello"));
        }

        [Test]
        public void Compare()
        {
            QUnit.AreEqual(string.Compare("a", "b"), -1);
            QUnit.AreEqual(string.Compare("b", "a"), 1);
            QUnit.AreEqual(string.Compare("a", "a"), 0);
        }

        [Test]
        public void IndexOf()
        {
            var s = "12341234";
            String two = "2";
            QUnit.AreEqual(s.IndexOf(two), 1);
            QUnit.AreEqual(s.IndexOf("2", 4), 5);
            QUnit.AreEqual(s.IndexOf('2'), 1);
            QUnit.AreEqual(s.IndexOf('2', 4), 5);
        }

        [Test]
        public void LastIndexOf()
        {
            var s = "12341234";
            QUnit.AreEqual(s.LastIndexOf("2"), 5);
            QUnit.AreEqual(s.LastIndexOf("2", 4), 1);
            QUnit.AreEqual(s.LastIndexOf('2'), 5);
            QUnit.AreEqual(s.LastIndexOf('2', 4), 1);
        }

        [Test]
        public void Substring()
        {
            var s = "12341234";
            QUnit.AreEqual(s.Substring(4, 2), "12");
            QUnit.AreEqual(s.Substring(6), "34");
        }

        [Test]
        public void SplitCharArray()
        {
            var s = "12a34b56c78";
            var parts = s.Split('a', 'b', 'c');
            QUnit.AreEqual(parts.Length, 4);
            QUnit.AreEqual(parts[0], "12");
            QUnit.AreEqual(parts[1], "34");
            QUnit.AreEqual(parts[2], "56");
            QUnit.AreEqual(parts[3], "78");
        }

        [Test]
        public void SplitCharArrayWithCount()
        {
            var s = "12a34b56c78";
            var parts = s.Split(new[] { 'a', 'b', 'c' }, 3);
            QUnit.AreEqual(parts.Length, 3);    
            QUnit.AreEqual(parts[0], "12");
            QUnit.AreEqual(parts[1], "34");
            QUnit.AreEqual(parts[2], "56");
        }

        [Test]
        public void CharCode()
        {
            char b = 'b';
            char a = 'a';
            int i = b - a;
            QUnit.AreEqual(i, 1);
        }

        [Test]
        public void PreIncrementCharacter()
        {
            char b = 'b';
            var c = ++b;
            QUnit.AreEqual(b, 'c');
            QUnit.AreEqual(c, 'c');
        }

        [Test]
        public void PostIncrementCharacter()
        {
            char b = 'b';
            var stillB = b++;
            QUnit.AreEqual(b, 'c');
            QUnit.AreEqual(stillB, 'b');
        }
    }
}