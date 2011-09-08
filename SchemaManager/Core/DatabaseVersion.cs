using System;
using System.Linq;

namespace SchemaManager.Core
{
	public class DatabaseVersion : IComparable<DatabaseVersion>, IComparable
	{
		#region IComparable/Comparisons

		public static bool operator <(DatabaseVersion left, DatabaseVersion right)
		{
			return left.CompareTo(right) < 0;
		}

		public static bool operator >(DatabaseVersion left, DatabaseVersion right)
		{
			return left.CompareTo(right) > 0;
		}

		public static bool operator <=(DatabaseVersion left, DatabaseVersion right)
		{
			return left.CompareTo(right) <= 0;
		}

		public static bool operator >=(DatabaseVersion left, DatabaseVersion right)
		{
			return left.CompareTo(right) >= 0;
		}

		int IComparable.CompareTo(object obj)
		{
			return CompareTo((DatabaseVersion)obj);
		}

		public int CompareTo(DatabaseVersion otherVersion)
		{
			if (this.MajorVersion.CompareTo(otherVersion.MajorVersion) != 0)
			{
				return this.MajorVersion.CompareTo(otherVersion.MajorVersion);
			}

			return this.MinorVersion.CompareTo(otherVersion.MinorVersion);
		}

		#endregion

		public static readonly DatabaseVersion Max = new DatabaseVersion(double.MaxValue, double.MaxValue);

		public static DatabaseVersion FromString(string value)
		{
			var pieces = value.Split('.');

			if (pieces.Length < 2)
			{
				throw new ArgumentException("The database version should be specified using format ##.##.##");
			}

			var majorVersion = double.Parse(string.Join(".", pieces.Take(2).ToArray()));
			var minorVersion = double.Parse(pieces.Last());

			if (pieces.Length == 2)
			{
				minorVersion = double.MaxValue;
			}

			return new DatabaseVersion(majorVersion, minorVersion);
		}

		public double MajorVersion { get; private set; }
		public double MinorVersion { get; private set; }

		public DatabaseVersion() : this(0,0)
		{
		}

		public DatabaseVersion(double majorVersion, double minorVersion)
		{
			MajorVersion = majorVersion;
			MinorVersion = minorVersion;
		}

		public override string ToString()
		{
			return string.Format("{0:F2}.{1:F0}", 
				MajorVersion == double.MaxValue ? (object)"*" : MajorVersion, 
				MinorVersion == double.MaxValue ? (object)"*" : MinorVersion);
		}
	}
}