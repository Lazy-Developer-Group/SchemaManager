using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Utilities.Testing
{
	public static class AssertExtensions
	{
		private static readonly Type[] ValueTypes = new[]
								 {
									 typeof(string),
									 typeof(int),
									 typeof(decimal),
									 typeof(bool),
									 typeof(double),
									 typeof(float),
									 typeof(short),
									 typeof(byte),
									 typeof(long),
									 typeof(DateTime),
									 typeof(Guid),

									 typeof(int?),
									 typeof(decimal?),
									 typeof(bool?),
									 typeof(double?),
									 typeof(float?),
									 typeof(short?),
									 typeof(byte?),
									 typeof(long?),
									 typeof(DateTime?),
									 typeof(Guid?),
							   };
		
		public static void LookLikeEachOtherWithArrayChild<T, U>(T expected, T actual)
		{
			var type = typeof(T);

			PropertyInfo[] myProperties = type.GetProperties(BindingFlags.DeclaredOnly |
															 BindingFlags.Public |
															 BindingFlags.Instance);

			foreach (var myProperty in myProperties)
			{
				if (ValueTypes.Contains(myProperty.PropertyType))
				{
					Assert.AreEqual(myProperty.GetValue(expected, null), myProperty.GetValue(actual, null),
									string.Format(@"The value of the property {0} on instance a is different from the value on instance b.", myProperty.Name));
				}
				else if (myProperty.PropertyType.IsArray ||
						 (typeof(IEnumerable).IsAssignableFrom(myProperty.PropertyType) &&
						 myProperty.PropertyType.GetGenericArguments()[0] == typeof(U)))
				{
					var expectedVal = myProperty.GetValue(expected, null);
					var actualVal = myProperty.GetValue(actual, null);

					if (expectedVal == null || actualVal == null)
					{
						Assert.IsTrue(expectedVal == null && actualVal == null,
									  string.Format(@"The value of the property {0} on instance a is different from the value on instance b.", myProperty.Name));
					}
					else
					{
						AssertExtensions.LookLikeEachOtherArray(
							(IEnumerable<U>)myProperty.GetValue(expected, null),
							(IEnumerable<U>)myProperty.GetValue(actual, null));
					}
				}
				else
				{
					AssertExtensions.LookLikeEachOther(myProperty.GetValue(expected, null), myProperty.GetValue(actual, null));
				}

			}
		}

		public static void LookLikeEachOther<T>(T expected, T actual)
		{
			if (expected == null)
			{
				Assert.AreEqual(expected, actual);
				return;
			}

			var type = expected.GetType();

			if (typeof(IComparable).IsAssignableFrom(type))
			{
				Assert.AreEqual(expected, actual);
				return;
			}

			PropertyInfo[] myProperties = type.GetProperties(BindingFlags.DeclaredOnly |
															 BindingFlags.Public |
															 BindingFlags.Instance);

			foreach (var myProperty in myProperties)
			{
				if (ValueTypes.Contains(myProperty.PropertyType))
				{
					Assert.AreEqual(myProperty.GetValue(expected, null), myProperty.GetValue(actual, null),
									string.Format(@"The value of the property {0} on instance a is different from the value on instance b.", myProperty.Name));
				}
				else if (typeof(IEnumerable).IsAssignableFrom(myProperty.PropertyType))
				{
					var expectedVal = myProperty.GetValue(expected, null);
					var actualVal = myProperty.GetValue(actual, null);

					if (expectedVal == null || actualVal == null)
					{
						Assert.IsTrue(expectedVal == null && actualVal == null,
									  string.Format(@"The value of the property {0} on instance a is different from the value on instance b.", myProperty.Name));
					}
					else if(myProperty.PropertyType == typeof(byte[]))
					{
						AssertExtensions.LookLikeEachOtherArray((byte[])myProperty.GetValue(expected, null), (byte[])myProperty.GetValue(actual, null));
					}
					else
					{
						Assert.Inconclusive(string.Format(@"The value of the property {0} on instance a is an array of an unknown type. Please use LookLikeEachOtherWithArrayChild.", myProperty.Name));
					}
				}
				else
				{
					AssertExtensions.LookLikeEachOther(myProperty.GetValue(expected, null), myProperty.GetValue(actual, null));
				}

			}
		}


		public static void LookLikeEachOtherArrayWithArrayChild<T, U>(IEnumerable<T> expected, IEnumerable<T> actual)
		{
			Assert.AreEqual(expected.Count(), actual.Count(),
							string.Format("The count of the two objects differ. Expected {0}, Actual {1}", expected.Count(), actual.Count()));

			var expectedList = expected.ToList();
			var actualList = actual.ToList();

			if (ValueTypes.Contains(typeof(T)))
			{
				for (int i = 0; i < expected.Count(); i++)
				{
					Assert.AreEqual(expectedList[i], actualList[i]);
				}

			}
			else
			{
				for (int i = 0; i < expected.Count(); i++)
				{
					AssertExtensions.LookLikeEachOtherWithArrayChild<T,U>(expectedList[i], actualList[i]);
				}
			}
		}

		public static void LookLikeEachOtherArray<T>(IEnumerable<T> expected, IEnumerable<T> actual)
		{
			Assert.AreEqual(expected.Count(), actual.Count(),
							string.Format("The count of the two objects differ. Expected {0}, Actual {1}", expected.Count(), actual.Count()));

			var expectedList = expected.ToList();
			var actualList = actual.ToList();

			for (int i = 0; i < expected.Count(); i++)
			{
				AssertExtensions.LookLikeEachOther(expectedList[i], actualList[i]);
			}
		}
	}
}