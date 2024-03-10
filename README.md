# Exponential Golomb Coding

This project implements [Exponential Golomb Coding](https://en.wikipedia.org/wiki/Exponential-Golomb_coding) in C# with zero allocation

## Usage

There are two ways to use this project:

1. Copy the project source file: Include the source code directly in your project.
1. Use NuGet: Install the package using the following command:

```
dotnet add package ExponentialGolombCoding
```

## Encoding

Encode a single unsigned integer with order 0:

```csharp
var bytes = new ExpGolombCoder().Encode(863, 0);
```

Encode multiple unsigned integer with order 0:

```csharp
var bytes = new ExpGolombCoder().Encode([22,33,77,88], 0);
```

Encode multiple unsigned integer with order 0 to Base64:

```csharp
var str = new ExpGolombCoder().EncodeToBase64([22,33,77,88], 0);
```

## Decoding

The `ExpGolombCoder` class also provides equivalent methods for decoding the encoded data.

## Zero Allocation
All methods in `ExpGolombCoder` avoid allocation during processing, except for the final result (bytes or string). This improves performance for memory-intensive tasks.

The project includes the `ExpGolombCoderCore` class. This class offers methods for encoding and decoding with absolutely no allocation.  Refer to the unit tests for usage examples.

**Note**: Using `ExpGolombCoderCore` might require more complex code compared to the simpler `ExpGolombCoder` class.