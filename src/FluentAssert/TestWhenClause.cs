using System;
using System.Collections.Generic;
using System.Diagnostics;

using NUnit.Framework;

namespace FluentAssert
{
	[DebuggerNonUserCode]
	[DebuggerStepThrough]
	public class TestWhenClause<T>
	{
		private readonly T _actionContainer;
		private readonly IWhenActionWrapper _actionUnderTest;
		private readonly IList<IAssertionActionWrapper> _assertions = new List<IAssertionActionWrapper>();
		private readonly IList<IParameterActionWrapper> _initializationsForActionParameters;
		private Action<T> _performAction;

		internal TestWhenClause(T actionContainer, IWhenActionWrapper actionUnderTest,
		                        IList<IParameterActionWrapper> initializationsForActionParameters)
		{
			_actionContainer = actionContainer;
			_actionUnderTest = actionUnderTest;
			_initializationsForActionParameters = initializationsForActionParameters;
			_performAction = (T item) => _actionUnderTest.Act();
		}

		public TestWhenClause<T> Should(Action assertion)
		{
			_assertions.Add(new AssertionActionWrapper(assertion));
			return this;
		}

		public TestWhenClause<T> Should(Action<T> assertion)
		{
			_assertions.Add(new AssertionActionWrapper<T>(_actionContainer, assertion));
			return this;
		}

		public TestWhenClause<T> ShouldThrowException<TExceptionType>() where TExceptionType : Exception
		{
			_performAction = (T item) => Assert.Throws<TExceptionType>(() => _actionUnderTest.Act());
			return this;
		}

		public TestWhenClause<T> ShouldThrowException<TExceptionType>(string message) where TExceptionType : Exception
		{
			_performAction = (T item) => Assert.Throws<TExceptionType>(() => _actionUnderTest.Act(), message);
			return this;
		}

		public void Verify()
		{
			TestRunner.Verify(_actionUnderTest.Description,
			                  _initializationsForActionParameters,
			                  () => _performAction(_actionContainer),
			                  _assertions);
		}
	}

	[DebuggerNonUserCode]
	[DebuggerStepThrough]
	public class TestWhenClause<T, TContext>
	{
		private readonly T _actionContainer;
		private readonly IWhenActionWrapper _actionUnderTest;
		private readonly IList<IAssertionActionWrapper> _assertions = new List<IAssertionActionWrapper>();
		private readonly TContext _context;
		private readonly IList<IParameterActionWrapper> _initializationsForActionParameters;
		private Action<T, TContext> _performAction;

		internal TestWhenClause(T actionContainer, IWhenActionWrapper actionUnderTest,
		                        IList<IParameterActionWrapper> initializationsForActionParameters, TContext context)
		{
			_actionContainer = actionContainer;
			_actionUnderTest = actionUnderTest;
			_initializationsForActionParameters = initializationsForActionParameters;
			_context = context;
			_performAction = (T item, TContext testContext) => _actionUnderTest.Act();
		}

		public TestWhenClause<T, TContext> Should<TBaseContext>(Action<T, TBaseContext> assertion) where TBaseContext : class
		{
			var baseContext = _context as TBaseContext;
			if (baseContext == null)
			{
				throw new InvalidCastException(typeof(TContext).Name + " must inherit from " + typeof(TBaseContext) +
				                               " in order to call " + assertion.Method.Name);
			}

			_assertions.Add(new AssertionActionWrapper<T, TBaseContext>(_actionContainer, baseContext, assertion));
			return this;
		}

		public TestWhenClause<T, TContext> Should(Action<T, TContext> assertion)
		{
			_assertions.Add(new AssertionActionWrapper<T, TContext>(_actionContainer, _context, assertion));
			return this;
		}

		public TestWhenClause<T, TContext> Should(Action<TContext> assertion)
		{
			_assertions.Add(new AssertionActionWrapper<TContext>(_context, assertion));
			return this;
		}

		public TestWhenClause<T, TContext> Should(Action<T> assertion)
		{
			_assertions.Add(new AssertionActionWrapper<T>(_actionContainer, assertion));
			return this;
		}

		public TestWhenClause<T, TContext> Should(Action assertion)
		{
			_assertions.Add(new AssertionActionWrapper(assertion));
			return this;
		}

		public TestWhenClause<T, TContext> ShouldThrowException<TExceptionType>() where TExceptionType : Exception
		{
			_performAction = (T item, TContext testContext) => Assert.Throws<TExceptionType>(() => _actionUnderTest.Act());
			return this;
		}

		public TestWhenClause<T, TContext> ShouldThrowException<TExceptionType>(string message)
			where TExceptionType : Exception
		{
			_performAction =
				(T item, TContext testContext) => Assert.Throws<TExceptionType>(() => _actionUnderTest.Act(), message);
			return this;
		}

		public void Verify()
		{
			TestRunner.Verify(_actionUnderTest.Description,
			                  _initializationsForActionParameters,
			                  () => _performAction(_actionContainer, _context),
			                  _assertions
				);
		}
	}
}