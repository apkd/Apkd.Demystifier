using System;
using System.Reflection;
using System.Diagnostics;
using static System.Reflection.BindingFlags;

namespace Apkd.Internal
{
    [UnityEditor.InitializeOnLoad]
    static class Detour
    {
        internal static unsafe void TryDetourFromTo(MethodInfo src, MethodInfo dst)
        {
            try
            {
                if (IntPtr.Size == sizeof(Int64))
                {
                    // 64-bit systems use 64-bit absolute address and jumps
                    // 12 byte destructive

                    // Get function pointers
                    long Source_Base = src.MethodHandle.GetFunctionPointer().ToInt64();
                    long Destination_Base = dst.MethodHandle.GetFunctionPointer().ToInt64();

                    // Native source address
                    byte* Pointer_Raw_Source = (byte*)Source_Base;

                    // Pointer to insert jump address into native code
                    long* Pointer_Raw_Address = (long*)(Pointer_Raw_Source + 0x02);

                    // Insert 64-bit absolute jump into native code (address in rax)
                    // mov rax, immediate64
                    // jmp [rax]
                    *(Pointer_Raw_Source + 0x00) = 0x48;
                    *(Pointer_Raw_Source + 0x01) = 0xB8;
                    *Pointer_Raw_Address = Destination_Base; // ( Pointer_Raw_Source + 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 )
                    *(Pointer_Raw_Source + 0x0A) = 0xFF;
                    *(Pointer_Raw_Source + 0x0B) = 0xE0;

                }
                else
                {
                    // 32-bit systems use 32-bit relative offset and jump
                    // 5 byte destructive

                    // Get function pointers
                    int Source_Base = src.MethodHandle.GetFunctionPointer().ToInt32();
                    int Destination_Base = dst.MethodHandle.GetFunctionPointer().ToInt32();

                    // Native source address
                    byte* Pointer_Raw_Source = (byte*)Source_Base;

                    // Pointer to insert jump address into native code
                    int* Pointer_Raw_Address = (int*)(Pointer_Raw_Source + 1);

                    // Jump offset (less instruction size)
                    int offset = (Destination_Base - Source_Base) - 5;

                    // Insert 32-bit relative jump into native code
                    *Pointer_Raw_Source = 0xE9;
                    *Pointer_Raw_Address = offset;
                }
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError($"Unable to detour: {src.Name}: {exception.Message}");
            }
        }

        static Detour()
        {
            TryDetourFromTo(
                src: typeof(UnityEngine.StackTraceUtility).GetMethod(nameof(UnityEditorOverrides.PostprocessStacktrace), NonPublic | Static),
                dst: typeof(UnityEditorOverrides).GetMethod(nameof(UnityEditorOverrides.PostprocessStacktrace), NonPublic | Static)
            );

            TryDetourFromTo(
                src: typeof(UnityEngine.StackTraceUtility).GetMethod(nameof(UnityEditorOverrides.ExtractStringFromExceptionInternal), NonPublic | Static),
                dst: typeof(UnityEditorOverrides).GetMethod(nameof(UnityEditorOverrides.ExtractStringFromExceptionInternal), NonPublic | Static)
            );

            TryDetourFromTo(
                src: typeof(UnityEngine.StackTraceUtility).GetMethod(nameof(UnityEditorOverrides.ExtractFormattedStackTrace), NonPublic | Static),
                dst: typeof(UnityEditorOverrides).GetMethod(nameof(UnityEditorOverrides.ExtractFormattedStackTrace), NonPublic | Static)
            );
        }
    }
}