namespace Rhodopsin.NativeInterop

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
    /// The number of bytes the whole string occupies.
    /// </summary>
    /// <remarks>
    /// This does not necessarily equals to <c>String.Length</c> since UTF-8 is a variable-length encoding. Also, according to Rust document this equals to <c>String.len()</c>.
    /// </remarks>
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
    /// Encodes a UTF-16 encoded <c>System.String</c> and returns a UTF-8 encoded Rust string represented as <see cref="T:Rhodopsin.NativeInterop.RustString"/>. This method requires allocation. For security notices, please see the remarks section.
    /// </summary>
    /// <remarks>
    /// Do not forget to wrap and export the Marshal.FreeHGlobal method (currently you have to do it manually. If you want your library to be more accessible with the corresponding <c>rhodopsin</c> Rust crate, export it with the signature <c>void ffi_free(size_t pointer)</c>) and call it after finishing using the string. Forgetting doing so might cause memory leak. <b>DO NOT try to free the pointer using the free function or equivalent in the FFI-bridged code.</b> The pointer is allocated using .NET NativeAOT's allocator that is independent from the allocator of the FFI-bridged code. Doing so will trigger an UB and will <b>almost always cause unpredictable chaos.</b>
    /// </remarks>
    /// <param name="str">The UTF-16 encoded .NET <c>System.String</c>.</param>
    /// <returns>The UTF-8 encoded Rust string represented as struct <see cref="T:Rhodopsin.NativeInterop.RustString"/>.</returns>
    static member FromString (str: string) =
        let utf8Bytes = Encoding.UTF8.GetBytes str
        let pointer = utf8Bytes.Length * sizeof<byte> |> Marshal.AllocHGlobal
        Marshal.Copy(utf8Bytes, 0, pointer, utf8Bytes.Length)
        RustString(pointer, unativeint utf8Bytes.Length)

/// <summary>
/// Represents raw parts of a .NET CLR <c>System.String</c>, encoded in UTF-16. Content in ptr does not need to be null-terminated and can contain embedded null character.
/// </summary>
[<Struct; StructLayout(LayoutKind.Sequential)>]
type CLRString(strPtr: nativeint, strSize: unativeint) =
    /// <summary>
    /// The pointer to the string buffer.
    /// </summary>
    member this.Pointer = strPtr
    /// <summary>
    /// The number of bytes the whole string occupies.
    /// </summary>
    /// <remarks>
    /// This does not necessarily equals to <c>String.Length</c> since UTF-16 is a variable-length encoding.
    /// </remarks>
    member this.Size = strSize

    /// <summary>
    /// Returns the original <c>System.String</c> representation of the string. Note that this requires allocation, thus you should avoid using this whenever possible and use normal <c>System.String</c> instead.
    /// </summary>
    override this.ToString() =
        let rawPtr = NativeInterop.NativePtr.ofNativeInt<char> this.Pointer
        let charSpan = ReadOnlySpan<char>(NativeInterop.NativePtr.toVoidPtr rawPtr, int this.Size)
        String(charSpan)
        
    /// <summary>
    /// Copies a UTF-16 encoded <c>System.String</c> into unmanaged memory and returns its representation in <see cref="T:Rhodopsin.NativeInterop.CLRString"/>. This method requires allocation. For security notices, please see the remarks section.
    /// </summary>
    /// <remarks>
    /// Do not forget to wrap and export the Marshal.FreeHGlobal method (currently you have to do it manually. If you want your library to be more accessible with the corresponding <c>rhodopsin</c> Rust crate, export it with the signature <c>void ffi_free(size_t pointer)</c>) and call it after finishing using the string. Forgetting doing so might cause memory leak. <b>DO NOT try to free the pointer using the free function or equivalent in the FFI-bridged code.</b> The pointer is allocated using .NET NativeAOT's allocator that is independent from the allocator of the FFI-bridged code. Doing so will trigger an UB and will <b>almost always cause unpredictable chaos.</b>
    /// </remarks>
    /// <param name="str">The UTF-16 encoded .NET <c>System.String</c>.</param>
    /// <returns>The UTF-16 encoded <c>System.String</c> represented as struct <see cref="T:Rhodopsin.NativeInterop.CLRString"/>.</returns>
    static member FromString (str: string) =
        let utf16Bytes = Encoding.Unicode.GetBytes str
        let pointer = utf16Bytes.Length * sizeof<byte> |> Marshal.AllocHGlobal
        Marshal.Copy(utf16Bytes, 0, pointer, utf16Bytes.Length)
        CLRString(pointer, unativeint utf16Bytes.Length)
