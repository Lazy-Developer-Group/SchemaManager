using System;
using AutoMapper;

namespace Utilities.Mapping
{
	public interface IMemberMappingExpression<T1, T2>
	{
		IMappingExpression<T1,T2> MapFrom<TResult>(Func<T1, TResult> sourceMember);
		IMappingExpression<T1,T2> Ignore();
	}
}