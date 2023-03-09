module Rhodopsin.NativeInterop.Struct

open System
open System.Runtime.InteropServices

// #[repr(C)]
[<StructLayout(LayoutKind.Sequential)>]
type FFIVector =
    struct
        val buffer: nativeint
        val len: unativeint
        val capacity: unativeint
    end
