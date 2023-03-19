module Rhodopsin.NativeInterop.Destroy

open System.Runtime.InteropServices

// This currently does nothing due to lack of support from the F# compiler.
[<UnmanagedCallersOnly(EntryPoint = "ffi_destroy")>]
let freeHGlobal = Marshal.DestroyStructure

