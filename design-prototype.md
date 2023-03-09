# Rhodopsin.NativeInterop

This is a design document of Rhodopsin.NativeInterop, a library for interoperation between Native AOT .NET code and languages that supports C-style FFI.

## Primitive Types

Marshalling primitive types with `System.Runtime.InteropService.Marshal` is easy. Use it directly.

## Vectors (`List<T>`)

This part is for working with a Rust style `Vec`. C++ `std::vector` also works as long as the the struct passed from FFI is properly formatted.

```rust
#[repr(C)]
struct FFI_Vector<T> {
    buffer: *mut T
    len: usize
    capacity: usize
}
```

### Receiving Vectors from FFI `module Vector.Marshal`

> **Warning:** `&CStr` is not a FFI safe type in Rust. Before passing it to FFI, make sure you have called `as_ptr` to transform it into a pointer (this will be `IntPtr` in .NET CLR). 
>
> Please also be aware that you need to make sure that the underlying memory is not freed too early. This happens because the pointer returned by `as_ptr` does not carry any lifetime information and the `CString` can be potentially deallocated immediately after the `as_ptr`. Make sure the original `&CStr` is bound to something that doesn't get disposed right after the `as_ptr` call.
>
> Also, Rust 

## References

1. Rust, Passing a Vec from Rust to C, [link](https://users.rust-lang.org/t/pass-a-vec-from-rust-to-c/59184/2)