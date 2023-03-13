namespace Rhodopsin.NativeInterop.String

open System
open System.Runtime.InteropServices
open System.Text

/// <summary>
/// Represents raw parts of a Rust <c>String</c>, encoded in UTF-8. Content in ptr does not need to be null-terminated and can contain embedded null character.
/// </summary>
[<Struct; StructLayout(LayoutKind.Sequential)>]
type RustString(strPtr: nativeint, strSize: unativeint) =
    /// <summary>
    /// The pointer to the string buffer.
    /// </summary>
    member this.Pointer = strPtr
    /// <summary>
    /// The number of bytes the whole string occupies. Note that this does not necessarily equals to <c>String.Length</c> since UTF-8 is a variable-length encoding. Also note that according to Rust document this equals to <c>String.len()</c>.
    /// </summary>
    member this.Size = strSize

    /// <summary>
    /// Decodes UTF-8 encoded Rust string and returns a UTF-16 encoded standard <c>System.String</c>.
    /// </summary>
    /// <remarks>
    /// This will not alter the original string, thus it is safe to pass in a non-mut Rust pointer.
    /// </remarks>
    override this.ToString() =
        let rawPtr = NativeInterop.NativePtr.ofNativeInt<byte> this.Pointer
        Encoding.UTF8.GetString(ReadOnlySpan<byte>(NativeInterop.NativePtr.toVoidPtr rawPtr, int this.Size))

/// <summary>
/// Represents raw parts of a .NET CLR <c>System.String</c>, encoded in UTF-16. Content in ptr does not need to be null-terminated and can contain embedded null character.
/// </summary>
/// <param name="strPtr">The pointer to the string buffer.</param>
/// <param name="strSize">The number of bytes the whole string occupies. Note that this does not necessarily equals to <c>String.Length</c> since UTF-16 is a variable-length encoding.</param>
[<Struct; StructLayout(LayoutKind.Sequential)>]
type CLRString(strPtr: nativeint, strSize: unativeint) =
    member this.Pointer = strPtr
    member this.Size = strSize

    /// <summary>
    /// Returns the original <c>System.String</c> representation of the string. Note that this requires allocation, thus you should avoid using this whenever possible and use normal <c>System.String</c> instead.
    /// </summary>
    override this.ToString() =
        let rawPtr = NativeInterop.NativePtr.ofNativeInt<char> this.Pointer
        let charSpan = ReadOnlySpan<char>(NativeInterop.NativePtr.toVoidPtr rawPtr, int this.Size)
        String(charSpan)
