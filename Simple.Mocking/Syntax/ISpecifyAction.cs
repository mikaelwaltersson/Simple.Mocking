using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Mocking.Syntax
{
	public interface ISpecifyAction
	{
		void Executes(Action action);
		
		void Executes(ActionWithParameters action);

		void Throws(Exception ex);

		ISpecifyAction SetsOutOrRefParameter(int index, object value);
	}

	public interface ISpecifyAction<T>
	{
		void Executes(Func<T> func);

		void Executes(FuncWithParameters<T> func);

		void Returns(T value);

		void Throws(Exception ex);

		ISpecifyAction<T> SetsOutOrRefParameter(int index, object value);
	}
}
