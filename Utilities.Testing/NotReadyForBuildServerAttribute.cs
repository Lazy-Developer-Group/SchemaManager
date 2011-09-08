using System;
using NUnit.Framework;

namespace Utilities.Testing
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class NotReadyForBuildServerAttribute : CategoryAttribute
	{
		
	}
}