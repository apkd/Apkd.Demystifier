// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Collections.Generic.Enumerable;
using System.Reflection;
using System.Text;

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
#if !APKD_STACKTRACE_NOFORMAT
            builder.Append('‹');
#endif
            
            if (IsAsync)
                builder.Append("async ");

            if (ReturnParameter != null)
                ReturnParameter.Append(builder);

#if !APKD_STACKTRACE_NOFORMAT
            builder.Append('›');
#endif
            builder.Append(' ');

            if (DeclaringType != null)
            {

                if (Name == ".ctor")
                {
                    if (string.IsNullOrEmpty(SubMethod) && !IsLambda)
                    {
                        builder
                            .Append(".new ");

                        AppendDeclaringTypeName(builder)
                            .Append(Name);

#if !APKD_STACKTRACE_NOFORMAT
                        builder.Append('‼');
#endif
                    }
                }
                else if (Name == ".cctor")
                {
                    builder.Append("static ");

                    AppendDeclaringTypeName(builder);

#if !APKD_STACKTRACE_NOFORMAT
                    builder.Append('‼');
#endif
                }
                else
                {
                    AppendDeclaringTypeName(builder)
                        .Append('.')
                        .Append(Name);


#if !APKD_STACKTRACE_NOFORMAT
                    builder.Append('‼');
#endif
                }
            }
            else
            {
                builder
                    .Append('.')
                    .Append(Name);
                
#if !APKD_STACKTRACE_NOFORMAT
                builder.Append('‼');
#endif
            }
            builder.Append(GenericArguments);

#if APKD_STACKTRACE_SHOWPARAMS
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
#endif

            if (!string.IsNullOrEmpty(SubMethod) || IsLambda)
            {
                builder.Append("()+");
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
                    builder.Append("=>{}");

                    if (Ordinal.HasValue)
                    {
                        builder.Append(' ');
                        builder.Append('[');
                        builder.Append(Ordinal);
                        builder.Append(']');
                    }
                }
            }

            return builder;
        }

        private StringBuilder AppendDeclaringTypeName(StringBuilder builder)
        {
            return DeclaringType != null ? builder.AppendTypeDisplayName(DeclaringType, fullName: true, includeGenericParameterNames: true) : builder;
        }
    }
}
