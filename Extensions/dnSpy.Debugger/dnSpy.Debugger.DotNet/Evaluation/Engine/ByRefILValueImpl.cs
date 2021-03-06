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

using dnSpy.Contracts.Debugger.DotNet.Evaluation;
using dnSpy.Debugger.DotNet.Metadata;

namespace dnSpy.Debugger.DotNet.Evaluation.Engine {
	sealed class ByRefILValueImpl : AddressILValue, IDebuggerRuntimeILValue {
		public override DmdType Type => byRefValue.Type;
		DbgDotNetValue IDebuggerRuntimeILValue.GetDotNetValue() => byRefValue;

		readonly DbgDotNetValue byRefValue;

		public ByRefILValueImpl(DebuggerRuntimeImpl runtime, DbgDotNetValue byRefValue)
			: base(runtime, byRefValue.Type.GetElementType()) {
			this.byRefValue = byRefValue;
		}

		protected override DbgDotNetValue ReadValue() {
			var value = byRefValue.LoadIndirect();
			if (value != null)
				return runtime.RecordValue(value);
			return null;
		}

		protected override void WriteValue(object value) => runtime.StoreIndirect(byRefValue, value);

		public override bool Equals(AddressILValue other) =>
			other is ByRefILValueImpl addr &&
			runtime.Equals(byRefValue, addr.byRefValue);
	}
}
