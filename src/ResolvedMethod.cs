// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic.Enumerable;
using System.Reflection;
using System.Text;

namespace System.Diagnostics
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
            builder.Append("<i>");
            
            if (IsAsync)
                builder.Append("async ");

            if (ReturnParameter != null)
                ReturnParameter.Append(builder);

            builder.Append("</i> ");

            if (DeclaringType != null)
            {

                if (Name == ".ctor")
                {
                    if (string.IsNullOrEmpty(SubMethod) && !IsLambda)
                    {
                        builder
                            .Append(".<b><i>")
                            .Append("new ");

                        AppendDeclaringTypeName(builder)
                            .Append(Name)
                            .Append("</i></b>");
                    }
                }
                else if (Name == ".cctor")
                {
                    builder
                        .Append("<b><i>")
                        .Append("static ");

                    AppendDeclaringTypeName(builder)
                        .Append("</i></b>");
                }
                else
                {
                    AppendDeclaringTypeName(builder)
                        .Append(".<b><i>")
                        .Append(Name)
                        .Append("</i></b>");
                }
            }
            else
            {
                builder
                    .Append(".<b><i>")
                    .Append(Name)
                    .Append("</i></b>");
            }
            builder.Append(GenericArguments);

            // builder.Append("(");
            // if (MethodBase != null)
            // {
            //     var isFirst = true;
            //     foreach (var param in Parameters)
            //     {
            //         if (isFirst)
            //             isFirst = false;
            //         else
            //             builder.Append(", ");

            //         param.Append(builder);
            //     }
            // }
            // else
            // {
            //     builder.Append("?");
            // }
            // builder.Append(")");

            if (!string.IsNullOrEmpty(SubMethod) || IsLambda)
            {
                builder.Append("()+");
                builder.Append(SubMethod);
                if (IsLambda)
                {
                    builder.Append("(");
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
                                builder.Append(", ");
                            }
                            param.Append(builder);
                        }
                    }
                    else
                    {
                        builder.Append("?");
                    }
                    builder.Append(")");
                    builder.Append("=>{}");

                    if (Ordinal.HasValue)
                    {
                        builder.Append(" [");
                        builder.Append(Ordinal);
                        builder.Append("]");
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
