module Rhodopsin.NativeInterop.Struct

open System
open System.Runtime.InteropServices

/// <summary>
/// Represents raw parts of a Rust `Vec<T>`, 
/// </summary>
[<StructLayout(LayoutKind.Sequential)>]
type FFIVector =
    struct
        val buffer: nativeint
        val len: unativeint
        val capacity: unativeint
    end
