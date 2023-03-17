# Introduction

Since the introduction of Native AOT in .NET 7, it is possible to write native libraries that exposes FFI interfaces in .NET CLR Languages. Unfortunately, due to .NET's managed memory model and its unusual UTF-16 encoding, passing data across the FFI-boundary still requires a lot of boilerplate code. This library is created to make passing data across the FFI-boundary easier.

## Features

- For utilities of strings, please refer to [Converting between .NET CLR String to other common string format](./string).
- For utilities of Rust's `Vec<T>` or C++'s `std::vector<T>` can be found in the [Working with vectors](./vector) article.
- Currently, due to lack of Source Code Generation support from F#, it is not possible to avoid boilerplate code when dealing with tagged unions as of now. The article [Working with Tagged Unions](./tagged-unions) can guide you through the whole process of conversion, though.
