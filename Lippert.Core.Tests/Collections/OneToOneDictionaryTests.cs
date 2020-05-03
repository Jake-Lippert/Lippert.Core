using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Lippert.Core.Collections;
using Lippert.Core.Collections.Extensions;
using NUnit.Framework;

namespace Lippert.Core.Tests.Collections
{
	[TestFixture]
	public class OneToOneDictionaryTests
	{
		private readonly Random _random = new Random();

		[Test]
		public void TestPrimaryKeyAccessor([Values(true, false)] bool shouldContain)
		{
			//--Arrange
			var key = Guid.NewGuid();
			var value = _random.Next();
			var underTest = new OneToOneDictionary<Guid, int>
			{
				[key] = value
			};

			//--Act/Assert
			int actualValue;
			if (shouldContain)
			{
				actualValue = underTest[key];
				Assert.AreEqual(value, actualValue);
				Assert.IsTrue(underTest.ContainsKey(value));
			}
			else
			{
				Assert.Throws<KeyNotFoundException>(() => actualValue = underTest[Guid.NewGuid()]);
			}
		}
		[Test]
		public void TestSecondaryKeyAccessor([Values(true, false)] bool shouldContain)
		{
			//--Arrange
			var key = _random.Next();
			var value = Guid.NewGuid();
			var underTest = new OneToOneDictionary<Guid, int>
			{
				[key] = value
			};

			//--Act/Assert
			Guid actualValue;
			if (shouldContain)
			{
				actualValue = underTest[key];
				Assert.AreEqual(value, actualValue);
				Assert.IsTrue(underTest.ContainsKey(value));
			}
			else
			{
				Assert.Throws<KeyNotFoundException>(() => actualValue = underTest[_random.Next()]);
			}
		}

		[Test]
		public void TestCount()
		{
			//--Arrange
			var underTest = new OneToOneDictionary<int, string>(new Dictionary<int, string>
			{
				[12] = "12",
				[23] = "23"
			}, new Dictionary<string, int>
			{
				["23"] = 23,
				["34"] = 34
			});

			//--Act
			var count = underTest.Count;

			//--Assert
			Assert.AreEqual(3, count);
		}

		[Test]
		public void TestIsReadOnly([Values(true, false)] bool primaryIsReadOnly, [Values(true, false)] bool secondaryIsReadOnly)
		{
			//--Arrange
			IDictionary<int, string> primarySeed = new Dictionary<int, string>
			{
				[12] = "12",
				[23] = "23"
			};
			IDictionary<string, int> secondarySeed = new Dictionary<string, int>
			{
				["12"] = 12,
				["23"] = 23
			};
			var underTest = new OneToOneDictionary<int, string>(
				primaryIsReadOnly ? new ReadOnlyDictionary<int, string>(primarySeed) : primarySeed,
				secondaryIsReadOnly ? new ReadOnlyDictionary<string, int>(secondarySeed) : secondarySeed);

			//--Act
			var count = underTest.Count;

			//--Assert
			Assert.AreEqual(primaryIsReadOnly | secondaryIsReadOnly, underTest.IsReadOnly);
			Assert.AreEqual(2, count);
		}

		[Test]
		public void TestKeys()
		{
			//--Arrange
			var underTest = new OneToOneDictionary<int, string>
			{
				{ 12, "12" },
				{ 23, "23" },
				//{ "23", 23 },
				{ "34", 34 }
			};

			//--Act
			var keys = (underTest as IDictionary<int, string>).Keys;

			//--Assert
			CollectionAssert.AreEquivalent(new[] { 12, 23, 34 }, keys);
		}

		[Test]
		public void TestValues()
		{
			//--Arrange
			var underTest = new OneToOneDictionary<int, string>
			{
				{ 12, "12" },
				{ 23, "23" },
				//{ "23", 23 },
				{ "34", 34 }
			};

			//--Act
			var values = (underTest as IDictionary<int, string>).Values;

			//--Assert
			CollectionAssert.AreEquivalent(new[] { "12", "23", "34" }, values);
		}

		[Test]
		public void TestCollectionAdd()
		{
			//--Arrange
			var key = Guid.NewGuid();
			var value = _random.Next();
			var underTest = new OneToOneDictionary<Guid, int>();
			Assert.IsFalse(underTest.ContainsKey(key));

			//--Act
			(underTest as ICollection<KeyValuePair<Guid, int>>).Add(new KeyValuePair<Guid, int>(key, value));

			//--Assert
			Assert.IsTrue(underTest.ContainsKey(key));
			Assert.AreEqual(value, underTest[key]);
		}

		[Test]
		public void TestAddRange([Values(10, 100)] int count, [Values(false, true)] bool targetPrimary)
		{
			//--Arrange
			var toAdd = Enumerable.Range(0, count).Select(x => (Guid.NewGuid(), _random.Next())).ToList();
			var underTest = new OneToOneDictionary<Guid, int>();
			Assert.Zero(underTest.Count);

			//--Act
			if (targetPrimary)
			{
				underTest.AddRange(toAdd);
			}
			else
			{
				underTest.AddRange(toAdd.Select(x => (x.Item2, x.Item1)));
			}

			//--Assert
			Assert.AreEqual(count, underTest.Count);
			foreach (var (item1, item2) in toAdd)
			{
				if (targetPrimary)
				{
					Assert.IsTrue(underTest.ContainsKey(item1));
					Assert.AreEqual(item2, underTest[item1]);
					Assert.IsTrue(underTest.ContainsKey(item2));
				}
				else
				{
					Assert.IsTrue(underTest.ContainsKey(item2));
					Assert.AreEqual(item1, underTest[item2]);
					Assert.IsTrue(underTest.ContainsKey(item1));
				}
			}
		}

		[Test]
		public void TestClear([Values(10, 100)] int count)
		{
			//--Arrange
			var underTest = new OneToOneDictionary<Guid, int>(Enumerable.Range(0, count).Select(x => new KeyValuePair<Guid, int>(Guid.NewGuid(), x)));
			Assert.AreEqual(count, underTest.Count);

			//--Act
			underTest.Clear();

			//--Assert
			Assert.Zero(underTest.Count);
		}

		[Test]
		public void TestCollectionContains([Values(true, false)] bool shouldContain)
		{
			//--Arrange
			var key = Guid.NewGuid();
			var value = _random.Next();
			var underTest = new OneToOneDictionary<Guid, int>();
			if (shouldContain)
			{
				underTest[key] = value;
				Assert.IsTrue(underTest.ContainsKey(key));
			}
			else
			{
				Assert.IsFalse(underTest.ContainsKey(key));
			}

			//--Act
			var containsValue = (underTest as ICollection<KeyValuePair<Guid, int>>).Contains(new KeyValuePair<Guid, int>(key, value));

			//--Assert
			Assert.AreEqual(shouldContain, containsValue);
		}

		[Test]
		public void TestRemove([Values(0, 1, 2)] int targetPath, [Values(true, false)] bool shouldContain)
		{
			//--Arrange
			var key = Guid.NewGuid();
			var value = _random.Next();
			var underTest = new OneToOneDictionary<Guid, int>();
			if (shouldContain)
			{
				underTest[key] = value;
				Assert.IsTrue(underTest.ContainsKey(key));
			}
			else
			{
				Assert.IsFalse(underTest.ContainsKey(key));
			}

			//--Act
			var removedValue = targetPath switch
			{
				0 => (underTest as ICollection<KeyValuePair<Guid, int>>).Remove(new KeyValuePair<Guid, int>(key, value)),
				1 => underTest.Remove(key),
				2 => underTest.Remove(value),
				_ => throw new ArgumentException(nameof(targetPath))
			};

			//--Assert
			Assert.AreEqual(shouldContain, removedValue);
			Assert.IsFalse(underTest.ContainsKey(key));
		}

		[Test]
		public void TestPrimaryTryGetValue([Values(true, false)] bool shouldContain)
		{
			//--Arrange
			var key = Guid.NewGuid();
			var value = _random.Next();
			var underTest = new OneToOneDictionary<Guid, int>();
			if (shouldContain)
			{
				underTest[key] = value;
			}

			//--Act
			var contains = underTest.TryGetValue(key, out var actualValue);

			//--Assert
			Assert.AreEqual(shouldContain, contains);
			if (shouldContain)
			{
				actualValue = underTest[key];
				Assert.AreEqual(value, actualValue);
				Assert.IsTrue(underTest.ContainsKey(value));
			}
			else
			{
				Assert.AreEqual(default(int), actualValue);
			}
		}
		[Test]
		public void TestSecondaryTryGetValue([Values(true, false)] bool shouldContain)
		{
			//--Arrange
			var key = _random.Next();
			var value = Guid.NewGuid();
			var underTest = new OneToOneDictionary<Guid, int>();
			if (shouldContain)
			{
				underTest[key] = value;
			}

			//--Act
			var contains = underTest.TryGetValue(key, out var actualValue);

			//--Assert
			Assert.AreEqual(shouldContain, contains);
			if (shouldContain)
			{
				actualValue = underTest[key];
				Assert.AreEqual(value, actualValue);
				Assert.IsTrue(underTest.ContainsKey(value));
			}
			else
			{
				Assert.AreEqual(default(Guid), actualValue);
			}
		}

		/// <remarks>
		/// CopyTo tested by enumeration of IDictionary<TKey, TValue>
		/// </remarks>
		[Test]
		public void TestGetEnumerator([Values(0, 1, 2)] int targetPath, [Values(5, 50)] int count)
		{
			//--Arrange
			var source = Enumerable.Range(0, count).ToDictionary(x => x, x => Guid.NewGuid());
			var underTest = new OneToOneDictionary<Guid, int>(source);

			//--Act
			var enumerated = targetPath switch
			{
				0 => underTest.ToList(),
				1 => (underTest as IEnumerable<KeyValuePair<Guid, int>>).ToList(),
				2 => (underTest as IEnumerable).Cast<object>().Cast<KeyValuePair<Guid, int>>().ToList(),//--Force GetEnumerator() enumeration
				_ => throw new ArgumentException(nameof(targetPath))
			};

			//--Assert
			Assert.AreEqual(source.Count, enumerated.Count);
			foreach (var kvp in enumerated)
			{
				Assert.AreEqual(source[kvp.Value], kvp.Key);
			}
		}

		[Test]
		public void TestImplicitConversion()
		{
			//--Arrange
			var source = Enumerable.Range(0, 1)
				.Select(x => Guid.NewGuid())
				.ToDictionary(x => x, x => x.ToString());
			var underTest = new OneToOneDictionary<string, Guid>(source);

			//--Act
			OneToOneDictionary<Guid, string> reversed = underTest;

			//--Assert
			CollectionAssert.AreEquivalent(source, reversed.ToList());
		}


		[Test]
		public void TestRetrievalDictionaryPrimary([Values(true, false)] bool useTryGetValue, [Values(true, false)] bool targetPrimary)
		{
			//--Arrange
			var primary = RetrievalDictionary.Build((Guid id) => id.ToString());
			var underTest = new OneToOneDictionary<Guid, string>(primary);

			//--Act/Assert
			foreach (var key in Enumerable.Range(0, 10).Select(x => Guid.NewGuid()))
			{
				if (targetPrimary)
				{
					Assert.IsFalse(underTest.ContainsKey(key));
					Assert.IsFalse(underTest.ContainsKey(key.ToString()));

					var value = underTest[key];
					Assert.AreEqual(key.ToString(), value);
					Assert.IsTrue(underTest.ContainsKey(key));
					Assert.IsTrue(underTest.ContainsKey(key.ToString()));
				}
				else if (useTryGetValue)
				{
					var success = underTest.TryGetValue(key.ToString(), out var value);
					Assert.IsFalse(success);
					Assert.AreEqual(Guid.Empty, value);
				}
				else
				{
					Guid value;
					Assert.Throws<KeyNotFoundException>(() => value = underTest[key.ToString()]);
				}
			}
		}

		[Test]
		public void TestBackingDictionarySynchronization()
		{
			//--Arrange
			var primary = new Dictionary<Guid, string>();
			var secondary = RetrievalDictionary.Build((string id) => Guid.Parse(id));
			var underTest = new OneToOneDictionary<Guid, string>(primary, secondary);

			//--Act/Assert
			var id = Guid.NewGuid();
			Assert.IsFalse(primary.ContainsKey(id));
			Assert.IsFalse(secondary.ContainsKey(id.ToString()));

			primary[id] = id.ToString();
			Assert.AreEqual(id.ToString(), underTest[id]);
			Assert.IsTrue(primary.ContainsKey(id));
			Assert.IsTrue(secondary.ContainsKey(id.ToString()));


			id = Guid.NewGuid();
			Assert.IsFalse(primary.ContainsKey(id));
			Assert.IsFalse(secondary.ContainsKey(id.ToString()));

			Assert.AreEqual(id, underTest[id.ToString()]);
			Assert.IsTrue(primary.ContainsKey(id));
			Assert.IsTrue(secondary.ContainsKey(id.ToString()));


			id = Guid.NewGuid();
			Assert.IsFalse(primary.ContainsKey(id));
			Assert.IsFalse(secondary.ContainsKey(id.ToString()));

			primary[id] = id.ToString();
			Assert.IsTrue(underTest.TryGetValue(id, out var value));
			Assert.AreEqual(id.ToString(), value);
			Assert.IsTrue(primary.ContainsKey(id));
			Assert.IsTrue(secondary.ContainsKey(id.ToString()));
		}
	}
}