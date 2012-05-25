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
			var myPieces = new[] {MajorVersion, MinorVersion, PatchVersion, ScriptVersion};
			var theirPieces = new[] { otherVersion.MajorVersion, otherVersion.MinorVersion, otherVersion.PatchVersion, otherVersion.ScriptVersion };

			for (var i = 0; i < myPieces.Length; i++)
			{
				if (myPieces[i].CompareTo(theirPieces[i]) != 0)
				{
					return myPieces[i].CompareTo(theirPieces[i]);
				}
			}

			return 0;
		}

		#endregion

		public static readonly DatabaseVersion Max = new DatabaseVersion(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);

		public static DatabaseVersion FromString(string value)
		{
			var pieces = value.Split('.');

			if (pieces.Length < 3)
			{
				throw new ArgumentException("The database version should be specified using format ##.##.##.##");
			}

			var majorVersion = int.Parse(pieces[0]);
			var minorVersion = int.Parse(pieces[1]);
			var patchVersion = int.Parse(pieces[2]);
			var scriptVersion = int.MaxValue;

			if (pieces.Length > 3)
			{
				scriptVersion = int.Parse(pieces[3]);
			}

			return new DatabaseVersion(majorVersion, minorVersion, patchVersion, scriptVersion);
		}

		public int MajorVersion { get; private set; }
		public int MinorVersion { get; private set; }
		public int PatchVersion { get; private set; }
		public int ScriptVersion { get; private set; }

		public DatabaseVersion() : this(0, 0, 0, 0)
		{
		}

		public DatabaseVersion(int majorVersion, int minorVersion, int patchVersion, int scriptVersion)
		{
			MajorVersion = majorVersion;
			MinorVersion = minorVersion;
			PatchVersion = patchVersion;
			ScriptVersion = scriptVersion;
		}

		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}.{3}", 
				MajorVersion == int.MaxValue ? (object)"*" : MajorVersion, 
				MinorVersion == int.MaxValue ? (object)"*" : MinorVersion,
				PatchVersion == int.MaxValue ? (object)"*" : PatchVersion,
				ScriptVersion == int.MaxValue ? (object)"*" : ScriptVersion
				);
		}
	}
}