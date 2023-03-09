#nowarn "9" // disables NativePtr warning
namespace Rhodopsin.NativeInterop.Vector

open System
open System.Runtime.InteropServices
open Rhodopsin.NativeInterop.Struct

module FromFFI =
    let AsSpan<'T when 'T: unmanaged> (vecStruct: FFIVector) =
        let rawPtr = NativeInterop.NativePtr.ofNativeInt<'T> vecStruct.buffer
        // int (this.len) might possibly truncate length
        // you will not be able to access any element beyond i32::MAX, due to limit of CLR
        new Span<'T>(NativeInterop.NativePtr.toVoidPtr rawPtr, int vecStruct.len)

    let AsReadOnlySpan<'T when 'T: unmanaged> (vecStruct: FFIVector) =
        let rawPtr = NativeInterop.NativePtr.ofNativeInt<'T> vecStruct.buffer
        // int (this.len) might possibly truncate length
        // you will not be able to access any element beyond i32::MAX, due to limit of CLR
        new ReadOnlySpan<'T>(NativeInterop.NativePtr.toVoidPtr rawPtr, int vecStruct.len)

    let FromVecStrAsPtr (vecStruct: FFIVector) =
        let rawPtr = NativeInterop.NativePtr.ofNativeInt<nativeint> vecStruct.buffer

        seq {
            for i in 0 .. (int vecStruct.len - 1) do
                Marshal.PtrToStringAnsi(NativeInterop.NativePtr.add rawPtr i |> NativeInterop.NativePtr.toNativeInt)
        }

    let FromVecVecUTF16Char (vecStruct: FFIVector) =
        let rawPtr = NativeInterop.NativePtr.ofNativeInt<FFIVector> vecStruct.buffer

        let charVecSeq =
            seq {
                for i in 0 .. (int vecStruct.len - 1) do
                    NativeInterop.NativePtr.add rawPtr i |> NativeInterop.NativePtr.read
            }

        seq {
            for vec in charVecSeq do
                let charSpan = AsReadOnlySpan vec

                String(charSpan)
        }
