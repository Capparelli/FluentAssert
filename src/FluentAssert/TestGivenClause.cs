using System;
using System.Diagnostics;

namespace FluentAssert
{
	public class TestGivenClause<T>
	{
		private readonly ActionContainerSource<T> _actionContainerSource;

		internal TestGivenClause(ActionContainerSource<T> actionContainerSource)
		{
			_actionContainerSource = actionContainerSource;
		}

		[DebuggerNonUserCode]
		public TestWithClause<T> When(Action<T> actionUnderTest)
		{
			var actionContainer = _actionContainerSource.GetActionContainer();
			return new TestWithClause<T>(actionContainer, new WhenActionWrapper<T>(actionContainer, actionUnderTest));
		}

		[DebuggerNonUserCode]
		public TestWithClause<T> When(Action actionUnderTest)
		{
			var actionContainer = _actionContainerSource.GetActionContainer();
			return new TestWithClause<T>(actionContainer, new WhenActionWrapper(actionUnderTest));
		}

		[DebuggerNonUserCode]
		public TestGivenClause<T, TContext> WithContext<TContext>(TContext context)
		{
			var actionContainerSource = new ActionContainerSource<T, TContext>
				{
					GetActionContainer = delegate { return _actionContainerSource.GetActionContainer(); }
				};
			return new TestGivenClause<T, TContext>(actionContainerSource, context);
		}
	}

	public class TestGivenClause<T, TContext>
	{
		private readonly ActionContainerSource<T, TContext> _actionContainerSource;
		private readonly TContext _context;

		internal TestGivenClause(ActionContainerSource<T, TContext> actionContainerSource, TContext context)
		{
			_actionContainerSource = actionContainerSource;
			_context = context;
		}

		[DebuggerNonUserCode]
		public TestWithClause<T, TContext> When(Action<T, TContext> actionUnderTest)
		{
			var actionContainer = _actionContainerSource.GetActionContainer(_context);
			return new TestWithClause<T, TContext>(actionContainer,
			                                       new WhenActionWrapper<T, TContext>(actionContainer, _context, actionUnderTest),
			                                       _context);
		}

		[DebuggerNonUserCode]
		public TestWithClause<T, TContext> When(Action<T> actionUnderTest)
		{
			var actionContainer = _actionContainerSource.GetActionContainer(_context);
			return new TestWithClause<T, TContext>(actionContainer,
			                                       new WhenActionWrapper<T>(actionContainer, actionUnderTest),
			                                       _context);
		}

		[DebuggerNonUserCode]
		public TestWithClause<T, TContext> When(Action actionUnderTest)
		{
			var actionContainer = _actionContainerSource.GetActionContainer(_context);
			return new TestWithClause<T, TContext>(actionContainer,
			                                       new WhenActionWrapper(actionUnderTest),
			                                       _context);
		}
	}
}