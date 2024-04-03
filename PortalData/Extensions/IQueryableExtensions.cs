using System.Linq.Expressions;

namespace VoteUp.PortalData.Extensions;

public static class IQueryableExtensions
{
	public static IQueryable<TSource> SortByColumnName<TSource>(
		this IQueryable<TSource> source,
		string sortBy,
		string sortOrder
	)
	{
		if (string.IsNullOrEmpty(sortBy))
			sortBy = "Created";

		var param = Expression.Parameter(typeof(TSource), "x");
		Expression conversion = GetMemberExpression(param, sortBy);
		var expression = Expression.Lambda<Func<TSource, object>>(
			Expression.Convert(conversion, typeof(object)),
			param
		);

		if (string.Compare("desc", sortOrder, StringComparison.OrdinalIgnoreCase) == 0)
			return source.OrderByDescending(expression);
		else
			return source.OrderBy(expression);
	}

	public static Expression GetMemberExpression(Expression pe, string propertyName)
	{
		if (string.IsNullOrWhiteSpace(propertyName))
			throw new Exception("Property name must be specified");

		int index = propertyName.IndexOf('.');
		if (index < 0)
		{
			return Expression.Property(pe, propertyName);
		}
		else
		{
			string property = propertyName.Substring(0, index);
			propertyName = propertyName.Substring(index + 1);
			return GetMemberExpression(Expression.Property(pe, property), propertyName);
		}
	}
}
