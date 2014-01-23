﻿using System.Collections;
using System.Runtime.WootzJs;

namespace System
{
    [Js(Name = "Array", BuiltIn = true)]
    public class Array : IList
    {
        [Js(Export = false, Name = "length")]
        public int Length { get; private set; }

        public IEnumerator GetEnumerator()
        {
            var array = this.As<JsArray>();
            return new ArrayEnumerator(array);
        }

        /// <summary>
        /// Hack implementation until we have proper generic support for arrays.
        /// </summary>
        [Js(Name = "System$Collections$Generic$IEnumerable$1$GetEnumerator")]
        public IEnumerator GetGenericEnumeratorHACK()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return Length; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// Copies a range of elements from an <see cref="T:System.Array"/> starting at the first element and pastes them into another <see cref="T:System.Array"/> starting at the first element. The length is specified as a 32-bit integer.
        /// </summary>
        /// <param name="sourceArray">The <see cref="T:System.Array"/> that contains the data to copy.</param><param name="destinationArray">The <see cref="T:System.Array"/> that receives the data.</param><param name="length">A 32-bit integer that represents the number of elements to copy.</param><exception cref="T:System.ArgumentNullException"><paramref name="sourceArray"/> is null.-or-<paramref name="destinationArray"/> is null.</exception><exception cref="T:System.RankException"><paramref name="sourceArray"/> and <paramref name="destinationArray"/> have different ranks.</exception><exception cref="T:System.ArrayTypeMismatchException"><paramref name="sourceArray"/> and <paramref name="destinationArray"/> are of incompatible types.</exception><exception cref="T:System.InvalidCastException">At least one element in <paramref name="sourceArray"/> cannot be cast to the type of <paramref name="destinationArray"/>.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="length"/> is less than zero.</exception><exception cref="T:System.ArgumentException"><paramref name="length"/> is greater than the number of elements in <paramref name="sourceArray"/>.-or-<paramref name="length"/> is greater than the number of elements in <paramref name="destinationArray"/>.</exception><filterpriority>1</filterpriority>
        public static void Copy(Array sourceArray, Array destinationArray, int length)
        {
            Copy(sourceArray, sourceArray.GetLowerBound(0), destinationArray, destinationArray.GetLowerBound(0), length);
        }

        /// <summary>
        /// Copies a range of elements from an <see cref="T:System.Array"/> starting at the specified source index and pastes them to another <see cref="T:System.Array"/> starting at the specified destination index. The length and the indexes are specified as 32-bit integers.
        /// </summary>
        /// <param name="sourceArray">The <see cref="T:System.Array"/> that contains the data to copy.</param><param name="sourceIndex">A 32-bit integer that represents the index in the <paramref name="sourceArray"/> at which copying begins.</param><param name="destinationArray">The <see cref="T:System.Array"/> that receives the data.</param><param name="destinationIndex">A 32-bit integer that represents the index in the <paramref name="destinationArray"/> at which storing begins.</param><param name="length">A 32-bit integer that represents the number of elements to copy.</param><exception cref="T:System.ArgumentNullException"><paramref name="sourceArray"/> is null.-or-<paramref name="destinationArray"/> is null.</exception><exception cref="T:System.RankException"><paramref name="sourceArray"/> and <paramref name="destinationArray"/> have different ranks.</exception><exception cref="T:System.ArrayTypeMismatchException"><paramref name="sourceArray"/> and <paramref name="destinationArray"/> are of incompatible types.</exception><exception cref="T:System.InvalidCastException">At least one element in <paramref name="sourceArray"/> cannot be cast to the type of <paramref name="destinationArray"/>.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="sourceIndex"/> is less than the lower bound of the first dimension of <paramref name="sourceArray"/>.-or-<paramref name="destinationIndex"/> is less than the lower bound of the first dimension of <paramref name="destinationArray"/>.-or-<paramref name="length"/> is less than zero.</exception><exception cref="T:System.ArgumentException"><paramref name="length"/> is greater than the number of elements from <paramref name="sourceIndex"/> to the end of <paramref name="sourceArray"/>.-or-<paramref name="length"/> is greater than the number of elements from <paramref name="destinationIndex"/> to the end of <paramref name="destinationArray"/>.</exception><filterpriority>1</filterpriority>
        public static void Copy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length)
        {
            for (int i = sourceIndex, j = destinationIndex; i < sourceArray.Length && j < destinationArray.Length && i < sourceIndex + length; i++, j++)
            {
                destinationArray[j] = sourceArray[i];
            }
        }

        /// <summary>
        /// Copies a range of elements from an <see cref="T:System.Array"/> starting at the specified source index and pastes them to another <see cref="T:System.Array"/> starting at the specified destination index.  Guarantees that all changes are undone if the copy does not succeed completely.
        /// </summary>
        /// <param name="sourceArray">The <see cref="T:System.Array"/> that contains the data to copy.</param><param name="sourceIndex">A 32-bit integer that represents the index in the <paramref name="sourceArray"/> at which copying begins.</param><param name="destinationArray">The <see cref="T:System.Array"/> that receives the data.</param><param name="destinationIndex">A 32-bit integer that represents the index in the <paramref name="destinationArray"/> at which storing begins.</param><param name="length">A 32-bit integer that represents the number of elements to copy.</param><exception cref="T:System.ArgumentNullException"><paramref name="sourceArray"/> is null.-or-<paramref name="destinationArray"/> is null.</exception><exception cref="T:System.RankException"><paramref name="sourceArray"/> and <paramref name="destinationArray"/> have different ranks.</exception><exception cref="T:System.ArrayTypeMismatchException">The <paramref name="sourceArray"/> type is neither the same as nor derived from the <paramref name="destinationArray"/> type.</exception><exception cref="T:System.InvalidCastException">At least one element in <paramref name="sourceArray"/> cannot be cast to the type of <paramref name="destinationArray"/>.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="sourceIndex"/> is less than the lower bound of the first dimension of <paramref name="sourceArray"/>.-or-<paramref name="destinationIndex"/> is less than the lower bound of the first dimension of <paramref name="destinationArray"/>.-or-<paramref name="length"/> is less than zero.</exception><exception cref="T:System.ArgumentException"><paramref name="length"/> is greater than the number of elements from <paramref name="sourceIndex"/> to the end of <paramref name="sourceArray"/>.-or-<paramref name="length"/> is greater than the number of elements from <paramref name="destinationIndex"/> to the end of <paramref name="destinationArray"/>.</exception>
        public static void ConstrainedCopy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length)
        {
            Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
        }

        /// <summary>
        /// Copies a range of elements from an <see cref="T:System.Array"/> starting at the first element and pastes them into another <see cref="T:System.Array"/> starting at the first element. The length is specified as a 64-bit integer.
        /// </summary>
        /// <param name="sourceArray">The <see cref="T:System.Array"/> that contains the data to copy.</param><param name="destinationArray">The <see cref="T:System.Array"/> that receives the data.</param><param name="length">A 64-bit integer that represents the number of elements to copy. The integer must be between zero and <see cref="F:System.Int32.MaxValue"/>, inclusive.</param><exception cref="T:System.ArgumentNullException"><paramref name="sourceArray"/> is null.-or-<paramref name="destinationArray"/> is null.</exception><exception cref="T:System.RankException"><paramref name="sourceArray"/> and <paramref name="destinationArray"/> have different ranks.</exception><exception cref="T:System.ArrayTypeMismatchException"><paramref name="sourceArray"/> and <paramref name="destinationArray"/> are of incompatible types.</exception><exception cref="T:System.InvalidCastException">At least one element in <paramref name="sourceArray"/> cannot be cast to the type of <paramref name="destinationArray"/>.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="length"/> is less than 0 or greater than <see cref="F:System.Int32.MaxValue"/>.</exception><exception cref="T:System.ArgumentException"><paramref name="length"/> is greater than the number of elements in <paramref name="sourceArray"/>.-or-<paramref name="length"/> is greater than the number of elements in <paramref name="destinationArray"/>.</exception><filterpriority>1</filterpriority>
        public static void Copy(Array sourceArray, Array destinationArray, long length)
        {
            Copy(sourceArray, destinationArray, (int) length);
        }

        /// <summary>
        /// Copies a range of elements from an <see cref="T:System.Array"/> starting at the specified source index and pastes them to another <see cref="T:System.Array"/> starting at the specified destination index. The length and the indexes are specified as 64-bit integers.
        /// </summary>
        /// <param name="sourceArray">The <see cref="T:System.Array"/> that contains the data to copy.</param><param name="sourceIndex">A 64-bit integer that represents the index in the <paramref name="sourceArray"/> at which copying begins.</param><param name="destinationArray">The <see cref="T:System.Array"/> that receives the data.</param><param name="destinationIndex">A 64-bit integer that represents the index in the <paramref name="destinationArray"/> at which storing begins.</param><param name="length">A 64-bit integer that represents the number of elements to copy. The integer must be between zero and <see cref="F:System.Int32.MaxValue"/>, inclusive.</param><exception cref="T:System.ArgumentNullException"><paramref name="sourceArray"/> is null.-or-<paramref name="destinationArray"/> is null.</exception><exception cref="T:System.RankException"><paramref name="sourceArray"/> and <paramref name="destinationArray"/> have different ranks.</exception><exception cref="T:System.ArrayTypeMismatchException"><paramref name="sourceArray"/> and <paramref name="destinationArray"/> are of incompatible types.</exception><exception cref="T:System.InvalidCastException">At least one element in <paramref name="sourceArray"/> cannot be cast to the type of <paramref name="destinationArray"/>.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="sourceIndex"/> is outside the range of valid indexes for the <paramref name="sourceArray"/>.-or-<paramref name="destinationIndex"/> is outside the range of valid indexes for the <paramref name="destinationArray"/>.-or-<paramref name="length"/> is less than 0 or greater than <see cref="F:System.Int32.MaxValue"/>.</exception><exception cref="T:System.ArgumentException"><paramref name="length"/> is greater than the number of elements from <paramref name="sourceIndex"/> to the end of <paramref name="sourceArray"/>.-or-<paramref name="length"/> is greater than the number of elements from <paramref name="destinationIndex"/> to the end of <paramref name="destinationArray"/>.</exception><filterpriority>1</filterpriority>
        public static void Copy(Array sourceArray, long sourceIndex, Array destinationArray, long destinationIndex, long length)
        {
            Copy(sourceArray, (int) sourceIndex, destinationArray, (int) destinationIndex, (int) length);
        }

        /// <summary>
        /// Copies all the elements of the current one-dimensional <see cref="T:System.Array"/> to the specified one-dimensional <see cref="T:System.Array"/> starting at the specified destination <see cref="T:System.Array"/> index. The index is specified as a 32-bit integer.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from the current <see cref="T:System.Array"/>.</param>
        /// <param name="index">A 32-bit integer that represents the index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than the lower bound of <paramref name="array"/>.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-The number of elements in the source <see cref="T:System.Array"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.</exception><exception cref="T:System.ArrayTypeMismatchException">The type of the source <see cref="T:System.Array"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception><exception cref="T:System.RankException">The source <see cref="T:System.Array"/> is multidimensional.</exception><exception cref="T:System.InvalidCastException">At least one element in the source <see cref="T:System.Array"/> cannot be cast to the type of destination <paramref name="array"/>.</exception><filterpriority>2</filterpriority>
        public void CopyTo(Array array, int index)
        {
            Copy(this, index, array, 0, array.Length);
//            var sourceArray = this.As<JsArray>();
//            var destArray = array.As<JsArray>();
//            destArray = destArray.slice(index);
//            Jsni.apply(() => destArray.push(), sourceArray);
        }

        public void CopyTo(Array array, long index)
        {
            CopyTo(array, index.As<int>());         // All numbers are the same type in Javascript, so just coerce it into an "int" so we don't have to duplicate the code
        }

        [Js(Export = false)]
        public object this[int index]
        {
            get { return null; }
            set { ; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsFixedSize
        {
            get { return true; }
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            return IndexOf(value) >= 0;
        }

        public static void Clear(Array array, int startIndex, int length)
        {
            for (var i = startIndex; i <startIndex + length; i++)
            {
                array[i] = null;
            }
        }

        public void Clear()
        {
            Clear(this, 0, Length);
        }

        [Js(Name = "indexOf", Export = false)]
        public int IndexOf(object value)
        {
            return 0;
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the upper bound of the specified dimension in the <see cref="T:System.Array"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The upper bound of the specified dimension in the <see cref="T:System.Array"/>.
        /// </returns>
        /// <param name="dimension">A zero-based dimension of the <see cref="T:System.Array"/> whose upper bound needs to be determined.</param><exception cref="T:System.IndexOutOfRangeException"><paramref name="dimension"/> is less than zero.-or-<paramref name="dimension"/> is equal to or greater than <see cref="P:System.Array.Rank"/>.</exception><filterpriority>1</filterpriority>
        public int GetUpperBound(int dimension)
        {
            return Length - 1;
        }

        /// <summary>
        /// Gets the lower bound of the specified dimension in the <see cref="T:System.Array"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The lower bound of the specified dimension in the <see cref="T:System.Array"/>.
        /// </returns>
        /// <param name="dimension">A zero-based dimension of the <see cref="T:System.Array"/> whose lower bound needs to be determined.</param><exception cref="T:System.IndexOutOfRangeException"><paramref name="dimension"/> is less than zero.-or-<paramref name="dimension"/> is equal to or greater than <see cref="P:System.Array.Rank"/>.</exception><filterpriority>1</filterpriority>
        public int GetLowerBound(int dimension)
        {
            return 0;
        }
    }
}