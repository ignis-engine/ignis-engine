using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Ignis.Interop;

/// <summary>
/// Wrapper class around an allocated heap to make sure it gets freed when it goes out of scope.
/// </summary>
public sealed unsafe class NativeHeap : IDisposable
{
    public NativeHeap() { }

    /// <summary>
    /// Take ownership of an existing native heap pointer
    /// </summary>
    public NativeHeap(byte* nativePtr, int size)
    {
        HeapPtr = nativePtr;
        Size = size;
    }

    /// <summary>
    /// Take ownership of an existing native heap pointer
    /// </summary>
    public NativeHeap(IntPtr nativePtr, int size)
    {
        HeapPtr = (byte*)nativePtr;
        Size = size;
    }

    /// <summary>
    /// The allocated pointer to the heap (or nullptr)
    /// </summary>
    public byte* HeapPtr { get; private set; } = (byte*)IntPtr.Zero;

    /// <summary>
    /// The size of the heap
    /// </summary>
    public int Size { get; private set; }

    public void Dispose()
    {
        if (!Valid())
            return;

        Marshal.FreeHGlobal((IntPtr)HeapPtr);
        HeapPtr = (byte*)IntPtr.Zero;
    }

    /// <summary>
    /// Returns true if the internal pointer is not null and points to a valid heap.
    /// </summary>
    public bool Valid()
    {
        return HeapPtr != (byte*)IntPtr.Zero;
    }

    /// <summary>
    /// Release ownership over the internal pointer so that it can be passed along to a native library
    /// to be cleaned up elsewhere.
    ///
    /// Be aware that something must call <see cref="Marshal.FreeHGlobal"/> manually somewhere.
    /// </summary>
    public byte* Release()
    {
        var ptr = HeapPtr;
        HeapPtr = (byte*)IntPtr.Zero;
        return ptr;
    }

    /// <summary>
    /// Allocate a new native heap
    /// </summary>
    /// <param name="size">Size in bytes</param>
    public static NativeHeap Allocate(int size)
    {
        return new NativeHeap(Marshal.AllocHGlobal(size), size);
    }

    public static NativeHeap CopyFrom(byte[] array)
    {
        var heap = Allocate(array.Length);
        Marshal.Copy(array, 0, (nint)heap.HeapPtr, array.Length);
        return heap;
    }

    public static NativeHeap FromStruct<T>([DisallowNull] T structure)
    {
        var heap = Allocate(Marshal.SizeOf(structure));
        Marshal.StructureToPtr(structure, (IntPtr)heap.HeapPtr, false);
        return heap;
    }

    public static implicit operator byte*(NativeHeap heap) => heap.HeapPtr;
}
