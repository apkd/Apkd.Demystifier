using System;
using System.Diagnostics;

namespace Apkd.Internal
{
    static class UnityEditorOverrides
    {
        static readonly System.Threading.ThreadLocal<StringBuilder> builderLarge
            = new System.Threading.ThreadLocal<StringBuilder>(() => new StringBuilder(capacity: 4096));

        static readonly System.Threading.ThreadLocal<StringBuilder> builderSmall
            = new System.Threading.ThreadLocal<StringBuilder>(() => new StringBuilder(capacity: 1024));

        static StringBuilder CachedBuilderLarge => builderLarge.Value;
        static StringBuilder CachedBuilderSmall => builderSmall.Value;

        // Method used to build the stack trace string (eg. when using UnityEngine.Debug.Log).
        internal static string ExtractFormattedStackTrace(StackTrace stackTrace)
        {
            try
            {
                var temp = CachedBuilderLarge.Clear();
                return PostprocessStacktrace(new EnhancedStackTrace(stackTrace).ToString(temp), true);
            }
            catch (Exception e)
            {
                return $"Failed to extract stacktrace:\n{stackTrace}\n=== Extractor error: ===\n{e}";
            }
        }

        // Unity uses this method to post-process the stack trace before displaying it in the editor.
        // This doesn't affect the output in the log file.
        internal static string PostprocessStacktrace(string oldStackTrace, bool stripEngineInternalInformation)
        {
#if APKD_STACKTRACE_NOPOSTPROCESS
            return oldStackTrace;
#else
            try
            {
                if (oldStackTrace == null)
                    return String.Empty;

                var output = CachedBuilderLarge.Clear();
                output.Append('\n');

                var temp = CachedBuilderSmall.Clear();
                for (int i = 0; i < oldStackTrace.Length; ++i)
                {
                    char c = oldStackTrace[i];
                    temp.Append(c);

                    // check if we've collected a line into temp
                    if (c == '\n' || i == oldStackTrace.Length - 1)
                    {
                        (bool skip, bool exit) = PostProcessLine(temp, stripEngineInternalInformation);

                        if (exit)
                            break;

                        if (!skip)
                            output.Append(temp); // copy line to output

                        temp.Clear();
                    }
                }

                return output.ToString();

                (bool skip, bool exit) PostProcessLine(StringBuilder line, bool ignoreInternal)
                {
                    // ignore empty lines
                    if (line.Length == 0 || line.Length == 1 && line[0] == '\n')
                        return (skip: true, exit: false);

                    // mke GameView GUI stack traces skip editor GUI part
                    if (ignoreInternal && line.StartsWith("UnityEditor.EditorGUIUtility:RenderGameViewCameras"))
                        return (skip: false, exit: true);

                    line.Insert(0, "│ ");

                    // unify path names to unix style
                    line.Replace('\\', '/');

#if !APKD_STACKTRACE_NOFORMAT
                    // emphasized method return type
                    {
                        line.Replace("‹", "<i>");
                        line.Replace("›", "</i>");
                    }
                    // emphasized method name
                    int boldEnd = line.IndexOf('‼');
                    if (boldEnd >= 0)
                    {
                        int boldStart = 0;
                        for (int i = boldEnd; i >= 0; --i)
                        {
                            char c = line[i];
                            if (c == '.' || c == '+')
                            {
                                boldStart = i;
                                break;
                            }
                        }
                        line.Replace("‼", "</i></b>");
                        line.Insert(boldStart + 1, "<b><i>");
                    }
                    // smaller filename and line number
                    if (line.IndexOf('→') >= 0)
                    {
#if APKD_STACKTRACE_FILEPATH_FONTSIZE_7
                        const string fontsize = "7";
#elif APKD_STACKTRACE_FILEPATH_FONTSIZE_9
                        const string fontsize = "9";
#elif APKD_STACKTRACE_FILEPATH_FONTSIZE_10
                        const string fontsize = "10";
#elif APKD_STACKTRACE_FILEPATH_FONTSIZE_11
                        const string fontsize = "11";
#else
                        const string fontsize = "8";
#endif
                        line.Replace("→", "<size=" + fontsize + ">");
                        bool isLastLine = line[line.Length - 1] != '\n';
                        int endIndex = isLastLine ? line.Length : line.Length - 1;
                        line.Insert(endIndex, "</size>");
                    }
#endif
                    return (skip: false, exit: false);
                }
            }
            catch (Exception e)
            {
                return $"Failed to post-process stacktrace:\n{oldStackTrace}\n=== Post-Processing error: ===\n{e}";
            }
#endif
        }

        // Method used to extract the stack trace from an exception.
        internal static void ExtractStringFromExceptionInternal(object topLevel, out string message, out string stackTrace)
        {
            try
            {
                StringBuilder temp = CachedBuilderLarge;
                Exception current = (Exception)topLevel;

                temp.Clear();

                message = current.Message != null
                    ? temp
                        .Append(current.GetType().ToString())
                        .Append(": ")
                        .Append(current.Message)
                        .ToString()
                    : current.GetType().ToString();

                temp.Clear();
                while (current != null)
                {
                    if (current == topLevel)
                    {
                        new EnhancedStackTrace(current).Append(temp);
                    }
                    else
                    {
                        temp.Append(" ---> ");
                        temp.Append('\n');

                        if (!string.IsNullOrEmpty(current.Message))
                            temp.Append(current.GetType()).Append(": ").Append(current.Message).Append('\n');
                        else
                            temp.Append(current.GetType()).Append('\n');

                        new EnhancedStackTrace(current).Append(temp);

                        temp.Append('\n');
                        temp.Append("Rethrown as:");
                    }

                    current = current.InnerException;
                }
                new EnhancedStackTrace(new StackTrace(skipFrames: 1, fNeedFileInfo: true)).Append(temp);

                stackTrace = PostprocessStacktrace(temp.ToString(), true);
            }
            catch (Exception ex)
            {
                message = $"{topLevel}\n\nDemystifier: Unable to extract stack trace from exception: {(topLevel as Exception).GetType().Name}.";
                stackTrace = ex.ToString();
            }
        }
    }
}
