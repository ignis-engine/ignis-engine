using Ignis.Renderer;

namespace IgnisGame;

internal class Program
{
    private static void Main(string[] args)
    {
        using var renderer = new Renderer();
        renderer.Test();
    }
}
