// Copyright (c) Ben A Adams. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Apkd.Internal 
{
    internal class ValueTupleResolvedParameter : ResolvedParameter
    {
        internal IList<string> TupleNames { get; set; }

        protected override void AppendTypeName(StringBuilder sb)
        {
            if (ResolvedType.IsValueTuple())
            {
                AppendValueTupleParameterName(sb, ResolvedType);
            }
            else
            {
                // Need to unwrap the first generic argument first.
                sb.Append(TypeNameHelper.GetTypeNameForGenericType(ResolvedType));
                sb.Append('<');
                AppendValueTupleParameterName(sb, ResolvedType.GetGenericArguments()[0]);
                sb.Append('>');
            }
        }


        void AppendValueTupleParameterName(StringBuilder sb, System.Type parameterType)
        {
            sb.Append('(');
            var args = parameterType.GetGenericArguments();
            for (var i = 0; i < args.Length; i++)
            {
                if (i > 0)
                    sb.Append(',').Append(' ');

                sb.AppendTypeDisplayName(args[i], fullName: false, includeGenericParameterNames: true);

                if (i >= TupleNames.Count)
                    continue;

                var argName = TupleNames[i];
                if (argName == null)
                    continue;

                sb.Append(' ');
                sb.Append(argName);
            }

            sb.Append(')');
        }
    }
}
