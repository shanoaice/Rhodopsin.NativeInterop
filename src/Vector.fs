namespace Rhodopsin.NativeInterop

#nowarn "9" // disables NativePtr warning

open System
open System.Runtime.InteropServices

/// <summary>
/// Represents raw parts of a Rust <c>Vec&lt;T&gt;</c>. Viable for other FFI too as long as the format is obeyed.
/// </summary>
/// <param name="vecBuffer">The pointer that points to the content of the Vector.</param>
/// <param name="vecLength">The length of the Vector.</param>
/// <param name="vecCapacity">The capacity of the Vector (can contain <c>len &lt;= capacity</c> without realloc).</param>
[<StructLayout(LayoutKind.Sequential); Struct>]
type Vector(vecBuffer: nativeint, vecLength: unativeint, vecCapacity: unativeint) =
    /// The pointer that points to the content of the Vector.
    member this.buffer = vecBuffer
    /// The length of the Vector.
    member this.len = vecLength
    /// <summary>
    /// The capacity of the Vector (can contain <c>len &lt;= capacity</c> without realloc).
    /// </summary>
    member this.capacity = vecCapacity

    /// <summary>
    /// Represents the underlying <c>Vec&lt;T&gt;</c> data as a <c>System.Span&lt;T&gt;</c>.
    /// </summary>
    member this.AsSpan<'T when 'T: unmanaged>() =
        let rawPtr = NativeInterop.NativePtr.ofNativeInt<'T> this.buffer
        // int (this.len) might possibly truncate length
        // you will not be able to access any element beyond i32::MAX, due to limit of CLR
        new Span<'T>(NativeInterop.NativePtr.toVoidPtr rawPtr, int this.len)

    member this.AsReadOnlySpan<'T when 'T: unmanaged>() =
        let rawPtr = NativeInterop.NativePtr.ofNativeInt<'T> this.buffer
        // int (this.len) might possibly truncate length
        // you will not be able to access any element beyond i32::MAX, due to limit of CLR
        new ReadOnlySpan<'T>(NativeInterop.NativePtr.toVoidPtr rawPtr, int this.len)

    member this.FromVecStrAsPtr() =
        let rawPtr = NativeInterop.NativePtr.ofNativeInt<nativeint> this.buffer
        let length = this.len

        seq {
            for i in 0 .. (int length - 1) do
                Marshal.PtrToStringAnsi(NativeInterop.NativePtr.add rawPtr i |> NativeInterop.NativePtr.toNativeInt)
        }

    member this.FromVecVecUTF16Char() =
        let rawPtr = NativeInterop.NativePtr.ofNativeInt<Vector> this.buffer
        let length = this.len

        let charVecSeq =
            seq {
                for i in 0 .. (int length - 1) do
                    NativeInterop.NativePtr.add rawPtr i |> NativeInterop.NativePtr.read
            }

        seq {
            for vec in charVecSeq do
                let charSpan = vec.AsReadOnlySpan()

                String(charSpan)
        }
        
    static member FromEnumerable () =
        ()
