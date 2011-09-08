using System;
using System.Linq.Expressions;
using AutoMapper;

namespace Utilities.Mapping
{
	//TODO: Eventually we should move this to our Utilities project if/when we ever add AutoMapper to
	//		the rest of our projects. 
	public static class AutoMapperExtensions
	{
		public static MemberMappingExpression<T1,T2> ForMember<T1,T2>(this IMappingExpression<T1,T2> expression, Expression<Func<T2,object>> member)
		{
			return new MemberMappingExpression<T1,T2>(expression, member);
		}
	}
}