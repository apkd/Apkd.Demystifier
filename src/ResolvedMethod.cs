// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Collections.Generic.Enumerable;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Apkd.Internal
{
    internal class ResolvedMethod
    {
        internal MethodBase MethodBase { get; set; }

        internal Type DeclaringType { get; set; }

        internal bool IsAsync { get; set; }

        internal bool IsLambda { get; set; }

        internal ResolvedParameter ReturnParameter { get; set; }

        internal string Name { get; set; }

        internal int? Ordinal { get; set; }

        internal string GenericArguments { get; set; }

        internal Type[] ResolvedGenericArguments { get; set; }

        internal MethodBase SubMethodBase { get; set; }

        internal string SubMethod { get; set; }

        internal EnumerableIList<ResolvedParameter> Parameters { get; set; }

        internal EnumerableIList<ResolvedParameter> SubMethodParameters { get; set; }

        public override string ToString() => Append(new StringBuilder()).ToString();

        internal StringBuilder Append(StringBuilder builder)
        {
            if (ReturnParameter != null)
            {
                if (IsAsync)
                    ReturnParameter.Prefix2 = "async";

                ReturnParameter.Append(builder);
                builder.Append(' ');
            }

            bool hasSubMethodOrLambda = !string.IsNullOrEmpty(SubMethod) || IsLambda;

            if (DeclaringType != null)
            {

                if (Name == ".ctor")
                {
                    if (!hasSubMethodOrLambda)
                    {
                        builder
                            .Append(".new ");

                        AppendDeclaringTypeName(builder)
                            .Append(Name);
                    }
                }
                else if (Name == ".cctor")
                {
                    builder.Append("static ");

                    AppendDeclaringTypeName(builder);
                }
                else
                {
                    AppendDeclaringTypeName(builder)
                        .Append('.')
                        .Append(Name);
                }
            }
            else
            {
                builder
                    .Append('.')
                    .Append(Name);

            }
            builder.Append(GenericArguments);

            if (!hasSubMethodOrLambda)
                builder.AppendFormattingChar('‼');

#if !APKD_STACKTRACE_HIDEPARAMS
#if APKD_STACKTRACE_FULLPARAMS
            builder.Append('(');
            if (MethodBase != null)
            {
                var isFirst = true;
                foreach (var param in Parameters)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        builder.Append(',').Append(' ');

                    param.Append(builder);
                }
            }
            else
            {
                builder.Append('?');
            }
            builder.Append(')');
#else
            char GetParamAlphabeticalName(int index) => (char)((int)'a' + index);
            char? GetParamNameFirstLetter(ResolvedParameter param) => string.IsNullOrEmpty(param?.Name) ? null as char? : param.Name[0];

            builder.Append('(');
            if (MethodBase != null)
            {
                var isFirst = true;
                for (int i = 0, n = Parameters.Count; i < n; ++i)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        builder.Append(',').Append(' ');

                    builder.Append(GetParamNameFirstLetter(Parameters[i]) ?? GetParamAlphabeticalName(i));
                }
            }
            else
            {
                builder.Append('?');
            }
            builder.Append(')');
#endif
#endif

            if (hasSubMethodOrLambda)
            {
                builder.Append('+');
                builder.Append(SubMethod);
                if (IsLambda)
                {
                    builder.Append('(');
                    if (SubMethodBase != null)
                    {
                        var isFirst = true;
                        foreach (var param in SubMethodParameters)
                        {
                            if (isFirst)
                            {
                                isFirst = false;
                            }
                            else
                            {
                                builder.Append(',').Append(' ');
                            }
                            param.Append(builder);
                        }
                    }
                    else
                    {
                        builder.Append('?');
                    }
                    builder.Append(')');
                    builder.Append("=>{…}");

                    if (Ordinal.HasValue)
                    {
                        builder.Append(' ');
                        builder.Append('[');
                        builder.Append(Ordinal);
                        builder.Append(']');
                    }
                }
                builder.AppendFormattingChar('‼');
            }

            return builder;
        }

        private StringBuilder AppendDeclaringTypeName(StringBuilder builder)
        {
            return DeclaringType != null ? builder.AppendTypeDisplayName(DeclaringType, fullName: true, includeGenericParameterNames: true) : builder;
        }
    }
}
