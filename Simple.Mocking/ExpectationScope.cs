using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Simple.Mocking.SetUp;
using Simple.Mocking.SetUp.Proxies;

namespace Simple.Mocking
{
	public sealed class ExpectationScope : IExpectationScope, IMockNameScope
	{
		Stack<ChildScope> scopeStack;
		HashSet<string> mockNames;
		List<IInvocation> unexpectedInvocations;

		public ExpectationScope()
		{
			this.scopeStack = new Stack<ChildScope>();
			this.mockNames = new HashSet<string>();
			this.unexpectedInvocations = new List<IInvocation>();

			scopeStack.Push(new UnorderedChildScope(null));
		}

		public IDisposable BeginUnordered()
		{
			return BeginChildScope(new UnorderedChildScope(this));
		}

		public IDisposable BeginOrdered()
		{
			return BeginChildScope(new OrderedChildScope(this));
		}

		IDisposable BeginChildScope(ChildScope childScope)
		{
			RootScope.Add(childScope, false);
			scopeStack.Push(childScope);
			return childScope;
		}

		ChildScope RootScope
		{
			get { return scopeStack.First(); }
		}

		ChildScope CurrentScope
		{
			get { return scopeStack.Peek(); }
		}
		

		bool IExpectation.TryMeet(IInvocation invocation)
		{
			if (invocation == null)
				throw new ArgumentNullException("invocation");

			return RootScope.TryMeet(invocation);
		}

		bool IExpectation.HasBeenMet
		{
			get { return RootScope.HasBeenMet && (unexpectedInvocations.Count == 0); }
		}

		void IExpectationScope.Add(IExpectation expectation, bool hasHigherPrecedence)
		{
			if (expectation == null)
				throw new ArgumentNullException("expectation");

			CurrentScope.Add(expectation, hasHigherPrecedence);
		}

		void IExpectationScope.OnUnexpectedInvocation(IInvocation invocation)
		{
			unexpectedInvocations.Add(invocation);
		}

		bool IMockNameScope.Register(string name)
		{
			return mockNames.Add(name);
		}

		public override string ToString()
		{			
			using (var writer = new StringWriter())
			{
				RootScope.DescribeContent(writer, 0);				
				DescribeUnexpectedInvocations(writer, 0);
				
				return writer.GetStringBuilder().ToString();
			}						
		}

		void DescribeUnexpectedInvocations(StringWriter writer, int indentLevel)
		{
			if (unexpectedInvocations.Count == 0)
				return;

			writer.WriteLine();
			WriteLine(writer, indentLevel, "Unexpected invocations {");
			unexpectedInvocations.ForEach(invocation => WriteLine(writer, indentLevel + 1, invocation));			
			WriteLine(writer, indentLevel, "}");
		}

		void ExitChildScope(ChildScope scope)
		{
			if (scope != scopeStack.Peek())
				throw new InvalidOperationException("Invalid expectation scope dispose order");

			scopeStack.Pop();
		}


		static void WriteLine(TextWriter writer, int indentLevel, object obj)
		{
			const char IndentationChar = ' ';
			const int IndentationLength = 2;

			writer.Write(new string(IndentationChar, IndentationLength * indentLevel));
			writer.WriteLine(obj);
		}

		abstract class ChildScope : IExpectation, IDisposable
		{
			ExpectationScope parent;
			protected List<IExpectation> expectationList = new List<IExpectation>();

			protected ChildScope(ExpectationScope parent)
			{
				this.parent = parent;
				this.expectationList = new List<IExpectation>();
			}

			protected abstract bool DoTryMeet(IInvocation invocation);

			public bool TryMeet(IInvocation invocation)
			{
				return DoTryMeet(invocation);
			}

			public bool HasBeenMet
			{
				get { return expectationList.All(expectation => expectation.HasBeenMet); }
			}

		
			public void Add(IExpectation expectation, bool hasHigherPrecedence)
			{
				if (hasHigherPrecedence)
					expectationList.Insert(0, expectation);
				else
					expectationList.Add(expectation);
			}

			protected abstract void DescribeHeader(TextWriter writer, int indentLevel);

			static void DescribeFooter(TextWriter writer, int indentLevel)
			{
				WriteLine(writer, indentLevel, "}");
			}	

			public void DescribeContent(TextWriter writer, int indentLevel)
			{
				foreach (var expectation in expectationList)
				{
					if (expectation is ChildScope)
					{
						var childScope = (ChildScope)expectation;

						childScope.DescribeHeader(writer, indentLevel);
						childScope.DescribeContent(writer, indentLevel + 1);
						DescribeFooter(writer, indentLevel);
					}
					else
						WriteLine(writer, indentLevel, expectation);
				}
			}

			public void Dispose()
			{
				parent.ExitChildScope(this);
			}
		}		

		class UnorderedChildScope : ChildScope
		{
			public UnorderedChildScope(ExpectationScope parent) : base(parent)
			{
			}

			protected override bool DoTryMeet(IInvocation invocation)
			{				
				foreach (var expectation in expectationList)
				{
					if (expectation.TryMeet(invocation))
						return true;
				}

				return false;			
			}

			protected override void DescribeHeader(TextWriter writer, int indentLevel)
			{
				WriteLine(writer, indentLevel, "Unordered {");
			}
		}

		class OrderedChildScope : ChildScope
		{
			int nextToMeetIndex;

			public OrderedChildScope(ExpectationScope parent) : base(parent)
			{
			}

			protected override bool DoTryMeet(IInvocation invocation)
			{
				var nextToMeet = expectationList[nextToMeetIndex];

				if (nextToMeet.TryMeet(invocation))
					return true;

				int subsequentToMeetIndex = nextToMeetIndex + 1;

				if (nextToMeet.HasBeenMet && subsequentToMeetIndex < expectationList.Count && expectationList[subsequentToMeetIndex].TryMeet(invocation))
				{
					nextToMeetIndex = subsequentToMeetIndex;
					return true;
				}

				return false;			
			}

			protected override void DescribeHeader(TextWriter writer, int indentLevel)
			{
				WriteLine(writer, indentLevel, "In order {");
			}
		}


	}
}
