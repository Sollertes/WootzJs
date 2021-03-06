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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Runtime.WootzJs
{
    /// <summary>
    /// This is a special class handled by the compiler such that each method is declared as
    /// a global function.
    /// </summary>
    public static class SpecialFunctions
    {
        [Js(Name = "$define")]
        public static JsTypeFunction Define(string name, JsTypeFunction prototype)
        {
            JsTypeFunction typeFunction = null;

            // Create constructor function, which is a superconstructor that takes in the actual
            // constructor as the first argument, and the rest of the arguments are passed directly 
            // to that constructor.  These subconstructors are not Javascript constructors -- they 
            // are not called via new, they exist for initialization only.
            typeFunction = Jsni.function(constructor =>
            {
                if (constructor != null || !(Jsni.instanceof(Jsni.@this(), typeFunction)))
                {
                    Jsni.invoke(Jsni.member(typeFunction, SpecialNames.StaticInitializer));
                }
                if (constructor != null) 
                    Jsni.apply(constructor, Jsni.@this(), Jsni.call(Jsni.reference("Array.prototype.slice"), Jsni.arguments(), 1.As<JsObject>()).As<JsArray>());
                if (!Jsni.instanceof(Jsni.@this(), typeFunction))
                    return typeFunction;
                else
                    return Jsni.@this();
            }).As<JsTypeFunction>();
            Jsni.memberset(typeFunction, "toString", Jsni.function(() => name.As<JsObject>()));
            Jsni.memberset(typeFunction, SpecialNames.TypeName, name.As<JsString>());
            Jsni.memberset(typeFunction, "prototype", Jsni.@new(prototype));
            return typeFunction;
        }

        [Js(Name = "$definetypeparameter")]
        public static JsTypeFunction DefineTypeParameter(string name, JsTypeFunction prototype)
        {
            var result = Define(name, prototype);
            result.memberset(SpecialNames.CreateType, Jsni.function(() =>
            {
                var type = Type.CreateTypeParameter(name, prototype);
                result.Type = type;
                return result;
            }));
            return result;
        }

        [Js(Name = "$cast")]
        public static T ObjectCast<T>(object o)
        {
            if (o == null)
                return default(T);

            var type = o.GetType();
            if (!typeof(T).IsAssignableFrom(type))
                throw new InvalidCastException("Cannot cast object of type " + o.GetType().FullName + " to type " + typeof(T).FullName);

            return o.As<T>();
        }

        [Js(Name = "$delegate")]
        public static JsFunction CreateDelegate(JsObject thisExpression, JsTypeFunction delegateType, JsFunction lambda, string delegateKey = null)
        {
            if (delegateKey != null)
            {
                if (thisExpression[delegateKey])
                    return thisExpression[delegateKey].As<JsFunction>();
            }
            else
            {
                if (lambda.member("$delegate") != null)
                    return lambda.member("$delegate").As<JsFunction>();
            }

            JsFunction delegateFunc = null;
            delegateFunc = Jsni.function(() =>
            {
                return lambda.apply(delegateFunc.As<Delegate>().Target.As<JsObject>(), Jsni.arguments().As<JsArray>());
            });
            delegateFunc.prototype = Jsni.@new(delegateType);
            Jsni.type<object>().TypeInitializer.invoke(delegateFunc, delegateFunc);
            Jsni.type<Delegate>().TypeInitializer.invoke(delegateFunc, delegateFunc);
            Jsni.type<MulticastDelegate>().TypeInitializer.invoke(delegateFunc, delegateFunc);
            delegateType.TypeInitializer.invoke(delegateFunc, delegateFunc);
            Jsni.invoke(Jsni.member(Jsni.member(Jsni.type<MulticastDelegate>().prototype, "$ctor"), "call"), delegateFunc, thisExpression, new[] { delegateFunc }.As<JsArray>());
            Jsni.memberset(delegateFunc, SpecialNames.TypeField, delegateType);
            if (delegateKey != null)
                thisExpression[delegateKey] = delegateFunc;
            else
                lambda.memberset("$delegate", delegateFunc);
            return delegateFunc;
        }

        [Js(Name=SpecialNames.InitializeArray)]
        internal static JsArray InitializeArray(JsArray array, JsTypeFunction elementType)
        {
            if (array.member("$isInitialized"))
                return array;

            array.memberset("$isInitialized", true);
            var arrayType = MakeArrayType(elementType);
            foreach (var property in arrayType.member("prototype"))
            {
                array[property] = arrayType.member("prototype")[property];
            }

            arrayType.member("prototype").member("$ctor").member("call").invoke(array);

            return array;
        }

        [Js(Name = SpecialNames.MakeGenericTypeConstructor)]
        internal static JsTypeFunction MakeGenericTypeFactory(JsTypeFunction unconstructedType, JsArray typeArgs)
        {
            var cache = Jsni.member(unconstructedType, SpecialNames.TypeCache);
            if (cache == null)
            {
                cache = new JsObject();
                unconstructedType.memberset(SpecialNames.TypeCache, cache);
            }
            var keyArray = Jsni.call<JsArray>(x => x.slice(0), typeArgs, 0.As<JsNumber>()).As<JsArray>();
            var keyParts = new JsArray();
            for (var i = 0; i < keyArray.length; i++)
            {
                keyParts[i] = keyArray[i].member(SpecialNames.TypeName);
            }
            var keyString = keyParts.join(", ");
            var result = cache[keyString];
            if (result == null)
            {
                var lastIndexOfDollar = unconstructedType.TypeName.LastIndexOf('`');
                var newTypeName = unconstructedType.TypeName.Substring(0, lastIndexOfDollar) + "<" + keyString + ">";
                var prototype = unconstructedType.BaseType;
                if (prototype.member("$"))
                    prototype = prototype.member("$").apply(null, typeArgs).As<JsTypeFunction>();
                var generic = Define(newTypeName, prototype);
                generic.memberset(SpecialNames.UnconstructedType, unconstructedType);

                // unconstructedType.$TypeInitializer.apply(this, [generic, generic.prototype].concat(Array.prototype.slice.call(arguments, 0)));
                Jsni.apply(
                    Jsni.member(unconstructedType, SpecialNames.TypeInitializer), 
                    unconstructedType, 
                    Jsni.invoke(
                        Jsni.member(Jsni.array(generic, generic.prototype), "concat"), 
                        keyArray
                    ).As<JsArray>());

                generic.TypeInitializer = Jsni.procedure((t, p) =>
                {
                    p.___type = generic;
                    t.As<JsTypeFunction>().BaseType = unconstructedType;
                    t.As<JsTypeFunction>().GetTypeFromType = Jsni.function(() => Type._GetTypeFromTypeFunc(Jsni.@this().As<JsTypeFunction>()).As<JsObject>());
                    t.As<JsTypeFunction>().CreateTypeField = Jsni.function(() =>
                    {
                        var unconstructedTypeType = Type._GetTypeFromTypeFunc(unconstructedType);
                        var type = new Type(newTypeName, new Attribute[0]);
                        generic.Type = type;
                        type.Init(
                            newTypeName, 
                            unconstructedTypeType.typeAttributes,
                            generic, 
                            unconstructedType.BaseType,
                            unconstructedTypeType.interfaces, 
                            InitializeArray(typeArgs, Jsni.type<JsTypeFunction>()).As<JsTypeFunction[]>(),
                            unconstructedTypeType.fields, 
                            unconstructedTypeType.methods, 
                            unconstructedTypeType.constructors, 
                            unconstructedTypeType.properties, 
                            unconstructedTypeType.events, 
                            false, 
                            false,
                            true,
                            false,
                            null, 
                            unconstructedType);
                        return type.As<JsObject>();
                    });

                }, SpecialNames.TypeInitializerTypeFunction, SpecialNames.TypeInitializerPrototype);
                Jsni.call(generic.TypeInitializer, Jsni.@this(), generic, generic.prototype);
                result = generic;
                cache[keyString] = result;
            }

            return result.As<JsTypeFunction>();
        }

        [Js(Name = SpecialNames.MakeArrayType)]
        internal static JsTypeFunction MakeArrayType(JsTypeFunction elementType)
        {
            if (elementType.ArrayType == null)
            {
                var baseType = MakeGenericTypeFactory(Jsni.type(typeof(GenericArray<>)), Jsni.array(elementType));
                var arrayType = Jsni.procedure(() => {}).As<JsTypeFunction>();
                arrayType.prototype = Jsni.@new(baseType);
                Jsni.apply(
                    Jsni.type<Object>().TypeInitializer, 
                    Jsni.@this(), 
                    Jsni.array(arrayType, arrayType.prototype));
                Jsni.apply(
                    Jsni.type<Array>().TypeInitializer, 
                    Jsni.@this(), 
                    Jsni.invoke(
                        Jsni.member(Jsni.array(arrayType, arrayType.prototype), "concat"), 
                        elementType
                    ).As<JsArray>());
                arrayType.TypeInitializer = Jsni.procedure((t, p) =>
                {
                    p.___type = arrayType;
                    t.As<JsTypeFunction>().TypeName = elementType.TypeName + "[]";
                    t.As<JsTypeFunction>().BaseType = baseType;
                    t.As<JsTypeFunction>().ElementType = elementType;
                    t.As<JsTypeFunction>().GetTypeFromType = Jsni.function(() => Type._GetTypeFromTypeFunc(Jsni.@this().As<JsTypeFunction>()).As<JsObject>());
                    t.As<JsTypeFunction>().CreateTypeField = Jsni.function(() =>
                    {
                        var lastIndex = elementType.TypeName.LastIndexOf('.');
                        if (lastIndex == -1)
                            lastIndex = 0;
                        else
                            lastIndex++;
                        var type = new Type(elementType.TypeName.Substring(lastIndex) + "[]", new Attribute[0]);
                        arrayType.Type = type;
                        type.Init(
                            elementType.TypeName + "[]", 
                            TypeAttributes.Public,
                            elementType, 
                            Jsni.type<Array>(), 
                            typeof(Array).interfaces.Concat(new[] { SpecialFunctions.MakeGenericTypeFactory(Jsni.type(typeof(IEnumerable<>)), Jsni.array(elementType)) }).ToArray(), 
                            new JsTypeFunction[0],
                            new FieldInfo[0], 
                            new MethodInfo[0], 
                            new ConstructorInfo[0], 
                            new PropertyInfo[0], 
                            new EventInfo[0], 
                            false, 
                            false,
                            false,
                            false,
                            elementType,
                            null);
                        return type.As<JsObject>();
                    });
                }, SpecialNames.TypeInitializerTypeFunction, SpecialNames.TypeInitializerPrototype);
                Jsni.call(arrayType.TypeInitializer, Jsni.@this(), arrayType, arrayType.prototype);
                var result = arrayType;
                elementType.ArrayType = result;
            }
            return elementType.ArrayType;
        }

        [Js(Name = SpecialNames.SafeToString)]
        public static string SafeToString(object o)
        {
            return o == null ? "" : Jsni._typeof(o.As<JsObject>()) == "boolean" ? o.As<JsObject>().toString().As<string>() : o.ToString();
        }

        [Js(Name = SpecialNames.Truncate)]
        public static JsNumber Truncate(JsNumber number)
        {
            return number < 0 ? JsMath.ceil(number) : JsMath.floor(number);
        }

        [Js(Name = SpecialNames.DefaultOf)]
        public static JsObject DefaultOf(JsTypeFunction type)
        {
            var typeName = type.TypeName;
            switch (typeName)
            {
                case "System.Boolean":
                    return false;
                case "System.Byte":
                case "System.SByte":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Single":
                case "System.Double":
                case "System.Decimal":
                    return 0.As<JsNumber>();
                default:
                    return null;
            }
        }
    }
}
