module Rhodopsin.NativeInterop.Free

open System.Runtime.InteropServices

// This currently does nothing due to lack of support from the F# compiler.
[<UnmanagedCallersOnly(EntryPoint = "ffi_free")>]
let freeHGlobal = Marshal.FreeHGlobal

