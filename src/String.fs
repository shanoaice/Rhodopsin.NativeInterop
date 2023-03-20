namespace Rhodopsin.NativeInterop
#nowarn "9"

open System
open System.Runtime.InteropServices
open System.Text

/// <summary>
/// Represents the string marshal policy.
/// </summary>
type FFIStringPolicy =
    /// <summary>
    /// Represents the case when string is stored with a pointer to a C-style string, its encoding relating to the one specified in either <c>System.Runtime.InteropServices.MarshalAsAttribute</c> or <c>System.InteropServices.StructLayoutAttribute.CharSet</c>. You should avoid using this because this will not produce UTF-8 encoded string in any circumstances on Windows, and dealing with either ANSI codepage / UTF-16 wide-string is painful in either C++ or Rust.
    /// This essentially means <c>&amp;CStr</c> in Rust.
    /// </summary>
    | CharSet = 0
    /// <summary>
    /// Represents the case when string is stored in forms of <see cref="T:Rhodopsin.NativeInterop.UTF8String"/>.
    /// </summary>
    | UTF8String = 1
    /// <summary>
    /// Represents the case when string is stored in forms of <see cref="T:Rhodopsin.NativeInterop.UTF16String"/>.
    /// </summary>
    | UTF16String = 2

/// <summary>
/// Represents raw parts of any string, encoded in UTF-8. The string does not need to be null-terminated and can contain embedded null character.
/// </summary>
[<Struct; StructLayout(LayoutKind.Sequential)>]
type UTF8String(strPtr: nativeint, strSize: unativeint) =
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
    /// This <b>requires allocation</b> and <b>will not alter the original string</b>, thus it is safe to pass in a non-mut Rust pointer.
    /// </remarks>
    override this.ToString() =
        let rawPtr = NativeInterop.NativePtr.ofNativeInt<byte> this.Pointer
        Encoding.UTF8.GetString(ReadOnlySpan<byte>(NativeInterop.NativePtr.toVoidPtr rawPtr, int this.Size))

    /// <summary>
    /// Instantiate a <see cref="T:Rhodopsin.NativeInterop.RustString"/> by encoding the <c>System.String</c> in raw UTF-8 bytes and copy it into unmanaged memory. Requires allocation. For security notices, please see the remarks section.
    /// </summary>
    /// <remarks>
    /// Do not forget to wrap and export the Marshal.DestroyStructure method (currently you have to do it manually. If you want your library to be more accessible with the corresponding <c>rhodopsin</c> Rust crate, export it with the signature <c>fn ffi_destroy(isize pointer) -> ()</c>) and call it after finishing using the string. Forgetting doing so might cause memory leak. <b>DO NOT try to free the pointer using the free function or equivalent in the FFI-bridged code.</b> The pointer is allocated using .NET NativeAOT's allocator that is independent from the allocator of the FFI-bridged code. Doing so will trigger an UB and will <b>almost always cause unpredictable chaos.</b>
    /// </remarks>
    /// <param name="str">The UTF-16 encoded .NET <c>System.String</c>.</param>
    new(str: string) =
        let utf8Bytes = Encoding.UTF8.GetBytes str
        let pointer = utf8Bytes.Length * sizeof<byte> |> Marshal.AllocHGlobal
        Marshal.Copy(utf8Bytes, 0, pointer, utf8Bytes.Length)
        UTF8String(pointer, unativeint utf8Bytes.Length)

/// <summary>
/// Represents raw parts of a string, encoded in UTF-16. The string does not need to be null-terminated and can contain embedded null character.
/// </summary>
[<Struct; StructLayout(LayoutKind.Sequential)>]
type UTF16String(strPtr: nativeint, strSize: unativeint) =
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
    /// Returns the original <c>System.String</c> representation of the string. Note that this requires allocation, thus you should avoid using this unless you are receiving a <c>CLRString</c> instance from FFI.
    /// </summary>
    override this.ToString() =
        let rawPtr = NativeInterop.NativePtr.ofNativeInt<byte> this.Pointer
        Encoding.Unicode.GetString(ReadOnlySpan(NativeInterop.NativePtr.toVoidPtr rawPtr, int this.Size))

    /// <summary>
    /// Instantiate a <see cref="T:Rhodopsin.NativeInterop.UTF16String"/> by copying the <c>System.String</c> into unmanaged memory. Requires allocation. For security notices, please see the remarks section.
    /// </summary>
    /// <remarks>
    /// Do not forget to wrap and export the Marshal.DestroyStructure method (currently you have to do it manually. If you want your library to be more accessible with the corresponding <c>rhodopsin</c> Rust crate, export it with the signature <c>fn ffi_destroy(isize pointer) -> ()</c>) and call it after finishing using the string. Forgetting doing so might cause memory leak. <b>DO NOT try to free the pointer using the free function or equivalent in the FFI-bridged code.</b> The pointer is allocated using .NET NativeAOT's allocator that is independent from the allocator of the FFI-bridged code. Doing so will trigger an UB and will <b>almost always cause unpredictable chaos.</b>
    /// </remarks>
    /// <param name="str">The UTF-16 encoded .NET <c>System.String</c>.</param>
    new(str: string) =
        let utf16Bytes = Encoding.Unicode.GetBytes str
        let pointer = utf16Bytes.Length * sizeof<byte> |> Marshal.AllocHGlobal
        Marshal.Copy(utf16Bytes, 0, pointer, utf16Bytes.Length)
        UTF16String(pointer, unativeint utf16Bytes.Length)
