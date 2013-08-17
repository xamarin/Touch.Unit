using System;
using System.ComponentModel;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using System.Collections;


namespace NUnit.Framework
{
	public partial class Assert
	{
		//Extra stuff not included in nUnit lite


		#region GreaterOrEqual

		#region Ints

		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void GreaterOrEqual(int arg1, int arg2, string message, params object[] args)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, args);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void GreaterOrEqual(int arg1, int arg2, string message)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, null);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		public static void GreaterOrEqual(int arg1, int arg2)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,null, null);
		}

		#endregion

		#region Unsigned Ints

		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		[CLSCompliant(false)]
		public static void GreaterOrEqual(uint arg1, uint arg2, string message, params object[] args)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, args);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		[CLSCompliant(false)]
		public static void GreaterOrEqual(uint arg1, uint arg2, string message)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, null);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		[CLSCompliant(false)]
		public static void GreaterOrEqual(uint arg1, uint arg2)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,null, null);
		}

		#endregion

		#region Longs

		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void GreaterOrEqual(long arg1, long arg2, string message, params object[] args)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, args);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void GreaterOrEqual(long arg1, long arg2, string message)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, null);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		public static void GreaterOrEqual(long arg1, long arg2)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,null, null);
		}

		#endregion

		#region Unsigned Longs

		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		[CLSCompliant(false)]
		public static void GreaterOrEqual(ulong arg1, ulong arg2, string message, params object[] args)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, args);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		[CLSCompliant(false)]
		public static void GreaterOrEqual(ulong arg1, ulong arg2, string message)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, null);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		[CLSCompliant(false)]
		public static void GreaterOrEqual(ulong arg1, ulong arg2)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,null, null);
		}

		#endregion

		#region Decimals

		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void GreaterOrEqual(decimal arg1, decimal arg2, string message, params object[] args)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, args);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void GreaterOrEqual(decimal arg1, decimal arg2, string message)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, null);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		public static void GreaterOrEqual(decimal arg1, decimal arg2)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,null, null);
		}

		#endregion

		#region Doubles

		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void GreaterOrEqual(double arg1, double arg2, string message, params object[] args)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, args);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void GreaterOrEqual(double arg1, double arg2, string message)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, null);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		public static void GreaterOrEqual(double arg1, double arg2)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,null, null);
		}

		#endregion

		#region Floats

		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void GreaterOrEqual(float arg1, float arg2, string message, params object[] args)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, args);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void GreaterOrEqual(float arg1, float arg2, string message)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, null);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		public static void GreaterOrEqual(float arg1, float arg2)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,null, null);
		}

		#endregion

		#region IComparables

		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void GreaterOrEqual(IComparable arg1, IComparable arg2, string message, params object[] args)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, args);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void GreaterOrEqual(IComparable arg1, IComparable arg2, string message)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,message, null);
		}
		/// <summary>
		/// Verifies that the first value is greater than or equal to the second
		/// value. If it is not, then an
		/// <see cref="AssertionException"/> is thrown. 
		/// </summary>
		/// <param name="arg1">The first value, expected to be greater</param>
		/// <param name="arg2">The second value, expected to be less</param>
		public static void GreaterOrEqual(IComparable arg1, IComparable arg2)
		{
			Assert.That(arg1, Is.GreaterThanOrEqualTo(arg2) ,null, null);
		}

		#endregion

		#endregion

		#region IsNullOrEmpty

		/// <summary>
		/// Assert that a string is either null or equal to string.Empty
		/// </summary>
		/// <param name="aString">The string to be tested</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void IsNullOrEmpty(string aString, string message, params object[] args)
		{
			Assert.That(aString, new NullOrEmptyStringConstraint() ,message, args);
		}
		/// <summary>
		/// Assert that a string is either null or equal to string.Empty
		/// </summary>
		/// <param name="aString">The string to be tested</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void IsNullOrEmpty(string aString, string message)
		{
			Assert.That(aString, new NullOrEmptyStringConstraint() ,message, null);
		}
		/// <summary>
		/// Assert that a string is either null or equal to string.Empty
		/// </summary>
		/// <param name="aString">The string to be tested</param>
		public static void IsNullOrEmpty(string aString)
		{
			Assert.That(aString, new NullOrEmptyStringConstraint() ,null, null);
		}

		#endregion

		#region IsNotNullOrEmpty

		/// <summary>
		/// Assert that a string is not null or empty
		/// </summary>
		/// <param name="aString">The string to be tested</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void IsNotNullOrEmpty(string aString, string message, params object[] args)
		{
			Assert.That(aString, new NotConstraint( new NullOrEmptyStringConstraint()) ,message, args);
		}
		/// <summary>
		/// Assert that a string is not null or empty
		/// </summary>
		/// <param name="aString">The string to be tested</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void IsNotNullOrEmpty(string aString, string message)
		{
			Assert.That(aString, new NotConstraint( new NullOrEmptyStringConstraint()) ,message, null);
		}
		/// <summary>
		/// Assert that a string is not null or empty
		/// </summary>
		/// <param name="aString">The string to be tested</param>
		public static void IsNotNullOrEmpty(string aString)
		{
			Assert.That(aString, new NotConstraint( new NullOrEmptyStringConstraint()) ,null, null);
		}

		#endregion


		#region IsInstanceOf

		/// <summary>
		/// Asserts that an object is an instance of a given type.
		/// </summary>
		/// <param name="expected">The expected Type</param>
		/// <param name="actual">The object being examined</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void IsInstanceOf(Type expected, object actual, string message, params object[] args)
		{
			Assert.That(actual, Is.InstanceOf(expected) ,message, args);
		}
		/// <summary>
		/// Asserts that an object is an instance of a given type.
		/// </summary>
		/// <param name="expected">The expected Type</param>
		/// <param name="actual">The object being examined</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void IsInstanceOf(Type expected, object actual, string message)
		{
			Assert.That(actual, Is.InstanceOf(expected) ,message, null);
		}
		/// <summary>
		/// Asserts that an object is an instance of a given type.
		/// </summary>
		/// <param name="expected">The expected Type</param>
		/// <param name="actual">The object being examined</param>
		public static void IsInstanceOf(Type expected, object actual)
		{
			Assert.That(actual, Is.InstanceOf(expected) ,null, null);
		}

		#endregion

		#region IsInstanceOf<T>

		/// <summary>
		/// Asserts that an object is an instance of a given type.
		/// </summary>
		/// <typeparam name="T">The expected Type</typeparam>
		/// <param name="actual">The object being examined</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void IsInstanceOf<T>(object actual, string message, params object[] args)
		{
			Assert.That(actual, Is.InstanceOf(typeof(T)) ,message, args);
		}
		/// <summary>
		/// Asserts that an object is an instance of a given type.
		/// </summary>
		/// <typeparam name="T">The expected Type</typeparam>
		/// <param name="actual">The object being examined</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void IsInstanceOf<T>(object actual, string message)
		{
			Assert.That(actual, Is.InstanceOf(typeof(T)) ,message, null);
		}
		/// <summary>
		/// Asserts that an object is an instance of a given type.
		/// </summary>
		/// <typeparam name="T">The expected Type</typeparam>
		/// <param name="actual">The object being examined</param>
		public static void IsInstanceOf<T>(object actual)
		{
			Assert.That(actual, Is.InstanceOf(typeof(T)) ,null, null);
		}

		#endregion
		#region AreEqual

		#region Ints

		/// <summary>
		/// Verifies that two ints are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void AreEqual(int expected, int actual, string message, params object[] args)
		{
			Assert.That(actual, Is.EqualTo(expected), message, args);
		}
		/// <summary>
		/// Verifies that two ints are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void AreEqual(int expected, int actual, string message)
		{
			Assert.That(actual, Is.EqualTo(expected), message, null);
		}
		/// <summary>
		/// Verifies that two ints are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		public static void AreEqual(int expected, int actual)
		{
			Assert.That(actual, Is.EqualTo(expected), null, null);
		}

		#endregion

		#region Longs

		/// <summary>
		/// Verifies that two longs are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void AreEqual(long expected, long actual, string message, params object[] args)
		{
			Assert.That(actual, Is.EqualTo(expected), message, args);
		}
		/// <summary>
		/// Verifies that two longs are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void AreEqual(long expected, long actual, string message)
		{
			Assert.That(actual, Is.EqualTo(expected), message, null);
		}
		/// <summary>
		/// Verifies that two longs are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		public static void AreEqual(long expected, long actual)
		{
			Assert.That(actual, Is.EqualTo(expected), null, null);
		}

		#endregion

		#region Unsigned Ints

		/// <summary>
		/// Verifies that two unsigned ints are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		[CLSCompliant(false)]
		public static void AreEqual(uint expected, uint actual, string message, params object[] args)
		{
			Assert.That(actual, Is.EqualTo(expected), message, args);
		}
		/// <summary>
		/// Verifies that two unsigned ints are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to display in case of failure</param>
		[CLSCompliant(false)]
		public static void AreEqual(uint expected, uint actual, string message)
		{
			Assert.That(actual, Is.EqualTo(expected), message, null);
		}
		/// <summary>
		/// Verifies that two unsigned ints are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		[CLSCompliant(false)]
		public static void AreEqual(uint expected, uint actual)
		{
			Assert.That(actual, Is.EqualTo(expected), null, null);
		}

		#endregion

		#region Unsigned Longs

		/// <summary>
		/// Verifies that two unsigned longs are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		[CLSCompliant(false)]
		public static void AreEqual(ulong expected, ulong actual, string message, params object[] args)
		{
			Assert.That(actual, Is.EqualTo(expected), message, args);
		}
		/// <summary>
		/// Verifies that two unsigned longs are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to display in case of failure</param>
		[CLSCompliant(false)]
		public static void AreEqual(ulong expected, ulong actual, string message)
		{
			Assert.That(actual, Is.EqualTo(expected), message, null);
		}
		/// <summary>
		/// Verifies that two unsigned longs are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		[CLSCompliant(false)]
		public static void AreEqual(ulong expected, ulong actual)
		{
			Assert.That(actual, Is.EqualTo(expected), null, null);
		}

		#endregion

		#region Decimals

		/// <summary>
		/// Verifies that two decimals are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void AreEqual(decimal expected, decimal actual, string message, params object[] args)
		{
			Assert.That(actual, Is.EqualTo(expected), message, args);
		}
		/// <summary>
		/// Verifies that two decimals are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void AreEqual(decimal expected, decimal actual, string message)
		{
			Assert.That(actual, Is.EqualTo(expected), message, null);
		}
		/// <summary>
		/// Verifies that two decimals are equal. If they are not, then an 
		/// <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		public static void AreEqual(decimal expected, decimal actual)
		{
			Assert.That(actual, Is.EqualTo(expected), null, null);
		}

		#endregion

		#region Doubles

		/// <summary>
		/// Verifies that two doubles are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equal then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void AreEqual(double expected, double actual, double delta, string message, params object[] args)
		{
			AssertDoublesAreEqual(expected, actual, delta, message, args);
		}
		/// <summary>
		/// Verifies that two doubles are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equal then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void AreEqual(double expected, double actual, double delta, string message)
		{
			AssertDoublesAreEqual(expected, actual, delta, message, null);
		}
		/// <summary>
		/// Verifies that two doubles are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equal then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
		public static void AreEqual(double expected, double actual, double delta)
		{
			AssertDoublesAreEqual(expected, actual, delta, null, null);
		}

		#if CLR_2_0 || CLR_4_0
		/// <summary>
		/// Verifies that two doubles are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equal then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void AreEqual(double expected, double? actual, double delta, string message, params object[] args)
		{
			AssertDoublesAreEqual(expected, (double)actual, delta, message, args);
		}
		/// <summary>
		/// Verifies that two doubles are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equal then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void AreEqual(double expected, double? actual, double delta, string message)
		{
			AssertDoublesAreEqual(expected, (double)actual, delta, message, null);
		}
		/// <summary>
		/// Verifies that two doubles are equal considering a delta. If the
		/// expected value is infinity then the delta value is ignored. If 
		/// they are not equal then an <see cref="AssertionException"/> is
		/// thrown.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="delta">The maximum acceptable difference between the
		/// the expected and the actual</param>
		public static void AreEqual(double expected, double? actual, double delta)
		{
			AssertDoublesAreEqual(expected, (double)actual, delta, null, null);
		}
		#endif

		#endregion

		#region Objects

		/// <summary>
		/// Verifies that two objects are equal.  Two objects are considered
		/// equal if both are null, or if both have the same value. NUnit
		/// has special semantics for some object types.
		/// If they are not equal an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The value that is expected</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to display in case of failure</param>
		/// <param name="args">Array of objects to be used in formatting the message</param>
		public static void AreEqual(object expected, object actual, string message, params object[] args)
		{
			Assert.That(actual, Is.EqualTo(expected), message, args);
		}
		/// <summary>
		/// Verifies that two objects are equal.  Two objects are considered
		/// equal if both are null, or if both have the same value. NUnit
		/// has special semantics for some object types.
		/// If they are not equal an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The value that is expected</param>
		/// <param name="actual">The actual value</param>
		/// <param name="message">The message to display in case of failure</param>
		public static void AreEqual(object expected, object actual, string message)
		{
			Assert.That(actual, Is.EqualTo(expected), message, null);
		}
		/// <summary>
		/// Verifies that two objects are equal.  Two objects are considered
		/// equal if both are null, or if both have the same value. NUnit
		/// has special semantics for some object types.
		/// If they are not equal an <see cref="AssertionException"/> is thrown.
		/// </summary>
		/// <param name="expected">The value that is expected</param>
		/// <param name="actual">The actual value</param>
		public static void AreEqual(object expected, object actual)
		{
			Assert.That(actual, Is.EqualTo(expected), null, null);
		}

		#endregion

		#endregion



		///
	}
}

