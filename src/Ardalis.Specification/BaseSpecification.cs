﻿using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ardalis.Specification
{
    // Keeping both classes here for clarity. Will fix it afterward

    public abstract class BaseSpecification<T, TResult> : BaseSpecification<T>, ISpecification<T, TResult>
    {
        protected BaseSpecification(Expression<Func<T, bool>> criteria) : base(criteria) { }

        public Expression<Func<T, TResult>> Selector { get; set; }
    }

    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        protected BaseSpecification()
        {
        }
        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            AddCriteria(criteria);
        }

        public IEnumerable<Expression<Func<T, bool>>> Criterias { get; } = new List<Expression<Func<T, bool>>>();
        public IEnumerable<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public IEnumerable<string> IncludeStrings { get; } = new List<string>();
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public Expression<Func<T, object>> GroupBy { get; private set; }

        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; } = false;

        public string CacheKey { get; protected set; }
        public bool CacheEnabled { get; private set; }

        protected virtual void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            ((List<Expression<Func<T, bool>>>)Criterias).Add(criteria);
        }
        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            ((List<Expression<Func<T, object>>>)Includes).Add(includeExpression);
        }

        protected virtual void AddInclude(string includeString)
        {
            ((List<string>)IncludeStrings).Add(includeString);
        }

        protected virtual void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        //Not used anywhere at the moment, but someone requested an example of setting this up.
        protected virtual void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
        {
            GroupBy = groupByExpression;
        }

        /// <summary>
        /// Must be called after specifying criteria
        /// </summary>
        /// <param name="specificationName"></param>
        /// <param name="args">Any arguments used in defining the specification</param>
        protected void EnableCache(string specificationName, params object[] args)
        {
            Guard.Against.NullOrEmpty(specificationName, nameof(specificationName));

            // Just to be able to throw exception if the list is empty. "Guard.Against.EmptyList" might be nice extension for GuardClauses.
            var criteria = Criterias.FirstOrDefault();
            Guard.Against.Null(criteria, nameof(criteria));

            CacheKey = $"{specificationName}-{string.Join("-", args)}";

            CacheEnabled = true;
        }
    }
}
