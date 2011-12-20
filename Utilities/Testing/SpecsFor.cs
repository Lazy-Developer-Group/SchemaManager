using System;
using NUnit.Framework;
using Moq;
using Ninject;
using Ninject.Moq;

namespace Utilities.Testing
{
	[TestFixture]
	public abstract partial class SpecsFor<T> where T : class
	{
		protected MockingKernel Mocker;

		protected T SUT { get; set; }

		/// <summary>
		/// Gets the mock for the specified type from the underlying container. 
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <returns></returns>
		protected Mock<TType> GetMockFor<TType>() where TType : class
		{
			return Mock.Get(Mocker.Get<TType>());
		}

		[SetUp]
		public virtual void SetupEachSpec()
		{
			Mocker = new MockingKernel();

			ConfigureKernel(Mocker);
			
			InitializeClassUnderTest();

			BeforeEachSpec();

			Given();

			When();
		}

		protected virtual void BeforeEachSpec()
		{
		}

		protected virtual void InitializeClassUnderTest()
		{
			SUT = Mocker.Get<T>();
		}

		protected virtual void ConfigureKernel(IKernel kernel)
		{
		}

		[TearDown]
		public virtual void TearDown()
		{
			Mocker.Reset();
			AfterEachSpec();
		}

		protected virtual void Given()
		{

		}

		protected virtual void AfterEachSpec()
		{
			if (SUT is IDisposable)
			{
				(SUT as IDisposable).Dispose();
			}
		}

		protected abstract void When();
	}
}