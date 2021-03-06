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

using Roslyn.Compilers.CSharp;

namespace WootzJs.Compiler
{
    public static class WootzJsExtensions
    {
        public static string GetName(this Symbol symbol)
        {
            var result = symbol.GetAttributeValue<string>(Context.Instance.JsAttributeType, "Name");
            return result ?? symbol.Name;
        }

        public static bool IsExported(this Symbol symbol)
        {
            if (symbol.IsExtern)
                return false;

            var result = symbol.GetAttributeValue(Context.Instance.JsAttributeType, "Export", true);
            if (!(symbol is TypeSymbol))
                result = result && symbol.ContainingType.IsExported();
            return result;
        }

        public static bool IsBuiltIn(this Symbol symbol)
        {
            return symbol.GetAttributeValue(Context.Instance.JsAttributeType, "BuiltIn", false);
        }

        public static bool IsExtension(this Symbol symbol)
        {
            return symbol.GetAttributeValue(Context.Instance.JsAttributeType, "Extension", false);
        }

        public static bool IsAutoNotifyPropertyChange(this Symbol symbol, out MethodSymbol method)
        {
            var classType = symbol is NamedTypeSymbol ? (NamedTypeSymbol)symbol : symbol.ContainingType;

            // If the type is INotifyPropertyChanged and it duck types to NotifyPropertyChanged
            var isNotifyPropertyChanged = classType.Interfaces.Any(x => Context.Instance.INotifyPropertyChanged.IsAssignableFrom(x));
            var isDuckTypedToNotifyPropertyChanged = classType.TryGetMethod("NotifyPropertyChanged", out method, Context.Instance.String);
            return isNotifyPropertyChanged && isDuckTypedToNotifyPropertyChanged;
        }
    }
}
