﻿/*
    Copyright (C) 2014-2017 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Globalization;
using System.Threading;
using dnSpy.Contracts.Debugger.CallStack;
using dnSpy.Contracts.Debugger.Evaluation;

namespace dnSpy.Contracts.Debugger.Engine.Evaluation {
	/// <summary>
	/// A value shown in a treeview (eg. in locals window)
	/// </summary>
	public abstract class DbgEngineValueNode : DbgObject {
		/// <summary>
		/// Gets the error message or null
		/// </summary>
		public abstract string ErrorMessage { get; }

		/// <summary>
		/// Gets the value or null if none
		/// </summary>
		public abstract DbgEngineValue Value { get; }

		/// <summary>
		/// Gets the expression that is used when adding an expression to the watch window or
		/// when assigning a new value to the source.
		/// </summary>
		public abstract string Expression { get; }

		/// <summary>
		/// Image name, see <see cref="PredefinedDbgValueNodeImageNames"/>
		/// </summary>
		public abstract string ImageName { get; }

		/// <summary>
		/// true if this is a read-only value
		/// </summary>
		public abstract bool IsReadOnly { get; }

		/// <summary>
		/// true if the expression causes side effects
		/// </summary>
		public abstract bool CausesSideEffects { get; }

		/// <summary>
		/// Returns true if it has children, false if it has no children and null if it's unknown (eg. it's too expensive to calculate it now).
		/// UI code can use this property to decide if it shows the treeview node expander ("|>").
		/// </summary>
		public abstract bool? HasChildren { get; }

		/// <summary>
		/// Number of children. This property is called as late as possible and can be lazily initialized.
		/// It's assumed to be 0 if <see cref="HasChildren"/> is false.
		/// </summary>
		/// <param name="context">Evaluation context</param>
		/// <param name="frame">Frame</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
		public abstract ulong GetChildCount(DbgEvaluationContext context, DbgStackFrame frame, CancellationToken cancellationToken);

		/// <summary>
		/// Creates new children. This method blocks the current thread until the children have been created.
		/// </summary>
		/// <param name="context">Evaluation context</param>
		/// <param name="frame">Frame</param>
		/// <param name="index">Index of first child</param>
		/// <param name="count">Max number of children to return</param>
		/// <param name="options">Options</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
		public abstract DbgEngineValueNode[] GetChildren(DbgEvaluationContext context, DbgStackFrame frame, ulong index, int count, DbgValueNodeEvaluationOptions options, CancellationToken cancellationToken);

		/// <summary>
		/// Formats the name, value, and type. This method blocks the current thread until all requested values have been formatted
		/// </summary>
		/// <param name="context">Evaluation context</param>
		/// <param name="frame">Frame</param>
		/// <param name="options">Options</param>
		/// <param name="cultureInfo">Culture or null to use invariant culture</param>
		/// <param name="cancellationToken">Cancellation token</param>
		public abstract void Format(DbgEvaluationContext context, DbgStackFrame frame, IDbgValueNodeFormatParameters options, CultureInfo cultureInfo, CancellationToken cancellationToken);

		/// <summary>
		/// Writes a new value. It blocks the current thread until the assignment is complete.
		/// </summary>
		/// <param name="context">Evaluation context</param>
		/// <param name="frame">Frame</param>
		/// <param name="expression">Source expression (rhs)</param>
		/// <param name="options">Options</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns></returns>
		public abstract DbgEngineValueNodeAssignmentResult Assign(DbgEvaluationContext context, DbgStackFrame frame, string expression, DbgEvaluationOptions options, CancellationToken cancellationToken);
	}

	/// <summary>
	/// Assignment result
	/// </summary>
	public struct DbgEngineValueNodeAssignmentResult {
		/// <summary>
		/// Gets the error message (also see <see cref="PredefinedEvaluationErrorMessages"/>) or null if none
		/// </summary>
		public string Error { get; }

		/// <summary>
		/// Gets the flags
		/// </summary>
		public DbgEEAssignmentResultFlags Flags { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="flags">Result flags</param>
		/// <param name="error">Error message or one of the errors in <see cref="PredefinedEvaluationErrorMessages"/></param>
		public DbgEngineValueNodeAssignmentResult(DbgEEAssignmentResultFlags flags, string error) {
			Flags = flags;
			Error = error;
		}
	}
}
